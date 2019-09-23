using BackgroundManagement.Interfaces;
using BackgroundManagement.Models;
using BackgroundManagement.Models.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TcpConnector;

namespace BackgroundManagement.DataHandlers
{
    public class ChatHandler : IChat, ITcpLogger
    {
        private enum Type
        {
            GlobalChat,
            PrivateChat,
            Log,
            All
        }

        public static int ChatMessageCountLimit { get; private set; } = 30;
        private static readonly object _dataLock = new object();
        private static ChatHandler _instance = null;

        private string[] _globalChatContent = null;
        private string[] _privateChatContent = null;
        private string[] _logContent = null;
        
        public static ChatHandler GetInstance()
        {
            if (_instance == null)
                _instance = new ChatHandler();

            return _instance;
        }

        public Task<ChatPackage> GetMessagesTaskStart(bool getGlobal, bool getPrivate, bool getLog)
        {
            var t = new Task<ChatPackage>
            (
                () =>
                {
                    ChatPackage chatPack = new ChatPackage();
                    
                    lock (_dataLock)
                    {
                        if (getGlobal)
                        {
                            foreach (string content in _globalChatContent)
                                chatPack.GlobalChatMessageList.Add(content);
                        }

                        if (getPrivate)
                        {
                            foreach (string content in _privateChatContent)
                                chatPack.PrivateChatMessageList.Add(content);
                        }

                        if (getLog)
                        {
                            foreach (string content in _logContent)
                                chatPack.LogMessageList.Add(content);
                        }
                    }
                    
                    return chatPack;
                }
            );

            t.Start();
            return t;
        }

        private ChatHandler()
        {
            lock (_dataLock)
            {
                _globalChatContent = new string[ChatHandler.ChatMessageCountLimit];
                _privateChatContent = new string[ChatHandler.ChatMessageCountLimit];
                _logContent = new string[ChatHandler.ChatMessageCountLimit];
            }

            FillEmpty(Type.All);
        }
        
        private void FillEmpty(Type type)
        {
            lock (_dataLock)
            {
                for (int i = 0; i < ChatHandler.ChatMessageCountLimit; i++)
                {
                    if (type == Type.All || type == Type.GlobalChat)
                        _globalChatContent[i] = String.Empty;

                    if (type == Type.All || type == Type.PrivateChat)
                        _privateChatContent[i] = String.Empty;

                    if (type == Type.All || type == Type.Log)
                        _logContent[i] = String.Empty;
                }
            }
        }

        public void UpdateGlobalChat(string text, string fromAuthor)
        {
            UpdateGlobalChatAsync(text, fromAuthor);
        }
        
        public void UpdatePrivateChat(string text, string fromAuthor, string toAuthor)
        {
            UpdatePrivateChatAsync(text, fromAuthor, toAuthor);
        }

        public void UpdateLog(string text)
        {
            UpdateLogAsync(text);
        }

        private async void UpdateGlobalChatAsync(string text, string fromAuthor)
        {
            await Task.Factory.StartNew
            (
                () =>
                {
                    lock (_dataLock)
                    {
                        for (int i = 0; i < _globalChatContent.Length - 1; i++)
                        {
                            _globalChatContent[i] = _globalChatContent[i + 1];
                        }

                        _globalChatContent[_globalChatContent.Length - 1] = $"[{fromAuthor}] {text}";
                    }
                }
            );
        }

        private async void UpdatePrivateChatAsync(string text, string fromAuthor, string toAuthor)
        {
            await Task.Factory.StartNew
            (
                () =>
                {
                    lock (_dataLock)
                    {
                        for (int i = 0; i < _privateChatContent.Length - 1; i++)
                        {
                            _privateChatContent[i] = _privateChatContent[i + 1];
                        }
                        
                        _privateChatContent[_privateChatContent.Length - 1] = $"[{fromAuthor} to {toAuthor}] {text}";
                    }
                }
            );
        }

        private async void UpdateLogAsync(string text)
        {
            await Task.Factory.StartNew
            (
                () =>
                {
                    lock (_dataLock)
                    {
                        for (int i = 0; i < _logContent.Length - 1; i++)
                        {
                            _logContent[i] = _logContent[i + 1];
                        }
                        
                        _logContent[_logContent.Length - 1] = $"[Log] {text}";
                    }
                }
            );
        }

        public void ClearGlobalChat()
        {
            ClearGlobalChatAsync();
        }

        public void ClearPrivateChat()
        {
            ClearPrivateChatAsync();
        }

        public void ClearLog()
        {
            ClearLogAsync();
        }

        public void ClearAll()
        {
            ClearAllAsync();
        }

        private async void ClearGlobalChatAsync()
        {
            await Task.Factory.StartNew(() => FillEmpty(Type.GlobalChat));
        }

        private async void ClearPrivateChatAsync()
        {
            await Task.Factory.StartNew(() => FillEmpty(Type.PrivateChat));
        }

        private async void ClearLogAsync()
        {
            await Task.Factory.StartNew(() => FillEmpty(Type.Log));
        }

        private async void ClearAllAsync()
        {
            await Task.Factory.StartNew(() => FillEmpty(Type.All));
        }
    }
}
