using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArgbConverter.ProcessingClasses
{
    public static class FileDataManager
    {
        public static void CreateDirectoryIfNotExists(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Błąd tworzenia folderu [{dirPath}]: {exception.Message}");
            }
        }
    }
}
