using MMOGS.DataHandlers.CommandHandling;
using MMOGS.Interfaces;
using MMOGS.Measurement.Units;
using MMOGS.Models;
using MMOGS.Models.Database;
using MMOGS.Models.GameState;
using MMOGS.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector;

namespace MMOGS.DataHandlers
{
    public class GameManager
    {
        public enum GameState
        {
            Inactive,
            Active,
            Error
        }

        public static GameManager _instance = null;

        public GameState State { get; private set; } = GameState.Inactive;

        private AccountData _accountData = null;
        private GameWorldData _gameWorldData = null;
        private ILogger _logger = null;
        private ITcpLogger _tcpLogger = null;
        private UiHandler _uiHandler = null;
        private MySqlConnectionSettings _mySqlConnSettings = null;
        private GameStateHandler _gameStateHandler = null;
        private TcpConnectionHandler _tcpConnectionHandler = null;
        private CommandHandler _commandHandler = null;
        private PlayerHandler _playerHandler = null;
        private RegularOperationHandler _regularOperationHandler = null;
        private ChatHandler _chatHandler = null;

        private bool _loadingInProgress = false;

        /// <summary>
        /// Gets singleton instance of game manager
        /// </summary>
        public static GameManager GetInstance(ILogger logger, ITcpLogger tcpLogger, UiHandler uiHandler)
        {
            if (GameManager._instance == null)
                GameManager._instance = new GameManager();

            GameManager._instance._logger = logger;
            GameManager._instance._tcpLogger = tcpLogger;
            GameManager._instance._uiHandler = uiHandler;
            return GameManager._instance;
        }

        private GameManager()
        {
        }

        public async void Test()
        {
            try
            {
                _logger.UpdateLog("getting geo data...");
                BoxedData bxData = await _gameWorldData.GetLocalGeoDataTaskStart(1, -1, new Point3<int>(2, 2, 3), new Point3<int>(10, 10, 10));

                List<GeoDataElement> geoDataList = (List<GeoDataElement>)bxData.Data;
                if (!String.IsNullOrEmpty(bxData.Msg)) _logger.UpdateLog(bxData.Msg);

                foreach (GeoDataElement geoElement in geoDataList)
                {
                    _logger.UpdateLog
                    (
                        $"loc [{geoElement.Location.X}, {geoElement.Location.Y}, {geoElement.Location.Z}] " + 
                        $"collision [{geoElement.ColliderSize.X}, {geoElement.ColliderSize.Y}, {geoElement.ColliderSize.Z}] " + 
                        $"type [{geoElement.ElementType.ToString()}]"
                    );
                }

                _accountData.ShowAccountsInLog();
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"(TEST) Error: {exception.Message} | {exception.StackTrace}");
            }
        }

