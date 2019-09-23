using MMOGS.Interfaces;

namespace MMOGS.DataHandlers.CommandBuilding
{
    public static class CommandCreator
    {
        public static void CreateCommand(ICommandBuilder cmdBuilder)
        {
            cmdBuilder.AddKeyword();
            cmdBuilder.AddCommandElements();
        }
    }
}
