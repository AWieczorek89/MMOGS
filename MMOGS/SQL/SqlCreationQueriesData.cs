using System.Collections.Generic;

namespace MMOGS.SQL
{
    public class SqlCreationQueriesData
    {
        private static string[] _mySqlDbTableCreationQueries = new string[]
        {
            #region Accounts table query
            //ACCOUNTS TABLE
            @"
                CREATE TABLE `accounts`
                (
                    `acc_id` INT PRIMARY KEY AUTO_INCREMENT,
                    `acc_login` VARCHAR(50) NOT NULL,
                    `acc_pass` VARCHAR(255) NOT NULL,
                    `acc_access_level` INT NOT NULL DEFAULT 0,
                    CONSTRAINT `acc_constraint_unique_login` UNIQUE (`acc_login`)
                );
            ",
            #endregion

            #region World map settings table query
            //WORLD MAP SETTINGS TABLE
            @"
                CREATE TABLE `world_map_settings`
                (
                    `wms_id` INT PRIMARY KEY AUTO_INCREMENT,
                    `wms_type` INT NOT NULL,
                    `wms_description` VARCHAR(50) NULL,
                    `wms_value` VARCHAR(50) NOT NULL,
                    CONSTRAINT `wms_constraint_unique_type` UNIQUE (`wms_type`)
                );
            ",
            #endregion

            #region World map table query
            //WORLD MAP TABLE
            @"
                CREATE TABLE `world_map`
                (
                    `wm_id` INT PRIMARY KEY AUTO_INCREMENT,
                    `wm_world_pos_x` INT NOT NULL,
                    `wm_world_pos_y` INT NOT NULL,
                    `wm_place_type` INT NOT NULL,
                    `wm_is_random_place` TINYINT(1) NOT NULL DEFAULT 1,
                    `wm_place_name` VARCHAR(50) NULL,
                    CONSTRAINT `wm_constraint_unique_position` UNIQUE (`wm_world_pos_x`, `wm_world_pos_y`),
                    CONSTRAINT `wm_constraint_unique_place_name` UNIQUE (`wm_place_name`)
                );
            ",
            #endregion

            #region Terrain object definitions table query
            //TERRAIN OBJECT DEFINITIONS TABLE
            @"
                CREATE TABLE `terrain_object_definitions`
                (
                    `tod_id` INT PRIMARY KEY AUTO_INCREMENT,
                    `tod_code` VARCHAR(50) NOT NULL,
                    `tod_collision_x` INT NOT NULL DEFAULT 1,
                    `tod_collision_y` INT NOT NULL DEFAULT 1,
                    `tod_collision_z` INT NOT NULL DEFAULT 1,
                    `tod_is_terrain` TINYINT(1) NOT NULL,
                    `tod_is_platform` TINYINT(1) NOT NULL,
                    `tod_is_obstacle` TINYINT(1) NOT NULL,
                    CONSTRAINT `tod_constraint_unique_code` UNIQUE (`tod_code`)
                );
            ",
            #endregion

            #region Terrain objects table query
            //TERRAIN OBJECTS TABLE
            @"
                CREATE TABLE `terrain_objects`
                (
                    `to_id` INT PRIMARY KEY AUTO_INCREMENT,
                    `to_wm_id` INT NOT NULL,
                    `to_local_pos_x` INT NOT NULL,
                    `to_local_pos_y` INT NOT NULL,
                    `to_local_pos_z` INT NOT NULL,
                    `to_tod_id` INT NOT NULL,
                    `to_parent_id` INT NOT NULL DEFAULT -1,
                    `to_is_parental_teleport` TINYINT(1) NOT NULL DEFAULT 0,
                    `to_is_exit` TINYINT(1) NOT NULL DEFAULT 0,
                    FOREIGN KEY (`to_wm_id`) REFERENCES `world_map`(`wm_id`),
                    FOREIGN KEY (`to_tod_id`) REFERENCES `terrain_object_definitions`(`tod_id`)
                );
            ",
            #endregion

            #region Characters table query
            //CHARACTERS TABLE
            @"
                CREATE TABLE `characters`
                (
                    `char_id` INT PRIMARY KEY AUTO_INCREMENT,
                    `char_name` VARCHAR(50) NOT NULL,
                    `char_acc_id` INT NOT NULL,
                    `char_is_npc` TINYINT(1) NOT NULL,
                    `char_npc_alt_name` VARCHAR(50) NULL,
                    `char_is_on_world_map` TINYINT(1) NOT NULL,
                    `char_wm_id` INT NOT NULL,
                    `char_terrain_parent_id` INT NOT NULL DEFAULT -1,
                    `char_local_pos_x` DECIMAL(15, 4) NOT NULL,
                    `char_local_pos_y` DECIMAL(15, 4) NOT NULL,
                    `char_local_pos_z` DECIMAL(15, 4) NOT NULL,
                    `char_local_angle` DECIMAL(15, 4) NOT NULL,
                    `char_model_code` VARCHAR(50) NOT NULL,
                    `char_hairstyle_id` INT NOT NULL,
                    CONSTRAINT `char_constraint_unique_name` UNIQUE (`char_name`),
                    FOREIGN KEY (`char_wm_id`) REFERENCES `world_map`(`wm_id`)
                );
            "
            #endregion
        };

