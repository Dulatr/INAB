using Windows.UI.Xaml.Controls;

using INAB.ViewModels;
using INAB.Views;
using INAB.Models;

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace INAB
{ 
    public sealed partial class MainPage : Page
    {
        public RelayCommand ShowAccountFlyout { get; set; }
        public RelayCommand ShowTransactionFlyout { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainVM();

            NavView.ItemInvoked += NavView_ItemInvoked;

            Display.Navigate(ReturnPage("Home"));
            (Display.Content as Page).DataContext = ViewModel.DataVM;

            ShowAccountFlyout = new RelayCommand(() =>
            {
                NewAccountFlyoutButton.Flyout.ShowAt(NewAccountFlyoutButton);
            });
            ShowTransactionFlyout = new RelayCommand(() =>
            {
                NewTransactionFlyoutButton.Flyout.ShowAt(NewTransactionFlyoutButton);
            });

            WeakReferenceMessenger.Default.Register<ValueChangedMessage<Notification>>(this, (r,m) =>
            {
                NotifyBar.Show(m.Value.Message);
            });
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Display.Navigate(ReturnPage(args.InvokedItem as string));

            if ((args.InvokedItem as string) == "Home" || (args.InvokedItem as string) == "Transactions" || (args.InvokedItem as string) == "Budget")
                (Display.Content as Page).DataContext = ViewModel.DataVM;
        }

        private System.Type ReturnPage(string _pageName)
        {
            switch (_pageName)
            {
                case "Home":
                    NavView.Header = "Home";
                    NewTransactionFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    NewAccountFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    return typeof(HomePage);
                case "Transactions":
                    NavView.Header = "Transactions";
                    NewTransactionFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    NewAccountFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    return typeof(TransactionsPage);
                case "Budget":
                    NavView.Header = "Budget";
                    NewTransactionFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    NewAccountFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    return typeof(BudgetPage);
                default:
                    NavView.Header = "Not Home";
                    NewAccountFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    NewTransactionFlyoutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    return typeof(Page);
            }
        }

        private MainVM ViewModel
        {
            get { return DataContext as MainVM; }
        }
    }
}
