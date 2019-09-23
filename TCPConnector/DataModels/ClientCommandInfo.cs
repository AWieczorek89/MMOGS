namespace TcpConnector.DataModels
{
    public class ClientCommandInfo
    {
        public ClientHandlerInfo ClientInfo { get; private set; }
        public string CommandTxt { get; private set; }

        public ClientCommandInfo(ClientHandlerInfo clientInfo, string commandTxt)
        {
            this.ClientInfo = clientInfo;
            this.CommandTxt = commandTxt;
        }
    }
}
