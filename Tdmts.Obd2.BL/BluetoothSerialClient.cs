using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Tdmts.Obd2.BL
{
    public class BluetoothSerialClient : IDisposable
    {
        public event EventHandler<string> OnConnect;
        public event EventHandler<string> OnSent;
        public event EventHandler<string> OnReceived;
        public event EventHandler<string> OnDisconnect;
        public event EventHandler<Exception> OnException;

        private CancellationTokenSource readCancellationTokenSource;
        private object readCancelLock;

        private CancellationTokenSource writeCancellationTokenSource;
        private object writeCancelLock;

        private RfcommDeviceService rfCommDeviceService;
        private StreamSocket streamSocket;
        private DataWriter writer;
        private DataReader reader;

        public BluetoothSerialClient()
        {
        }

        private async Task<bool> Connect()
        {
            if (rfCommDeviceService == null)
            {
                string selector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);
                DeviceInformation device = devices.SingleOrDefault(x => x.Name.Equals("OBDII", StringComparison.OrdinalIgnoreCase));

                if (device != null)
                {
                    rfCommDeviceService = await RfcommDeviceService.FromIdAsync(device.Id);

                    streamSocket = new StreamSocket();

                    await streamSocket.ConnectAsync(rfCommDeviceService.ConnectionHostName, rfCommDeviceService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    
                    reader = new DataReader(streamSocket.InputStream);
                    readCancellationTokenSource = new CancellationTokenSource();
                    readCancelLock = new object();

                    writer = new DataWriter(streamSocket.OutputStream);
                    writeCancellationTokenSource = new CancellationTokenSource();
                    writeCancelLock = new object();

                    OnConnect(this, "Connected.");

                    return true;
                }
                else
                {
                    OnException(this, new Exception("Could not connect."));
                    return false;
                }
            }
            return true;
        }
        
        public async Task<List<byte>> WriteReadAsync(string message)
        {
            string result = string.Empty;

            if (await Connect() == true)
            {
                await WriteAsync(message).ConfigureAwait(true);

                result = await ReadAsync().ConfigureAwait(true);
            }
            return Encoding.ASCII.GetBytes(result).ToList();
        }

        public async Task<int> WriteAsync(string message)
        {
            int result = 0;
            if (await Connect() == true)
            {
                if (writer == null)
                {
                    writer = new DataWriter(streamSocket.OutputStream);
                }
                if (writeCancellationTokenSource == null)
                {
                    writeCancellationTokenSource = new CancellationTokenSource();
                }

                writer.WriteString(message);
                result = await WriteAsync(writeCancellationTokenSource.Token);

                OnSent(this, message);

                writer.DetachStream();
                writer.DetachBuffer();
                writer.Dispose();
                writer = null;
            }
            return result;
        }


        private async Task<int> WriteAsync(CancellationToken cancellationToken)
        {
            Task<uint> storeAsyncTask;

            lock (writeCancelLock)
            {
                cancellationToken.ThrowIfCancellationRequested();
                storeAsyncTask = writer.StoreAsync().AsTask(cancellationToken);
            }

            return (int)(await storeAsyncTask);
        }

        public async Task<string> ReadAsync()
        {
            string result = string.Empty;
            if (await Connect() == true)
            {
                if (reader == null)
                {
                    reader = new DataReader(streamSocket.InputStream);
                }
                if (readCancellationTokenSource == null)
                {
                    readCancellationTokenSource = new CancellationTokenSource();
                }

                result = await ReadAsync(readCancellationTokenSource.Token);

                OnReceived(this, result);

                reader.DetachStream();
                reader.DetachBuffer();
                reader.Dispose();
                reader = null;
            }
            return result;
        }

        private async Task<string> ReadAsync(CancellationToken cancellationToken)
        {
            string result = string.Empty;

            Task<uint> loadAsyncTask;
            uint readBufferLength = 1024;

            StringBuilder stringBuilder = new StringBuilder();

            do
            {
                lock (readCancelLock)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    loadAsyncTask = reader.LoadAsync(readBufferLength).AsTask(cancellationToken);
                }

                uint bytesRead = await loadAsyncTask;

                if (bytesRead > 0)
                {
                    stringBuilder.Append(reader.ReadString(bytesRead));
                }
            } while (reader.UnconsumedBufferLength > 0);

            result = stringBuilder.ToString();
            
            return result;
        }
        
        private void CancelReadTask()
        {
            if (readCancellationTokenSource != null)
            {
                if (!readCancellationTokenSource.IsCancellationRequested)
                {
                    readCancellationTokenSource.Cancel();
                }
            }
        }

        private void CancelWriteTask()
        {
            if (writeCancellationTokenSource != null)
            {
                if (!writeCancellationTokenSource.IsCancellationRequested)
                {
                    writeCancellationTokenSource.Cancel();
                }
            }
        }

        public void Dispose()
        {
            CancelWriteTask();
            CancelReadTask();

            if (readCancellationTokenSource != null)
            {
                readCancellationTokenSource.Dispose();
                readCancellationTokenSource = null;
            }

            if (writeCancellationTokenSource != null)
            {
                writeCancellationTokenSource.Dispose();
                writeCancellationTokenSource = null;
            }

            if (rfCommDeviceService != null)
            {
                rfCommDeviceService.Dispose();
            }

            if (reader != null)
            {
                reader.Dispose();
            }

            if (writer != null)
            {
                writer.Dispose();
            }

            OnDisconnect(this, "Disconnected.");
        }
    }
}