using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector.DataContainers;
using TcpConnector.DataModels;

namespace TcpConnector
{
    public class TcpServerClient
    {
        public static int ReceivingInsertionTimeout { get; private set; } = 30;
        public static int SendingTickRate { get; private set; } = 20;
        public static int CommandInsertionTimeLimit { get; private set; } = 5;

        private static TcpServerClient serverClientInstance = null;

        private ITcpLogger logger = null;
        private ConnectionData connData = null;

        private TcpClient client = null;
        private StreamReader sReader = null;
        private StreamWriter sWriter = null;
        private CommandContainer receivedCmdContainer = null; //receiving container
        private CommandContainer toSendCmdContainer = null; //sending container
        private PacketObjectMsg receivingInfoPacket = new PacketObjectMsg();
        private PacketObjectMsg sendingInfoPacket = new PacketObjectMsg();

        private bool isConnected = false;

        private static int containerSize = 100;

        #region Parameter setters

        /// /// <summary>
        /// Sets time limit (s), which throws an error after reaching this time if receiving container is not released (full)
        /// </summary>
        public static void SetReceivingInsertionTimeout(int recInsertionTimeout)
        {
            ReceivingInsertionTimeout = recInsertionTimeout;
        }
        
        /// <summary>
        /// Sets receiving and sending containers size. This method can be called only before RunServerClient(),
        /// to prevent problems related to container discrepancies while new connection creation
        /// </summary>
        public static void SetContainerSize(int containerSize)
        {
            if (serverClientInstance != null)
            {
                throw new Exception("working TCP client instance detected. Cannot change container size!");
            }

            TcpServerClient.containerSize = containerSize;
        }
        
        /// <summary>
        /// Sets tick rate (ms) which defines intervals of sending commands to server (from command container)
        /// </summary>
        public static void SetSendingTickRate(int tickRateMs)
        {
            SendingTickRate = tickRateMs;
        }
        
        /// <summary>
        /// Sets time limit (s) for stopping single command sending, if sending container is not released (full)
        /// </summary>
        public static void SetCommandInsertionTimeLimit(int cmdInsertionTimeLimit)
        {
            CommandInsertionTimeLimit = cmdInsertionTimeLimit;
        }

        #endregion

        /// <summary>
        /// Starts TCP client
        /// </summary>
        public static TcpServerClient RunServerClient(ConnectionData connData, ITcpLogger logger)
        {
            if (connData == null)
            {
                throw new Exception("connection data is NULL!");
            }

            if (logger == null)
            {
                throw new Exception("logger is NULL!");
            }

            if (serverClientInstance != null)
            {
                throw new Exception("a working TCP client detected!");
            }

            serverClientInstance = new TcpServerClient(connData, logger);
            serverClientInstance.StartServerClient();
            return serverClientInstance;
        }

        /// <summary>
        /// Gets current client instance
        /// </summary>
        public static TcpServerClient GetServerClientInstance()
        {
            return serverClientInstance;
        }

        /// <summary>
        /// Stops working of TCP client
        /// </summary>
        public static void StopServerClient()
        {
            if(serverClientInstance == null)
            {
                return;
            }

            try
            {
                serverClientInstance.client.GetStream().Close();
                serverClientInstance.client.Close();
            }
            catch (Exception exception)
            {
                serverClientInstance.logger.UpdateLog($"TCP client stopping exception: {exception.Message}");
            }
            
            serverClientInstance.isConnected = false;
            serverClientInstance = null;
        }

        /// <summary>
        /// Checks if client is connected
        /// </summary>
        public static bool CheckIfConnected()
        {
            bool connected = false;
            if (serverClientInstance == null)
            {
                return connected;
            }
            
            return connected = serverClientInstance.isConnected;
        }

        private TcpServerClient(ConnectionData connData, ITcpLogger logger)
        {
            this.connData = connData;
            this.logger = logger;
        }

        private void StartServerClient()
        {
            try
            {
                this.receivedCmdContainer = new CommandContainer(containerSize);
                this.toSendCmdContainer = new CommandContainer(containerSize);

                this.client = new TcpClient();
                this.client.ReceiveBufferSize = ConnectorGlobalSettings.BufferSize;
                this.client.SendBufferSize = ConnectorGlobalSettings.BufferSize;
                this.client.ReceiveTimeout = ConnectorGlobalSettings.Timeout;
                this.client.SendTimeout = ConnectorGlobalSettings.Timeout;

                this.client.Connect(this.connData.Ip, this.connData.Port);
                
                logger.UpdateLog($"Connected!");
                this.isConnected = true;
            }
            catch (Exception exception)
            {
                this.isConnected = false;
                logger.UpdateLog($"Connection error: {exception.Message}");
            }

            HandleReceivingAsync();
            HandleSendingAsync();
        }

        #region Async methods
        
