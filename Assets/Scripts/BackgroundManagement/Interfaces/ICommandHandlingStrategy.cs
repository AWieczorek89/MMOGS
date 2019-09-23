namespace BackgroundManagement.Interfaces
{
    public interface ICommandHandlingStrategy
    {
        bool ValidateExecution(string command);
        bool Execute();
    }
}
