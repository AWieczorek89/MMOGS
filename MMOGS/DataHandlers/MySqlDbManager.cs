using MMOGS.Models;
using MMOGS.Models.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MMOGS.DataHandlers
{
    public class MySqlDbManager : IDisposable
    {
        MySqlConnectionSettings _connectionSettings = null;
        MySqlConnection _connection = null;
        private readonly object _connectionLock = new object();

        public MySqlDbManager(MySqlConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings ?? throw new Exception($"mySQL connection settings cannot be NULL!");
        }
        
        ~MySqlDbManager()
        {
            CloseConnection();
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public ConnectionState GetConnectionState()
        {
            if (_connection == null)
                throw new Exception("Getting connection state failed - connection is NULL!");

            return _connection.State;
        }

        #region Task starters

        public Task<BoxedData> StartConnectionTaskStart()
        {
            var t = new Task<BoxedData>(() => StartConnection());
            t.Start();
            return t;
        }

        public Task<BoxedData> CloseConnectionTaskStart()
        {
            var t = new Task<BoxedData>(() => CloseConnection());
            t.Start();
            return t;
        }

        public Task<BoxedData> CheckConnectionTaskStart()
        {
            var t = new Task<BoxedData>(() => CheckConnection());
            t.Start();
            return t;
        }

        public Task<BoxedData> ExecuteQueryTaskStart(string query)
        {
            var t = new Task<BoxedData>(() => ExecuteQuery(query));
            t.Start();
            return t;
        }

        /// <summary>
        /// Adds new record in table 'accounts' (awaitable)
        /// </summary>
        public Task<BoxedData> AddAccountsDataTaskStart(DbAccountsData accountData)
        {
            var t = new Task<BoxedData>(() => AddAccountsData(accountData));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets data from table 'accounts' (awaitable)
        /// </summary>
        public Task<BoxedData> GetAccountsDataTaskStart(string sqlFilter = "")
        {
            var t = new Task<BoxedData>(() => GetAccountsData(sqlFilter));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets data from table 'world_map_settings' (awaitable)
        /// </summary>
        public Task<BoxedData> GetWorldMapSettingsDataTaskStart(string sqlFilter = "")
        {
            var t = new Task<BoxedData>(() => GetWorldMapSettingsData(sqlFilter));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets data from table 'world_map' (awaitable)
        /// </summary>
        public Task<BoxedData> GetWorldMapDataTaskStart(string sqlFilter = "")
        {
            var t = new Task<BoxedData>(() => GetWorldMapData(sqlFilter));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets data from table 'terrain_object_definitions' (awaitable)
        /// </summary>
        public Task<BoxedData> GetTerrainObjectDefinitionsDataTaskStart(string sqlFilter = "")
        {
            var t = new Task<BoxedData>(() => GetTerrainObjectDefinitionsData(sqlFilter));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets data from table 'terrain_objects' (awaitable)
        /// </summary>
        public Task<BoxedData> GetTerrainObjectsDataTaskStart(string sqlFilter = "")
        {
            var t = new Task<BoxedData>(() => GetTerrainObjectsData(sqlFilter));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets all parent IDs from terrain objects in specific location (by world map loc. ID) - (awaitable)
        /// </summary>
        public Task<BoxedData> GetParentObjectIdsTaskStart(int wmId)
        {
            var t = new Task<BoxedData>(() => GetParentObjectIds(wmId));
            t.Start();
            return t;
        }

        /// <summary>
        /// Gets data from table 'characters' (awaitable)
        /// </summary>
        public Task<BoxedData> GetCharactersDataTaskStart(string sqlFilter = "")
        {
            var t = new Task<BoxedData>(() => GetCharactersData(sqlFilter));
            t.Start();
            return t;
        }

        #endregion

        #region Boxed-packet returning methods

        /// <summary>
        /// Gets data from table 'characters'
        /// </summary>
        public BoxedData GetCharactersData(string sqlFilter = "")
        {
            BoxedData data = new BoxedData();
            List<DbCharactersData> resultList = new List<DbCharactersData>();
            string msg = "";

            try
            {
                int charId = -1;
                string name = "";
                int accId = -1;
                bool isNpc = false;
                string npcAltName = "";
                bool isOnWorldMap = false;
                int wmId = -1;
                int terrainParentId = -1;
                decimal localPosX = 0.0000M;
                decimal localPosY = 0.0000M;
                decimal localPosZ = 0.0000M;
                decimal localAngle = 0.0000M;
                string modelCode = "";
                int hairstyleId = 0;

                string query = @"
                    SELECT
                        `char_id`,
                        `char_name`,
                        `char_acc_id`,
                        `char_is_npc`,
                        `char_npc_alt_name`,
                        `char_is_on_world_map`,
                        `char_wm_id`,
                        `char_terrain_parent_id`,
                        `char_local_pos_x`,
                        `char_local_pos_y`,
                        `char_local_pos_z`,
                        `char_local_angle`,
                        `char_model_code`,
                        `char_hairstyle_id`
                    FROM
                        `characters`
                ";

                if (!String.IsNullOrWhiteSpace(sqlFilter))
                    query += $" {(!sqlFilter.ToUpper().Contains("WHERE") ? "WHERE" : "")} {sqlFilter}";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                charId = reader.GetInt32("char_id");
                                name = reader["char_name"].ToString();
                                accId = reader.GetInt32("char_acc_id");
                                isNpc = (reader.GetInt32("char_is_npc") == 1);
                                npcAltName = reader["char_npc_alt_name"].ToString();
                                isOnWorldMap = (reader.GetInt32("char_is_on_world_map") == 1);
                                wmId = reader.GetInt32("char_wm_id");
                                terrainParentId = reader.GetInt32("char_terrain_parent_id");
                                localPosX = reader.GetDecimal("char_local_pos_x");
                                localPosY = reader.GetDecimal("char_local_pos_y");
                                localPosZ = reader.GetDecimal("char_local_pos_z");
                                localAngle = reader.GetDecimal("char_local_angle");
                                modelCode = reader["char_model_code"].ToString();
                                hairstyleId = reader.GetInt32("char_hairstyle_id");

                                DbCharactersData result = new DbCharactersData
                                (
                                    charId,
                                    name,
                                    accId,
                                    isNpc,
                                    npcAltName,
                                    isOnWorldMap,
                                    wmId,
                                    terrainParentId,
                                    localPosX,
                                    localPosY,
                                    localPosZ,
                                    localAngle,
                                    modelCode,
                                    hairstyleId
                                );
                                resultList.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get data from 'characters' table, SQL filter [{sqlFilter}]: {exception.Message}";
            }

            data.Data = resultList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Gets all parent IDs from terrain objects in specific location (by world map loc. ID)
        /// </summary>
        public BoxedData GetParentObjectIds(int wmId)
        {
            BoxedData data = new BoxedData();
            List<int> parentIdsList = new List<int>();
            string msg = "";

            try
            {
                string query = @"
                    SELECT DISTINCT `to_parent_id`
                    FROM `terrain_objects`
                    WHERE `to_wm_id` = @wmId
                ";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@wmId", wmId);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                parentIdsList.Add(reader.GetInt32("to_parent_id"));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get parent IDs from 'terrain_objects' table, wm_id [{wmId}]: {exception.Message}";
            }

            data.Data = parentIdsList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Gets data from table 'terrain_objects'
        /// </summary>
        public BoxedData GetTerrainObjectsData(string sqlFilter = "")
        {
            BoxedData data = new BoxedData();
            List<DbTerrainObjectsData> resultList = new List<DbTerrainObjectsData>();
            string msg = "";

            try
            {
                int toId = -1;
                int wmId = -1;
                int localPosX = -1;
                int localPosY = -1;
                int localPosZ = -1;
                int todId = -1;
                int parentId = -1;
                bool isParentalTeleport = false;
                bool isExit = false;

                string query = @"
                    SELECT
                        `to_id`,
                        `to_wm_id`,
                        `to_local_pos_x`,
                        `to_local_pos_y`,
                        `to_local_pos_z`,
                        `to_tod_id`,
                        `to_parent_id`,
                        `to_is_parental_teleport`,
                        `to_is_exit`
                    FROM
                        `terrain_objects`
                ";

                if (!String.IsNullOrWhiteSpace(sqlFilter))
                    query += $" {(!sqlFilter.ToUpper().Contains("WHERE") ? "WHERE" : "")} {sqlFilter}";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                toId = reader.GetInt32("to_id");
                                wmId = reader.GetInt32("to_wm_id");
                                localPosX = reader.GetInt32("to_local_pos_x");
                                localPosY = reader.GetInt32("to_local_pos_y");
                                localPosZ = reader.GetInt32("to_local_pos_z");
                                todId = reader.GetInt32("to_tod_id");
                                parentId = reader.GetInt32("to_parent_id");
                                isParentalTeleport = (reader.GetInt32("to_is_parental_teleport") == 1);
                                isExit = (reader.GetInt32("to_is_exit") == 1);

                                DbTerrainObjectsData result = new DbTerrainObjectsData
                                (
                                    toId,
                                    wmId,
                                    localPosX,
                                    localPosY,
                                    localPosZ,
                                    todId,
                                    parentId,
                                    isParentalTeleport,
                                    isExit
                                );
                                resultList.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get data from 'terrain_objects' table, SQL filter [{sqlFilter}]: {exception.Message}";
            }

            data.Data = resultList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Gets data from table 'terrain_object_definitions'
        /// </summary>
        public BoxedData GetTerrainObjectDefinitionsData(string sqlFilter = "")
        {
            BoxedData data = new BoxedData();
            List<DbTerrainObjectDefinitions> resultList = new List<DbTerrainObjectDefinitions>();
            string msg = "";

            try
            {
                int todId = -1;
                string code = "";
                int collisionX = 0;
                int collisionY = 0;
                int collisionZ = 0;
                bool isTerrain = false;
                bool isPlatform = false;
                bool isObstacle = false;

                string query = @"
                    SELECT
                        `tod_id`,
                        `tod_code`,
                        `tod_collision_x`,
                        `tod_collision_y`,
                        `tod_collision_z`,
                        `tod_is_terrain`,
                        `tod_is_platform`,
                        `tod_is_obstacle`
                    FROM
                        `terrain_object_definitions`
                ";

                if (!String.IsNullOrWhiteSpace(sqlFilter))
                    query += $" {(!sqlFilter.ToUpper().Contains("WHERE") ? "WHERE" : "")} {sqlFilter}";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                todId = reader.GetInt32("tod_id");
                                code = reader["tod_code"].ToString();
                                collisionX = reader.GetInt32("tod_collision_x");
                                collisionY = reader.GetInt32("tod_collision_y");
                                collisionZ = reader.GetInt32("tod_collision_z");
                                isTerrain = (reader.GetInt32("tod_is_terrain") == 1);
                                isPlatform = (reader.GetInt32("tod_is_platform") == 1);
                                isObstacle = (reader.GetInt32("tod_is_obstacle") == 1);

                                DbTerrainObjectDefinitions result = new DbTerrainObjectDefinitions
                                (
                                    todId,
                                    code,
                                    collisionX,
                                    collisionY,
                                    collisionZ,
                                    isTerrain,
                                    isPlatform,
                                    isObstacle
                                );
                                resultList.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get data from 'terrain_object_definitions' table, SQL filter [{sqlFilter}]: {exception.Message}";
            }

            data.Data = resultList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Gets data from table 'world_map'
        /// </summary>
        public BoxedData GetWorldMapData(string sqlFilter = "")
        {
            BoxedData data = new BoxedData();
            List<DbWorldMapData> resultList = new List<DbWorldMapData>();
            string msg = "";

            try
            {
                int wmId = -1;
                int worldPosX = -1;
                int worldPosY = -1;
                int placeType = -1;
                int isRandomPlace = 0;
                string placeName = "";

                string query = @"
                    SELECT
                        `wm_id`,
                        `wm_world_pos_x`,
                        `wm_world_pos_y`,
                        `wm_place_type`,
                        `wm_is_random_place`,
                        `wm_place_name`
                    FROM
                        `world_map`
                ";

                if (!String.IsNullOrWhiteSpace(sqlFilter))
                    query += $" {(!sqlFilter.ToUpper().Contains("WHERE") ? "WHERE" : "")} {sqlFilter}";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                wmId = reader.GetInt32("wm_id");
                                worldPosX = reader.GetInt32("wm_world_pos_x");
                                worldPosY = reader.GetInt32("wm_world_pos_y");
                                placeType = reader.GetInt32("wm_place_type");
                                isRandomPlace = reader.GetInt32("wm_is_random_place");
                                placeName = reader["wm_place_name"].ToString();

                                DbWorldMapData result = new DbWorldMapData
                                (
                                    wmId,
                                    worldPosX,
                                    worldPosY,
                                    placeType,
                                    (isRandomPlace == 1),
                                    placeName
                                );
                                resultList.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get data from 'world_map' table, SQL filter [{sqlFilter}]: {exception.Message}";
            }

            data.Data = resultList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Gets data from table 'world_map_settings'
        /// </summary>
        public BoxedData GetWorldMapSettingsData(string sqlFilter = "")
        {
            BoxedData data = new BoxedData();
            List<DbWorldMapSettingsData> resultList = new List<DbWorldMapSettingsData>();
            string msg = "";

            try
            {
                int id = -1;
                int type = -1;
                string description = "";
                string value = "";

                string query = @"
                    SELECT
                        `wms_id`,
                        `wms_type`,
                        `wms_description`,
                        `wms_value`
                    FROM
                        `world_map_settings`
                ";

                if (!String.IsNullOrWhiteSpace(sqlFilter))
                    query += $" {(!sqlFilter.ToUpper().Contains("WHERE") ? "WHERE" : "")} {sqlFilter}";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = reader.GetInt32("wms_id");
                                type = reader.GetInt32("wms_type");
                                description = reader["wms_description"].ToString();
                                value = reader["wms_value"].ToString();

                                DbWorldMapSettingsData result = new DbWorldMapSettingsData(id, type, description, value);
                                resultList.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get data from 'world_map_settings' table, SQL filter [{sqlFilter}]: {exception.Message}";
            }

            data.Data = resultList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Gets data from table 'accounts'
        /// </summary>
        public BoxedData GetAccountsData(string sqlFilter = "")
        {
            BoxedData data = new BoxedData();
            List<DbAccountsData> resultList = new List<DbAccountsData>();
            string msg = "";

            try
            {
                int id = -1;
                string login = "";
                string passEncrypted = "";
                int accessLevel = 0;

                string query = @"
                    SELECT
                        `acc_id`,
                        `acc_login`,
                        `acc_pass`,
                        `acc_access_level`
                    FROM
                        `accounts`
                ";

                if (!String.IsNullOrWhiteSpace(sqlFilter))
                    query += $" {(!sqlFilter.ToUpper().Contains("WHERE") ? "WHERE" : "")} {sqlFilter}";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = reader.GetInt32("acc_id");
                                login = reader["acc_login"].ToString();
                                passEncrypted = reader["acc_pass"].ToString();
                                accessLevel = reader.GetInt32("acc_access_level");

                                DbAccountsData result = new DbAccountsData
                                (
                                    id,
                                    login,
                                    passEncrypted,
                                    DbAccountsData.PasswordType.Encrypted,
                                    accessLevel
                                );

                                resultList.Add(result);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot get data from 'accounts' table, SQL filter [{sqlFilter}]: {exception.Message}";
            }

            data.Data = resultList;
            data.Msg = msg;
            return data;
        }

        /// <summary>
        /// Adds new record in table 'accounts'
        /// </summary>
        public BoxedData AddAccountsData(DbAccountsData accountData)
        {
            BoxedData data = new BoxedData();
            int accId = -1;
            string msg = "";

            try
            {
                string query = @"
                    INSERT INTO `accounts`
                    (
                        `acc_login`,
                        `acc_pass`,
                        `acc_access_level`
                    )
                    VALUES
                    (
                        @login,
                        @pass,
                        @accessLevel
                    );

                    SELECT LAST_INSERT_ID() AS `acc_id`;
                ";

                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@login", accountData.Login);
                        command.Parameters.AddWithValue("@pass", accountData.PassEncrypted);
                        command.Parameters.AddWithValue("@accessLevel", accountData.AccessLevel);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!Int32.TryParse(reader["acc_id"].ToString(), out accId))
                                    msg = $"Cannot convert acc_id!";
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot insert new record in table 'accounts': {exception.Message}";
            }

            data.Data = accId;
            data.Msg = msg;
            return data;
        }

        public BoxedData ExecuteQuery(string query)
        {
            BoxedData data = new BoxedData();
            bool success = false;
            string msg = "";

            try
            {
                lock (_connectionLock)
                {
                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.ExecuteNonQuery();
                        success = true;
                    }
                }
            }
            catch (Exception exception)
            {
                msg = $"Cannot execute query: {exception.Message}";
            }

            data.Data = success;
            data.Msg = msg;
            return data;
        }

        public BoxedData StartConnection()
        {
            BoxedData data = new BoxedData();
            bool success = false;
            string msg = "";

            try
            {
                lock (_connectionLock)
                {
                    if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                        throw new Exception("connection is open. It should be closed before starting new connection!");

                    _connection = new MySqlConnection(_connectionSettings.ConnectionString);
                    _connection.Open();
                    success = true;
                }
            }
            catch (Exception exception)
            {
                msg = $"An error occured during mySQL connection starting: {exception.Message}";
            }

            data.Data = success;
            data.Msg = msg;
            return data;
        }

        public BoxedData CloseConnection()
        {
            BoxedData data = new BoxedData();
            bool success = false;
            string msg = "";

            try
            {
                lock (_connectionLock)
                {
                    if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                        _connection.Close();

                    success = true;
                }
            }
            catch (Exception exception)
            {
                msg = $"An error occured during mySQL connection closing: {exception.Message}";
            }

            data.Data = success;
            data.Msg = msg;
            return data;
        }

        public BoxedData CheckConnection()
        {
            BoxedData data = new BoxedData();
            bool connOpen = false;
            string msg = "";

            try
            {
                lock (_connectionLock)
                {
                    if (_connection == null)
                        throw new Exception("connector is NULL!");

                    if (_connection.State == System.Data.ConnectionState.Open)
                        connOpen = true;
                }
            }
            catch (Exception exception)
            {
                msg = $"An error occured during mySQL connection checking: {exception.Message}";
            }

            data.Data = connOpen;
            data.Msg = msg;
            return data;
        }
        
        #endregion
    }
}