        private async void HandleSendingAsync()
        {
            try
            {
                this.sWriter = new StreamWriter(client.GetStream(), Encoding.Unicode);
                this.sendingInfoPacket.Obj = true;
                this.sendingInfoPacket.Msg = "";

                while (this.isConnected)
                {
                    await HandleSendingTaskStart();

                    if ((bool)this.sendingInfoPacket.Obj == false)
                    {
                        throw new Exception(this.sendingInfoPacket.Msg);
                    }
                }
            }
            catch (Exception exception)
            {
                this.isConnected = false;
                logger.UpdateLog($"Command sending error: {exception.Message}");
            }
        }
        
        private async void HandleReceivingAsync()
        {
            try
            {
                this.sReader = new StreamReader(this.client.GetStream(), Encoding.Unicode);
                this.receivingInfoPacket.Obj = true;
                this.receivingInfoPacket.Msg = "";

                while (this.isConnected)
                {
                    await HandleReceivingTaskStart();

                    if ((bool)this.receivingInfoPacket.Obj == false)
                    {
                        throw new Exception(this.receivingInfoPacket.Msg);
                    }
                }
            }
            catch (Exception exception)
            {
                this.isConnected = false;
                logger.UpdateLog($"Command receiving error: {exception.Message}");
            }
        }

        #endregion

        #region Task starters

        private Task HandleSendingTaskStart()
        {
            var t = new Task
            (
                () => HandleSending()
            );
            t.Start();

            return t;
        }

        private Task HandleReceivingTaskStart()
        {
            var t = new Task
            (
                () => HandleReceiving()
            );
            t.Start();
            return t;
        }

        #endregion

        #region Handling methods

        private void HandleSending()
        {
            try
            {
                string cmd = toSendCmdContainer.GetCommand();

                if (!String.IsNullOrEmpty(cmd))
                {
                    sWriter.WriteLine(cmd);
                    sWriter.Flush();
                }

                Thread.Sleep(SendingTickRate);
            }
            catch (Exception exception)
            {
                this.receivingInfoPacket.Obj = false;
                this.receivingInfoPacket.Msg = exception.Message;
            }
        }

        private void HandleReceiving()
        {
            try
            {
                string cmd = this.sReader.ReadLine();
                string errorMsg = "";

                DateTime timeoutFrom = DateTime.Now;
                int loopCounter = 0;
                TimeSpan span;
                int timeoutSeconds;

                while (!receivedCmdContainer.CheckIfEmpty())
                {
                    loopCounter++;
                    if (loopCounter % 100 != 0) { continue; } //makes CheckIfEmpty() calling faster

                    span = DateTime.Now - timeoutFrom;
                    timeoutSeconds = (int)span.Seconds;

                    if (timeoutSeconds > ReceivingInsertionTimeout)
                    {
                        throw new Exception("timeout - command container not released");
                    }
                }

                if(!receivedCmdContainer.InsertCommand(cmd))
                {
                    throw new Exception("cannot insert command to received command container");
                }
            }
            catch (Exception exception)
            {
                this.receivingInfoPacket.Obj = false;
                this.receivingInfoPacket.Msg = exception.Message;
            }
        }

        #endregion

        #region Command methods
        
        /// <summary>
        /// Gets last received command from receivedCmdContainer
        /// NOTE: this method should be called faster than insertion frequency (to container)
        /// </summary>
        private string GetCommand()
        {
            return this.receivedCmdContainer.GetCommand();
        }

        /// <summary>
        /// Inserts new queued command to send for client to toSendCmdContainer
        /// </summary>
        private void InsertCommandToServer(string cmd)
        {
            bool insertionSuccess = false;
            DateTime timeStart = DateTime.Now;
            double diffInSeconds;

            do
            {
                insertionSuccess = this.toSendCmdContainer.InsertCommand(cmd);
                
                //TIME LIMIT
                if (!insertionSuccess)
                {
                    diffInSeconds = (DateTime.Now - timeStart).TotalSeconds;
                    if (diffInSeconds > CommandInsertionTimeLimit)
                    {
                        this.logger.UpdateLog($"SENDING COMMAND TO SERVER FAILURE - TIMEOUT REACHED ({CommandInsertionTimeLimit})");
                        break;
                    }
                }
            }
            while (!insertionSuccess);
        }

        #endregion

        #region Communication

        /// <summary>
        /// Sends command to server
        /// </summary>
        public static void SendCommandToServer(string cmd)
        {
            if(String.IsNullOrEmpty(cmd) || serverClientInstance == null)
            {
                return;
            }

            serverClientInstance.InsertCommandToServer(cmd);
        }

        /// <summary>
        /// Receives command from server
        /// </summary>
        public static string GetCommandFromServer()
        {
            string cmd = "";

            if(serverClientInstance != null)
            {
                cmd = serverClientInstance.GetCommand();
            }

            return cmd;
        }

        #endregion
    }
}
