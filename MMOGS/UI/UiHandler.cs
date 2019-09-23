using MMOGS.DataHandlers;
using MMOGS.Encryption;
using MMOGS.Interfaces;
using MMOGS.Models;
using MMOGS.Models.Database;
using MMOGS.Performance;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TcpConnector;

namespace MMOGS.UI
{
    public class UiHandler : IDisposable, ITcpLogger, ILogger
    {
        private static UiHandler _instance = null;

        private UiReferenceContainer _uiRefContainer = null;
        private int _logRowLimit = 1000;

        /// <summary>
        /// Gets singleton instance of UiHandler. Argument must be given in the first call.
        /// </summary>
        public static UiHandler GetInstance(UiReferenceContainer uiRefContainer = null)
        {
            if (uiRefContainer == null && UiHandler._instance == null)
                throw new Exception("UI handler - UI reference container cannot be NULL while object instance is not initialized!");

            if (UiHandler._instance == null)
                UiHandler._instance = new UiHandler(uiRefContainer);

            return UiHandler._instance;
        }
        
        private UiHandler(UiReferenceContainer uiRefContainer)
        {
            _uiRefContainer = uiRefContainer;

            //MEMORY USAGE INFO HANDLING
            PerformanceAnalyzer.HandleMemoryUsageInfo memUsageMethodDelegate = new PerformanceAnalyzer.HandleMemoryUsageInfo(HandleMemoryUsageInfo);
            PerformanceAnalyzer.BeginAnalyzing(memUsageMethodDelegate);
        }
        
        /// <summary>
        /// A method which refreshes information about performance (RAM, CPU usage)
        /// </summary>
        private void HandleMemoryUsageInfo(string cpuInfo, string ramInfo)
        {
            _uiRefContainer.CpuUsageTextBox.Text = cpuInfo;
            _uiRefContainer.RamUsageTextBox.Text = ramInfo;
        }

        /// <summary>
        /// Changes tabs on main tab control by name of tab
        /// </summary>
        public void ShowTab(string tabName)
        {
            for (int i = 0; i < _uiRefContainer.MainTabControl.TabPages.Count; i++)
            {
                if (_uiRefContainer.MainTabControl.TabPages[i].Name.ToLower() == tabName.ToLower())
                {
                    _uiRefContainer.MainTabControl.SelectedIndex = i;
                    break;
                }
            }
        }

        #region Tools section

