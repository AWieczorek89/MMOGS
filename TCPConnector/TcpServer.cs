using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpConnector.DataModels;

namespace TcpConnector
{
    public class TcpServer
    {
        private static TcpServer serverInstance = null;

        private ITcpLogger logger = null;
        private ConnectionData connData = null;
        private TcpListener listener;
        private bool isRunning;
        private List<ClientHandler> clientList = new List<ClientHandler>();
        private readonly object clientHandlerListLocker = new object();

        private static int containerSize = 100;

        #region Parameter setters

        /// <summary>
        /// Sets receiving and sending containers size. This method can be called only before RunServer(),
        /// to prevent problems related to container discrepancies while new connection creation
        /// </summary>
        public static void SetContainerSize(int containerSize)
        {
            if(serverInstance != null)
            {
                throw new Exception("A working TCP server instance detected. Cannot change container size!");
            }

            TcpServer.containerSize = containerSize;
        }

        #endregion
        
        /// <summary>
        /// Starts TCP server
        /// </summary>
        public static TcpServer RunServer(ConnectionData connData, ITcpLogger logger)
        {
            if (connData == null)
            {
                throw new Exception("connection data is NULL!");
            }

            if(logger == null)
            {
                throw new Exception("logger is NULL!");
            }

            if (serverInstance != null)
            {
                throw new Exception("A working TCP server instance detected. It should by stopped by StopServer() method first!");
            }

            serverInstance = new TcpServer(connData, logger);
            serverInstance.StartServer();
            return serverInstance;
        }

        /// <summary>
        /// Gets current server instance
        /// </summary>
        public static TcpServer GetServerInstance()
        {
            return serverInstance;
        }

        /// <summary>
        /// Stops TCP server working
        /// </summary>
        public static void StopServer()
        {
            if(serverInstance == null)
            {
                return;
            }

            serverInstance.listener.Stop();

            lock (serverInstance.clientHandlerListLocker)
            {
                string disMsg;

                for (int i = 0; i < serverInstance.clientList.Count; i++)
                {
                    disMsg = serverInstance.clientList[i].Disable();

                    if (!String.IsNullOrEmpty(disMsg))
                    {
                        serverInstance.logger.UpdateLog(disMsg);
                    }
                }

                serverInstance.clientList.Clear();
            }
            
            serverInstance.isRunning = false;
            serverInstance = null;
        }

        private TcpServer(ConnectionData connData, ITcpLogger logger)
        {
            this.connData = connData;
            this.logger = logger;
        }

        private void StartServer()
        {
            this.listener = new TcpListener
            (
                (!String.IsNullOrWhiteSpace(this.connData.Ip) ? IPAddress.Parse(this.connData.Ip) : IPAddress.Any),
                this.connData.Port
            );

            this.listener.Start();
            this.isRunning = true;
            this.logger.UpdateLog("Starting listening...");

            LoopClientsAsync();
        }

        #region Async methods

        private async void LoopClientsAsync()
        {
            try
            {
                while (this.isRunning)
                {
                    //TcpClient newClient = this.listener.AcceptTcpClient();
                    TcpClient newClient = await this.listener.AcceptTcpClientAsync();
                    newClient.ReceiveBufferSize = ConnectorGlobalSettings.BufferSize;
                    newClient.SendBufferSize = ConnectorGlobalSettings.BufferSize;
                    newClient.ReceiveTimeout = ConnectorGlobalSettings.Timeout;
                    newClient.SendTimeout = ConnectorGlobalSettings.Timeout;
                    
                    if (!this.isRunning)
                    {
                        newClient.GetStream().Close();
                        newClient.Close();
                        break;
                    }

                    this.logger.UpdateLog("Client connection started!");

                    ClientHandler newClientHandler = new ClientHandler(newClient, this.logger, containerSize);

                    //this.clientList.Add(newClientHandler);
                    await AddClientHandlerTaskStart(newClientHandler);
                }

                this.logger.UpdateLog("Listening method stopped working.");
            }
            catch (ObjectDisposedException objDispException)
            {
                this.logger.UpdateLog($"Client listening stopped. ObjectDisposedException thrown: {objDispException.Message}");
            }
            catch (Exception exception)
            {
                this.logger.UpdateLog($"Client listening exception: {exception.Message}");
            }
        }

        #endregion

        #region Task starters

        private Task AddClientHandlerTaskStart(ClientHandler handler)
        {
            var t = new Task
            (
                () => AddClientHandler(handler)
            );
            t.Start();
            return t;
        }

        #endregion

        #region Handling methods

