using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpConnector.DataContainers;

namespace TcpConnector
{
    internal class ClientHandler
    {
        public static int SendingTickRate { get; private set; } = 20;
        public static int ReceivingInsertionTimeout { get; private set; } = 30;
        public static int CommandInsertionTimeLimit { get; private set; } = 5;

        private static int clientIdCounter = 0; //used to give unique IDs for ClientHandler instances

        public bool IsWorking { get; private set; } = false;
        public string ClientIp { get; private set; } = "";
        public int ClientId { get; private set; } = -1;

        private ITcpLogger logger = null;
        private TcpClient client = null;
        private CommandContainer toSendCmdContainer = null;
        private CommandContainer receivedCmdContainer = null;

        #region Parameter setters

        /// <summary>
        /// Sets tick rate (ms) which defines intervals of sending commands to clients (from command container)
        /// </summary>
        public static void SetSendingTickRate(int tickRateMs)
        {
            SendingTickRate = tickRateMs;
        }

        /// <summary>
        /// Sets time limit (s), which throws an error after reaching this time if receiving container is not released (full)
        /// </summary>
        public static void SetReceivingInsertionTimeout(int receivingInsertionTimeout)
        {
            ReceivingInsertionTimeout = receivingInsertionTimeout;
        }

        /// <summary>
        /// Sets time limit (s) for stopping single command sending, if sending container is not released (full)
        /// </summary>
        public static void SetCommandInsertionTimeLimit(int cmdInsertionTimeLimit)
        {
            CommandInsertionTimeLimit = cmdInsertionTimeLimit;
        }

        #endregion

        public ClientHandler(TcpClient client, ITcpLogger logger, int cmdContainerSize)
        {
            this.IsWorking = true;
            this.client = client;
            this.logger = logger;

            this.ClientId = clientIdCounter;
            clientIdCounter++;

            this.ClientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            this.logger.UpdateLog($"Creating new ClientHandler object for connection: [{this.ClientIp}], ID [{this.ClientId}]...");

            this.toSendCmdContainer = new CommandContainer(cmdContainerSize);
            this.receivedCmdContainer = new CommandContainer(cmdContainerSize);

            HandleSendingAsync();
            HandleReceivingAsync();
        }

        /// <summary>
        /// Disables sending and receiving process, sets IsWorking property to false, which releases this object for next connection
        /// </summary>
        public string Disable()
        {
            string msg = "";

            try
            {
                this.client.GetStream().Close();
                this.client.Close();
            }
            catch (Exception exception)
            {
                msg = $"A method ClientHandler.Disable for [{this.ClientIp}] throws an exception: {exception.Message}";
            }

            this.IsWorking = false;
            return msg;
        }

        #region Async methods

        /// <summary>
        /// Starts a new thread, which sends commands to client from toSendCmdContainer
        /// </summary>
        private async void HandleSendingAsync()
        {
            string msg = await HandleSendingTaskStart();

            if (!String.IsNullOrEmpty(msg))
            {
                this.logger.UpdateLog($"IP [{this.ClientIp}] {msg}"); //message after command execution
            }
        }

        /// <summary>
        /// Starts a new thread, which receives commands from client and puts them to receivedCmdContainer
        /// </summary>
        private async void HandleReceivingAsync()
        {
            string msg = await HandleReceivingTaskStart();

            if (!String.IsNullOrEmpty(msg))
            {
                this.logger.UpdateLog($"IP [{this.ClientIp}] {msg}"); //message after command execution
            }
        }

        #endregion

        #region Task starters

        private Task<string> HandleSendingTaskStart()
        {
            var t = new Task<string>
            (
                () => HandleSending()
            );
            t.Start();
            return t;
        }

        private Task<string> HandleReceivingTaskStart()
        {
            var t = new Task<string>
            (
                () => HandleReceiving()
            );
            t.Start();
            return t;
        }

        #endregion

        #region Handling methods

        private string HandleSending()
        {
            string message = "";

            try
            {
                string cmd;
                StreamWriter streamWriter = new StreamWriter(this.client.GetStream(), Encoding.Unicode);

                while (this.IsWorking)
                {
                    cmd = "";

                    do
                    {
                        Thread.Sleep(SendingTickRate);
                        cmd = this.toSendCmdContainer.GetCommand();
                    }
                    while (String.IsNullOrEmpty(cmd) && this.IsWorking);

                    if (!this.IsWorking) { break; }

                    streamWriter.WriteLine(cmd);
                    streamWriter.Flush();
                }

                message = "ClientHandler sending process ended";
            }
            catch (Exception exception)
            {
                string disMsg = Disable();
                message = $"ClientHandler sending process exception: {exception.Message}";

                if (!String.IsNullOrEmpty(disMsg))
                {
                    message += $" | {disMsg}";
                }
            }

            return message;
        }

        private string HandleReceiving()
        {
            string message = "";

            try
            {
                StreamReader streamReader = new StreamReader(this.client.GetStream(), Encoding.Unicode);
                string streamData = "";

                bool insertionSuccess;
                DateTime timeoutFrom;
                TimeSpan span;
                int timeoutSeconds;

                while (this.IsWorking)
                {
                    streamData = streamReader.ReadLine();

                    if (!String.IsNullOrWhiteSpace(streamData))
                    {
                        insertionSuccess = false;
                        timeoutFrom = DateTime.Now;

                        do
                        {
                            span = DateTime.Now - timeoutFrom;
                            timeoutSeconds = (int)span.Seconds;

                            if (timeoutSeconds > ReceivingInsertionTimeout)
                            {
                                throw new Exception($"timeout - command container not released / {message}");
                            }

                            insertionSuccess = receivedCmdContainer.InsertCommand(streamData);
                        }
                        while (!insertionSuccess && this.IsWorking);
                    }
                }

                message = "ClientHandler receiving process ended";
            }
            catch (Exception exception)
            {
                string disMsg = Disable();
                message = $"ClientHandler receiving process exception: {exception.Message}";

                if (!String.IsNullOrEmpty(disMsg))
                {
                    message += $" | {disMsg}";
                }
            }

            return message;
        }

        #endregion

        #region Command methods

        /// <summary>
        /// Inserts new queued command to send for client to toSendCmdContainer
        /// </summary>
        public void InsertCommandToClient(string cmd)
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
                        this.logger.UpdateLog($"IP [{this.ClientIp}] COMMAND SENDING FAILURE - TIMEOUT REACHED ({CommandInsertionTimeLimit})");
                        break;
                    }
                }
            }
            while (!insertionSuccess);
        }

        /// <summary>
        /// Gets last received command from receivedCmdContainer
        /// NOTE: this method should be called faster than insertion frequency (to container)
        /// </summary>
        public string GetCommandFromClient()
        {
            string cmd = this.receivedCmdContainer.GetCommand();
            return cmd;
        }

        #endregion
    }
}
