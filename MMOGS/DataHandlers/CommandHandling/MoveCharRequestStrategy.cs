using MMOGS.Interfaces;
using MMOGS.Measurement.Units;
using MMOGS.Models.GameState;
using System;
using TcpConnector.DataModels;

namespace MMOGS.DataHandlers.CommandHandling
{
    public class MoveCharRequestStrategy : ICommandHandlingStrategy
    {
        private static readonly string _keyWord = "move";

        private ILogger _logger;
        private ICharacterActionManager _charActionManager;
        private ICharacterInfo _charInfo;
        private PlayerHandler _playerHandler;

        private string[] _cmdElements = null;
        private ClientCommandInfo _cmdInfo = null;

        public MoveCharRequestStrategy(ILogger logger, ICharacterActionManager charActionManager, ICharacterInfo charInfo, PlayerHandler playerHandler)
        {
            _logger = logger ?? throw new Exception("MoveCharRequestStrategy - logger cannot be NULL!");
            _charActionManager = charActionManager ?? throw new Exception("MoveCharRequestStrategy - char. action manager cannot be NULL!");
            _charInfo = charInfo ?? throw new Exception("MoveCharRequestStrategy - char. info cannot be NULL!");
            _playerHandler = playerHandler ?? throw new Exception("MoveCharRequestStrategy - player handler cannot be NULL!");
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

            try
            {
                double newPosX = 0.00;
                double newPosY = 0.00;
                double newPosZ = 0.00;
                int timeArrivalMs = 0;

                if (_cmdElements.Length != 5)
                    throw new Exception($"wrong count of command elements [{_cmdElements.Length}]");

                if 
                (
                    !Double.TryParse(_cmdElements[1], out newPosX) || 
                    !Double.TryParse(_cmdElements[2], out newPosY) || 
                    !Double.TryParse(_cmdElements[3], out newPosZ) ||
                    !Int32.TryParse(_cmdElements[4], out timeArrivalMs)
                )
                {
                    throw new Exception($"cannot convert parameters from command [{_cmdInfo.CommandTxt}]");
                }

                int charId = _playerHandler.GetPlayerCharacterId(playerDetails.TcpClientId);
                if (charId < 0)
                    throw new Exception("cannot find active character!");

                CharacterData charData = _charInfo.GetCharacterById(charId);
                if (charData == null)
                    throw new Exception($"cannot get character data of char. ID [{charId}]");

                if (charData.IsOnWorldMap)
                {
                    _charActionManager.MoveCharacterWorld
                    (
                        charId,
                        charData.CurrentWorldLoc,
                        new Point2<int>
                        (
                            Convert.ToInt32(Math.Round(newPosX)),
                            Convert.ToInt32(Math.Round(newPosY))
                        )
                    );
                }
                else
                {
                    _charActionManager.MoveCharacterLocal
                    (
                        charId,
                        charData.CurrentLoc,
                        new Point3<double>
                        (
                            newPosX,
                            newPosY,
                            newPosZ
                        ),
                        timeArrivalMs
                    );
                }

                executed = true;
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"Character movement request command execution error: {exception.Message}");
            }

            return executed;
        }
    }
}
