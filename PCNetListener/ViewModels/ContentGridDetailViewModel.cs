using System;
using System.Linq;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using PCNetListener.Contracts.ViewModels;
using PCNetListener.Core.Contracts.Services;
using PCNetListener.Core.Models;

namespace PCNetListener.ViewModels
{
    public class ContentGridDetailViewModel : ObservableObject, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;
        private SampleOrder _item;

        public SampleOrder Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public ContentGridDetailViewModel(ISampleDataService sampleDataService)
        {
            _sampleDataService = sampleDataService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            if (parameter is long orderID)
            {
                var data = await _sampleDataService.GetContentGridDataAsync();
                Item = data.First(i => i.OrderID == orderID);
            }
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
