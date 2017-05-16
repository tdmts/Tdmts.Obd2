using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdmts.Obd2.BL
{
    public class ObdClient
    {
        public event EventHandler<string> OnConnect;
        public event EventHandler<string> OnSent;
        public event EventHandler<string> OnReceived;
        public event EventHandler<string> OnDisconnect;
        public event EventHandler<Exception> OnException;

        private BluetoothSerialClient bluetoothSerialClient = new BluetoothSerialClient();
        
        private List<Mode> _modes = new List<Mode>();
        
        public ObdClient()
        {
            Modes.Add(ModeFactory.createMode1());
            bluetoothSerialClient.OnConnect += BluetoothSerialClient_OnConnect;
            bluetoothSerialClient.OnDisconnect += BluetoothSerialClient_OnDisconnect;
            bluetoothSerialClient.OnReceived += BluetoothSerialClient_OnReceived;
            bluetoothSerialClient.OnSent += BluetoothSerialClient_OnSent;
            bluetoothSerialClient.OnException += BluetoothSerialClient_OnException;
        }
        
        public async Task<string> Reset()
        {
            return await SendReceive("AT Z\r");
        }

        public async Task<string> SetProtocolAuto()
        {
            return await SendReceive("AT SP 0\r");
        }

        public async Task<string> SendReceive(string message)
        {
            await bluetoothSerialClient.WriteAsync(message).ConfigureAwait(true);
            return await bluetoothSerialClient.ReadAsync().ConfigureAwait(true);
        }

        public async Task<string> QueryModeParameterId(int mId, string description)
        {
            if (mId <= Modes.Count)
            {
                Mode mode = Modes[mId - 1];
                if (mode.IsSupported(description))
                {
                    ParameterID parameterId = mode.Get(description);
                    return await QueryParameterID(mode.Id, parameterId);
                }
            }
            return string.Empty;
        }

        //mode = 1, pid = 0x0D
        public async Task<string> QueryModeParameterId(int mId, int pId)
        {
            if(mId <= Modes.Count)
            {
                Mode mode = Modes[mId];
                if(mode.IsSupported(pId))
                {
                    ParameterID parameterId = mode.Get(pId);
                    return await QueryParameterID(mode.Id, parameterId);
                }
            }
            return string.Empty;
        }

        private async Task<string> QueryParameterID(int mId, ParameterID parameterId)
        {
            parameterId.Machinevalue = await bluetoothSerialClient.WriteReadAsync(string.Format("{0}{1}{2}", mId.ToString("00"), parameterId.Pid.ToString("X2"), "\r"));
            parameterId.Calculate();
            return parameterId.HumanValue;
        }

        public List<Mode> Modes
        {
            get
            {
                return _modes;
            }
            set
            {
                _modes = value;
            }
        }

        private void BluetoothSerialClient_OnSent(object sender, string e)
        {
            OnSent(sender, e);
        }

        private void BluetoothSerialClient_OnReceived(object sender, string e)
        {
            OnReceived(sender, e);
        }

        private void BluetoothSerialClient_OnDisconnect(object sender, string e)
        {
            OnDisconnect(sender, e);
        }

        private void BluetoothSerialClient_OnConnect(object sender, string e)
        {
            OnConnect(sender, e);
        }

        private void BluetoothSerialClient_OnException(object sender, Exception e)
        {
            OnException(sender, e);
        }
    }
}
