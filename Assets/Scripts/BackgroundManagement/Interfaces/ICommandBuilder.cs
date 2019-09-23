namespace BackgroundManagement.Interfaces
{
    public interface ICommandBuilder
    {
        void AddKeyword();
        void AddCommandElements();
        string GetCommand();
    }
}
