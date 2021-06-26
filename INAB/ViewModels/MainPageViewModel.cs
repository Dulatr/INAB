using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

using INAB.Models;
using Windows.UI.Xaml.Controls;

namespace INAB.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        public RelayCommand RequestAddingAccount { get; set; }
        public RelayCommand RequestAddingTransaction { get; set; }

        public MainPageViewModel()
        {
            StatusText = "Welcome!";
            StatusIcon = new SymbolIcon(Symbol.Play);

            WeakReferenceMessenger.Default.Register<ValueChangedMessage<Status>>(this,(r,m) => 
            {
                StatusChange(m.Value.Message,m.Value.Type);
            });
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<bool>>(this, (r,m) =>
            {
                _isBusy = m.Value;
            });

            RequestAddingAccount = new RelayCommand(AccountAddRequest);
            RequestAddingTransaction = new RelayCommand(TransactionAddRequest);

        }

        public void StatusChange(string _status, StatusType type = 0)
        {
            StatusText = _status;

            if (type == StatusType.ERR)
                StatusIcon = new SymbolIcon(Symbol.Repair);
            else if (type == StatusType.BUSY)
                StatusIcon = new SymbolIcon(Symbol.Clock);
            else
                StatusIcon = new SymbolIcon(Symbol.Play);
        }

        public void AccountAddRequest()
        {
            bool _isValid = double.TryParse(AccountValueField, out double _requestedAmount);

            if (!_isValid)
            {
                StatusChange($"Add Account Failure: {AccountNameField}",StatusType.ERR);
                AccountValueField = "0.00";
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Notification>(new Notification($@"Failed to add '{AccountNameField}' reason: Balance is not a number.")));
                return;
            }

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Account>(new Account()
            {
                Name = AccountNameField != null ? AccountNameField : "",
                Balance = _requestedAmount
            }));
        }

        public void TransactionAddRequest()
        {
            bool _isValid = double.TryParse(TransactionValueField, out double _requestedAmount);

            if (!_isValid)
            {
                StatusChange($"Add Transaction Failure: {TransactionValueField}");
                TransactionValueField = "0.00";
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Notification>(new Notification($"Failed to add the transaction reason: Amount is not a number.")));
                return;
            }

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Transaction>(new Transaction
            {
                Amount = _requestedAmount,
            }));
        }

        private string _statusText;
        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                SetProperty(ref _statusText, value);
            }
        }

        private SymbolIcon _statusIcon;
        public SymbolIcon StatusIcon
        {
            get
            {
                return _statusIcon;
            }
            set
            {
                SetProperty(ref _statusIcon, value);
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }

        private string _notificationMsg;
        public string NotificationMsg
        {
            get { return _notificationMsg; }
            set
            {
                SetProperty(ref _notificationMsg, value);
            }
        }

        private string _accountNameField;
        public string AccountNameField
        {
            get
            {
                return _accountNameField;
            }
            set
            {
                SetProperty(ref _accountNameField, value);
            }
        }

        private string _accountValueField;
        public string AccountValueField
        {
            get
            {
                return _accountValueField;
            }
            set
            {
                SetProperty(ref _accountValueField, value);
            }
        }

        private System.DateTime _txDate;
        public System.DateTime TxDate
        {
            get
            {
                if (_txDate == null)
                    _txDate = new System.DateTime();
                return _txDate;
            }
            set
            {
                SetProperty(ref _txDate, value);
            }
        }

        private string _transactionValueField;
        public string TransactionValueField
        {
            get
            {
                return _transactionValueField;
            }
            set
            {
                SetProperty(ref _transactionValueField, value);
            }
        }

        private string _transactionTypeField;
        public string TransactionTypeField
        {
            get
            {
                return _transactionTypeField;
            }
            set
            {
                SetProperty(ref _transactionTypeField, value);
            }
        }
    }
}
