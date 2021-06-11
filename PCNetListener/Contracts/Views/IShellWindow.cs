using System.Windows.Controls;

namespace PCNetListener.Contracts.Views
{
    public interface IShellWindow
    {
        Frame GetNavigationFrame();

        void ShowWindow();

        void CloseWindow();
    }
}
