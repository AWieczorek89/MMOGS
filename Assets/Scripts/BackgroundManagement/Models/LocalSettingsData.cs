namespace BackgroundManagement.Models
{
    public class LocalSettingsData
    {
        public TcpConnectionSettings TcpConnSettings { get; set; } = null;
        
        public LocalSettingsData Copy()
        {
            return new LocalSettingsData()
            {
                TcpConnSettings = 
                (
                    this.TcpConnSettings != null ? 
                    new TcpConnectionSettings(this.TcpConnSettings.Ip, this.TcpConnSettings.Port) :
                    null
                )
            };
        }
    }
}
