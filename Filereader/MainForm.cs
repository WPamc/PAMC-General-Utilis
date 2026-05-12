using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileHelpers;
using Krypton.Toolkit;
using PAMC.Formats.Medihelp;
using Serilog;
using Toastr.Winforms;

namespace Filereader
{
    public partial class MainForm : Form
    {
        private bool _isInitializing = true;
        private List<FormatConfig> _configs = new List<FormatConfig>();
        private FormatConfig _activeConfig;
        private List<PAMC.Formats.FormatInfo> _allFormats = new List<PAMC.Formats.FormatInfo>();
        private KryptonDataGridView _masterGrid;
        private KryptonDataGridView _detailGrid;
        private BindingSource _claimsSource;
        private BindingSource _detailSource;

        // Filter controls
        private KryptonTextBox txtMasterFilter;
        private ContextMenuStrip menuFilterColumns;
        private ToolStripCheckedListBox _columnSelector;
        private ToolStripTextBox _txtColumnFilter;
        private List<string> _allMasterColumns = new List<string>();
        private HashSet<string> _checkedColumns = new HashSet<string>();
        private ToolStripStatusLabel lblStatus;

        public MainForm()
        {
            Log.Information("Initializing MainForm...");
            InitializeComponent();
            // this.AutoScaleMode = AutoScaleMode.Dpi;
            this.StartPosition = FormStartPosition.Manual; // Required for Location restoration
            ConfigureMasterDetailGrid();

            lblStatus = new ToolStripStatusLabel { Text = "Ready" };
            statusStrip1.Items.Add(lblStatus);

            // 1. Attach event handlers first
            txtPath.TextChanged += txtPath_TextChanged;
            tvFormats.BeforeExpand += tvFormats_BeforeExpand;
            tvFormats.AfterSelect += tvFormats_AfterSelect;
            cbFormat.SelectedIndexChanged += (s, e) => { if (!_isInitializing) UpdateColumnSelector(); };
            
            // Splitter persistence
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            int savedDistance = Properties.Settings.Default.SplitterDistance;
            if (savedDistance > 0 && savedDistance < this.Width)
            {
                splitContainer1.SplitterDistance = savedDistance;
            }
            splitContainer1.SplitterMoved += (s, e) => {
                if (!_isInitializing)
                {
                    Properties.Settings.Default.SplitterDistance = splitContainer1.SplitterDistance;
                    Properties.Settings.Default.Save();
                }
            };

            // Form state persistence
            this.Shown += (s, e) => {
                new Toast(this, ToastrPosition.TopRight).ShowSuccess("Ready! Please select a file from the directory tree to begin.");
            };
            this.Load += (s, e) => {
                LoadFormState();
            };
            this.ResizeEnd += (s, e) => SaveFormState();
            this.LocationChanged += (s, e) => SaveFormState();
            this.FormClosing += (s, e) => SaveFormState();

            // 2. Initial data population
            LoadConfigurations();
            txtPath.Text = LoadLastPathSetting(); // Triggers txtPath_TextChanged correctly
            PopulateFormatDropdown();             // Triggers SelectedIndexChanged, but guarded

            // 3. Manual final sync for initialization
            UpdateColumnSelector();
            
            _isInitializing = false;
            Log.Information("MainForm initialized.");
        }

