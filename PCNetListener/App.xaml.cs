using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;

using PCNetListener.Activation;
using PCNetListener.Contracts.Activation;
using PCNetListener.Contracts.Services;
using PCNetListener.Contracts.Views;
using PCNetListener.Core.Contracts.Services;
using PCNetListener.Core.Services;
using PCNetListener.Models;
using PCNetListener.Services;
using PCNetListener.ViewModels;
using PCNetListener.Views;
using Serilog;
using Serilog.Core;

namespace PCNetListener
{
    // For more inforation about application lifecyle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

    // WPF UI elements use language en-US by default.
    // If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
    // Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
    public partial class App : Application
    {
        private IHost _host;

        public T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public App()
        {
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            // Check the current directory that the application is running on 
            // Then once the file 'appsetting.json' is found, we are adding it.
            // We add env variables, which can override the configs in appsettings.json
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            // https://docs.microsoft.com/windows/uwp/design/shell/tiles-and-notifications/send-local-toast?tabs=desktop
            ToastNotificationManagerCompat.OnActivated += (toastArgs) =>
            {
                Current.Dispatcher.Invoke(async () =>
                {
                    var config = GetService<IConfiguration>();
                    config[ToastNotificationActivationHandler.ActivationArguments] = toastArgs.Argument;
                    await _host.StartAsync();
                });
            };

            // TODO: Register arguments you want to use on App initialization
            var activationArgs = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { ToastNotificationActivationHandler.ActivationArguments, string.Empty }
            };

            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration() // initiate the logger configuration
                .ReadFrom.Configuration(builder.Build()) // connect serilog to our configuration folder
                .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
                .WriteTo.Console() // decide where the logs are going to be shown
                .CreateLogger(); //initialise the logger

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(appLocation);
                        c.AddInMemoryCollection(activationArgs);
                    })
                    .ConfigureServices(ConfigureServices)
                    .UseSerilog()
                    .Build();

            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                // ToastNotificationActivator code will run after this completes and will show a window if necessary.
                return;
            }

            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // TODO WTS: Register your services, viewmodels and pages here

            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Activation Handlers
            services.AddSingleton<IActivationHandler, ToastNotificationActivationHandler>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IPcNetworkListener, PcNetworkListener>();

            // Services
            services.AddSingleton<IToastNotificationsService, ToastNotificationsService>();
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<ISampleDataService, SampleDataService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();

            services.AddTransient<DataGridViewModel>();
            services.AddTransient<DataGridPage>();

            services.AddTransient<BlankViewModel>();
            services.AddTransient<BlankPage>();

            services.AddTransient<ContentGridViewModel>();
            services.AddTransient<ContentGridPage>();

            services.AddTransient<ContentGridDetailViewModel>();
            services.AddTransient<ContentGridDetailPage>();

            services.AddTransient<ListDetailsViewModel>();
            services.AddTransient<ListDetailsPage>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }
}
