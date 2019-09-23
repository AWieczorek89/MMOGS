using ArgbConverter.AdvancedTools;
using ArgbConverter.DataModels;
using ArgbConverter.ProcessingClasses;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ArgbConverter
{
    public partial class MainForm : Form
    {
        private bool imageCreationInProgress = false;

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void SetFilePath()
        {
            try
            {
                string path = "";

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    if(ofd.ShowDialog() == DialogResult.OK)
                    {
                        path = ofd.FileName;
                    }
                }

                if (String.IsNullOrWhiteSpace(path))
                    return;

                string extension = Path.GetExtension(path);
                if (extension.ToLower() != ".png" && extension.ToLower() != "png")
                    throw new Exception($"plik [{path}] musi mieć rozszerzenie .png, przekazywany format [{extension}]");

                filePathTextBox.Text = path;
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Błąd ustawiania pliku: {exception.Message}");
            }
        }

        private async void CreateOpacityMap()
        {
            DisableControls(true);
            opacityMapInfoLabel.Visible = true;

            try
            {
                string filePath = filePathTextBox.Text;
                string resultDirPath = GlobalData.ResultFileDirectory;
                FileDataManager.CreateDirectoryIfNotExists(resultDirPath);

                ArgbProcessor argbProc = new ArgbProcessor(filePath);
                PacketObjMsg fileCreationPacket = await argbProc.CreateOpacityMapAndNonTransparentImageTaskStart(resultDirPath);
                bool success = (bool)fileCreationPacket.Obj;

                if (!String.IsNullOrEmpty(fileCreationPacket.Msg))
                    MessageBox.Show(fileCreationPacket.Msg);

                if (success)
                {
                    MessageBox.Show("Pomyślnie utworzono pliki!");
                    Process.Start(resultDirPath);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Błąd tworzenia mapy przezroczystości: {exception.Message}");
            }

            opacityMapInfoLabel.Visible = false;
            DisableControls(false);
        }

        private void DisableControls(bool disable)
        {
            getFilePathButton.Enabled = !disable;
            createOpacityMapButton.Enabled = !disable;
        }

        #region Eventy

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void getFilePathButton_Click(object sender, EventArgs e)
        {
            SetFilePath();
        }

        private void createOpacityMapButton_Click(object sender, EventArgs e)
        {
            CreateOpacityMap(); //async
        }

        private void hairTexGeneratorButton_Click(object sender, EventArgs e)
        {
            HairTexGeneratorForm hairTexGenWindow = new HairTexGeneratorForm();
            hairTexGenWindow.ShowDialog();

            hairTexGenWindow = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion
        
    }
}