        private void LoadConfigurations()
        {
            try
            {
                string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "formats.json");
                if (System.IO.File.Exists(jsonPath))
                {
                    Log.Information("Loading configurations from {Path}", jsonPath);
                    string json = System.IO.File.ReadAllText(jsonPath);
                    var collection = Newtonsoft.Json.JsonConvert.DeserializeObject<FormatCollection>(json);
                    _configs = collection.Formats ?? new List<FormatConfig>();
                    Log.Information("Loaded {Count} formats.", _configs.Count);
                }
                else
                {
                    Log.Warning("formats.json not found at {Path}", jsonPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading formats.json");
                MessageBox.Show("Error loading formats.json: " + ex.Message);
            }
        }

        private void PopulateFormatDropdown()
        {
            cbFormat.Items.Clear();
            foreach (var config in _configs)
            {
                cbFormat.Items.Add(config.DisplayName);
            }
            if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = 0;
        }

        private void tvFormats_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            
            // Check if we have a dummy node
            if (node.Nodes.Count == 1 && node.Nodes[0].Tag == null && node.Nodes[0].Text == "...")
            {
                node.Nodes.Clear();
                string path = node.Tag as string;
                if (!string.IsNullOrEmpty(path) && System.IO.Directory.Exists(path))
                {
                    try
                    {
                        Log.Debug("Expanding directory: {Path}", path);
                        AddDirectoryContents(node, path);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to expand directory: {Path}", path);
                    }
                }
            }
        }

        private void tvFormats_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string path = e.Node.Tag as string;
            bool isFile = System.IO.File.Exists(path);
            txtSelectedFile.Text = isFile ? path : string.Empty;

            if (isFile && !_isInitializing)
            {
                new Toast(this, ToastrPosition.TopRight).ShowSuccess("File selected. Click View to parse!");
            }
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            SaveLastPathSetting(txtPath.Text);
            UpdateDirectoryList();

            if (!_isInitializing && System.IO.Directory.Exists(txtPath.Text))
            {
                new Toast(this, ToastrPosition.TopRight).ShowSuccess("Directory loaded: " + System.IO.Path.GetFileName(txtPath.Text));
            }
        }

        private string LoadLastPathSetting()
        {
            try
            {
                return Properties.Settings.Default.LastPath ?? string.Empty;
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
                return string.Empty;
            }
        }

        private void SaveLastPathSetting(string path)
        {
            try
            {
                Properties.Settings.Default.LastPath = path ?? string.Empty;
                Properties.Settings.Default.Save();
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
            }
        }

        private void UpdateDirectoryList()
        {
            tvFormats.Nodes.Clear();
            txtSelectedFile.Text = string.Empty;
            string path = txtPath.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (System.IO.Directory.Exists(path))
            {
                try
                {
                    var rootNode = new TreeNode(path);
                    rootNode.Tag = path;

                    AddDirectoryContents(rootNode, path);
                    
                    tvFormats.Nodes.Add(rootNode);
                    rootNode.Expand();
                }
                catch (Exception)
                {
                }
            }
        }

