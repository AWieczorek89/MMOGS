using MySql.Data.MySqlClient;
using System;

namespace MMOGS.Models
{
    public class MySqlConnectionSettings
    {
        public string Server { get; private set; } = "";
        public int Port { get; private set; } = 3306;
        public string Database { get; private set; } = "";
        public string User { get; private set; } = "";
        public string Pass { get; private set; } = "";
        public string ConnectionString { get; private set; } = "";

        public MySqlConnectionSettings
        (
            string server,
            int port,
            string database,
            string user,
            string pass
        )
        {
            this.Server = server;
            this.Port = port;
            this.Database = database;
            this.User = user;
            this.Pass = pass;
            this.ConnectionString = GetConnectionString();
        }

        public string GetConnectionString()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = this.Server;
            builder.Port = Convert.ToUInt32(this.Port);
            builder.Database = this.Database;
            builder.UserID = this.User;
            builder.Password = this.Pass;
            return builder.ToString();
        }
    }
}
