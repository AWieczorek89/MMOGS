namespace TcpConnector.DataModels
{
    public class ClientHandlerInfo
    {
        public bool IsWorking { get; private set; } = false;
        public string ClientIp { get; private set; } = "";
        public int ClientId { get; private set; } = -1;

        public ClientHandlerInfo
        (
            bool isWorking,
            string clientIp,
            int clientId
        )
        {
            this.IsWorking = isWorking;
            this.ClientIp = clientIp;
            this.ClientId = clientId;
        }
    }
}
