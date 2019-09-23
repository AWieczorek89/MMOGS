using MMOGS.Models.GameState;
using TcpConnector.DataModels;

namespace MMOGS.Interfaces
{
    public interface ICommandHandlingStrategy
    {
        bool ValidateExecution(ClientCommandInfo cmdInfo);
        bool Execute(PlayerDetails playerDetails);
    }
}
