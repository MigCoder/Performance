using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Cloo;

namespace RingClip
{
    public partial class OpenClSetting : Form
    {
        public OpenClSetting()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;


            Device = null;

            var platforms = ComputePlatform.Platforms;

            if (platforms.Count == 0)
            {
                platformComboBox.Enabled = false;
                deviceComboBox.Enabled = false;
                okButton.Enabled = false;

                messageLabel.Text = "There's no OpenCL platform installed on this computer";

                return;
            }

            platformComboBox.Items.AddRange(platforms.ToArray());
            platformComboBox.SelectedIndex = 0;
        }

        private void OpenClSetting_Load(object sender, EventArgs e)
        {
        }

        private void OpenClSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Device == null)
            {
                Application.Exit();
            }

        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void platformComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (platformComboBox.SelectedIndex < 0)
            {
                deviceComboBox.Enabled = false;
                deviceComboBox.Items.Clear();

                okButton.Enabled = false;
                vendorNameLabel.Text = "(unknown)";
                computeUnitLabel.Text = "(unknown)";
                deviceTypeLabel.Text = "(unknown)";

                return;
            }

            var platform = platformComboBox.SelectedItem as ComputePlatform;

            vendorNameLabel.Text = platform.Vendor;

            var devices = platform.Devices;

            if (devices.Count == 0)
            {
                deviceComboBox.Enabled = false;
                deviceComboBox.Items.Clear();

                okButton.Enabled = false;
                vendorNameLabel.Text = "(unknown)";
                computeUnitLabel.Text = "(unknown)";
                deviceTypeLabel.Text = "(unknown)";

                messageLabel.Text = "There's no compute device available on this platform.";

                return;
            }

            deviceComboBox.Items.Clear();
            deviceComboBox.Items.AddRange(devices.ToArray());
            deviceComboBox.SelectedIndex = 0;
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deviceComboBox.SelectedIndex < 0)
            {

                okButton.Enabled = false;
                computeUnitLabel.Text = "(unknown)";
                deviceTypeLabel.Text = "(unknown)";

                return;
            }

            var device = deviceComboBox.SelectedItem as ComputeDevice;

            computeUnitLabel.Text = device.MaxComputeUnits.ToString();
            deviceTypeLabel.Text = device.Type.ToString();

            okButton.Enabled = true;

            //query float point capability
        }

        public ComputePlatform Platform;
        public ComputeDevice Device;

        public int Count = 50;
        public int FillW = 100;
        public int FillH = 100;
        public bool UseGdi = false;

        public Size WindowSize;
        public SizeMode SizeMode;

        private void okButton_Click(object sender, EventArgs e)
        {
            Platform = platformComboBox.SelectedItem as ComputePlatform;
            Device = deviceComboBox.SelectedItem as ComputeDevice;

            Count = Convert.ToInt32(txt_Count.Text);
            FillW = Convert.ToInt32(txt_FillW.Text);
            FillH = Convert.ToInt32(txt_FillH.Text);

            if (size_Small.Checked) WindowSize = new Size(600, 600);
            if (size_Middle.Checked) WindowSize = new Size(1400, 900);
            if (size_Big.Checked) WindowSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            UseGdi = ck_useGdi.Checked;

            Close();
        }
    }

    public enum SizeMode
    {
        Small,
        Middle,
        Big
    }
}
