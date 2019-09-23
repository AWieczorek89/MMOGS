using MMOGS.Interfaces;
using MMOGS.Models;
using MMOGS.SQL;
using System;
using System.Collections.Generic;

namespace MMOGS.DataHandlers
{
    public class MySqlDataCreationTool
    {
        private ILogger _logger = null;
        MySqlConnectionSettings _connectionSettings = null;

        public MySqlDataCreationTool(ILogger logger, MySqlConnectionSettings connectionSettings)
        {
            _logger = logger;
            _connectionSettings = connectionSettings;
        }

        public async void CreateMySqlDataAsync(bool createTables, bool createData)
        {
            _logger.UpdateLog("Running mySQL data creation tool...");

            int successCounter = 0;
            int faultCounter = 0;

            try
            {
                _logger.UpdateLog("Starting connection...");
                
                using (MySqlDbManager dbManager = new MySqlDbManager(_connectionSettings))
                {
                    BoxedData startConnData = await dbManager.StartConnectionTaskStart();
                    bool startConnSuccess = (bool)startConnData.Data;

                    if (!String.IsNullOrEmpty(startConnData.Msg))
                        _logger.UpdateLog(startConnData.Msg);

                    if (!startConnSuccess)
                        throw new Exception("cannot connect to database!");

                    _logger.UpdateLog("Getting data creation queries...");
                    SqlCreationQueriesData queriesData = new SqlCreationQueriesData();
                    List<string> queryList = queriesData.GetMySqlDataCreationQueries(createTables, createData);
                    bool queryExecSuccess = false;

                    for (int i = 0; i < queryList.Count; i++)
                    {
                        _logger.UpdateLog($"Executing query {(i + 1)} of {queryList.Count}...");

                        using (BoxedData queryExecData = await dbManager.ExecuteQueryTaskStart(queryList[i]))
                        {
                            queryExecSuccess = (bool)queryExecData.Data;
                            if (!String.IsNullOrEmpty(queryExecData.Msg))
                                _logger.UpdateLog(queryExecData.Msg);
                        }

                        _logger.UpdateLog(queryExecSuccess ? "Query executed successfully!" : "An error occured!");

                        if (queryExecSuccess) successCounter++;
                        else faultCounter++;
                    }
                }

                _logger.UpdateLog($"MySql data creation ended! Successes [{successCounter}] faults [{faultCounter}].");
            }
            catch (Exception exception)
            {
                _logger.UpdateLog($"MySQL data creation failed: {exception.Message}");
            }
        }
    }
}
