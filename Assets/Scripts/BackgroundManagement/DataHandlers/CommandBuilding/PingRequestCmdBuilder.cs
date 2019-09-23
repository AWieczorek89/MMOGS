using BackgroundManagement.Interfaces;
using BackgroundManagement.Models.Commands;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class PingRequestCmdBuilder : ICommandBuilder
    {
        private static readonly string _keyWord = "ping";

        private CommandDetails _commandDetails = new CommandDetails();
        private int _pingId = -1;

        public PingRequestCmdBuilder(int pingId)
        {
            _pingId = pingId;
        }

        public void AddKeyword()
        {
            _commandDetails.CommandElementList.Add(PingRequestCmdBuilder._keyWord);
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
