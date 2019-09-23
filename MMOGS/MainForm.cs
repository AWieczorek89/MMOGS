using MMOGS.DataHandlers;
using MMOGS.Models;
using MMOGS.Models.Database;
using MMOGS.Models.GameState;
using MMOGS.UI;
using System;
using System.Windows.Forms;
using TcpConnector;

namespace MMOGS
{
    public partial class MainForm : Form
    {
        private UiHandler _uiHandler;
        private GameManager _gameManager;

        public MainForm()
        {
            InitializeComponent();
            InitializeHandlers();
            LoadLocalSettings();
        }

        private void LoadLocalSettings()
        {
            _uiHandler.UpdateLog("Loading local settings...");
            _uiHandler.LoadLocalSettings();
            _uiHandler.UpdateLog("Loading complete!");
        }

        private void InitializeHandlers()
        {
            try
            {
                UiReferenceContainer uiRefContainer = new UiReferenceContainer
                (
                    perfCpuTextBox,
                    perfRamTextBox,
                    logListBox,
                    mySqlServerTextBox,
                    mySqlPortNmrUpDown,
                    mySqlBaseTextBox,
                    mySqlUserTextBox,
                    mySqlPassTextBox,
                    mainTabControl,
                    mySqlInstallerCreateTablesCheckBox,
                    mySqlInstallerCreateDataCheckBox,
                    newAccLoginTextBox,
                    newAccPassTextBox,
                    newAccAccessLevelNmrUpDown,
                    newAccButton,
                    tcpIpTextBox,
                    tcpPortNumericUpDown
                );

                _uiHandler = UiHandler.GetInstance(uiRefContainer);
                _gameManager = GameManager.GetInstance(_uiHandler, _uiHandler, _uiHandler);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"A method InitializeHandlers() throws an exception: {exception.Message}");
            }
        }

        private async void CheckMySqlConnectionAsync()
        {
            _uiHandler.ShowTab("overviewTabPage");
            _uiHandler.UpdateLog("Checking mySQL connection...");
            MySqlConnectionSettings myConnSettings = _uiHandler.GetMySqlSettings();
            
            using (MySqlDbManager dbManager = new MySqlDbManager(myConnSettings))
            {
                _uiHandler.UpdateLog("Estabilishing mySQL connection...");
                BoxedData startConnData = await dbManager.StartConnectionTaskStart();
                bool startConnSuccess = (bool)startConnData.Data;
                _uiHandler.UpdateLog(startConnSuccess ? "Connection started!" : "Connection starting failed!");
                if (!String.IsNullOrEmpty(startConnData.Msg)) _uiHandler.UpdateLog(startConnData.Msg);

                BoxedData checkConnData = await dbManager.CheckConnectionTaskStart();
                bool checkConnSuccess = (bool)checkConnData.Data;
                _uiHandler.UpdateLog(checkConnSuccess ? "Connection successfully estabilished!" : "Connection not estabilished!");
                if (!String.IsNullOrEmpty(checkConnData.Msg)) _uiHandler.UpdateLog(checkConnData.Msg);

                BoxedData closeConnData = await dbManager.CloseConnectionTaskStart();
                bool closeConnSuccess = (bool)closeConnData.Data;
                _uiHandler.UpdateLog(closeConnSuccess ? "Connection closed!" : "Cannot close current connection!");
                if (!String.IsNullOrEmpty(closeConnData.Msg)) _uiHandler.UpdateLog(closeConnData.Msg);
            }
        }

        #region Events

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                TcpServer.StopServer();

                if (_uiHandler != null)
                    _uiHandler.Dispose();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"FormClosing() throws an exception: {exception.Message}");
            }
        }

        private void mySqlConnSaveButton_Click(object sender, EventArgs e)
        {
            _uiHandler.SaveMySqlSettings();
            MessageBox.Show("Done!");
        }

        private void tcpConnSaveButton_Click(object sender, EventArgs e)
        {
            _uiHandler.SaveTcpConnectionSettings();
            MessageBox.Show("Done!");
        }

        private void mySqlConnCheckButton_Click(object sender, EventArgs e)
        {
            CheckMySqlConnectionAsync();
        }

        private void logListBox_DoubleClick(object sender, EventArgs e)
        {
            _uiHandler.CopyLogTextToClipboard();
        }

        private void mySqlInstallerButton_Click(object sender, EventArgs e)
        {
            _uiHandler.RunMySqlDataInstaller();
        }

        private void newAccButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "new account", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                _uiHandler.ShowTab("overviewTabPage");
                DbAccountsData dbAccData = _uiHandler.GetNewAccountData();
                MySqlConnectionSettings connSettings = _uiHandler.GetMySqlSettings();
                _gameManager.CreateNewAccountAsync(dbAccData, connSettings);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Cannot create new account: {exception.Message}");
            }
        }

        private void startServerButton_Click(object sender, EventArgs e)
        {
            MySqlConnectionSettings myConnSettings = _uiHandler.GetMySqlSettings();
            _gameManager.StartAsync(myConnSettings);
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            
        }

        #endregion
        
    }
}
