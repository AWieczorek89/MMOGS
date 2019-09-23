using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.Commands;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class ChooseCharCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "choosechar";

        private CommandDetails _commandDetails = new CommandDetails();
        private int _charId = -1;

        public ChooseCharCmdBuilder(int charId)
        {
            _charId = charId;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(_keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(_charId.ToString());
        }

        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
