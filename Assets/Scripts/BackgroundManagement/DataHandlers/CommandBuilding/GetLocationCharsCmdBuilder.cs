using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.Commands;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class GetLocationCharsCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "getlocalchars";

        private CommandDetails _commandDetails = new CommandDetails();
        private int _charId;

        public GetLocationCharsCmdBuilder(int charId = -1) //NOTE: charId > -1 for specific character information
        {
            _charId = charId;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            if (_charId > -1)
                _commandDetails.CommandElementList.Add(_charId.ToString());
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
