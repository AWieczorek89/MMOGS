using MMOGS.Models.Commands;

namespace MMOGS.Interfaces
{
    public interface ICommandBuilder
    {
        void AddKeyword();
        void AddCommandElements();
        string GetCommand();
    }
}
