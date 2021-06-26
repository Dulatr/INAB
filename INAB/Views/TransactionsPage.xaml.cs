using INAB.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls;


namespace INAB.Views
{
    public sealed partial class TransactionsPage : Page
    {
        public TransactionsPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainVM();
        }
    }
}
