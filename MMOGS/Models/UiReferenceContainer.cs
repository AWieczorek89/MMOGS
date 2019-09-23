using System.Windows.Forms;

namespace MMOGS.Models
{
    public class UiReferenceContainer
    {
        public TextBox CpuUsageTextBox { get; private set; } = null;
        public TextBox RamUsageTextBox { get; private set; } = null;
        public ListBox LogListBox { get; private set; } = null;
        public TextBox MySqlServerTextBox { get; private set; } = null;
        public NumericUpDown MySqlPortNumericUpDown { get; private set; } = null;
        public TextBox MySqlDatabaseTextBox { get; private set; } = null;
        public TextBox MySqlUserTextBox { get; private set; } = null;
        public TextBox MySqlPassTextBox { get; private set; } = null;
        public TabControl MainTabControl { get; private set; } = null;
        public CheckBox MySqlInstallerCreateTablesCheckBox { get; private set; } = null;
        public CheckBox MySqlInstallerCreateDataCheckBox { get; private set; } = null;
        public TextBox NewAccountLoginTextBox { get; private set; } = null;
        public TextBox NewAccountPassTextBox { get; private set; } = null;
        public NumericUpDown NewAccountAccessLevelNumericUpDown { get; private set; } = null;
        public Button NewAccountButton { get; private set; } = null;
        public TextBox TcpIpTextBox { get; private set; } = null;
        public NumericUpDown TcpPortNumericUpDown { get; private set; } = null;

        public UiReferenceContainer
        (
            TextBox cpuUsageTextBox,
            TextBox ramUsageTextBox,
            ListBox logListBox,
            TextBox mySqlServerTextBox,
            NumericUpDown mySqlPortNumericUpDown,
            TextBox mySqlDatabaseTextBox,
            TextBox mySqlUserTextBox,
            TextBox mySqlPassTextBox,
            TabControl mainTabControl,
            CheckBox mySqlInstallerCreateTablesCheckBox,
            CheckBox mySqlInstallerCreateDataCheckBox,
            TextBox newAccountLoginTextBox,
            TextBox newAccountPassTextBox,
            NumericUpDown newAccountAccessLevelNmrUpDown,
            Button newAccountButton,
            TextBox tcpIpTextBox,
            NumericUpDown tcpPortNumericUpDown
        )
        {
            this.CpuUsageTextBox = cpuUsageTextBox;
            this.RamUsageTextBox = ramUsageTextBox;
            this.LogListBox = logListBox;
            this.MySqlServerTextBox = mySqlServerTextBox;
            this.MySqlPortNumericUpDown = mySqlPortNumericUpDown;
            this.MySqlDatabaseTextBox = mySqlDatabaseTextBox;
            this.MySqlUserTextBox = mySqlUserTextBox;
            this.MySqlPassTextBox = mySqlPassTextBox;
            this.MainTabControl = mainTabControl;
            this.MySqlInstallerCreateTablesCheckBox = mySqlInstallerCreateTablesCheckBox;
            this.MySqlInstallerCreateDataCheckBox = mySqlInstallerCreateDataCheckBox;
            this.NewAccountLoginTextBox = newAccountLoginTextBox;
            this.NewAccountPassTextBox = newAccountPassTextBox;
            this.NewAccountAccessLevelNumericUpDown = newAccountAccessLevelNmrUpDown;
            this.NewAccountButton = newAccountButton;
            this.TcpIpTextBox = tcpIpTextBox;
            this.TcpPortNumericUpDown = tcpPortNumericUpDown;
        }
    }
}
