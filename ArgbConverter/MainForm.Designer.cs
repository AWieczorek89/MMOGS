namespace ArgbConverter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.opacityMapToolPanel = new System.Windows.Forms.Panel();
            this.opacityMapInfoLabel = new System.Windows.Forms.Label();
            this.createOpacityMapButton = new System.Windows.Forms.Button();
            this.getFilePanel = new System.Windows.Forms.Panel();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.getFilePathButton = new System.Windows.Forms.Button();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.quickToolsTabPage = new System.Windows.Forms.TabPage();
            this.advancedToolsTabPage = new System.Windows.Forms.TabPage();
            this.hairTexGeneratorPanel = new System.Windows.Forms.Panel();
            this.hairTexGeneratorButton = new System.Windows.Forms.Button();
            this.opacityMapToolPanel.SuspendLayout();
            this.getFilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.mainTabControl.SuspendLayout();
            this.quickToolsTabPage.SuspendLayout();
            this.advancedToolsTabPage.SuspendLayout();
            this.hairTexGeneratorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // opacityMapToolPanel
            // 
            this.opacityMapToolPanel.AutoScroll = true;
            this.opacityMapToolPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.opacityMapToolPanel.Controls.Add(this.opacityMapInfoLabel);
            this.opacityMapToolPanel.Controls.Add(this.createOpacityMapButton);
            this.opacityMapToolPanel.Location = new System.Drawing.Point(7, 113);
            this.opacityMapToolPanel.Name = "opacityMapToolPanel";
            this.opacityMapToolPanel.Size = new System.Drawing.Size(359, 63);
            this.opacityMapToolPanel.TabIndex = 5;
            // 
            // opacityMapInfoLabel
            // 
            this.opacityMapInfoLabel.AutoSize = true;
            this.opacityMapInfoLabel.ForeColor = System.Drawing.Color.Red;
            this.opacityMapInfoLabel.Location = new System.Drawing.Point(15, 42);
            this.opacityMapInfoLabel.Name = "opacityMapInfoLabel";
            this.opacityMapInfoLabel.Size = new System.Drawing.Size(117, 13);
            this.opacityMapInfoLabel.TabIndex = 7;
            this.opacityMapInfoLabel.Text = "Przetwarzanie w toku...";
            this.opacityMapInfoLabel.Visible = false;
            // 
            // createOpacityMapButton
            // 
            this.createOpacityMapButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.createOpacityMapButton.Location = new System.Drawing.Point(18, 16);
            this.createOpacityMapButton.Name = "createOpacityMapButton";
            this.createOpacityMapButton.Size = new System.Drawing.Size(325, 23);
            this.createOpacityMapButton.TabIndex = 6;
            this.createOpacityMapButton.Text = "Utwórz mapę przezroczystości";
            this.createOpacityMapButton.UseVisualStyleBackColor = false;
            this.createOpacityMapButton.Click += new System.EventHandler(this.createOpacityMapButton_Click);
            // 
            // getFilePanel
            // 
            this.getFilePanel.AutoScroll = true;
            this.getFilePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.getFilePanel.Controls.Add(this.filePathLabel);
            this.getFilePanel.Controls.Add(this.getFilePathButton);
            this.getFilePanel.Controls.Add(this.filePathTextBox);
            this.getFilePanel.Location = new System.Drawing.Point(6, 6);
            this.getFilePanel.Name = "getFilePanel";
            this.getFilePanel.Size = new System.Drawing.Size(360, 100);
            this.getFilePanel.TabIndex = 1;
            // 
            // filePathLabel
            // 
            this.filePathLabel.AutoSize = true;
            this.filePathLabel.Location = new System.Drawing.Point(16, 46);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(48, 13);
            this.filePathLabel.TabIndex = 3;
            this.filePathLabel.Text = "Ścieżka:";
            // 
            // getFilePathButton
            // 
            this.getFilePathButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.getFilePathButton.Location = new System.Drawing.Point(19, 16);
            this.getFilePathButton.Name = "getFilePathButton";
            this.getFilePathButton.Size = new System.Drawing.Size(325, 23);
            this.getFilePathButton.TabIndex = 2;
            this.getFilePathButton.Text = "Wczytaj plik PNG...";
            this.getFilePathButton.UseVisualStyleBackColor = false;
            this.getFilePathButton.Click += new System.EventHandler(this.getFilePathButton_Click);
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Location = new System.Drawing.Point(19, 62);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.ReadOnly = true;
            this.filePathTextBox.Size = new System.Drawing.Size(325, 20);
            this.filePathTextBox.TabIndex = 4;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.logoPictureBox.Image = global::ArgbConverter.Properties.Resources.ARGBus;
            this.logoPictureBox.Location = new System.Drawing.Point(129, 1);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(152, 88);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 1;
            this.logoPictureBox.TabStop = false;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTabControl.Controls.Add(this.quickToolsTabPage);
            this.mainTabControl.Controls.Add(this.advancedToolsTabPage);
            this.mainTabControl.Location = new System.Drawing.Point(13, 98);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(379, 206);
            this.mainTabControl.TabIndex = 6;
            // 
            // quickToolsTabPage
            // 
            this.quickToolsTabPage.AutoScroll = true;
            this.quickToolsTabPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.quickToolsTabPage.Controls.Add(this.getFilePanel);
            this.quickToolsTabPage.Controls.Add(this.opacityMapToolPanel);
            this.quickToolsTabPage.Location = new System.Drawing.Point(4, 22);
            this.quickToolsTabPage.Name = "quickToolsTabPage";
            this.quickToolsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.quickToolsTabPage.Size = new System.Drawing.Size(371, 180);
            this.quickToolsTabPage.TabIndex = 0;
            this.quickToolsTabPage.Text = "Szybkie przetwarzanie";
            // 
            // advancedToolsTabPage
            // 
            this.advancedToolsTabPage.AutoScroll = true;
            this.advancedToolsTabPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.advancedToolsTabPage.Controls.Add(this.hairTexGeneratorPanel);
            this.advancedToolsTabPage.Location = new System.Drawing.Point(4, 22);
            this.advancedToolsTabPage.Name = "advancedToolsTabPage";
            this.advancedToolsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.advancedToolsTabPage.Size = new System.Drawing.Size(371, 180);
            this.advancedToolsTabPage.TabIndex = 1;
            this.advancedToolsTabPage.Text = "Narzędzia zaawansowane";
            // 
            // hairTexGeneratorPanel
            // 
            this.hairTexGeneratorPanel.AutoScroll = true;
            this.hairTexGeneratorPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.hairTexGeneratorPanel.Controls.Add(this.hairTexGeneratorButton);
            this.hairTexGeneratorPanel.Location = new System.Drawing.Point(7, 7);
            this.hairTexGeneratorPanel.Name = "hairTexGeneratorPanel";
            this.hairTexGeneratorPanel.Size = new System.Drawing.Size(358, 58);
            this.hairTexGeneratorPanel.TabIndex = 100;
            // 
            // hairTexGeneratorButton
            // 
            this.hairTexGeneratorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.hairTexGeneratorButton.Location = new System.Drawing.Point(18, 18);
            this.hairTexGeneratorButton.Name = "hairTexGeneratorButton";
            this.hairTexGeneratorButton.Size = new System.Drawing.Size(324, 23);
            this.hairTexGeneratorButton.TabIndex = 101;
            this.hairTexGeneratorButton.Text = "Generator tekstury włosów";
            this.hairTexGeneratorButton.UseVisualStyleBackColor = false;
            this.hairTexGeneratorButton.Click += new System.EventHandler(this.hairTexGeneratorButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(406, 316);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.logoPictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ARGBus";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.opacityMapToolPanel.ResumeLayout(false);
            this.opacityMapToolPanel.PerformLayout();
            this.getFilePanel.ResumeLayout(false);
            this.getFilePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.mainTabControl.ResumeLayout(false);
            this.quickToolsTabPage.ResumeLayout(false);
            this.advancedToolsTabPage.ResumeLayout(false);
            this.hairTexGeneratorPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel getFilePanel;
        private System.Windows.Forms.Label filePathLabel;
        private System.Windows.Forms.Button getFilePathButton;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Panel opacityMapToolPanel;
        private System.Windows.Forms.Button createOpacityMapButton;
        private System.Windows.Forms.Label opacityMapInfoLabel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage quickToolsTabPage;
        private System.Windows.Forms.TabPage advancedToolsTabPage;
        private System.Windows.Forms.Panel hairTexGeneratorPanel;
        private System.Windows.Forms.Button hairTexGeneratorButton;
    }
}

