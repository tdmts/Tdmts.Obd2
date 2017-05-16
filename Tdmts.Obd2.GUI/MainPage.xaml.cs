using Tdmts.Obd2.BL;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tdmts.Obd2.GUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObdClient odbClient;
        private TextBlock lblSpeed = default(TextBlock);
        private TextBlock lblEngineRpm = default(TextBlock);
        private TextBlock lblHorsepower = default(TextBlock);

        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeEvents();
        }

        private void InitializeEvents()
        {
            odbClient = new ObdClient();
            odbClient.OnConnect += OdbClient_OnConnect;
            odbClient.OnDisconnect += OdbClient_OnDisconnect;
            odbClient.OnException += OdbClient_OnException;
            odbClient.OnReceived += OdbClient_OnReceived;
            odbClient.OnSent += OdbClient_OnSent;
        }
        
        private async void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string engineRpm = await odbClient.QueryModeParameterId(1, "Engine RPM");
                lblEngineRpm.Text = engineRpm;

                string speed = await odbClient.QueryModeParameterId(1, "Vehicle speed");
                lblSpeed.Text = speed;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void lblSpeed_Loaded(object sender, RoutedEventArgs e)
        {
            lblSpeed = sender as TextBlock;
        }

        private void lblEngineRpm_Loaded(object sender, RoutedEventArgs e)
        {
            lblEngineRpm = sender as TextBlock;
        }

        private void lblHorsepower_Loaded(object sender, RoutedEventArgs e)
        {
            lblHorsepower = sender as TextBlock;
        }

        private void OdbClient_OnSent(object sender, string e)
        {
            
        }

        private void OdbClient_OnReceived(object sender, string e)
        {
            
        }

        private void OdbClient_OnException(object sender, System.Exception e)
        {
            
        }

        private void OdbClient_OnDisconnect(object sender, string e)
        {
            
        }

        private void OdbClient_OnConnect(object sender, string e)
        {
            
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await odbClient.Reset();
                await odbClient.SetProtocolAuto();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
