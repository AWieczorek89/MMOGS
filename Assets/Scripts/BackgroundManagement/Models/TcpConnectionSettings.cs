using TcpConnector.DataModels;

namespace BackgroundManagement.Models
{
    public class TcpConnectionSettings
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        public TcpConnectionSettings(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }

        /// <summary>
        /// Converts data for TCP connector
        /// </summary>
        public ConnectionData ToConnectionData()
        {
            return new ConnectionData(this.Ip, this.Port);
        }
    }
}
