using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using AutofacSerilogIntegration;
using Serilog;
using Serilog.Context;

namespace SerilogDemo.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        void OnApplicationStartup(object sender, StartupEventArgs se)
        {
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}|Level:{Level}|Guid:{Guid}|ThreadId:{ThreadId}|ContextValue:{ContextValue}|SourceContext:{SourceContext}|Message:{Message}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Guid", Guid.NewGuid())
                .Enrich.FromLogContext()
                .WriteTo.Trace(
                    outputTemplate: outputTemplate
                )
                .CreateLogger();

            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>().AsSelf();

            builder.RegisterLogger();

            var container = builder.Build();

            LogContext.PushProperty("ContextValue", 1);
            
            using (var scope = container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();
                logger.Debug("Checking context value before mainWindow.ShowDialog()");

                var mainWindow = scope.Resolve<MainWindow>();
                mainWindow.ShowDialog();

                logger.Debug("Checking context value after mainWindow.ShowDialog()");
            }
        }
    }
}
