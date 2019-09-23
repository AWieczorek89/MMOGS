using MMOGS.Interfaces;
using MMOGS.Models.Commands;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public class PingResponseCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "pingres";

        private CommandDetails _commandDetails = new CommandDetails();
        private int _pingId = -1;

        public PingResponseCmdBuilder(int pingId)
        {
            _pingId = pingId;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(PingResponseCmdBuilder._keyWord);
        }

        public void AddCommandElements()
        {
            _commandDetails.CommandElementList.Add(_pingId.ToString());
        }
        
        public string GetCommand()
        {
            return _commandDetails.GetFullCommand();
        }
    }
}
