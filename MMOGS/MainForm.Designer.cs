namespace MMOGS
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.overviewTabPage = new System.Windows.Forms.TabPage();
            this.serverManagementGroupBox = new System.Windows.Forms.GroupBox();
            this.startServerButton = new System.Windows.Forms.Button();
            this.logGroupBox = new System.Windows.Forms.GroupBox();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.performanceGroupBox = new System.Windows.Forms.GroupBox();
            this.perfRamTextBox = new System.Windows.Forms.TextBox();
            this.perfCpuTextBox = new System.Windows.Forms.TextBox();
            this.perfRamLabel = new System.Windows.Forms.Label();
            this.perfCpuLabel = new System.Windows.Forms.Label();
            this.settingsTabPage = new System.Windows.Forms.TabPage();
            this.tcpConnGroupBox = new System.Windows.Forms.GroupBox();
            this.tcpConnSaveButton = new System.Windows.Forms.Button();
            this.tcpPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tcpIpTextBox = new System.Windows.Forms.TextBox();
            this.tcpPortLabel = new System.Windows.Forms.Label();
            this.tcpIpLabel = new System.Windows.Forms.Label();
            this.mySqlConnGroupBox = new System.Windows.Forms.GroupBox();
            this.mySqlConnCheckButton = new System.Windows.Forms.Button();
            this.mySqlConnSaveButton = new System.Windows.Forms.Button();
            this.mySqlPassTextBox = new System.Windows.Forms.TextBox();
            this.mySqlUserTextBox = new System.Windows.Forms.TextBox();
            this.mySqlBaseTextBox = new System.Windows.Forms.TextBox();
            this.mySqlPortNmrUpDown = new System.Windows.Forms.NumericUpDown();
            this.mySqlServerTextBox = new System.Windows.Forms.TextBox();
            this.mySqlPassLabel = new System.Windows.Forms.Label();
            this.mySqlUserLabel = new System.Windows.Forms.Label();
            this.mySqlBaseLabel = new System.Windows.Forms.Label();
            this.mySqlPortLabel = new System.Windows.Forms.Label();
            this.mySqlServerLabel = new System.Windows.Forms.Label();
            this.toolsTabPage = new System.Windows.Forms.TabPage();
            this.testButton = new System.Windows.Forms.Button();
            this.accountCreatorGroupBox = new System.Windows.Forms.GroupBox();
            this.newAccButton = new System.Windows.Forms.Button();
            this.newAccAccessLevelNmrUpDown = new System.Windows.Forms.NumericUpDown();
            this.newAccAccessLevelLabel = new System.Windows.Forms.Label();
            this.newAccPassTextBox = new System.Windows.Forms.TextBox();
            this.newAccLoginTextBox = new System.Windows.Forms.TextBox();
            this.newAccPassLabel = new System.Windows.Forms.Label();
            this.newAccLoginLabel = new System.Windows.Forms.Label();
            this.mySqlDataInstallerToolGroupBox = new System.Windows.Forms.GroupBox();
            this.mySqlInstallerButton = new System.Windows.Forms.Button();
            this.mySqlInstallerCreateDataCheckBox = new System.Windows.Forms.CheckBox();
            this.mySqlInstallerCreateTablesCheckBox = new System.Windows.Forms.CheckBox();
            this.mainTabControl.SuspendLayout();
            this.overviewTabPage.SuspendLayout();
            this.serverManagementGroupBox.SuspendLayout();
            this.logGroupBox.SuspendLayout();
            this.performanceGroupBox.SuspendLayout();
            this.settingsTabPage.SuspendLayout();
            this.tcpConnGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPortNumericUpDown)).BeginInit();
            this.mySqlConnGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mySqlPortNmrUpDown)).BeginInit();
            this.toolsTabPage.SuspendLayout();
            this.accountCreatorGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newAccAccessLevelNmrUpDown)).BeginInit();
            this.mySqlDataInstallerToolGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTabControl.Controls.Add(this.overviewTabPage);
            this.mainTabControl.Controls.Add(this.settingsTabPage);
            this.mainTabControl.Controls.Add(this.toolsTabPage);
            this.mainTabControl.Location = new System.Drawing.Point(13, 13);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(846, 546);
            this.mainTabControl.TabIndex = 0;
            // 
            // overviewTabPage
            // 
            this.overviewTabPage.AutoScroll = true;
            this.overviewTabPage.Controls.Add(this.serverManagementGroupBox);
            this.overviewTabPage.Controls.Add(this.logGroupBox);
            this.overviewTabPage.Controls.Add(this.performanceGroupBox);
            this.overviewTabPage.Location = new System.Drawing.Point(4, 22);
            this.overviewTabPage.Name = "overviewTabPage";
            this.overviewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.overviewTabPage.Size = new System.Drawing.Size(838, 520);
            this.overviewTabPage.TabIndex = 0;
            this.overviewTabPage.Text = "Overview";
            this.overviewTabPage.UseVisualStyleBackColor = true;
            // 
            // serverManagementGroupBox
            // 
            this.serverManagementGroupBox.Controls.Add(this.startServerButton);
            this.serverManagementGroupBox.Location = new System.Drawing.Point(6, 111);
            this.serverManagementGroupBox.Name = "serverManagementGroupBox";
            this.serverManagementGroupBox.Size = new System.Drawing.Size(201, 99);
            this.serverManagementGroupBox.TabIndex = 2;
            this.serverManagementGroupBox.TabStop = false;
            this.serverManagementGroupBox.Text = "Server management";
            // 
            // startServerButton
            // 
            this.startServerButton.Location = new System.Drawing.Point(21, 29);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(164, 23);
            this.startServerButton.TabIndex = 0;
            this.startServerButton.Text = "START";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.startServerButton_Click);
            // 
            // logGroupBox
            // 
            this.logGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logGroupBox.Controls.Add(this.logListBox);
            this.logGroupBox.Location = new System.Drawing.Point(213, 6);
            this.logGroupBox.Name = "logGroupBox";
            this.logGroupBox.Size = new System.Drawing.Size(619, 508);
            this.logGroupBox.TabIndex = 1;
            this.logGroupBox.TabStop = false;
            this.logGroupBox.Text = "Log";
            // 
            // logListBox
            // 
            this.logListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logListBox.BackColor = System.Drawing.Color.Black;
            this.logListBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.logListBox.ForeColor = System.Drawing.Color.White;
            this.logListBox.FormattingEnabled = true;
            this.logListBox.HorizontalExtent = 2000;
            this.logListBox.HorizontalScrollbar = true;
            this.logListBox.ItemHeight = 14;
            this.logListBox.Location = new System.Drawing.Point(6, 19);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(607, 438);
            this.logListBox.TabIndex = 0;
            this.logListBox.DoubleClick += new System.EventHandler(this.logListBox_DoubleClick);
            // 
            // performanceGroupBox
            // 
            this.performanceGroupBox.Controls.Add(this.perfRamTextBox);
            this.performanceGroupBox.Controls.Add(this.perfCpuTextBox);
            this.performanceGroupBox.Controls.Add(this.perfRamLabel);
            this.performanceGroupBox.Controls.Add(this.perfCpuLabel);
            this.performanceGroupBox.Location = new System.Drawing.Point(6, 6);
            this.performanceGroupBox.Name = "performanceGroupBox";
            this.performanceGroupBox.Size = new System.Drawing.Size(201, 99);
            this.performanceGroupBox.TabIndex = 0;
            this.performanceGroupBox.TabStop = false;
            this.performanceGroupBox.Text = "Performance";
            // 
            // perfRamTextBox
            // 
            this.perfRamTextBox.Location = new System.Drawing.Point(68, 58);
            this.perfRamTextBox.Name = "perfRamTextBox";
            this.perfRamTextBox.ReadOnly = true;
            this.perfRamTextBox.Size = new System.Drawing.Size(117, 20);
            this.perfRamTextBox.TabIndex = 1;
            // 
            // perfCpuTextBox
            // 
            this.perfCpuTextBox.Location = new System.Drawing.Point(68, 29);
            this.perfCpuTextBox.Name = "perfCpuTextBox";
            this.perfCpuTextBox.ReadOnly = true;
            this.perfCpuTextBox.Size = new System.Drawing.Size(117, 20);
            this.perfCpuTextBox.TabIndex = 1;
            // 
            // perfRamLabel
            // 
            this.perfRamLabel.AutoSize = true;
            this.perfRamLabel.Location = new System.Drawing.Point(18, 61);
            this.perfRamLabel.Name = "perfRamLabel";
            this.perfRamLabel.Size = new System.Drawing.Size(31, 13);
            this.perfRamLabel.TabIndex = 0;
            this.perfRamLabel.Text = "RAM";
            // 
            // perfCpuLabel
            // 
            this.perfCpuLabel.AutoSize = true;
            this.perfCpuLabel.Location = new System.Drawing.Point(18, 32);
            this.perfCpuLabel.Name = "perfCpuLabel";
            this.perfCpuLabel.Size = new System.Drawing.Size(29, 13);
            this.perfCpuLabel.TabIndex = 0;
            this.perfCpuLabel.Text = "CPU";
            // 
            // settingsTabPage
            // 
            this.settingsTabPage.AutoScroll = true;
            this.settingsTabPage.Controls.Add(this.tcpConnGroupBox);
            this.settingsTabPage.Controls.Add(this.mySqlConnGroupBox);
            this.settingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.settingsTabPage.Name = "settingsTabPage";
            this.settingsTabPage.Size = new System.Drawing.Size(838, 520);
            this.settingsTabPage.TabIndex = 1;
            this.settingsTabPage.Text = "Settings";
            this.settingsTabPage.UseVisualStyleBackColor = true;
            // 
            // tcpConnGroupBox
            // 
            this.tcpConnGroupBox.Controls.Add(this.tcpConnSaveButton);
            this.tcpConnGroupBox.Controls.Add(this.tcpPortNumericUpDown);
            this.tcpConnGroupBox.Controls.Add(this.tcpIpTextBox);
            this.tcpConnGroupBox.Controls.Add(this.tcpPortLabel);
            this.tcpConnGroupBox.Controls.Add(this.tcpIpLabel);
            this.tcpConnGroupBox.Location = new System.Drawing.Point(3, 234);
            this.tcpConnGroupBox.Name = "tcpConnGroupBox";
            this.tcpConnGroupBox.Size = new System.Drawing.Size(284, 118);
            this.tcpConnGroupBox.TabIndex = 1;
            this.tcpConnGroupBox.TabStop = false;
            this.tcpConnGroupBox.Text = "TCP connection";
            // 
            // tcpConnSaveButton
            // 
            this.tcpConnSaveButton.Location = new System.Drawing.Point(112, 81);
            this.tcpConnSaveButton.Name = "tcpConnSaveButton";
            this.tcpConnSaveButton.Size = new System.Drawing.Size(156, 23);
            this.tcpConnSaveButton.TabIndex = 3;
            this.tcpConnSaveButton.Text = "Save";
            this.tcpConnSaveButton.UseVisualStyleBackColor = true;
            this.tcpConnSaveButton.Click += new System.EventHandler(this.tcpConnSaveButton_Click);
            // 
            // tcpPortNumericUpDown
            // 
            this.tcpPortNumericUpDown.Location = new System.Drawing.Point(112, 55);
            this.tcpPortNumericUpDown.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.tcpPortNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tcpPortNumericUpDown.Name = "tcpPortNumericUpDown";
            this.tcpPortNumericUpDown.Size = new System.Drawing.Size(156, 20);
            this.tcpPortNumericUpDown.TabIndex = 2;
            this.tcpPortNumericUpDown.Value = new decimal(new int[] {
            2222,
            0,
            0,
            0});
            // 
            // tcpIpTextBox
            // 
            this.tcpIpTextBox.Location = new System.Drawing.Point(112, 29);
            this.tcpIpTextBox.Name = "tcpIpTextBox";
            this.tcpIpTextBox.Size = new System.Drawing.Size(156, 20);
            this.tcpIpTextBox.TabIndex = 1;
            this.tcpIpTextBox.Text = "127.0.0.1";
            // 
            // tcpPortLabel
            // 
            this.tcpPortLabel.AutoSize = true;
            this.tcpPortLabel.Location = new System.Drawing.Point(17, 57);
            this.tcpPortLabel.Name = "tcpPortLabel";
            this.tcpPortLabel.Size = new System.Drawing.Size(29, 13);
            this.tcpPortLabel.TabIndex = 0;
            this.tcpPortLabel.Text = "Port:";
            // 
            // tcpIpLabel
            // 
            this.tcpIpLabel.AutoSize = true;
            this.tcpIpLabel.Location = new System.Drawing.Point(17, 32);
            this.tcpIpLabel.Name = "tcpIpLabel";
            this.tcpIpLabel.Size = new System.Drawing.Size(20, 13);
            this.tcpIpLabel.TabIndex = 0;
            this.tcpIpLabel.Text = "IP:";
            // 
            // mySqlConnGroupBox
            // 
            this.mySqlConnGroupBox.Controls.Add(this.mySqlConnCheckButton);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlConnSaveButton);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlPassTextBox);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlUserTextBox);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlBaseTextBox);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlPortNmrUpDown);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlServerTextBox);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlPassLabel);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlUserLabel);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlBaseLabel);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlPortLabel);
            this.mySqlConnGroupBox.Controls.Add(this.mySqlServerLabel);
            this.mySqlConnGroupBox.Location = new System.Drawing.Point(3, 3);
            this.mySqlConnGroupBox.Name = "mySqlConnGroupBox";
            this.mySqlConnGroupBox.Size = new System.Drawing.Size(284, 225);
            this.mySqlConnGroupBox.TabIndex = 0;
            this.mySqlConnGroupBox.TabStop = false;
            this.mySqlConnGroupBox.Text = "MySQL connection";
            // 
            // mySqlConnCheckButton
            // 
            this.mySqlConnCheckButton.Location = new System.Drawing.Point(112, 158);
            this.mySqlConnCheckButton.Name = "mySqlConnCheckButton";
            this.mySqlConnCheckButton.Size = new System.Drawing.Size(156, 23);
            this.mySqlConnCheckButton.TabIndex = 5;
            this.mySqlConnCheckButton.Text = "Check connection";
            this.mySqlConnCheckButton.UseVisualStyleBackColor = true;
            this.mySqlConnCheckButton.Click += new System.EventHandler(this.mySqlConnCheckButton_Click);
            // 
            // mySqlConnSaveButton
            // 
            this.mySqlConnSaveButton.Location = new System.Drawing.Point(112, 187);
            this.mySqlConnSaveButton.Name = "mySqlConnSaveButton";
            this.mySqlConnSaveButton.Size = new System.Drawing.Size(156, 23);
            this.mySqlConnSaveButton.TabIndex = 4;
            this.mySqlConnSaveButton.Text = "Save";
            this.mySqlConnSaveButton.UseVisualStyleBackColor = true;
            this.mySqlConnSaveButton.Click += new System.EventHandler(this.mySqlConnSaveButton_Click);
            // 
            // mySqlPassTextBox
            // 
            this.mySqlPassTextBox.Location = new System.Drawing.Point(112, 132);
            this.mySqlPassTextBox.MaxLength = 50;
            this.mySqlPassTextBox.Name = "mySqlPassTextBox";
            this.mySqlPassTextBox.PasswordChar = '#';
            this.mySqlPassTextBox.Size = new System.Drawing.Size(156, 20);
            this.mySqlPassTextBox.TabIndex = 3;
            // 
            // mySqlUserTextBox
            // 
            this.mySqlUserTextBox.Location = new System.Drawing.Point(112, 106);
            this.mySqlUserTextBox.MaxLength = 50;
            this.mySqlUserTextBox.Name = "mySqlUserTextBox";
            this.mySqlUserTextBox.Size = new System.Drawing.Size(156, 20);
            this.mySqlUserTextBox.TabIndex = 3;
            // 
            // mySqlBaseTextBox
            // 
            this.mySqlBaseTextBox.Location = new System.Drawing.Point(112, 80);
            this.mySqlBaseTextBox.MaxLength = 50;
            this.mySqlBaseTextBox.Name = "mySqlBaseTextBox";
            this.mySqlBaseTextBox.Size = new System.Drawing.Size(156, 20);
            this.mySqlBaseTextBox.TabIndex = 3;
            // 
            // mySqlPortNmrUpDown
            // 
            this.mySqlPortNmrUpDown.Location = new System.Drawing.Point(112, 54);
            this.mySqlPortNmrUpDown.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.mySqlPortNmrUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mySqlPortNmrUpDown.Name = "mySqlPortNmrUpDown";
            this.mySqlPortNmrUpDown.Size = new System.Drawing.Size(156, 20);
            this.mySqlPortNmrUpDown.TabIndex = 2;
            this.mySqlPortNmrUpDown.Value = new decimal(new int[] {
            3306,
            0,
            0,
            0});
            // 
            // mySqlServerTextBox
            // 
            this.mySqlServerTextBox.Location = new System.Drawing.Point(112, 28);
            this.mySqlServerTextBox.MaxLength = 50;
            this.mySqlServerTextBox.Name = "mySqlServerTextBox";
            this.mySqlServerTextBox.Size = new System.Drawing.Size(156, 20);
            this.mySqlServerTextBox.TabIndex = 1;
            // 
            // mySqlPassLabel
            // 
            this.mySqlPassLabel.AutoSize = true;
            this.mySqlPassLabel.Location = new System.Drawing.Point(17, 135);
            this.mySqlPassLabel.Name = "mySqlPassLabel";
            this.mySqlPassLabel.Size = new System.Drawing.Size(33, 13);
            this.mySqlPassLabel.TabIndex = 0;
            this.mySqlPassLabel.Text = "Pass:";
            // 
            // mySqlUserLabel
            // 
            this.mySqlUserLabel.AutoSize = true;
            this.mySqlUserLabel.Location = new System.Drawing.Point(17, 109);
            this.mySqlUserLabel.Name = "mySqlUserLabel";
            this.mySqlUserLabel.Size = new System.Drawing.Size(32, 13);
            this.mySqlUserLabel.TabIndex = 0;
            this.mySqlUserLabel.Text = "User:";
            // 
            // mySqlBaseLabel
            // 
            this.mySqlBaseLabel.AutoSize = true;
            this.mySqlBaseLabel.Location = new System.Drawing.Point(17, 83);
            this.mySqlBaseLabel.Name = "mySqlBaseLabel";
            this.mySqlBaseLabel.Size = new System.Drawing.Size(56, 13);
            this.mySqlBaseLabel.TabIndex = 0;
            this.mySqlBaseLabel.Text = "Database:";
            // 
            // mySqlPortLabel
            // 
            this.mySqlPortLabel.AutoSize = true;
            this.mySqlPortLabel.Location = new System.Drawing.Point(17, 56);
            this.mySqlPortLabel.Name = "mySqlPortLabel";
            this.mySqlPortLabel.Size = new System.Drawing.Size(29, 13);
            this.mySqlPortLabel.TabIndex = 0;
            this.mySqlPortLabel.Text = "Port:";
            // 
            // mySqlServerLabel
            // 
            this.mySqlServerLabel.AutoSize = true;
            this.mySqlServerLabel.Location = new System.Drawing.Point(17, 31);
            this.mySqlServerLabel.Name = "mySqlServerLabel";
            this.mySqlServerLabel.Size = new System.Drawing.Size(41, 13);
            this.mySqlServerLabel.TabIndex = 0;
            this.mySqlServerLabel.Text = "Server:";
            // 
            // toolsTabPage
            // 
            this.toolsTabPage.AutoScroll = true;
            this.toolsTabPage.Controls.Add(this.testButton);
            this.toolsTabPage.Controls.Add(this.accountCreatorGroupBox);
            this.toolsTabPage.Controls.Add(this.mySqlDataInstallerToolGroupBox);
            this.toolsTabPage.Location = new System.Drawing.Point(4, 22);
            this.toolsTabPage.Name = "toolsTabPage";
            this.toolsTabPage.Size = new System.Drawing.Size(838, 520);
            this.toolsTabPage.TabIndex = 2;
            this.toolsTabPage.Text = "Tools";
            this.toolsTabPage.UseVisualStyleBackColor = true;
            // 
            // testButton
            // 
            this.testButton.Enabled = false;
            this.testButton.Location = new System.Drawing.Point(33, 466);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 2;
            this.testButton.Text = "TEST";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Visible = false;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // accountCreatorGroupBox
            // 
            this.accountCreatorGroupBox.Controls.Add(this.newAccButton);
            this.accountCreatorGroupBox.Controls.Add(this.newAccAccessLevelNmrUpDown);
            this.accountCreatorGroupBox.Controls.Add(this.newAccAccessLevelLabel);
            this.accountCreatorGroupBox.Controls.Add(this.newAccPassTextBox);
            this.accountCreatorGroupBox.Controls.Add(this.newAccLoginTextBox);
            this.accountCreatorGroupBox.Controls.Add(this.newAccPassLabel);
            this.accountCreatorGroupBox.Controls.Add(this.newAccLoginLabel);
            this.accountCreatorGroupBox.Location = new System.Drawing.Point(4, 134);
            this.accountCreatorGroupBox.Name = "accountCreatorGroupBox";
            this.accountCreatorGroupBox.Size = new System.Drawing.Size(288, 166);
            this.accountCreatorGroupBox.TabIndex = 1;
            this.accountCreatorGroupBox.TabStop = false;
            this.accountCreatorGroupBox.Text = "Account creation";
            // 
            // newAccButton
            // 
            this.newAccButton.Location = new System.Drawing.Point(102, 128);
            this.newAccButton.Name = "newAccButton";
            this.newAccButton.Size = new System.Drawing.Size(154, 23);
            this.newAccButton.TabIndex = 4;
            this.newAccButton.Text = "Create new account";
            this.newAccButton.UseVisualStyleBackColor = true;
            this.newAccButton.Click += new System.EventHandler(this.newAccButton_Click);
            // 
            // newAccAccessLevelNmrUpDown
            // 
            this.newAccAccessLevelNmrUpDown.Location = new System.Drawing.Point(102, 85);
            this.newAccAccessLevelNmrUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.newAccAccessLevelNmrUpDown.Name = "newAccAccessLevelNmrUpDown";
            this.newAccAccessLevelNmrUpDown.Size = new System.Drawing.Size(154, 20);
            this.newAccAccessLevelNmrUpDown.TabIndex = 3;
            // 
            // newAccAccessLevelLabel
            // 
            this.newAccAccessLevelLabel.AutoSize = true;
            this.newAccAccessLevelLabel.Location = new System.Drawing.Point(16, 87);
            this.newAccAccessLevelLabel.Name = "newAccAccessLevelLabel";
            this.newAccAccessLevelLabel.Size = new System.Drawing.Size(70, 13);
            this.newAccAccessLevelLabel.TabIndex = 2;
            this.newAccAccessLevelLabel.Text = "Access level:";
            // 
            // newAccPassTextBox
            // 
            this.newAccPassTextBox.Location = new System.Drawing.Point(102, 58);
            this.newAccPassTextBox.MaxLength = 20;
            this.newAccPassTextBox.Name = "newAccPassTextBox";
            this.newAccPassTextBox.Size = new System.Drawing.Size(154, 20);
            this.newAccPassTextBox.TabIndex = 1;
            // 
            // newAccLoginTextBox
            // 
            this.newAccLoginTextBox.Location = new System.Drawing.Point(102, 32);
            this.newAccLoginTextBox.MaxLength = 50;
            this.newAccLoginTextBox.Name = "newAccLoginTextBox";
            this.newAccLoginTextBox.Size = new System.Drawing.Size(154, 20);
            this.newAccLoginTextBox.TabIndex = 1;
            // 
            // newAccPassLabel
            // 
            this.newAccPassLabel.AutoSize = true;
            this.newAccPassLabel.Location = new System.Drawing.Point(16, 61);
            this.newAccPassLabel.Name = "newAccPassLabel";
            this.newAccPassLabel.Size = new System.Drawing.Size(33, 13);
            this.newAccPassLabel.TabIndex = 0;
            this.newAccPassLabel.Text = "Pass:";
            // 
            // newAccLoginLabel
            // 
            this.newAccLoginLabel.AutoSize = true;
            this.newAccLoginLabel.Location = new System.Drawing.Point(16, 35);
            this.newAccLoginLabel.Name = "newAccLoginLabel";
            this.newAccLoginLabel.Size = new System.Drawing.Size(36, 13);
            this.newAccLoginLabel.TabIndex = 0;
            this.newAccLoginLabel.Text = "Login:";
            // 
            // mySqlDataInstallerToolGroupBox
            // 
            this.mySqlDataInstallerToolGroupBox.Controls.Add(this.mySqlInstallerButton);
            this.mySqlDataInstallerToolGroupBox.Controls.Add(this.mySqlInstallerCreateDataCheckBox);
            this.mySqlDataInstallerToolGroupBox.Controls.Add(this.mySqlInstallerCreateTablesCheckBox);
            this.mySqlDataInstallerToolGroupBox.Location = new System.Drawing.Point(4, 4);
            this.mySqlDataInstallerToolGroupBox.Name = "mySqlDataInstallerToolGroupBox";
            this.mySqlDataInstallerToolGroupBox.Size = new System.Drawing.Size(288, 123);
            this.mySqlDataInstallerToolGroupBox.TabIndex = 0;
            this.mySqlDataInstallerToolGroupBox.TabStop = false;
            this.mySqlDataInstallerToolGroupBox.Text = "MySQL data";
            // 
            // mySqlInstallerButton
            // 
            this.mySqlInstallerButton.Location = new System.Drawing.Point(102, 85);
            this.mySqlInstallerButton.Name = "mySqlInstallerButton";
            this.mySqlInstallerButton.Size = new System.Drawing.Size(154, 23);
            this.mySqlInstallerButton.TabIndex = 1;
            this.mySqlInstallerButton.Text = "Start";
            this.mySqlInstallerButton.UseVisualStyleBackColor = true;
            this.mySqlInstallerButton.Click += new System.EventHandler(this.mySqlInstallerButton_Click);
            // 
            // mySqlInstallerCreateDataCheckBox
            // 
            this.mySqlInstallerCreateDataCheckBox.AutoSize = true;
            this.mySqlInstallerCreateDataCheckBox.Location = new System.Drawing.Point(16, 52);
            this.mySqlInstallerCreateDataCheckBox.Name = "mySqlInstallerCreateDataCheckBox";
            this.mySqlInstallerCreateDataCheckBox.Size = new System.Drawing.Size(109, 17);
            this.mySqlInstallerCreateDataCheckBox.TabIndex = 0;
            this.mySqlInstallerCreateDataCheckBox.Text = "Create basic data";
            this.mySqlInstallerCreateDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // mySqlInstallerCreateTablesCheckBox
            // 
            this.mySqlInstallerCreateTablesCheckBox.AutoSize = true;
            this.mySqlInstallerCreateTablesCheckBox.Location = new System.Drawing.Point(16, 29);
            this.mySqlInstallerCreateTablesCheckBox.Name = "mySqlInstallerCreateTablesCheckBox";
            this.mySqlInstallerCreateTablesCheckBox.Size = new System.Drawing.Size(88, 17);
            this.mySqlInstallerCreateTablesCheckBox.TabIndex = 0;
            this.mySqlInstallerCreateTablesCheckBox.Text = "Create tables";
            this.mySqlInstallerCreateTablesCheckBox.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(871, 571);
            this.Controls.Add(this.mainTabControl);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MMOGS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainTabControl.ResumeLayout(false);
            this.overviewTabPage.ResumeLayout(false);
            this.serverManagementGroupBox.ResumeLayout(false);
            this.logGroupBox.ResumeLayout(false);
            this.performanceGroupBox.ResumeLayout(false);
            this.performanceGroupBox.PerformLayout();
            this.settingsTabPage.ResumeLayout(false);
            this.tcpConnGroupBox.ResumeLayout(false);
            this.tcpConnGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPortNumericUpDown)).EndInit();
            this.mySqlConnGroupBox.ResumeLayout(false);
            this.mySqlConnGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mySqlPortNmrUpDown)).EndInit();
            this.toolsTabPage.ResumeLayout(false);
            this.accountCreatorGroupBox.ResumeLayout(false);
            this.accountCreatorGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newAccAccessLevelNmrUpDown)).EndInit();
            this.mySqlDataInstallerToolGroupBox.ResumeLayout(false);
            this.mySqlDataInstallerToolGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage overviewTabPage;
        private System.Windows.Forms.GroupBox performanceGroupBox;
        private System.Windows.Forms.TextBox perfRamTextBox;
        private System.Windows.Forms.TextBox perfCpuTextBox;
        private System.Windows.Forms.Label perfRamLabel;
        private System.Windows.Forms.Label perfCpuLabel;
        private System.Windows.Forms.GroupBox logGroupBox;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.TabPage settingsTabPage;
        private System.Windows.Forms.GroupBox mySqlConnGroupBox;
        private System.Windows.Forms.TextBox mySqlPassTextBox;
        private System.Windows.Forms.TextBox mySqlUserTextBox;
        private System.Windows.Forms.TextBox mySqlBaseTextBox;
        private System.Windows.Forms.NumericUpDown mySqlPortNmrUpDown;
        private System.Windows.Forms.TextBox mySqlServerTextBox;
        private System.Windows.Forms.Label mySqlPassLabel;
        private System.Windows.Forms.Label mySqlUserLabel;
        private System.Windows.Forms.Label mySqlBaseLabel;
        private System.Windows.Forms.Label mySqlPortLabel;
        private System.Windows.Forms.Label mySqlServerLabel;
        private System.Windows.Forms.Button mySqlConnCheckButton;
        private System.Windows.Forms.Button mySqlConnSaveButton;
        private System.Windows.Forms.TabPage toolsTabPage;
        private System.Windows.Forms.GroupBox mySqlDataInstallerToolGroupBox;
        private System.Windows.Forms.Button mySqlInstallerButton;
        private System.Windows.Forms.CheckBox mySqlInstallerCreateDataCheckBox;
        private System.Windows.Forms.CheckBox mySqlInstallerCreateTablesCheckBox;
        private System.Windows.Forms.GroupBox accountCreatorGroupBox;
        private System.Windows.Forms.Button newAccButton;
        private System.Windows.Forms.NumericUpDown newAccAccessLevelNmrUpDown;
        private System.Windows.Forms.Label newAccAccessLevelLabel;
        private System.Windows.Forms.TextBox newAccPassTextBox;
        private System.Windows.Forms.TextBox newAccLoginTextBox;
        private System.Windows.Forms.Label newAccPassLabel;
        private System.Windows.Forms.Label newAccLoginLabel;
        private System.Windows.Forms.GroupBox serverManagementGroupBox;
        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.GroupBox tcpConnGroupBox;
        private System.Windows.Forms.Button tcpConnSaveButton;
        private System.Windows.Forms.NumericUpDown tcpPortNumericUpDown;
        private System.Windows.Forms.TextBox tcpIpTextBox;
        private System.Windows.Forms.Label tcpPortLabel;
        private System.Windows.Forms.Label tcpIpLabel;
    }
}

