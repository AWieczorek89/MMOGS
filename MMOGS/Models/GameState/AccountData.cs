using MMOGS.DataHandlers;
using MMOGS.Interfaces;
using MMOGS.Models.Database;
using System;
using System.Collections.Generic;

namespace MMOGS.Models.GameState
{
    public class AccountData : IDisposable
    {
        public bool IsLoaded { get; private set; } = false;
        public bool LoadingError { get; private set; } = false;

        private ILogger _logger = null;
        private List<DbAccountsData> _accountList = new List<DbAccountsData>();
        
        public AccountData(ILogger logger)
        {
            _logger = logger ?? throw new Exception("Account data - logger cannot be NULL!");
        }

        public bool AccountValidation(string login, string pass, DbAccountsData.PasswordType passType)
        {
            bool valid = false;
            if (String.IsNullOrWhiteSpace(login))
                return valid;

            foreach (DbAccountsData account in _accountList)
            {
                if (account.Login.Equals(login, GlobalData.InputDataStringComparison))
                {
                    if (passType == DbAccountsData.PasswordType.Decrypted)
                    {
                        valid = (account.PassDecrypted.Equals(pass, GlobalData.InputDataStringComparison));
                    }
                    else
                    if (passType == DbAccountsData.PasswordType.Encrypted)
                    {
                        valid = (account.PassEncrypted.Equals(pass, GlobalData.InputDataStringComparison));
                    }

                    break;
                }
            }

            return valid;
        }

        public void ShowAccountsInLog()
        {
            foreach (DbAccountsData acc in _accountList)
                _logger.UpdateLog($"====Account: ID [{acc.AccId}] login [{acc.Login}] pass [{acc.PassDecrypted}][{acc.PassEncrypted}] lv [{acc.AccessLevel}]");
        }

        public DbAccountsData GetAccountData(string login)
        {
            DbAccountsData accountData = null;
            if (String.IsNullOrWhiteSpace(login))
                return accountData;

            foreach (DbAccountsData data in _accountList)
            {
                if (data.Login.Equals(login, GlobalData.InputDataStringComparison))
                {
                    accountData = data;
                    break;
                }
            }

            return accountData;
        }

        public void AddAccountData(DbAccountsData accData)
        {
            if (String.IsNullOrWhiteSpace(accData.Login)) throw new Exception("Account data - login cannot be empty!");
            if (accData.AccId < 0) throw new Exception("Account data - acc_id cannot be less than 0!");

            foreach (DbAccountsData data in _accountList)
            {
                if (data.Login.Equals(accData.Login, GlobalData.InputDataStringComparison))
                    throw new Exception($"An account with login [{accData.Login}] already exists!");

                if (data.AccId == accData.AccId)
                    throw new Exception($"An account with acc_id [{accData.AccId}] already exists!");
            }

            _accountList.Add(accData);
            _logger.UpdateLog($"Account data - added new account, login [{accData.Login}] level [{accData.AccessLevel}]");
        }

        public async void LoadAccountsAsync(MySqlDbManager dbManager)
        {
            this.IsLoaded = false;
            this.LoadingError = false;
            
            _accountList.Clear();
            _logger.UpdateLog("Loading accounts data...");

            try
            {
                if (dbManager.GetConnectionState() != System.Data.ConnectionState.Open)
                    throw new Exception("connection not open!");

                using (BoxedData accBoxedData = await dbManager.GetAccountsDataTaskStart())
                {
                    _accountList = (List<DbAccountsData>)accBoxedData.Data;
                    if (!String.IsNullOrEmpty(accBoxedData.Msg))
                        _logger.UpdateLog(accBoxedData.Msg);
                }

                this.IsLoaded = true;
            }
            catch (Exception exception)
            {
                this.LoadingError = true;
                _logger.UpdateLog($"An error occured during account data loading: {exception.Message}");
            }

            if (this.IsLoaded)
                _logger.UpdateLog($"Loaded [{_accountList.Count}] accounts");
            
            _logger.UpdateLog($"Loading account data ended with {(this.IsLoaded ? "success" : "failure")}.");
        }

        public void Dispose()
        {
            _accountList.Clear();
        }
    }
}
