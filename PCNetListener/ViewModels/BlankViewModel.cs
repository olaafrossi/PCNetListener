// Created by Three Byte Intermedia, Inc.
// project: PCNetListener
// Created: 2021 06 11
// by Olaaf Rossi

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PCNetListener.Contracts.Services;
using PCNetListener.Contracts.ViewModels;
using PCNetListener.Core.Models;
using PCNetListener.Core.Services;

namespace PCNetListener.ViewModels
{
    public class BlankViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly IPcNetworkListener _pcNet;
        private readonly IToastNotificationsService _toastNotificationsService;

        public BlankViewModel(IPcNetworkListener pcNet, INavigationService navigationService, IToastNotificationsService toastNotificationsService)
        {
            _pcNet = pcNet;
            _navigationService = navigationService;
            _toastNotificationsService = toastNotificationsService;
            pcNet.MessageHit += PcNet_MessageHit;
        }

        public async void OnNavigatedTo(object parameter)
        {
            var data = _pcNet.GetAppSettingsDataUdpPort();
            Console.WriteLine(data);
            Console.WriteLine("hello from the view model");
            _toastNotificationsService.ShowToastNotificationSample();
        }

        public void OnNavigatedFrom()
        {
            //throw new NotImplementedException();
        }

        private void PcNet_MessageHit(object sender, NetworkMessagesEventArgs e)
        {
            Console.WriteLine($"Message Hit {e}");
        }
    }
}