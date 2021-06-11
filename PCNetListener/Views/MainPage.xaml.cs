using System.Windows.Controls;

using PCNetListener.ViewModels;

namespace PCNetListener.Views
{
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
