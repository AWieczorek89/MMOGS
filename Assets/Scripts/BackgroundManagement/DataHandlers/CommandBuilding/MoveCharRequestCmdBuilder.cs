using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.Commands;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class MoveCharRequestCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "move";

        private CommandDetails _commandDetails = new CommandDetails();
        private float _newPosX = 0f;
        private float _newPosY = 0f;
        private float _newPosZ = 0f;
        private int _timeArrivalMs = 0;

        public MoveCharRequestCmdBuilder(float newPosX, float newPosY) //for moving on world map
        {
            _newPosX = newPosX;
            _newPosY = newPosY;
            _newPosZ = 0f;
            _timeArrivalMs = 0;
        }

        public MoveCharRequestCmdBuilder(float newPosX, float newPosY, float newPosZ, int timeArrivalMs) //for moving in local place
        {
            _newPosX = newPosX;
            _newPosY = newPosY;
            _newPosZ = newPosZ;
            _timeArrivalMs = timeArrivalMs;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(_newPosX.ToString());
            _commandDetails.CommandElementList.Add(_newPosY.ToString());
            _commandDetails.CommandElementList.Add(_newPosZ.ToString());
            _commandDetails.CommandElementList.Add(_timeArrivalMs.ToString());
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
