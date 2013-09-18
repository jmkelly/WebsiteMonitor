using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using Timer = System.Timers.Timer;
using EasyHttp.Http;
using System.Windows.Media;

namespace WebsiteMonitor
{
    public class CheckWebsiteAtInterval
    {
        private readonly Rectangle _statusIndicator;
        private readonly TextBlock _lastUpdatedTextBlock;
        private readonly TextBlock _messageTextBox;
        private readonly Dispatcher _threadDispatcher;
        private string _url;
        private string _textToMatch;
        private Timer _timer;

        public CheckWebsiteAtInterval(Rectangle statusIndicator, TextBlock lastUpdatedTextBlock, TextBlock messageTextBox, Dispatcher threadDispatcher)
        {
            _statusIndicator = statusIndicator;
            _lastUpdatedTextBlock = lastUpdatedTextBlock;
            _messageTextBox = messageTextBox;
            _threadDispatcher = threadDispatcher;
        }

        public void Start(int interval, string address, string textToMatch)
        {
            _url = address;
            _textToMatch = textToMatch;
            _timer = new Timer {Interval = TimeSpan.FromSeconds(interval).TotalMilliseconds};
            _timer.Elapsed += Check;
            _timer.Start();
        }

        private void Check(object sender, ElapsedEventArgs e)
        {
            var http = new HttpClient();
            var result = false;
            string message = "";

            try
            {
                var response = http.Get(_url);
                result =  response.RawText.Contains(_textToMatch);
                message += result ?  string.Format(@"Message:  ""{0}"" found in page", _textToMatch) : string.Format(@"Message:  ""{0}"" not found in page", _textToMatch);

            }
            catch (Exception exception)
            {
                message += exception.Message;
            }


            _threadDispatcher.Invoke(DispatcherPriority.Input, new Action(() =>
            {
                _statusIndicator.Fill = result
                    ? new SolidColorBrush(Colors.Chartreuse)
                    : new SolidColorBrush(Colors.Red);
                _lastUpdatedTextBlock.Text = string.Format("Last tested at {0}", DateTime.Now);
                _messageTextBox.Text = message;

            }));
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}
