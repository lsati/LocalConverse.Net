using Serilog;
using System.Diagnostics;
using System.Text;
using System.Windows;
using SerilogTrace = SerilogTraceListener;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using LocalConverseClient.Net.Settings;

namespace LocalConverseClient.Net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            SetupLogging();

            Log.Logger.Information($"LocalConverse.Net [{Assembly.GetExecutingAssembly().GetName().Version}]");
            MainFrame.Navigate(new Uri("Pages\\WelcomePage.xaml", UriKind.Relative));

            // load settings
            var settings = LocalConverseSettingsManager.GetInstance();

        }


        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleButton.IsChecked == true)
            {
                MenuPanel.Visibility = Visibility.Collapsed;
                ToggleButton.Content = "<";
            }
            else
            {
                MenuPanel.Visibility = Visibility.Visible;
                ToggleButton.Content = ">";
            }
        }

        private void SetupLogging()
        {
            Log.Logger = new LoggerConfiguration()
                  .WriteTo.File("logs/logfile.log", rollingInterval: RollingInterval.Day)
                  .CreateLogger();

            Trace.Listeners.Add(new SerilogTrace.SerilogTraceListener());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && !string.IsNullOrWhiteSpace(button.Name))
            {
                var page = "Pages\\" + button.Name?.Replace("Button", string.Empty) + ".xaml";
                MainFrame.Navigate(new Uri(page, UriKind.Relative));
            }
        }
    }
}