        private void AddDirectoryContents(TreeNode parentNode, string path)
        {
            parentNode.Nodes.Clear();

            foreach (var dir in System.IO.Directory.GetDirectories(path))
            {
                var dirNode = new TreeNode(System.IO.Path.GetFileName(dir));
                dirNode.Tag = dir;
                dirNode.Nodes.Add(new TreeNode("...") { Tag = null }); // Add dummy
                parentNode.Nodes.Add(dirNode);
            }

            foreach (var file in System.IO.Directory.GetFiles(path))
            {
                var fileNode = new TreeNode(System.IO.Path.GetFileName(file));
                fileNode.Tag = file;
                parentNode.Nodes.Add(fileNode);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (cbFormat.SelectedIndex < 0 || cbFormat.SelectedIndex >= _configs.Count)
            {
                ShowGridMessage("Format", "Select a supported target format.");
                return;
            }

            try
            {
                if (!System.IO.File.Exists(txtSelectedFile.Text))
                {
                    Log.Warning("Attempted to view non-existent file: {Path}", txtSelectedFile.Text);
                    ShowGridMessage("File", "Select an existing file before viewing.");
                    return;
                }

                _activeConfig = _configs[cbFormat.SelectedIndex];
                Log.Information("Viewing file: {FilePath} using format: {FormatName}", txtSelectedFile.Text, _activeConfig.DisplayName);

                var engine = CreateEngineFromConfig(_activeConfig);
                var res = engine.ReadFile(txtSelectedFile.Text);
                
                Log.Information("Successfully parsed {Count} records.", res.Length);
                BindRecordsToMasterDetailGrid(res, _activeConfig);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to view file: {Path}", txtSelectedFile.Text);
                ShowGridMessage(ex.GetType().Name, ex.Message);
            }
        }

        private void btnManageFormats_Click(object sender, EventArgs e)
        {
            new Toast(this, ToastrPosition.TopRight).ShowSuccess("Use the formats button to add or modify fields!");
            var selectedConfig = cbFormat.SelectedIndex >= 0 ? _configs[cbFormat.SelectedIndex] : null;
            
            // Phase 1: Implement Cloning for FormatConfig
            FormatConfig configToEdit;
            if (selectedConfig != null)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(selectedConfig);
                configToEdit = Newtonsoft.Json.JsonConvert.DeserializeObject<FormatConfig>(json);
            }
            else
            {
                configToEdit = new FormatConfig();
            }

            using (var wizard = new ConfigWizardForm(configToEdit))
            {
                if (wizard.ShowDialog() == DialogResult.OK)
                {
                    var result = wizard.GetResult();
                    
                    // Phase 1: Update MainForm Wizard Invocation
                    if (selectedConfig != null)
                    {
                        int index = _configs.IndexOf(selectedConfig);
                        _configs[index] = result;
                    }
                    else
                    {
                        _configs.Add(result);
                    }
                    
                    SaveConfigurations();
                    LoadConfigurations();
                    PopulateFormatDropdown();
                    
                    // Re-select the edited/added format
                    cbFormat.SelectedIndex = _configs.FindIndex(c => c.DisplayName == result.DisplayName);
                }
            }
        }

        private void btnLogSettings_Click(object sender, EventArgs e)
        {
            using (var settings = new LogSettingsForm())
            {
                settings.ShowDialog();
            }
        }