        private void AddClientHandler(ClientHandler handler)
        {
            lock (clientHandlerListLocker)
            {
                bool addedToList = false;

                for (int i = 0; i < clientList.Count; i++)
                {
                    if (!clientList[i].IsWorking)
                    {
                        clientList[i] = handler;
                        Console.WriteLine($"Client added on position [{i}]");
                        addedToList = true;
                        break;
                    }
                }

                if (!addedToList)
                {
                    this.clientList.Add(handler);
                    Console.WriteLine("Client added on new position");
                }
            }
        }

        #endregion

        #region Communication

        /// <summary>
        /// Gets info about specific connected client by client ID
        /// </summary>
        public static ClientHandlerInfo GetClientInfo(int clientId)
        {
            ClientHandlerInfo clientInfo = null;

            if (serverInstance == null)
                return clientInfo;

            lock (serverInstance.clientHandlerListLocker)
            {
                foreach (ClientHandler clientHandler in serverInstance.clientList)
                {
                    if (clientHandler.ClientId == clientId)
                    {
                        clientInfo = new ClientHandlerInfo
                        (
                            clientHandler.IsWorking,
                            clientHandler.ClientIp,
                            clientHandler.ClientId
                        );
                        break;
                    }
                }
            }

            return clientInfo;
        }

        /// <summary>
        /// Gets info about all connected clients
        /// </summary>
        public static List<ClientHandlerInfo> GetCurrentClientsInfo()
        {
            List<ClientHandlerInfo> clientInfoList = new List<ClientHandlerInfo>();

            if (serverInstance == null)
            {
                return clientInfoList;
            }

            lock (serverInstance.clientHandlerListLocker)
            {
                for (int i = 0; i < serverInstance.clientList.Count; i++)
                {
                    ClientHandlerInfo clientInfo = new ClientHandlerInfo
                    (
                        serverInstance.clientList[i].IsWorking,
                        serverInstance.clientList[i].ClientIp,
                        serverInstance.clientList[i].ClientId
                    );
                    clientInfoList.Add(clientInfo);
                }
            }

            return clientInfoList;
        }

        /// <summary>
        /// Disconnects a client by specific client ID
        /// </summary>
        public static void KickClient(int clientId)
        {
            if(serverInstance == null)
            {
                return;
            }

            lock (serverInstance.clientHandlerListLocker)
            {
                for (int i = 0; i < serverInstance.clientList.Count; i++)
                {
                    if (serverInstance.clientList[i].ClientId == clientId)
                    {
                        serverInstance.clientList[i].Disable();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sends command to all connected clients
        /// </summary>
        public static void SendCommandToAllClients(string cmd)
        {
            if(String.IsNullOrEmpty(cmd) || serverInstance == null)
            {
                return;
            }

            lock (serverInstance.clientHandlerListLocker)
            {
                for (int i = 0; i < serverInstance.clientList.Count; i++)
                {
                    if(serverInstance.clientList[i].IsWorking)
                    {
                        serverInstance.clientList[i].InsertCommandToClient(cmd);
                    }
                }
            }
        }

        /// <summary>
        /// Sends command to client by specific client ID
        /// </summary>
        public static void SendCommandToClient(string cmd, int clientId)
        {
            if(String.IsNullOrEmpty(cmd) || serverInstance == null)
            {
                return;
            }

            lock (serverInstance.clientHandlerListLocker)
            {
                for (int i = 0; i < serverInstance.clientList.Count; i++)
                {
                    if(serverInstance.clientList[i].ClientId == clientId)
                    {
                        serverInstance.clientList[i].InsertCommandToClient(cmd);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets commands (and details) from all clients
        /// </summary>
        public static List<ClientCommandInfo> GetCommandsFromAllCLients()
        {
            List<ClientCommandInfo> clientCommandsList = new List<ClientCommandInfo>();

            if (serverInstance == null)
            {
                return clientCommandsList;
            }

            lock (serverInstance.clientHandlerListLocker)
            {
                string commandTxt;

                for (int i = 0; i < serverInstance.clientList.Count; i++)
                {
                    commandTxt = serverInstance.clientList[i].GetCommandFromClient();
                    if(String.IsNullOrEmpty(commandTxt))
                    {
                        continue;
                    }

                    ClientHandlerInfo clientInfo = new ClientHandlerInfo
                    (
                        serverInstance.clientList[i].IsWorking,
                        serverInstance.clientList[i].ClientIp,
                        serverInstance.clientList[i].ClientId
                    );

                    clientCommandsList.Add(new ClientCommandInfo(clientInfo, commandTxt));
                }
            }

            return clientCommandsList;
        }
        
        #endregion
    }
}
