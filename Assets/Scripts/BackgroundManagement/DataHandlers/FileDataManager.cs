using BackgroundManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BackgroundManagement.DataHandlers
{
    public class FileDataManager : IDisposable
    {
        private string _configFilePath;
        private IChat _chat;
        private Dictionary<string, string> _mainData = new Dictionary<string, string>();

        public FileDataManager(IChat chat)
        {
            _chat = chat;
            _configFilePath = CreateConfigFilePathIfNotExists();
            LoadDataFromFile(_configFilePath);
        }

        public string GetConfigFilePath()
        {
            return _configFilePath;
        }

        public void ShowData()
        {
            _chat.UpdateLog("---- File data:");

            for (int i = 0; i < _mainData.Count; i++)
                _chat.UpdateLog($"Position [{i}] key [{_mainData.ElementAt(i).Key}] value [{_mainData.ElementAt(i).Value}]");

            _chat.UpdateLog("----");
        }

        public string GetValueByKey(string key)
        {
            string value = "";
            bool success = _mainData.TryGetValue(key, out value);

            if (!success)
                _chat.UpdateLog($"Cannot get value of key [{key}]. Returned empty result.");

            return value;
        }

        public void InsertOrUpdateValueByKey(string key, string value)
        {
            string tempValue = "";
            bool exists = _mainData.TryGetValue(key, out tempValue);

            if (exists)
            {
                _mainData[key] = value;
            }
            else
            {
                _mainData.Add(key, value);
            }
        }

        public void SaveData()
        {
            SaveData(_configFilePath);
        }

        public void SaveData(string filePath)
        {
            try
            {
                List<string> contentList = new List<string>();

                for (int i = 0; i < _mainData.Count; i++)
                    contentList.Add($"{_mainData.ElementAt(i).Key}={_mainData.ElementAt(i).Value}");

                File.WriteAllLines(filePath, contentList.ToArray(), Encoding.Unicode);
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Error occured during data saving, file [{filePath}]: {exception.Message}");
            }
        }

        public void LoadDataFromFile(string filePath)
        {
            _mainData.Clear();

            try
            {
                if (!File.Exists(filePath))
                {
                    _chat.UpdateLog($"Cannot find file [{filePath}], creating new file...");

                    try
                    {
                        File.WriteAllText(filePath, String.Empty, Encoding.Unicode);
                    }
                    catch (Exception exception)
                    {
                        _chat.UpdateLog($"An error occured during file creation [{filePath}]: {exception.Message}");
                    }

                    return;
                }

                string[] dataTable = File.ReadAllLines(filePath, Encoding.Unicode);

                for (int i = 0; i < dataTable.Length; i++)
                {
                    string dataLine = dataTable[i];

                    if (dataLine.Contains("="))
                    {
                        int equalsCharIndex = dataLine.IndexOf('=');

                        string[] lineElements = new string[2]
                        {
                            dataLine.Substring(0, equalsCharIndex),
                            dataLine.Substring(equalsCharIndex + 1)
                        };

                        string key = lineElements[0];
                        string value = lineElements[1];

                        _mainData.Add(key, value);
                    }
                }
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Cannot load data from file [{filePath}]: {exception.Message}");
            }
        }

        private string CreateConfigFilePathIfNotExists()
        {
            string configPath = "";
            string configDir = "";

            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (appDataPath.Length == 0)
                    throw new Exception("app data path is empty!");

                configPath = $"{appDataPath}\\MMOClientSettings\\setup.txt";
                configDir = Path.GetDirectoryName(configPath);

                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir);
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Config path creation error: {exception.Message}");
            }
            
            return configPath;
        }

        public void CreateDirIfNotExists(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
            catch (Exception exception)
            {
                _chat.UpdateLog($"Error occured during directory creation [{dirPath}]: {exception.Message}");
            }
        }

        public void Dispose()
        {
            _chat = null;
            _mainData.Clear();
        }
    }
}
