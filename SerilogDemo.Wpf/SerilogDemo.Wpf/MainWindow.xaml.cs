using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            _logger.Debug("MainWindow initialized");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _logger.Debug("Before logging user in");

            LogContext.PushProperty("ContextValue", 3);

            _logger.Debug("after logging user in");

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _logger.Debug("Log event");
        }
    }
}
