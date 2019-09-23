using TcpConnector.DataModels;

namespace TcpConnector.DataContainers
{
    public class CommandContainer
    {
        private string[] cmdArray;
        private readonly object locker = new object();

        private int startIndex = 0;
        private int filledSize = 0;

        private int newIndex = 0;

        public CommandContainer(int size)
        {
            if (size <= 0)
                size = 1;

            cmdArray = new string[size];
        }

        /// <summary>
        /// Checks if container has empty space
        /// </summary>
        public bool CheckIfEmpty()
        {
            bool empty = false;

            lock (locker)
            {
                empty = (filledSize < cmdArray.Length);
            }
            
            return empty;
        }

        /// <summary>
        /// Gets command from container (LIFO)
        /// </summary>
        public string GetCommand()
        {
            string cmdText = "";

            lock (locker)
            {
                if (filledSize > 0)
                {
                    cmdText = cmdArray[startIndex];

                    startIndex++;
                    if (startIndex == cmdArray.Length)
                        startIndex = 0;

                    filledSize--;
                }
            }

            return cmdText;
        }

        /// <summary>
        /// Puts a new command into container
        /// </summary>
        public bool InsertCommand(string cmdTxt)
        {
            bool success = false;

            lock (locker)
            {
                if (filledSize < cmdArray.Length)
                {
                    newIndex = startIndex + filledSize;
                    if (newIndex >= cmdArray.Length)
                        newIndex -= cmdArray.Length;

                    cmdArray[newIndex] = cmdTxt;
                    filledSize++;
                    success = true;
                }
            }
            
            return success;
        }
    }

    #region Old
    //public class CommandContainer
    //{
    //    private ServerCmd[] cmdArray;
    //    private readonly object locker = new object();

    //    public CommandContainer(int size)
    //    {
    //        cmdArray = new ServerCmd[size];

    //        for (int i = 0; i < cmdArray.Length; i++)
    //        {
    //            cmdArray[i] = new ServerCmd();
    //        }
    //    }

    //    /// <summary>
    //    /// Checks if container has empty space
    //    /// </summary>
    //    public bool CheckIfEmpty()
    //    {
    //        bool empty = true;

    //        lock (locker)
    //        {
    //            if (cmdArray.Length == 0)
    //            {
    //                empty = false;
    //            }
    //            else
    //            if (cmdArray[cmdArray.Length - 1].IsSet)
    //            {
    //                empty = false;
    //            }
    //        }

    //        return empty;
    //    }

    //    /// <summary>
    //    /// Gets command from container (LIFO)
    //    /// </summary>
    //    public string GetCommand()
    //    {
    //        string cmd = "";

    //        lock (locker)
    //        {
    //            for (int i = 0; i < cmdArray.Length; i++)
    //            {
    //                if (i == 0 && cmdArray[i].IsSet)
    //                {
    //                    cmd = cmdArray[i].CommandTxt;
    //                }
    //                else
    //                if (i > 0)
    //                {
    //                    cmdArray[i - 1] = cmdArray[i];
    //                }

    //                if (i == cmdArray.Length - 1)
    //                {
    //                    cmdArray[i].IsSet = false;
    //                    cmdArray[i].CommandTxt = "";
    //                }
    //            }
    //        }

    //        return cmd;
    //    }

    //    /// <summary>
    //    /// Puts a new command into container
    //    /// </summary>
    //    public bool InsertCommand(string cmdTxt, ref string errorMsg)
    //    {
    //        bool success = false;
    //        errorMsg = "";

    //        lock (locker)
    //        {
    //            for (int i = 0; i < cmdArray.Length; i++)
    //            {
    //                if (!cmdArray[i].IsSet)
    //                {
    //                    cmdArray[i].IsSet = true;
    //                    cmdArray[i].CommandTxt = cmdTxt;
    //                    success = true;
    //                    break;
    //                }
    //            }

    //            //if (!success)
    //            //{
    //            //    errorMsg = "Command container update error - full buffer!";
    //            //}
    //        }

    //        return success;
    //    }
    //}
    #endregion
}
