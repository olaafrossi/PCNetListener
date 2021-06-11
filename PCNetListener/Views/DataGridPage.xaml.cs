using System.Windows.Controls;

using PCNetListener.ViewModels;

namespace PCNetListener.Views
{
    public partial class DataGridPage : Page
    {
        public DataGridPage(DataGridViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
