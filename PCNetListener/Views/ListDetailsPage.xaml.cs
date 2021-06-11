using System.Windows.Controls;

using PCNetListener.ViewModels;

namespace PCNetListener.Views
{
    public partial class ListDetailsPage : Page
    {
        public ListDetailsPage(ListDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
