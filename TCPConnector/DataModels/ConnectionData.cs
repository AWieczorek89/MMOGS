using System;

namespace TcpConnector.DataModels
{
    public class ConnectionData
    {
        public string Ip { get; private set; } = "";
        public int Port { get; private set; } = -1;

        public ConnectionData(string ip, int port)
        {
            if(port <= 0)
            {
                throw new Exception($"TCP port [{port}] cannot by less or equal than 0!");
            }

            this.Ip = ip;
            this.Port = port;
        }
    }
}
