using BackgroundManagement.Interfaces;

namespace BackgroundManagement.DataHandlers.CommandBuilding
{
    public class CommandCreator
    {
        public static void CreateCommand(ICommandBuilder cmdBuilder)
        {
            cmdBuilder.AddKeyword();
            cmdBuilder.AddCommandElements();
        }
    }
}
