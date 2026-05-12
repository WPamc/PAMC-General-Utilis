using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ExcelMapper.Data;
using ExcelMapper.Models;
using ExcelMapper.Services;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelMapper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadSources();
        }

        private void LoadSources()
        {
            var sources = HardcodedSourceRegistry.GetSources();
            // Insert a dummy source at the top for guidance
            sources.Insert(0, new SqlSource("--- Select SQL Source ---", new List<string>()));
            
            cmbSources.DataSource = sources;
            lblStatus.Text = "Please select a SQL Source to begin.";
        }

        private void cmbSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSources.SelectedItem is SqlSource selectedSource)
            {
                lstFields.Items.Clear();
                foreach (var column in selectedSource.Columns)
                {
                    lstFields.Items.Add(column);
                }

                if (cmbSources.SelectedIndex > 0)
                {
                    lblStatus.Text = $"Source '{selectedSource.Name}' loaded with {selectedSource.Columns.Count} fields.";
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtExcelPath.Text = ofd.FileName;
                    LoadExcelHeaders(ofd.FileName);
                }
            }
        }

        private void LoadExcelHeaders(string filePath)
        {
            try
            {
                lstExcelHeaders.Items.Clear();
                dgvMapping.Rows.Clear();
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (IWorkbook workbook = new XSSFWorkbook(file))
                    {
                        ISheet sheet = workbook.GetSheetAt(0);
                        var headers = ExcelSchemaService.DetectHeaders(sheet);

                        foreach (var header in headers)
                        {
                            lstExcelHeaders.Items.Add(header);
                        }
                    }
                }
                lblStatus.Text = $"Excel Template loaded: {lstExcelHeaders.Items.Count} headers found.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading Excel file.";
                MessageBox.Show($"Error reading Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstExcelHeaders_DoubleClick(object sender, EventArgs e)
        {
            if (lstExcelHeaders.SelectedItem != null)
            {
                string header = lstExcelHeaders.SelectedItem.ToString();
                
                // Check if already in grid
                foreach (DataGridViewRow row in dgvMapping.Rows)
                {
                    if (row.Cells[0].Value?.ToString() == header) return;
                }

                dgvMapping.Rows.Add(header, "");
                lblStatus.Text = $"Added '{header}' to mapping.";
            }
        }

        private void lstFields_DoubleClick(object sender, EventArgs e)
        {
            if (lstFields.SelectedItem != null && dgvMapping.CurrentRow != null)
            {
                string field = lstFields.SelectedItem.ToString();
                var cell = dgvMapping.CurrentRow.Cells[1];
                string currentVal = cell.Value?.ToString() ?? "";

                if (string.IsNullOrEmpty(currentVal))
                {
                    cell.Value = $"{{{field}}}";
                }
                else
                {
                    cell.Value = $"{currentVal} {{{field}}}";
                }
                lblStatus.Text = $"Appended '{field}' to mapping.";
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            List<string> mappings = new List<string>();

            foreach (DataGridViewRow row in dgvMapping.Rows)
            {
                string header = row.Cells[0].Value?.ToString();
                string expression = row.Cells[1].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(header) && !string.IsNullOrWhiteSpace(expression))
                {
                    mappings.Add($"{header}={expression}");
                }
            }

            txtOutput.Text = string.Join("; ", mappings);
            lblStatus.Text = "Mapping DSL generated successfully.";
            Clipboard.SetText(txtOutput.Text);
            MessageBox.Show("Mapping string copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string defaultTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Reporting2026-Export-Template.xlsx");
            if (File.Exists(defaultTemplatePath))
            {
                txtExcelPath.Text = defaultTemplatePath;
                LoadExcelHeaders(defaultTemplatePath);
                lblStatus.Text = "Default template loaded automatically.";
            }
        }
    }
}
