using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Serilog;
using Serilog.Events;

namespace Filereader
{
    public partial class LogSettingsForm : Form
    {
        private ComboBox cbLogLevel;
        private TextBox txtLogPath;
        private Button btnBrowse;
        private Button btnSave;
        private Button btnCancel;

        public LogSettingsForm()
        {
            InitializeComponent();
            InitializeCustomControls();
            LoadCurrentSettings();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(450, 200);
            this.Text = "Logging Configuration";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeCustomControls()
        {
            Panel rootPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 2,
                Height = 65 // Reduced height to accommodate 30px rows
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            // Explicitly set row heights to 30px for tighter vertical alignment
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            table.Controls.Add(new Label { Text = "Log Level:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 0);
            cbLogLevel = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 3, 0, 3) };
            cbLogLevel.Items.AddRange(new object[] { "Verbose", "Debug", "Information", "Warning", "Error", "Fatal" });
            table.Controls.Add(cbLogLevel, 1, 0);

            table.Controls.Add(new Label { Text = "Log Path:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 1);
            Panel pathPanel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            txtLogPath = new TextBox { Location = new System.Drawing.Point(0, 3), Width = 250 };
            btnBrowse = new Button { 
                Text = "...", 
                Width = 30, 
                Height = 23, 
                Location = new System.Drawing.Point(260, 3), 
                Anchor = AnchorStyles.Top | AnchorStyles.Left // Using manual positioning within pathPanel for precise centering
            };
            btnBrowse.Click += BtnBrowse_Click;
            pathPanel.Controls.Add(txtLogPath);
            pathPanel.Controls.Add(btnBrowse);
            table.Controls.Add(pathPanel, 1, 1);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40 };
            btnCancel = new Button { Text = "Cancel", Width = 80 };
            btnCancel.Click += (s, e) => this.Close();
            btnSave = new Button { Text = "Save", Width = 80 };
            btnSave.Click += BtnSave_Click;

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnSave);

            rootPanel.Controls.Add(table);
            rootPanel.Controls.Add(buttonPanel);
            this.Controls.Add(rootPanel);
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                string level = config.AppSettings.Settings["serilog:minimum-level"]?.Value ?? "Debug";
                cbLogLevel.SelectedItem = level;

                string path = config.AppSettings.Settings["serilog:write-to:File.path"]?.Value ?? "logs/log-.txt";
                txtLogPath.Text = path;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load log settings from App.config");
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Select Log File Pattern";
                dialog.Filter = "Log files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.FileName = "log-.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtLogPath.Text = dialog.FileName;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                config.AppSettings.Settings["serilog:minimum-level"].Value = cbLogLevel.SelectedItem.ToString();
                config.AppSettings.Settings["serilog:write-to:File.path"].Value = txtLogPath.Text;
                
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                Log.Information("Logging configuration updated. Level: {Level}, Path: {Path}", cbLogLevel.SelectedItem, txtLogPath.Text);
                MessageBox.Show("Settings saved successfully. Please restart the application for changes to take full effect.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save log settings to App.config");
                MessageBox.Show("Error saving settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
