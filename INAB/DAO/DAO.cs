using System.IO;

using SQLite;

using INAB.Models;
using System.Collections.Generic;

namespace INAB.DAO
{
    public class DataServicer
    {
        private SQLiteConnection _db;
        private SQLiteConnection DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new SQLiteConnection(Path.Combine(App.AppDataPath,App.DB_Name));
                }
                return _db;
            }
        }

        public void InitializeTables()
        {
            if (!TableExists("accounts"))
                DB.CreateTable<Account>();

            if (!TableExists("transactions"))
                DB.CreateTable<Transaction>();

            if (!TableExists("types"))
                DB.CreateTable<Type>();
               
        }

        public void AddData<T>(T _object)
        {
            if (!IsTable(_object))
            {
                return;
            }

            DB.Insert(_object);
        }

        public void RemoveData<T>(T _object)
        {
            if (!IsTable(_object))
            {
                return;
            }

            DB.Delete(_object);
            
        }

        public T GetTableRow<T>(string _table, string _field = null,string _value = null, int _index = 0)
        {
            var _type = typeof(T);
            string query = $@"SELECT * FROM {_table}";
            object result;

            if (_field != null && _value != null)
                query = $@"SELECT * FROM {_table} WHERE {_field} = '{_value}'";
            
            if (_index < 0)
            {
                return default(T);
            }
            try
            {
                if (_type == typeof(Account))
                    result = DB.Query<Account>(query)[_index];
                else if (_type == typeof(Transaction))
                    result = DB.Query<Transaction>(query)[_index];
                else if (_type == typeof(Type))
                    result = DB.Query<Type>(query)[_index];
                else
                    return default(T);
            }
            catch
            {
                return default(T);
            }

            return (T)result;
        }

        public T GetAllRows<T>()
        {
            var _type = typeof(T);
            string query = $@"SELECT * FROM ";
            object result;

            if (_type == typeof(List<Account>))
            {
                query += "accounts";
                result = DB.Query<Account>(query);
            }
            else if (_type == typeof(List<Transaction>))
            {
                query += "transactions";
                result = DB.Query<Transaction>(query);
            }
            else if (_type == typeof(List<Type>))
            {
                query += "types";
                result = DB.Query<Type>(query);
            }
            else
                return default(T);

            return (T)result;
        }

        public List<Transaction> SearchTx(string _searchTerm)
        {
            return DB.Query<Transaction>(_searchTerm);
        }

        private bool TableExists(string _name)
        {
            string query = $@"SELECT * FROM sqlite_master WHERE type='table' AND name='{_name}'";

            if (_name == "accounts" || _name == "transactions" || _name == "types")
            {
                return DB.Query<Account>(query).Count >= 1;
            }

            return false;

        }

        public bool AccountNameExists(string _name)
        {
            var _accounts = GetAllRows<List<Account>>();
            return _accounts.Exists((x) => x.Name == _name);
        }

        public bool TransactionIDExists(int ID)
        {
            var _transactions = GetAllRows<List<Transaction>>();
            return _transactions.Exists((x) => x.ID == ID);
        }

        private bool IsTable<T>(T _object)
        {
            if (!(_object is Account) && !(_object is Transaction) && !(_object is Type))
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            DB.Close();
        }
    }
}
