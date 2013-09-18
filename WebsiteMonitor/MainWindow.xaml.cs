using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using EasyHttp.Http;

namespace WebsiteMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly CheckWebsiteAtInterval _checker;

        public MainWindow()
        {
            InitializeComponent();
            _checker = new CheckWebsiteAtInterval(StatusIndicator,LastTestAtTextBox,MessageTextBox, Dispatcher.CurrentDispatcher);
            SiteTextBox.Text = Properties.Settings.Default["address"].ToString();
            TextToMatchTextBox.Text = Properties.Settings.Default["textToMatch"].ToString();
            RecurIntervalTextBox.Text = Properties.Settings.Default["interval"].ToString();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default["address"] = SiteTextBox.Text;
            Properties.Settings.Default["textToMatch"] = TextToMatchTextBox.Text;
            Properties.Settings.Default["interval"] = RecurIntervalTextBox.Text;
            Properties.Settings.Default.Save();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {            var address = SiteTextBox.Text;
            var textToMatch = TextToMatchTextBox.Text;
            MessageTextBox.Text = "Message: ";

            var http = new HttpClient();
            bool result = false;
            try
            {
                var response = http.Get(address);
                result = response.RawText.Contains(textToMatch);
                MessageTextBox.Text = result ? string.Format(@"Message:  ""{0}"" found in page", textToMatch) : string.Format(@"Message:  ""{0}"" not found in page", textToMatch);
            }
            catch (Exception ex)
            {
                MessageTextBox.Text = "Message: " + ex.Message;
            }

                StatusIndicator.Fill = result ? new SolidColorBrush(Colors.Chartreuse) : new SolidColorBrush(Colors.Red);
                LastTestAtTextBox.Text = string.Format("Last tested at {0}", DateTime.Now);

        }

        private void StartTimeButton_Click(object sender, RoutedEventArgs e)
        {
            var address = SiteTextBox.Text;
            var textToMatch = TextToMatchTextBox.Text;
            int interval = Convert.ToInt32(RecurIntervalTextBox.Text);

            if ((string) StartTimerButton.Content == "Start Timer")
            {
                _checker.Start(interval, address, textToMatch);
                StartTimerButton.Content = "Stop Timer";
                DisableButtons();
            }
            else
            {
                _checker.Stop();
                StartTimerButton.Content = "Start Timer";
                EnableButtons();
            }
        }

        private void DisableButtons()
        {
            SiteTextBox.IsEnabled = false;
            TextToMatchTextBox.IsEnabled= false;
            RecurIntervalTextBox.IsEnabled = false;
        }
        
        private void EnableButtons()
        {
            SiteTextBox.IsEnabled = true;
            TextToMatchTextBox.IsEnabled = true;
            RecurIntervalTextBox.IsEnabled = true;
        }
    }

   
}
    