        private static string[] _mySqlDbDataCreationQueries = new string[]
        {
            #region World map settings data query
            //WORLD MAP SETTINGS DATA
            @"
                INSERT INTO `world_map_settings` 
                    (`wms_type`, `wms_description`, `wms_value`)
                VALUES
                    (1, 'Map width', '20'),
                    (2, 'Map height', '20'),
                    (3, 'Starting pos X', '10'),
                    (4, 'Starting pos Y', '10'),
                    (5, 'Local bound X', 50),
                    (6, 'Local bound Y', 50),
                    (7, 'Local bound Z', 20)
                ;
            ",
            #endregion

            #region World map data query
            //WORLD MAP DATA
            @"
                INSERT INTO `world_map`
                    (`wm_world_pos_x`, `wm_world_pos_y`, `wm_place_type`, `wm_is_random_place`, `wm_place_name`)
                VALUES
                    (10, 10, 1, 0, 'Some Place 0'),
                    (5, 7, 2, 1, 'Some Place 1'),
                    (5, 9, 2, 1, 'Some Place 2'),
                    (15, 12, 2, 1, 'Some Place 3')
                ;
            ",
            #endregion

            #region Terrain object definitions data query
            //TERRAIN OBJECT DEFINITIONS DATA
            @"
                INSERT INTO `terrain_object_definitions`
                    (`tod_code`, `tod_collision_x`, `tod_collision_y`, `tod_collision_z`, `tod_is_terrain`, `tod_is_platform`, `tod_is_obstacle`)
                VALUES
                    ('BASIC_TERRAIN', 1, 1, 1, 1, 0, 0),
                    ('BASIC_PLATFORM', 1, 1, 1, 0, 1, 1),
                    ('BASIC_WALL', 1, 1, 2, 0, 0, 1),
                    ('BASIC_SLOPE_N', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_S', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_W', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_E', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_CORNER_NW', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_CORNER_NE', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_CORNER_SW', 1, 1, 1, 0, 0, 0),
                    ('BASIC_SLOPE_CORNER_SE', 1, 1, 1, 0, 0, 0)
                ;
            ",
            #endregion

            #region Terrain objects data query
            //TERRAIN OBJECTS DATA
            @"
                INSERT INTO `terrain_objects`
                (
                    `to_wm_id`,
                    `to_local_pos_x`, `to_local_pos_y`, `to_local_pos_z`,
                    `to_tod_id`,
                    `to_parent_id`, `to_is_parental_teleport`, `to_is_exit`
                )
                VALUES
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    0, 0, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    0, 1, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    0, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    0, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    0, 4, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    0, 5, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    1, 0, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    1, 1, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    1, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    1, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    1, 4, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    1, 5, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    2, 0, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    2, 1, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    2, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    2, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    2, 4, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    2, 5, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 0, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 1, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 4, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 5, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    4, 0, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    4, 1, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    4, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    4, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    4, 4, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    4, 5, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    5, 0, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    5, 1, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    5, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    5, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    5, 4, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    5, 5, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 0
                ),



                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    6, 2, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 1
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    6, 3, 0,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_TERRAIN' LIMIT 1),
                    -1, 0, 1
                ),
                (
                    (SELECT `wm_id` FROM `world_map` WHERE `wm_place_name` = 'Some Place 0' LIMIT 1),
                    3, 2, 1,
                    (SELECT `tod_id` FROM `terrain_object_definitions` WHERE `tod_code` = 'BASIC_WALL' LIMIT 1),
                    -1, 0, 0
                )

                ;
            "
            #endregion
        };

        public List<string> GetMySqlDataCreationQueries(bool createTables, bool createData)
        {
            List<string> queryList = new List<string>();

            if (createTables)
            {
                foreach (string query in SqlCreationQueriesData._mySqlDbTableCreationQueries)
                    queryList.Add(query);
            }

            if (createData)
            {
                foreach (string query in SqlCreationQueriesData._mySqlDbDataCreationQueries)
                    queryList.Add(query);
            }

            return queryList;
        }
    }
}