        public void RunMySqlDataInstaller()
        {
            bool createTables = _uiRefContainer.MySqlInstallerCreateTablesCheckBox.Checked;
            bool createBasicData = _uiRefContainer.MySqlInstallerCreateDataCheckBox.Checked;

            if (!createTables && !createBasicData)
                return;

            if (MessageBox.Show("Are you sure?", "mySQL data", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            ShowTab("overviewTabPage");
            MySqlConnectionSettings connSettings = GetMySqlSettings();
            MySqlDataCreationTool dbCreationTool = new MySqlDataCreationTool(this, connSettings);
            dbCreationTool.CreateMySqlDataAsync(createTables, createBasicData);
        }
        
        //public async void CreateNewAccountAsync()
        //{
        //    _uiRefContainer.NewAccountButton.Enabled = false;

        //    try
        //    {
        //        if (MessageBox.Show("Are you sure?", "new account", MessageBoxButtons.YesNo) != DialogResult.Yes)
        //            return;

        //        DbAccountsData dbAccData = GetNewAccountData();

        //        if (String.IsNullOrWhiteSpace(dbAccData.Login))
        //        {
        //            MessageBox.Show("Login cannot be empty!");
        //            return;
        //        }

        //        MySqlConnectionSettings connSettings = GetMySqlSettings();
        //        bool success = false;

        //        using (MySqlDbManager dbManager = new MySqlDbManager(connSettings))
        //        {
        //            BoxedData connStartData = await dbManager.StartConnectionTaskStart();
        //            bool connStartSuccess = (bool)connStartData.Data;
        //            if (!String.IsNullOrEmpty(connStartData.Msg))
        //                UpdateLog(connStartData.Msg);

        //            if (connStartSuccess)
        //            {
        //                BoxedData accCreationData = await dbManager.AddAccountsDataTaskStart(dbAccData);
        //                int accId = (int)accCreationData.Data;

        //                if (!String.IsNullOrEmpty(accCreationData.Msg))
        //                    UpdateLog(accCreationData.Msg);

        //                if (accId > -1)
        //                {
        //                    UpdateLog($"Created new account, acc_id [{accId}]!");
        //                    success = true;
        //                }
        //            }
        //            else
        //            {
        //                UpdateLog("Cannot connect to database!");
        //            }
        //        }

        //        if (success)
        //        {
        //            ClearNewAccountForm();
        //            MessageBox.Show("Account created successfully!");
        //        }

        //        ShowTab("overviewTabPage");
        //    }
        //    finally
        //    {
        //        _uiRefContainer.NewAccountButton.Enabled = true;
        //    }
        //}
        
        public void ClearNewAccountForm()
        {
            _uiRefContainer.NewAccountLoginTextBox.Text = "";
            _uiRefContainer.NewAccountPassTextBox.Text = "";
            _uiRefContainer.NewAccountAccessLevelNumericUpDown.Value = 0;
        }

        public DbAccountsData GetNewAccountData()
        {
            return new DbAccountsData
            (
                -1,
                _uiRefContainer.NewAccountLoginTextBox.Text,
                _uiRefContainer.NewAccountPassTextBox.Text,
                DbAccountsData.PasswordType.Decrypted,
                Convert.ToInt32(_uiRefContainer.NewAccountAccessLevelNumericUpDown.Value)
            );
        }

        #endregion

        #region Settings management section
        
        public TcpConnectionSettings GetTcpConnectionSettings()
        {
            return new TcpConnectionSettings
            (
                _uiRefContainer.TcpIpTextBox.Text,
                Convert.ToInt32(_uiRefContainer.TcpPortNumericUpDown.Value)
            );
        }

        public MySqlConnectionSettings GetMySqlSettings()
        {
            return new MySqlConnectionSettings
            (
                _uiRefContainer.MySqlServerTextBox.Text,
                Convert.ToInt32(_uiRefContainer.MySqlPortNumericUpDown.Value),
                _uiRefContainer.MySqlDatabaseTextBox.Text,
                _uiRefContainer.MySqlUserTextBox.Text,
                _uiRefContainer.MySqlPassTextBox.Text
            );
        }
        
        public void SetTcpConnectionSettings(string ip, int port)
        {
            _uiRefContainer.TcpIpTextBox.Text = (!String.IsNullOrWhiteSpace(ip) ? ip : "127.0.0.1");
            _uiRefContainer.TcpPortNumericUpDown.Value = (port > 0 ? port : 2222);
        }

        public void SetMySqlSettings
        (
            string server,
            int port,
            string database,
            string user,
            string pass
        )
        {
            _uiRefContainer.MySqlServerTextBox.Text = server;
            _uiRefContainer.MySqlPortNumericUpDown.Value = (port > 0 ? port : 3306);
            _uiRefContainer.MySqlDatabaseTextBox.Text = database;
            _uiRefContainer.MySqlUserTextBox.Text = user;
            _uiRefContainer.MySqlPassTextBox.Text = pass;
        }

        #endregion

        #region Log section

        public void CopyLogTextToClipboard()
        {
            try
            {
                string text = "";
                _uiRefContainer.LogListBox.SelectionMode = SelectionMode.MultiSimple;

                for (int i = 0; i < _uiRefContainer.LogListBox.Items.Count; i++)
                {
                    _uiRefContainer.LogListBox.SetSelected(i, true);
                    text += $"{_uiRefContainer.LogListBox.Items[i].ToString()}{Environment.NewLine}";
                }

                if (String.IsNullOrEmpty(text))
                {
                    MessageBox.Show("Log text is empty!");
                    return;
                }

                Clipboard.SetText(text);
                MessageBox.Show("Log text copied to clipboard!");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Cannot copy log text to clipboard: {exception.Message}");
            }
        }

        public void UpdateLog(string logTxt)
        {
            _uiRefContainer.LogListBox.Items.Add(logTxt);
            LimitLogRows();
            _uiRefContainer.LogListBox.SelectedIndex = _uiRefContainer.LogListBox.Items.Count - 1;
            _uiRefContainer.LogListBox.SelectedIndex = -1;
        }

        public void UpdateLog(List<string> logTxtList)
        {
            foreach (string logTxt in logTxtList)
                _uiRefContainer.LogListBox.Items.Add(logTxt);

            LimitLogRows();
            _uiRefContainer.LogListBox.SelectedIndex = _uiRefContainer.LogListBox.Items.Count - 1;
            _uiRefContainer.LogListBox.SelectedIndex = -1;
        }

        public void ClearLog()
        {
            _uiRefContainer.LogListBox.Items.Clear();
        }

        private void LimitLogRows()
        {
            if (_uiRefContainer.LogListBox.Items.Count <= _logRowLimit || _logRowLimit < 1)
                return;

            for (int i = 0; i < _uiRefContainer.LogListBox.Items.Count - _logRowLimit; i++)
                _uiRefContainer.LogListBox.Items.RemoveAt(i);
        }

        #endregion

        #region Loading/saving section
        
        /// <summary>
        /// Loads local settings from configuration file
        /// </summary>
        public void LoadLocalSettings()
        {
            try
            {
                FileDataManager fdm = new FileDataManager(this);

                #region MySQL settings
                //MYSQL SETTINGS
                string server = fdm.GetValueByKey("mysql_server");
                int port = 3306;
                Int32.TryParse(fdm.GetValueByKey("mysql_port"), out port);
                string database = fdm.GetValueByKey("mysql_database");
                string user = fdm.GetValueByKey("mysql_user");
                string passEncrypted = fdm.GetValueByKey("mysql_pass");
                string passDecrypted = 
                (
                    !String.IsNullOrWhiteSpace(passEncrypted) ?
                    Crypto.DecryptStringAES(passEncrypted, GlobalData.PassKey) :
                    ""
                );

                SetMySqlSettings
                (
                    server,
                    port,
                    database,
                    user,
                    passDecrypted
                );

                #endregion

                #region TCP connection settings
                //TCP CONN. SETTINGS
                string tcpIp = fdm.GetValueByKey("tcp_ip");
                int tcpPort = 2222;
                Int32.TryParse(fdm.GetValueByKey("tcp_port"), out tcpPort);

                SetTcpConnectionSettings(tcpIp, tcpPort);

                #endregion
            }
            catch (Exception exception)
            {
                UpdateLog($"An error occured during mySQL settings loading: {exception.Message}");
            }
        }
        
        public void SaveTcpConnectionSettings()
        {
            TcpConnectionSettings settings = GetTcpConnectionSettings();
            SaveTcpConnectionSettings(settings);
        }

        public void SaveMySqlSettings()
        {
            MySqlConnectionSettings settings = GetMySqlSettings();
            SaveMySqlSettings(settings);
        }
        
        private void SaveTcpConnectionSettings(TcpConnectionSettings settings)
        {
            FileDataManager fdm = new FileDataManager(this);
            fdm.InsertOrUpdateValueByKey("tcp_ip", settings.Ip);
            fdm.InsertOrUpdateValueByKey("tcp_port", settings.Port.ToString());
            fdm.SaveData();
        }

        private void SaveMySqlSettings(MySqlConnectionSettings mySqlConnSettings)
        {
            FileDataManager fdm = new FileDataManager(this);
            fdm.InsertOrUpdateValueByKey("mysql_server", mySqlConnSettings.Server);
            fdm.InsertOrUpdateValueByKey("mysql_port", mySqlConnSettings.Port.ToString());
            fdm.InsertOrUpdateValueByKey("mysql_database", mySqlConnSettings.Database);
            fdm.InsertOrUpdateValueByKey("mysql_user", mySqlConnSettings.User);
            fdm.InsertOrUpdateValueByKey
            (
                "mysql_pass",
                (
                    !String.IsNullOrWhiteSpace(mySqlConnSettings.Pass) ?
                    Crypto.EncryptStringAES(mySqlConnSettings.Pass, GlobalData.PassKey) :
                    ""
                )
            );

            fdm.SaveData();
        }

        #endregion

        public void Dispose()
        {
            PerformanceAnalyzer.StopAnalyzing();
        }
        
    }
}
