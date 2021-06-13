using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PCNetListener.Core.Services;
using Serilog.Core;

namespace PCNetListener.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            //Ioc.Default.GetService<IPcNetworkListener>().GetAppSettingsDataUdpPort();
            //Console.WriteLine("Hello");
           // Console.WriteLine($"{Ioc.Default.GetService<PcNetworkListener>().GetAppSettingsDataUdpPort()}");
        }
    }
}
