using BackgroundManagement.Models.Chat;
using System.Threading.Tasks;

namespace BackgroundManagement.Interfaces
{
    public interface IChat
    {
        void UpdateGlobalChat(string text, string fromAuthor);
        void UpdatePrivateChat(string text, string fromAuthor, string toAuthor);
        void UpdateLog(string text);
        void ClearGlobalChat();
        void ClearPrivateChat();
        void ClearLog();
        void ClearAll();
        Task<ChatPackage> GetMessagesTaskStart(bool getGlobal, bool getPrivate, bool getLog);
    }
}
