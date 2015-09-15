using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Serilog;
using Serilog.Context;

namespace SerilogDemo.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ILogger _logger;

        public MainWindow(ILogger logger)
        {
            _logger = logger;

            InitializeComponent();

            LogContext.PushProperty("ContextValue", 2);

            _logger.Debug("MainWindow initialized - context value should be 2");
        }

        void loginButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.Debug("Before logging user in - context value should be 2");

            Task.Run(async () => await SimulateLogin()).Wait();

            _logger.Debug("after logging user in - context value should be 4 (but is 2!)");

        }

        void logEventButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.Debug("Log event");
        }

        async Task SimulateLogin()
        {
            LogContext.PushProperty("ContextValue", 3);
            _logger.Debug("SimulateLogin start - context value should be 3");

            await Task.Run(() =>
            {
                _logger.Debug("on login background thread... - context value should be 3");

                Thread.Sleep(2000);
            });

            LogContext.PushProperty("ContextValue", 4);
            _logger.Debug("SimulateLogin end - context value should be 4");
        }
    }
}
