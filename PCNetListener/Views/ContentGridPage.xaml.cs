using System.Windows.Controls;

using PCNetListener.ViewModels;

namespace PCNetListener.Views
{
    public partial class ContentGridPage : Page
    {
        public ContentGridPage(ContentGridViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
