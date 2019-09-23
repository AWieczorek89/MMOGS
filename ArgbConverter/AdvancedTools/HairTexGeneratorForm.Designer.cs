namespace ArgbConverter.AdvancedTools
{
    partial class HairTexGeneratorForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HairTexGeneratorForm));
            this.mainPanel = new System.Windows.Forms.Panel();
            this.editPanelMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.prevSplitContainer = new System.Windows.Forms.SplitContainer();
            this.hairMapPreviewLabel = new System.Windows.Forms.Label();
            this.hairMapPreviewPanel = new System.Windows.Forms.Panel();
            this.hairMapPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.resultPreviewLabel = new System.Windows.Forms.Label();
            this.resultPreviewPanel = new System.Windows.Forms.Panel();
            this.resultPictureBox = new System.Windows.Forms.PictureBox();
            this.loadFilePanel = new System.Windows.Forms.Panel();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.info2Label = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.deadZoneRangeBNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.deadZoneRangeGNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.deadZoneRangeRNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.deadZoneRangeLabel = new System.Windows.Forms.Label();
            this.saveResultImageButton = new System.Windows.Forms.Button();
            this.loadFileButton = new System.Windows.Forms.Button();
            this.asyncInfoTimer = new System.Windows.Forms.Timer(this.components);
            this.boundsCheckerCheckBox = new System.Windows.Forms.CheckBox();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editPanelMainSplitContainer)).BeginInit();
            this.editPanelMainSplitContainer.Panel1.SuspendLayout();
            this.editPanelMainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.prevSplitContainer)).BeginInit();
            this.prevSplitContainer.Panel1.SuspendLayout();
            this.prevSplitContainer.Panel2.SuspendLayout();
            this.prevSplitContainer.SuspendLayout();
            this.hairMapPreviewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hairMapPreviewPictureBox)).BeginInit();
            this.resultPreviewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultPictureBox)).BeginInit();
            this.loadFilePanel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deadZoneRangeBNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deadZoneRangeGNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deadZoneRangeRNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoScroll = true;
            this.mainPanel.Controls.Add(this.editPanelMainSplitContainer);
            this.mainPanel.Controls.Add(this.loadFilePanel);
            this.mainPanel.Location = new System.Drawing.Point(12, 12);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(879, 732);
            this.mainPanel.TabIndex = 0;
            // 
            // editPanelMainSplitContainer
            // 
            this.editPanelMainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editPanelMainSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editPanelMainSplitContainer.Location = new System.Drawing.Point(4, 65);
            this.editPanelMainSplitContainer.Name = "editPanelMainSplitContainer";
            this.editPanelMainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // editPanelMainSplitContainer.Panel1
            // 
            this.editPanelMainSplitContainer.Panel1.AutoScroll = true;
            this.editPanelMainSplitContainer.Panel1.Controls.Add(this.prevSplitContainer);
            // 
            // editPanelMainSplitContainer.Panel2
            // 
            this.editPanelMainSplitContainer.Panel2.AutoScroll = true;
            this.editPanelMainSplitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editPanelMainSplitContainer.Size = new System.Drawing.Size(870, 661);
            this.editPanelMainSplitContainer.SplitterDistance = 439;
            this.editPanelMainSplitContainer.TabIndex = 3;
            // 
            // prevSplitContainer
            // 
            this.prevSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prevSplitContainer.BackColor = System.Drawing.Color.Silver;
            this.prevSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.prevSplitContainer.Location = new System.Drawing.Point(4, 3);
            this.prevSplitContainer.Name = "prevSplitContainer";
            // 
            // prevSplitContainer.Panel1
            // 
            this.prevSplitContainer.Panel1.Controls.Add(this.hairMapPreviewLabel);
            this.prevSplitContainer.Panel1.Controls.Add(this.hairMapPreviewPanel);
            // 
            // prevSplitContainer.Panel2
            // 
            this.prevSplitContainer.Panel2.Controls.Add(this.resultPreviewLabel);
            this.prevSplitContainer.Panel2.Controls.Add(this.resultPreviewPanel);
            this.prevSplitContainer.Size = new System.Drawing.Size(860, 431);
            this.prevSplitContainer.SplitterDistance = 426;
            this.prevSplitContainer.TabIndex = 2;
            // 
            // hairMapPreviewLabel
            // 
            this.hairMapPreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hairMapPreviewLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.hairMapPreviewLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hairMapPreviewLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hairMapPreviewLabel.Location = new System.Drawing.Point(4, 4);
            this.hairMapPreviewLabel.Name = "hairMapPreviewLabel";
            this.hairMapPreviewLabel.Size = new System.Drawing.Size(415, 19);
            this.hairMapPreviewLabel.TabIndex = 11;
            this.hairMapPreviewLabel.Text = "Podgląd mapy rozłożenia";
            this.hairMapPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hairMapPreviewPanel
            // 
            this.hairMapPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hairMapPreviewPanel.AutoScroll = true;
            this.hairMapPreviewPanel.BackColor = System.Drawing.Color.White;
            this.hairMapPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hairMapPreviewPanel.Controls.Add(this.hairMapPreviewPictureBox);
            this.hairMapPreviewPanel.Location = new System.Drawing.Point(4, 26);
            this.hairMapPreviewPanel.Name = "hairMapPreviewPanel";
            this.hairMapPreviewPanel.Size = new System.Drawing.Size(415, 398);
            this.hairMapPreviewPanel.TabIndex = 12;
            // 
            // hairMapPreviewPictureBox
            // 
            this.hairMapPreviewPictureBox.Location = new System.Drawing.Point(3, 3);
            this.hairMapPreviewPictureBox.Name = "hairMapPreviewPictureBox";
            this.hairMapPreviewPictureBox.Size = new System.Drawing.Size(407, 390);
            this.hairMapPreviewPictureBox.TabIndex = 0;
            this.hairMapPreviewPictureBox.TabStop = false;
            // 
            // resultPreviewLabel
            // 
            this.resultPreviewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultPreviewLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.resultPreviewLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultPreviewLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.resultPreviewLabel.Location = new System.Drawing.Point(3, 4);
            this.resultPreviewLabel.Name = "resultPreviewLabel";
            this.resultPreviewLabel.Size = new System.Drawing.Size(420, 19);
            this.resultPreviewLabel.TabIndex = 13;
            this.resultPreviewLabel.Text = "Rezultat";
            this.resultPreviewLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // resultPreviewPanel
            // 
            this.resultPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultPreviewPanel.AutoScroll = true;
            this.resultPreviewPanel.BackColor = System.Drawing.Color.White;
            this.resultPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultPreviewPanel.Controls.Add(this.resultPictureBox);
            this.resultPreviewPanel.Location = new System.Drawing.Point(3, 26);
            this.resultPreviewPanel.Name = "resultPreviewPanel";
            this.resultPreviewPanel.Size = new System.Drawing.Size(420, 398);
            this.resultPreviewPanel.TabIndex = 14;
            // 
            // resultPictureBox
            // 
            this.resultPictureBox.Location = new System.Drawing.Point(3, 3);
            this.resultPictureBox.Name = "resultPictureBox";
            this.resultPictureBox.Size = new System.Drawing.Size(407, 390);
            this.resultPictureBox.TabIndex = 0;
            this.resultPictureBox.TabStop = false;
            // 
            // loadFilePanel
            // 
            this.loadFilePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadFilePanel.AutoScroll = true;
            this.loadFilePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.loadFilePanel.Controls.Add(this.boundsCheckerCheckBox);
            this.loadFilePanel.Controls.Add(this.infoPanel);
            this.loadFilePanel.Controls.Add(this.deadZoneRangeBNumericUpDown);
            this.loadFilePanel.Controls.Add(this.deadZoneRangeGNumericUpDown);
            this.loadFilePanel.Controls.Add(this.deadZoneRangeRNumericUpDown);
            this.loadFilePanel.Controls.Add(this.deadZoneRangeLabel);
            this.loadFilePanel.Controls.Add(this.saveResultImageButton);
            this.loadFilePanel.Controls.Add(this.loadFileButton);
            this.loadFilePanel.Location = new System.Drawing.Point(4, 4);
            this.loadFilePanel.Name = "loadFilePanel";
            this.loadFilePanel.Size = new System.Drawing.Size(870, 55);
            this.loadFilePanel.TabIndex = 0;
            // 
            // infoPanel
            // 
            this.infoPanel.AutoScroll = true;
            this.infoPanel.BackColor = System.Drawing.Color.White;
            this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.infoPanel.Controls.Add(this.info2Label);
            this.infoPanel.Controls.Add(this.infoLabel);
            this.infoPanel.Location = new System.Drawing.Point(549, 3);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(316, 49);
            this.infoPanel.TabIndex = 8;
            // 
            // info2Label
            // 
            this.info2Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.info2Label.AutoSize = true;
            this.info2Label.ForeColor = System.Drawing.Color.Black;
            this.info2Label.Location = new System.Drawing.Point(11, 25);
            this.info2Label.Name = "info2Label";
            this.info2Label.Size = new System.Drawing.Size(72, 13);
            this.info2Label.TabIndex = 10;
            this.info2Label.Text = "- wiadomość -";
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoLabel.AutoSize = true;
            this.infoLabel.ForeColor = System.Drawing.Color.Black;
            this.infoLabel.Location = new System.Drawing.Point(11, 7);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(72, 13);
            this.infoLabel.TabIndex = 9;
            this.infoLabel.Text = "- wiadomość -";
            // 
            // deadZoneRangeBNumericUpDown
            // 
            this.deadZoneRangeBNumericUpDown.Location = new System.Drawing.Point(485, 10);
            this.deadZoneRangeBNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.deadZoneRangeBNumericUpDown.Name = "deadZoneRangeBNumericUpDown";
            this.deadZoneRangeBNumericUpDown.Size = new System.Drawing.Size(54, 20);
            this.deadZoneRangeBNumericUpDown.TabIndex = 6;
            this.deadZoneRangeBNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // deadZoneRangeGNumericUpDown
            // 
            this.deadZoneRangeGNumericUpDown.Location = new System.Drawing.Point(425, 10);
            this.deadZoneRangeGNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.deadZoneRangeGNumericUpDown.Name = "deadZoneRangeGNumericUpDown";
            this.deadZoneRangeGNumericUpDown.Size = new System.Drawing.Size(54, 20);
            this.deadZoneRangeGNumericUpDown.TabIndex = 5;
            this.deadZoneRangeGNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // deadZoneRangeRNumericUpDown
            // 
            this.deadZoneRangeRNumericUpDown.Location = new System.Drawing.Point(365, 10);
            this.deadZoneRangeRNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.deadZoneRangeRNumericUpDown.Name = "deadZoneRangeRNumericUpDown";
            this.deadZoneRangeRNumericUpDown.Size = new System.Drawing.Size(54, 20);
            this.deadZoneRangeRNumericUpDown.TabIndex = 4;
            this.deadZoneRangeRNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // deadZoneRangeLabel
            // 
            this.deadZoneRangeLabel.AutoSize = true;
            this.deadZoneRangeLabel.Location = new System.Drawing.Point(200, 12);
            this.deadZoneRangeLabel.Name = "deadZoneRangeLabel";
            this.deadZoneRangeLabel.Size = new System.Drawing.Size(159, 13);
            this.deadZoneRangeLabel.TabIndex = 3;
            this.deadZoneRangeLabel.Text = "Tolerancja martwej strefy (RGB):";
            // 
            // saveResultImageButton
            // 
            this.saveResultImageButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.saveResultImageButton.Location = new System.Drawing.Point(15, 30);
            this.saveResultImageButton.Name = "saveResultImageButton";
            this.saveResultImageButton.Size = new System.Drawing.Size(166, 23);
            this.saveResultImageButton.TabIndex = 2;
            this.saveResultImageButton.Text = "Zapisz rezultat...";
            this.saveResultImageButton.UseVisualStyleBackColor = false;
            this.saveResultImageButton.Click += new System.EventHandler(this.saveResultImageButton_Click);
            // 
            // loadFileButton
            // 
            this.loadFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.loadFileButton.Location = new System.Drawing.Point(15, 7);
            this.loadFileButton.Name = "loadFileButton";
            this.loadFileButton.Size = new System.Drawing.Size(166, 23);
            this.loadFileButton.TabIndex = 1;
            this.loadFileButton.Text = "Wczytaj obraz...";
            this.loadFileButton.UseVisualStyleBackColor = false;
            this.loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // asyncInfoTimer
            // 
            this.asyncInfoTimer.Interval = 1000;
            this.asyncInfoTimer.Tick += new System.EventHandler(this.asyncInfoTimer_Tick);
            // 
            // boundsCheckerCheckBox
            // 
            this.boundsCheckerCheckBox.AutoSize = true;
            this.boundsCheckerCheckBox.Location = new System.Drawing.Point(203, 34);
            this.boundsCheckerCheckBox.Name = "boundsCheckerCheckBox";
            this.boundsCheckerCheckBox.Size = new System.Drawing.Size(227, 17);
            this.boundsCheckerCheckBox.TabIndex = 7;
            this.boundsCheckerCheckBox.Text = "Ramka wpływu końcowego przetwarzania";
            this.boundsCheckerCheckBox.UseVisualStyleBackColor = true;
            // 
            // HairTexGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(903, 750);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HairTexGeneratorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generator tekstury włosów";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HairTexGeneratorForm_FormClosing);
            this.Load += new System.EventHandler(this.HairTexGeneratorForm_Load);
            this.mainPanel.ResumeLayout(false);
            this.editPanelMainSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.editPanelMainSplitContainer)).EndInit();
            this.editPanelMainSplitContainer.ResumeLayout(false);
            this.prevSplitContainer.Panel1.ResumeLayout(false);
            this.prevSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.prevSplitContainer)).EndInit();
            this.prevSplitContainer.ResumeLayout(false);
            this.hairMapPreviewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hairMapPreviewPictureBox)).EndInit();
            this.resultPreviewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultPictureBox)).EndInit();
            this.loadFilePanel.ResumeLayout(false);
            this.loadFilePanel.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deadZoneRangeBNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deadZoneRangeGNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deadZoneRangeRNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel loadFilePanel;
        private System.Windows.Forms.Button loadFileButton;
        private System.Windows.Forms.Label resultPreviewLabel;
        private System.Windows.Forms.Label hairMapPreviewLabel;
        private System.Windows.Forms.Panel resultPreviewPanel;
        private System.Windows.Forms.Panel hairMapPreviewPanel;
        private System.Windows.Forms.PictureBox resultPictureBox;
        private System.Windows.Forms.PictureBox hairMapPreviewPictureBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.NumericUpDown deadZoneRangeBNumericUpDown;
        private System.Windows.Forms.NumericUpDown deadZoneRangeGNumericUpDown;
        private System.Windows.Forms.NumericUpDown deadZoneRangeRNumericUpDown;
        private System.Windows.Forms.Label deadZoneRangeLabel;
        private System.Windows.Forms.SplitContainer prevSplitContainer;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label info2Label;
        private System.Windows.Forms.Timer asyncInfoTimer;
        private System.Windows.Forms.SplitContainer editPanelMainSplitContainer;
        private System.Windows.Forms.Button saveResultImageButton;
        private System.Windows.Forms.CheckBox boundsCheckerCheckBox;
    }
}