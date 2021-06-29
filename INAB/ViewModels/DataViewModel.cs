using System.Collections.ObjectModel;
using System.Collections.Generic;

using INAB.Models;

using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace INAB.ViewModels
{
    public class DataViewModel : ObservableObject
    {
        public RelayCommand<int> RequestDeletingAccount { get; set; }
        public RelayCommand Query { get; set; }

        public DataViewModel()
        {
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<Account>>(this, (r,m) => AccountAddRequest(m.Value));
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<Transaction>>(this, (r, m) => TransactionAddRequest(m.Value));

            RequestDeletingAccount = new RelayCommand<int>((id)=>AccountDeleteRequest(id));
            Query = new RelayCommand(() => BuildAndExecuteQuery(ref _query));

            foreach (Account _acct in App.DataServicer.GetAllRows<List<Account>>())
            {
                Accounts.Add(_acct);
                _netValue += _acct.Balance;
            }
            foreach (Transaction _tx in App.DataServicer.GetAllRows<List<Transaction>>())
            {
                Transactions.Add(_tx);
            }
            foreach (Type _type in App.DataServicer.GetAllRows<List<Type>>())
            {
                BudgetItems.Add(new OrbitViewDataItem() { Diameter = 1.0, Label = _type.Identifier, Distance = 0.3, });
            }

            if (Accounts.Count != 0)
            {
                SelectedAccount = Accounts[0];
            }
        }

        public void AccountAddRequest(Account account)
        {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status("Adding account...",StatusType.BUSY)));

            if (App.DataServicer.AccountNameExists(account.Name))
            {
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status("Account name already exists..", StatusType.ERR)));
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Notification>(new Notification("Account name exists, try a different name.")));
                return;
            }

            List<Account> _accountList = new List<Account>();

            App.DataServicer.AddData(account);
            _accountList = App.DataServicer.GetAllRows<List<Account>>();

            UpdateCollection(_accountList);
        }
        
        public void AccountDeleteRequest(int ID)
        {
            var _account = App.DataServicer.GetTableRow<Account>("accounts", "id", $"{ID}");
            
            if (_account == null)
            {
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status($"Failed to remove account #{ID}",StatusType.ERR)));
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Notification>(new Notification($"Could not remove account #{ID}, it was not found.")));
                return;
            }

            App.DataServicer.RemoveData(_account);

            Accounts.Clear();
            foreach(Account _acc in App.DataServicer.GetAllRows<List<Account>>())
            {
                Accounts.Add(_acc);
            }

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status($"Account #{ID} successfully removed!")));
        }

        public void TransactionAddRequest(Transaction transaction)
        {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status("Adding transaction...", StatusType.BUSY)));

            if (App.DataServicer.TransactionIDExists(transaction.ID))
            {
                return;
            }

            transaction.AccountID = SelectedAccount.ID;

            List<Transaction> _transactionList = new List<Transaction>();

            App.DataServicer.AddData(transaction);
            _transactionList = App.DataServicer.GetAllRows<List<Transaction>>();

            UpdateCollection(_transactionList);
        }

        private void UpdateCollection<T>(List<T> _collection)
        {
            var _lastIn = _collection.Count > 0 ? _collection[_collection.Count - 1] : default(T);
            var _type = typeof(T);

            if (_lastIn == null)
            {
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status($"Failed to Add {typeof(T)}...", StatusType.ERR)));
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Notification>(new Notification(
                    $@"Couldn't find {typeof(T)} after trying to add to the database:
                       Try refreshing the page.")));
                return;
            }

            if (_type == typeof(Account))
                Accounts.Add(_lastIn as Account);
            else if (_type == typeof(Transaction))
                Transactions.Add(_lastIn as Transaction);
            else return;

            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Status>(new Status($"{typeof(T)} added successfully!")));
        }

        private void BuildAndExecuteQuery(ref string _searchTerm)
        {
            _searchTerm = "SELECT * FROM transactions ";

            if (SelectedAccountSearch != null)
                AccountIdSearchTerm = SelectedAccountSearch.ID.ToString();

            if (!string.IsNullOrEmpty(AccountIdSearchTerm) && string.IsNullOrEmpty(TxIdSearchTerm))
                _searchTerm += $"WHERE account_id = {AccountIdSearchTerm}";
            if (string.IsNullOrEmpty(AccountIdSearchTerm) && !string.IsNullOrEmpty(TxIdSearchTerm)) 
                _searchTerm += $"WHERE id = {TxIdSearchTerm}";
            if (!string.IsNullOrEmpty(AccountIdSearchTerm) && !string.IsNullOrEmpty(TxIdSearchTerm))
                _searchTerm += $"WHERE account_id = {AccountIdSearchTerm} AND id = {TxIdSearchTerm}";

            QueryTransactions(_searchTerm);
        }

        private void QueryTransactions(string _searchTerm)
        {
            Transactions.Clear();

            foreach (Transaction tx in App.DataServicer.SearchTx(_searchTerm))
            {
                Transactions.Add(tx);
            }
        }

        private string _query;

        private double _netValue;
        public double NetValue
        {
            get
            {
                return _netValue;
            }
            set
            {
                SetProperty(ref _netValue, value);
            }
        }

        private OrbitViewDataItemCollection _budgetItems;
        public OrbitViewDataItemCollection BudgetItems
        {
            get
            {
                if (_budgetItems == null)
                    _budgetItems = new OrbitViewDataItemCollection();
                return _budgetItems;
            }
            set
            {
                SetProperty(ref _budgetItems, value);
            }
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get
            {
                if (_selectedAccount == null)
                {
                    _selectedAccount = new Account();
                }
                return _selectedAccount;
            }
            set
            {
                SetProperty(ref _selectedAccount, value);
            }
        }

        private Account _selectedAccountSearch;
        public Account SelectedAccountSearch
        {
            get
            {
                if (_selectedAccountSearch == null)
                    _selectedAccountSearch = new Account();
                return _selectedAccountSearch;
            }
            set
            {
                SetProperty(ref _selectedAccountSearch, value);
            }
        }

        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get
            {
                if(_accounts == null)
                {
                    _accounts = new ObservableCollection<Account>();
                }
                return _accounts;
            }
            set
            {
                SetProperty(ref _accounts, value);
            }
        }

        private ObservableCollection<Transaction> _transactions;
        public ObservableCollection<Transaction> Transactions
        {
            get
            {
                if (_transactions == null)
                {
                    _transactions = new ObservableCollection<Transaction>();
                }
                return _transactions;
            }
            set
            {
                SetProperty(ref _transactions, value);
            }
        }

        private string _accountIdSearchTerm;
        public string AccountIdSearchTerm
        {
            get { return _accountIdSearchTerm; }
            set { SetProperty(ref _accountIdSearchTerm, value); }
        }

        private string _txIdSearchTerm;
        public string TxIdSearchTerm
        {
            get { return _txIdSearchTerm; }
            set { SetProperty(ref _txIdSearchTerm, value); }
        }
    }
}