        private void SaveConfigurations()
        {
            try
            {
                string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "formats.json");
                Log.Information("Saving {Count} configurations to {Path}", _configs.Count, jsonPath);
                var collection = new FormatCollection { Formats = _configs };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(collection, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonPath, json);
                Log.Information("Configurations saved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving formats.json");
                MessageBox.Show("Error saving formats.json: " + ex.Message);
            }
        }

        private MultiRecordEngine CreateEngineFromConfig(FormatConfig config)
        {
            Log.Debug("Creating MultiRecordEngine for format: {FormatName}", config.DisplayName);
            var types = new List<Type>();
            foreach (var typeName in config.RecordTypeNames)
            {
                var t = Type.GetType(typeName);
                if (t == null)
                {
                    Log.Error("Record type '{TypeName}' could not be loaded.", typeName);
                    throw new Exception(string.Format("Record type '{0}' could not be loaded. Ensure the assembly name is included (e.g. 'Namespace.Type, Assembly').", typeName));
                }
                types.Add(t);
                Log.Debug("Added record type: {TypeName}", t.FullName);
            }

            var engine = new MultiRecordEngine(types.ToArray());

            if (!string.IsNullOrEmpty(config.SelectorTypeName) && !string.IsNullOrEmpty(config.SelectorMethodName))
            {
                Log.Debug("Binding record selector: {SelectorTypeName}.{MethodName}", config.SelectorTypeName, config.SelectorMethodName);
                var selectorType = Type.GetType(config.SelectorTypeName);
                var method = selectorType?.GetMethod(config.SelectorMethodName, BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    engine.RecordSelector = (RecordTypeSelector)Delegate.CreateDelegate(typeof(RecordTypeSelector), method);
                    Log.Debug("Selector bound successfully.");
                }
                else
                {
                    Log.Warning("Selector method '{MethodName}' not found in type '{TypeName}'.", config.SelectorMethodName, config.SelectorTypeName);
                }
            }

            return engine;
        }

        private void ConfigureMasterDetailGrid()
        {
            datagridPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            datagridPanel.Padding = new Padding(3);

            var split = new SplitContainer
            {
                Name = "masterDetailSplitContainer",
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = Math.Max(120, datagridPanel.ClientSize.Height / 2)
            };

            _masterGrid = new KryptonDataGridView
            {
                Name = "masterGrid",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
            };

            _detailGrid = new KryptonDataGridView
            {
                Name = "detailGrid",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
            };

            _claimsSource = new BindingSource();
            _detailSource = new BindingSource();
            _masterGrid.DataSource = _claimsSource;
            _detailGrid.DataSource = _detailSource;
            _masterGrid.SelectionChanged += MasterGrid_SelectionChanged;

            // --- Filter UI (Option B) ---
            var filterPanel = new Panel { Dock = DockStyle.Top, Height = 35, Padding = new Padding(5, 5, 5, 0) };
            
            txtMasterFilter = new KryptonTextBox { Width = 200, Dock = DockStyle.Left };
            txtMasterFilter.CueHint.CueHintText = "Search...";
            txtMasterFilter.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) ApplyMasterFilter(); };

            var btnSearchFilter = new Button { Text = "Search", Width = 60, Dock = DockStyle.Left, Margin = new Padding(5, 0, 0, 0) };
            btnSearchFilter.Click += (s, e) => ApplyMasterFilter();

            var btnClearFilter = new Button { Text = "Clear", Width = 60, Dock = DockStyle.Left, Margin = new Padding(5, 0, 0, 0) };
            btnClearFilter.Click += (s, e) => { txtMasterFilter.Text = string.Empty; ApplyMasterFilter(); };

            var btnFilterCols = new Button { Text = "Columns...", Width = 80, Dock = DockStyle.Left, Margin = new Padding(5, 0, 0, 0) };
            menuFilterColumns = new ContextMenuStrip();
            
            _txtColumnFilter = new ToolStripTextBox();
            _txtColumnFilter.AutoToolTip = true;
            _txtColumnFilter.ToolTipText = "Type to filter columns";
            _txtColumnFilter.TextChanged += (s, e) => FilterColumnList();
            menuFilterColumns.Items.Add(_txtColumnFilter);
            menuFilterColumns.Items.Add(new ToolStripSeparator());

            _columnSelector = new ToolStripCheckedListBox();
            _columnSelector.ItemCheck += (s, e) => { 
                string item = _columnSelector.Items[e.Index].ToString();
                if (e.NewValue == CheckState.Checked) _checkedColumns.Add(item);
                else _checkedColumns.Remove(item);

                // Delay slightly to let the check state update, but only if handle is created
                if (this.IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(ApplyMasterFilter)); 
                }
            };
            menuFilterColumns.Items.Add(_columnSelector);
            btnFilterCols.Click += (s, e) => {
                _txtColumnFilter.Text = string.Empty;
                FilterColumnList();
                menuFilterColumns.Show(btnFilterCols, 0, btnFilterCols.Height);
            };

            filterPanel.Controls.Add(new Label { Text = "Filter:", Dock = DockStyle.Left, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(0, 5, 0, 0) });
            filterPanel.Controls.Add(txtMasterFilter);
            filterPanel.Controls.Add(new Panel { Width = 5, Dock = DockStyle.Left }); // spacer
            filterPanel.Controls.Add(btnSearchFilter);
            filterPanel.Controls.Add(new Panel { Width = 5, Dock = DockStyle.Left }); // spacer
            filterPanel.Controls.Add(btnClearFilter);
            filterPanel.Controls.Add(new Panel { Width = 5, Dock = DockStyle.Left }); // spacer
            filterPanel.Controls.Add(btnFilterCols);

            split.Panel1.Controls.Add(_masterGrid);
            split.Panel1.Controls.Add(filterPanel); // Filter panel at top of master grid
            split.Panel2.Controls.Add(_detailGrid);
            datagridPanel.Controls.Add(split);
        }

        private void UpdateColumnSelector()
        {
            if (cbFormat.SelectedIndex < 0) return;
            var config = _configs[cbFormat.SelectedIndex];
            
            _allMasterColumns = config.MasterColumns.Select(c => c.Name).ToList();
            _checkedColumns = new HashSet<string>(_allMasterColumns); // Default all checked
            
            FilterColumnList();
        }

        private void FilterColumnList()
        {
            _columnSelector.Items.Clear();
            string filter = _txtColumnFilter.Text.ToLower();
            
            foreach (var col in _allMasterColumns)
            {
                if (string.IsNullOrEmpty(filter) || col.ToLower().Contains(filter))
                {
                    _columnSelector.Items.Add(col, _checkedColumns.Contains(col));
                }
            }
        }

        private void ApplyMasterFilter()
        {
            if (_claimsSource == null || _allMasterColumns.Count == 0) return;

            string searchText = txtMasterFilter.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                _claimsSource.Filter = null;
                if (lblStatus != null) lblStatus.Text = "Ready";
                return;
            }

            string escapedSearchText = searchText.Replace("'", "''");

            var selectedCols = _checkedColumns.ToList();
            if (selectedCols.Count == 0)
            {
                // If nothing selected, search across ALL master columns
                selectedCols = _allMasterColumns;
            }

            // Stage 1: Filter on selected columns
            var filterParts = new List<string>();
            foreach (var col in selectedCols)
            {
                filterParts.Add(string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", col, escapedSearchText));
            }

            _claimsSource.Filter = string.Join(" OR ", filterParts);

            // Stage 2: If nothing found, search entire file contents
            if (_claimsSource.Count == 0)
            {
                SearchEntireFileContents(searchText);
            }
            else
            {
                if (lblStatus != null) lblStatus.Text = string.Format("Found {0} records in selected columns.", _claimsSource.Count);
                // feedback as you type
                new Toast(this, ToastrPosition.TopRight).ShowSuccess(string.Format("Matches: {0}", _claimsSource.Count));
            }
        }

        private void SearchEntireFileContents(string searchText)
        {
            if (_claimsSource.DataSource == null || _activeConfig == null) return;

            DataTable claimsTable = _claimsSource.DataSource as DataTable;
            DataTable detailTable = _detailSource.DataSource as DataTable;
            if (claimsTable == null || detailTable == null) return;

            var foundColumns = new HashSet<string>();
            var matchingPKs = new HashSet<string>();
            var pkCol = _activeConfig.MasterColumns.FirstOrDefault(c => c.IsPrimaryKey);
            string pkName = pkCol?.Name;

            if (string.IsNullOrEmpty(pkName)) return;

            // Search Claims table
            foreach (DataRow row in claimsTable.Rows)
            {
                bool rowMatches = false;
                foreach (DataColumn col in claimsTable.Columns)
                {
                    if (row[col].ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundColumns.Add(col.ColumnName);
                        rowMatches = true;
                    }
                }
                if (rowMatches)
                {
                    matchingPKs.Add(row[pkName].ToString());
                }
            }

            // Search ServiceLines table
            foreach (DataRow row in detailTable.Rows)
            {
                bool rowMatches = false;
                foreach (DataColumn col in detailTable.Columns)
                {
                    if (row[col].ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundColumns.Add(col.ColumnName);
                        rowMatches = true;
                    }
                }
                if (rowMatches)
                {
                    matchingPKs.Add(row[pkName].ToString());
                }
            }

            if (matchingPKs.Count > 0)
            {
                string pkFilter = string.Join(",", matchingPKs.Select(pk => string.Format("'{0}'", pk.Replace("'", "''"))));
                _claimsSource.Filter = string.Format("[{0}] IN ({1})", pkName, pkFilter);
                
                string colList = string.Join(", ", foundColumns.OrderBy(c => c));
                if (lblStatus != null)
                {
                    lblStatus.Text = string.Format("Stage 2: Found in columns: {0}", colList);
                }
                new Toast(this, ToastrPosition.TopRight).ShowSuccess("Found in columns: " + colList);
            }
            else
            {
                _claimsSource.Filter = "1=0";
                if (lblStatus != null) lblStatus.Text = "No matches found in entire file.";
                new Toast(this, ToastrPosition.TopRight).ShowWarning("No matches found in entire file.");
            }
        }

        private void BindRecordsToMasterDetailGrid(object[] records, FormatConfig config)
        {
            DataSet dataSet = BuildDataSetFromConfig(records, config);

            // Re-sync checked columns if config changed
            if (_allMasterColumns.Count == 0 || !config.MasterColumns.Any(c => _allMasterColumns.Contains(c.Name)))
            {
                UpdateColumnSelector();
            }

            _claimsSource.DataSource = dataSet.Tables["Claims"];
            _detailSource.DataSource = dataSet.Tables["ServiceLines"];
            
            // Ensure case-insensitive searching
            dataSet.Tables["Claims"].CaseSensitive = false;
            dataSet.Tables["ServiceLines"].CaseSensitive = false;

            ApplyDetailFilter();
        }

        // Helper class for multi-select column dropdown
        [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.All)]
        public class ToolStripCheckedListBox : ToolStripControlHost
        {
            public ToolStripCheckedListBox() : base(new CheckedListBox { BorderStyle = BorderStyle.None, CheckOnClick = true }) { }
            public CheckedListBox CheckedListBox => Control as CheckedListBox;
            public CheckedListBox.ObjectCollection Items => CheckedListBox.Items;
            public CheckedListBox.CheckedItemCollection CheckedItems => CheckedListBox.CheckedItems;
            public event ItemCheckEventHandler ItemCheck { add => CheckedListBox.ItemCheck += value; remove => CheckedListBox.ItemCheck -= value; }
        }

        private void MasterGrid_SelectionChanged(object sender, EventArgs e)
        {
            ApplyDetailFilter();
        }

        private void ApplyDetailFilter()
        {
            if (_masterGrid.CurrentRow == null || _detailSource == null || _activeConfig == null)
            {
                return;
            }

            var pkCol = _activeConfig.MasterColumns.FirstOrDefault(c => c.IsPrimaryKey);
            if (pkCol != null && _masterGrid.Columns.Contains(pkCol.Name))
            {
                object value = _masterGrid.CurrentRow.Cells[pkCol.Name].Value;
                string filterValue = value == null ? string.Empty : value.ToString().Replace("'", "''");
                _detailSource.Filter = string.Format("{0} = '{1}'", pkCol.Name, filterValue);
            }
            else
            {
                _detailSource.Filter = "1=0"; // Hide all if no PK match
            }
        }

        private DataSet BuildDataSetFromConfig(object[] records, FormatConfig config)
        {
            var dataSet = new DataSet("DynamicData");
            var claims = CreateTableFromConfig("Claims", config.MasterColumns);
            var serviceLines = CreateServiceLinesTableFromConfig(config.CuratedFields, config.MasterColumns);
            dataSet.Tables.Add(claims);
            dataSet.Tables.Add(serviceLines);

            int lineNumber = 0;
            var pkCol = config.MasterColumns.FirstOrDefault(c => c.IsPrimaryKey);
            string pkName = pkCol?.Name;

            foreach (var record in records)
            {
                var recordType = record.GetType();
                lineNumber++;
                
                string pkString = string.Empty;
                if (!string.IsNullOrEmpty(pkName))
                {
                    var pkField = recordType.GetField(pkName, BindingFlags.Public | BindingFlags.Instance);
                    if (pkField != null)
                    {
                        object pkValue = pkField.GetValue(record);
                        pkString = pkValue?.ToString() ?? string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(pkString) && !claims.Rows.Contains(pkString))
                {
                    DataRow claimRow = claims.NewRow();
                    foreach (var col in config.MasterColumns)
                    {
                        var f = recordType.GetField(col.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (f != null) claimRow[col.Name] = f.GetValue(record);
                    }
                    claims.Rows.Add(claimRow);
                }

                DataRow lineRow = serviceLines.NewRow();
                lineRow["LineNumber"] = lineNumber;
                if (!string.IsNullOrEmpty(pkName)) lineRow[pkName] = pkString;

                foreach (string fieldName in config.CuratedFields)
                {
                    if (fieldName == pkName) continue;
                    lineRow[fieldName] = GetFieldValue(record, fieldName);
                }
                serviceLines.Rows.Add(lineRow);
            }

            return dataSet;
        }

        private DataTable CreateTableFromConfig(string tableName, List<ColumnConfig> columns)
        {
            var table = new DataTable(tableName);
            var pkCols = new List<DataColumn>();

            foreach (var col in columns)
            {
                var dc = new DataColumn(col.Name, Type.GetType(col.DataType) ?? typeof(string));
                table.Columns.Add(dc);
                if (col.IsPrimaryKey) pkCols.Add(dc);
            }

            if (pkCols.Count > 0) table.PrimaryKey = pkCols.ToArray();
            return table;
        }

        private DataTable CreateServiceLinesTableFromConfig(List<string> curatedFields, List<ColumnConfig> masterColumns)
        {
            var table = new DataTable("ServiceLines");
            table.Columns.Add("LineNumber", typeof(int));
            
            // Add PK column to detail table for filtering
            var pkCol = masterColumns.FirstOrDefault(c => c.IsPrimaryKey);
            if (pkCol != null && !table.Columns.Contains(pkCol.Name))
            {
                table.Columns.Add(pkCol.Name, typeof(string));
            }

            foreach (string field in curatedFields)
            {
                if (!table.Columns.Contains(field))
                {
                    table.Columns.Add(field, typeof(string));
                }
            }
            return table;
        }

        private void ShowGridMessage(string title, string message)
        {
            var claims = new DataTable("Claims");
            claims.Columns.Add("Status", typeof(string));
            claims.Columns.Add("Message", typeof(string));
            
            var serviceLines = new DataTable("ServiceLines");

            DataRow row = claims.NewRow();
            row["Status"] = title;
            row["Message"] = message;
            claims.Rows.Add(row);

            _claimsSource.DataSource = claims;
            _detailSource.DataSource = serviceLines;
            _activeConfig = null; // Reset active config on error/message
        }

        private string GetFieldValue(object record, string fieldName)
        {
            FieldInfo field = record.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            object value = field == null ? null : field.GetValue(record);
            return value == null ? string.Empty : value.ToString();
        }

        private void LoadFormState()
        {
            var settings = Properties.Settings.Default;
            if (settings.FormSize.Width > 100 && settings.FormSize.Height > 100)
            {
                this.Size = settings.FormSize;
            }

            if (settings.FormLocation.X >= 0 && settings.FormLocation.Y >= 0)
            {
                // Verify location is visible on some screen
                bool isVisible = false;
                foreach (var screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.Contains(settings.FormLocation))
                    {
                        isVisible = true;
                        break;
                    }
                }
                if (isVisible) this.Location = settings.FormLocation;
            }

            this.WindowState = settings.FormWindowState;
        }

        private void SaveFormState()
        {
            if (_isInitializing) return;

            var settings = Properties.Settings.Default;
            settings.FormWindowState = this.WindowState;

            if (this.WindowState == FormWindowState.Normal)
            {
                settings.FormSize = this.Size;
                settings.FormLocation = this.Location;
            }
            else
            {
                // If maximized/minimized, save the RestoreBounds
                settings.FormSize = this.RestoreBounds.Size;
                settings.FormLocation = this.RestoreBounds.Location;
            }

            settings.Save();
        }
    }
}
