using MMOGS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MMOGS.DataHandlers
{
    public class FileDataManager : IDisposable
    {
        private static readonly string _configFilePath = @".\config\setup.txt";
        private ILogger _logger = null;
        private Dictionary<string, string> _mainData = new Dictionary<string, string>();
        
        public FileDataManager(ILogger logger)
        {
            _logger = logger;
            CreateDirIfNotExists(Path.GetDirectoryName(FileDataManager._configFilePath));
            LoadDataFromFile(FileDataManager._configFilePath);
        }

        public void ShowData()
        {
            _logger.UpdateLog("---- File data:");
            
            for (int i = 0; i < _mainData.Count; i++)
                _logger.UpdateLog($"Position [{i}] key [{_mainData.ElementAt(i).Key}] value [{_mainData.ElementAt(i).Value}]");
            
            _logger.UpdateLog("----");
        }

        public string GetValueByKey(string key)
        {
            string value = "";
            bool success = _mainData.TryGetValue(key, out value);

            if (!success)
                _logger.UpdateLog($"Cannot get value of key [{key}]. Returned empty result.");
            
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
            SaveData(FileDataManager._configFilePath);
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
                _logger.UpdateLog($"Error occured during data saving, file [{filePath}]: {exception.Message}");
            }
        }

        public void LoadDataFromFile(string filePath)
        {
            _mainData.Clear();

            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.UpdateLog($"Cannot find file [{filePath}], creating new file...");

                    try
                    {
                        File.WriteAllText(filePath, "", Encoding.Unicode);
                    }
                    catch (Exception exception)
                    {
                        _logger.UpdateLog($"An error occured during file creation [{filePath}]: {exception.Message}");
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
                _logger.UpdateLog($"Cannot load data from file [{filePath}]: {exception.Message}");
            }
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
                _logger.UpdateLog($"Error occured during directory creation [{dirPath}]: {exception.Message}");
            }
        }

        public void Dispose()
        {
            _logger = null;
            _mainData.Clear();
        }
    }
}
