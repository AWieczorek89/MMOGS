using System;

namespace MMOGS.Models.GameState
{
    public class PlayerDetails : ICloneable
    {
        public int TcpClientId { get; private set; }
        public string Login { get; set; } //determines if account is logged (empty for no logged acc.)
        public int CharId { get; set; }
        public int LoginAttemptCount { get; set; } = 0;

        private PlayerDetails()
        {
        }

        public PlayerDetails(int tcpClientId, string login = "", int charId = -1)
        {
            if (tcpClientId < 0)
                throw new Exception("TCP client ID cannot be less than 0 in player details!");
            
            this.TcpClientId = tcpClientId;
            this.Login = login;
            this.CharId = charId;
        }

        public object Clone()
        {
            PlayerDetails newInstance = new PlayerDetails();
            newInstance.TcpClientId = this.TcpClientId;
            newInstance.Login = this.Login;
            newInstance.CharId = this.CharId;
            newInstance.LoginAttemptCount = this.LoginAttemptCount;

            return newInstance;
        }
    }
}
