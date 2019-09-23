using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.Interfaces;
using MMOGS.Models.GameState;
using System;
using TcpConnector;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class CmdLoginStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "login";

        private ILogger _logger;
        private AccountData _accountData;
        private PlayerHandler _playerHandler;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;
        
        public CmdLoginStrategy(ILogger logger, AccountData accountData, PlayerHandler playerHandler)
        {
            _logger = logger ?? throw new Exception("CmdLoginStrategy - logger cannot be NULL!");
            _accountData = accountData ?? throw new Exception("CmdLoginStrategy - account data cannot be NULL!");
            _playerHandler = playerHandler ?? throw new Exception("CmdLoginStrategy - player handler cannot be NULL!");
        }

        public bool ValidateExecution(ClientCommandInfo cmdInfo)
        {
            bool valid = false;
            _cmdInfo = cmdInfo;
            _cmdElements = cmdInfo.CommandTxt.Split(' ');

            if (_cmdElements.Length > 0 && _cmdElements[0].Equals(_keyWord, GlobalData.InputDataStringComparison))
                valid = true;

            return valid;
        }

        public bool Execute(PlayerDetails playerDetails)
        {
            bool executed = false;
            bool error = false;
            string msg = "";

            try
            {
                if (_cmdElements.Length < 2 || _cmdElements.Length > 3)
                {
                    msg = "Wrong login command!";
                    return false;
                }

                string login = _cmdElements[1];
                string pass = (_cmdElements.Length == 3 ? _cmdElements[2] : "");

                if (_playerHandler.CheckIfLoginInUse(login))
                {
                    msg = "An account is already in use!";
                    return false;
                }

                if (!_accountData.AccountValidation(login, pass, Models.Database.DbAccountsData.PasswordType.Decrypted))
                {
                    msg = "Wrong login or password!";
                    return false;
                }

                playerDetails.Login = login;
                playerDetails.LoginAttemptCount = 0;
                msg = $"Player [{playerDetails.Login}] has logged in, TCP client ID [{playerDetails.TcpClientId}].";
                executed = true;
            }
            catch (Exception exception)
            {
                error = true;
                msg = $"Login execution error: {exception.Message}";
            }
            finally
            {
                _logger.UpdateLog(msg);
                SendLoginCommand(playerDetails, executed, (executed ? "" : (error ? "An error occured on the server!" : msg)));
            }

            return executed;
        }

        private void SendLoginCommand(PlayerDetails playerDetails, bool loginSuccess, string info = "")
        {
            CommandHandler.Send(new LoginSuccessCmdBuilder(loginSuccess, info), playerDetails);
        }
    }
}
