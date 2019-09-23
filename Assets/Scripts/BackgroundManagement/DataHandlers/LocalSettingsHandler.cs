using BackgroundManagement.Interfaces;
using BackgroundManagement.Models;
using System;

namespace BackgroundManagement.DataHandlers
{
    public class LocalSettingsHandler
    {
        private static LocalSettingsHandler _instance = null;
        private LocalSettingsData _settings;
        private IChat _chat;

        public static LocalSettingsHandler GetInstance(IChat chat)
        {
            if (_instance == null)
                _instance = new LocalSettingsHandler(chat);
            
            return _instance;
        }

        private LocalSettingsHandler(IChat chat)
        {
            _chat = chat;
            LoadSettings();
        }

        public LocalSettingsData GetSettings()
        {
            return _settings.Copy();
        }

        private void LoadSettings()
        {
            try
            {
                using (FileDataManager fm = new FileDataManager(_chat))
                {
                    _settings = new LocalSettingsData();
                    
                    #region TCP connection
                    //TCP CONNECTION
                    string ip = fm.GetValueByKey("tcp_ip");
                    int tcpPort = 2222;
                    if (String.IsNullOrWhiteSpace(ip)) ip = "127.0.0.1";
                    Int32.TryParse(fm.GetValueByKey("tcp_port"), out tcpPort);
                    if (tcpPort <= 0) tcpPort = 2222;

                    _settings.TcpConnSettings = new TcpConnectionSettings(ip, tcpPort);

                    #endregion

                    _chat.UpdateLog($"Settings loaded from [{fm.GetConfigFilePath()}]");
                    
                }
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Local settings loading error: {exception.Message} | {exception.StackTrace}");
            }
        }
    }
}
