using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

using INAB.ViewModels;
using INAB.Models;


namespace INAB.Views
{
    public sealed partial class HomePage : Page
    {

        public HomePage()
        {
            this.InitializeComponent();
            this.DataContext = new MainVM();
        }
    }
}
