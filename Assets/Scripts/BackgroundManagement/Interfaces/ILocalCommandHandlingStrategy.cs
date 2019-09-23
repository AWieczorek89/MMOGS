namespace BackgroundManagement.Interfaces
{
    public interface ILocalCommandHandlingStrategy
    {
        bool ValidateExecution(string command);
        bool Execute();
    }
}