        public async void CreateNewAccountAsync(DbAccountsData dbAccData, MySqlConnectionSettings connSettings)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dbAccData.Login))
                {
                    _logger.UpdateLog("Login cannot be empty!");
                    return;
                }
                
                bool success = false;

                using (MySqlDbManager dbManager = new MySqlDbManager(connSettings))
                {
                    BoxedData connStartData = await dbManager.StartConnectionTaskStart();
                    bool connStartSuccess = (bool)connStartData.Data;
                    if (!String.IsNullOrEmpty(connStartData.Msg))
                        _logger.UpdateLog(connStartData.Msg);

                    if (connStartSuccess)
                    {
                        BoxedData accCreationData = await dbManager.AddAccountsDataTaskStart(dbAccData);
                        int accId = (int)accCreationData.Data;

                        if (!String.IsNullOrEmpty(accCreationData.Msg))
                            _logger.UpdateLog(accCreationData.Msg);

                        if (accId > -1)
                        {
                            _logger.UpdateLog($"Created new account, acc_id [{accId}]!");

                            if (_accountData != null)
                            {
                                _accountData.AddAccountData
                                (
                                    new DbAccountsData
                                    (
                                        accId,
                                        dbAccData.Login,
                                        dbAccData.PassEncrypted,
                                        DbAccountsData.PasswordType.Encrypted,
                                        dbAccData.AccessLevel
                                    )
                                );
                            }

                            success = true;
                        }
                    }
                    else
                    {
                        _logger.UpdateLog("Cannot connect to database!");
                    }
                }

                if (success)
                {
                    _logger.UpdateLog("Account created successfully!");
                }
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Account creation failure: {exception.Message}");
            }
        }

        /// <summary>
        /// Starts the game and its management/handlers etc.
        /// </summary>
        public async void StartAsync(MySqlConnectionSettings mySqlConnSettings)
        {
            if (_loadingInProgress)
            {
                _logger.UpdateLog("Game loading in progress!");
                return;
            }

            _loadingInProgress = true;

            try
            {
                if (this.State == GameState.Active)
                {
                    _logger.UpdateLog("The game is already active!");
                    return;
                }

                _mySqlConnSettings = mySqlConnSettings;
                int counter;

                try
                {
                    //GAME DATA
                    using (MySqlDbManager gwdDbManager = new MySqlDbManager(_mySqlConnSettings))
                    {
                        #region Connection starting
                        //CONNECTION STARTING
                        _logger.UpdateLog("Starting connection for data loading...");
                        bool connectionSuccess = false;
                        using (BoxedData connStartData = await gwdDbManager.StartConnectionTaskStart())
                        {
                            if (!String.IsNullOrEmpty(connStartData.Msg)) _logger.UpdateLog(connStartData.Msg);
                            connectionSuccess = (bool)connStartData.Data;
                        }

                        if (!connectionSuccess)
                            throw new Exception("connection not estabilished!");

                        #endregion

                        #region Account data
                        //ACCOUNT DATA
                        _accountData = new AccountData(_logger);
                        _accountData.LoadAccountsAsync(gwdDbManager);

                        while (!_accountData.IsLoaded)
                        {
                            await Task.Factory.StartNew(() => Thread.Sleep(1000));

                            if (_accountData.LoadingError)
                                throw new Exception("account data loading failure!");
                        }
                        //_accountData.ShowAccountsInLog(); //NOTE: only for testing (shows secret account details)!

                        #endregion

                        #region Game world data
                        //GAME DATA LOADING
                        _gameWorldData = new GameWorldData(_logger);
                        _gameWorldData.LoadWorldDataAsync(gwdDbManager);
                        counter = 0;
                        while (!_gameWorldData.IsLoaded)
                        {
                            await Task.Factory.StartNew(() => Thread.Sleep(1000));
                            counter++;

                            if (_gameWorldData.LoadingError)
                                throw new Exception("game world data loading failure!");
                        }

                        _logger.UpdateLog($"Game data loading ended in {counter} seconds.");

                        #endregion
                    }

                    //PLAYER HANDLER
                    _playerHandler = new PlayerHandler(_logger);

                    //GAME STATE HANDLER
                    _gameStateHandler = new GameStateHandler(_logger, _gameWorldData, _gameWorldData, _playerHandler);

                    //CHAT HANDLER
                    _chatHandler = new ChatHandler(_logger, _gameWorldData, _playerHandler);

                    //COMMAND HANDLER
                    _commandHandler = new CommandHandler(_logger);
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdPingStrategy());
                    _commandHandler.RegisterCommandHandlingStrategy(new MoveCharRequestStrategy(_logger, _gameStateHandler, _gameWorldData, _playerHandler));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdChatStrategy(_logger, _chatHandler));
                    _commandHandler.RegisterCommandHandlingStrategy(new SwitchPlaceRequestStrategy(_logger, _gameStateHandler, _gameWorldData, _gameWorldData, _playerHandler));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdGetLocationCharsStrategy(_logger, _gameWorldData));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdGetWorldDetailsStrategy(_logger, _gameWorldData));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdLoginStrategy(_logger, _accountData, _playerHandler));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdLogoutStrategy(_playerHandler));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdGetAccountCharsStrategy(_logger, _accountData, _gameWorldData));
                    _commandHandler.RegisterCommandHandlingStrategy(new CmdChooseCharacterStrategy(_logger, _accountData, _gameWorldData, _playerHandler));
                    
                    //CONNECTION HANDLER
                    _tcpConnectionHandler = TcpConnectionHandler.GetInstance(_tcpLogger, _commandHandler, _playerHandler);
                    _tcpConnectionHandler.StartConnection(_uiHandler.GetTcpConnectionSettings());

                    //REGULAR OPERATION HANDLER
                    _regularOperationHandler = new RegularOperationHandler(_logger);
                    
                    //ETC
                    this.State = GameState.Active;
                }
                catch (Exception exception)
                {
                    this.State = GameState.Error;
                    _logger.UpdateLog($"An error occured during game starting: {exception.Message}");
                }
            }
            finally
            {
                _logger.UpdateLog($"*** ENDED WITH GAME STATE [{this.State.ToString()}]");
                _loadingInProgress = false;
            }
        }
    }
}
