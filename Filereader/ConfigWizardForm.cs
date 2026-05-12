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
using Krypton.Toolkit;
using Serilog;

namespace Filereader
{
    public partial class ConfigWizardForm : KryptonForm
    {
        private int _currentStep = 1;
        private const int TotalSteps = 3;
        private FormatConfig _editingConfig;
        private List<Type> _discoveredTemplates = new List<Type>();
        private List<string> _discoveredSelectors = new List<string>();

        // Dynamic controls
        private KryptonTextBox txtName;
        private KryptonTextBox txtTemplateFilter;
        private KryptonCheckedListBox clbTemplates;
        private KryptonCheckedListBox clbFields;
        private KryptonComboBox cbSelectors;
        private KryptonComboBox cbPrimaryKey;

        public ConfigWizardForm(FormatConfig config = null)
        {
            InitializeComponent();
            _editingConfig = config ?? new FormatConfig();
            InitializeDynamicControls();
            LoadTemplatesAndSelectors();
            UpdateStepUI();
        }

        private void InitializeDynamicControls()
        {
            txtName = new KryptonTextBox { Dock = DockStyle.Top };
            txtTemplateFilter = new KryptonTextBox { Dock = DockStyle.Top };
            txtTemplateFilter.TextChanged += (s, e) => FilterTemplates();

            cbSelectors = new KryptonComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            clbTemplates = new KryptonCheckedListBox { Dock = DockStyle.Fill };
            
            cbPrimaryKey = new KryptonComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            clbFields = new KryptonCheckedListBox { Dock = DockStyle.Fill };

            panelStep1.Controls.Add(clbTemplates);
            panelStep1.Controls.Add(new KryptonLabel { Text = "Select Templates (Multi-Record):", Dock = DockStyle.Top });
            panelStep1.Controls.Add(cbSelectors);
            panelStep1.Controls.Add(new KryptonLabel { Text = "Record Selector (Optional):", Dock = DockStyle.Top });
            panelStep1.Controls.Add(txtTemplateFilter);
            panelStep1.Controls.Add(new KryptonLabel { Text = "Filter (Templates & Selectors):", Dock = DockStyle.Top });
            panelStep1.Controls.Add(txtName);
            panelStep1.Controls.Add(new KryptonLabel { Text = "Format Name:", Dock = DockStyle.Top });

            txtName.Text = _editingConfig.DisplayName;
        }

        private void FilterTemplates()
        {
            string filter = txtTemplateFilter.Text.ToLower();
            
            // Sync checked items back to config temporarily so we don't lose them during filter
            SyncCheckedTemplatesToConfig();

            // Filter Templates
            clbTemplates.Items.Clear();
            foreach (var type in _discoveredTemplates)
            {
                if (string.IsNullOrEmpty(filter) || type.FullName.ToLower().Contains(filter))
                {
                    clbTemplates.Items.Add(type.FullName);
                    if (_editingConfig.RecordTypeNames.Any(rt => rt.StartsWith(type.FullName)))
                    {
                        clbTemplates.SetItemChecked(clbTemplates.Items.Count - 1, true);
                    }
                }
            }

            // Filter Selectors
            string currentSelector = cbSelectors.SelectedItem?.ToString();
            cbSelectors.Items.Clear();
            cbSelectors.Items.Add("(None)");
            foreach (var entry in _discoveredSelectors)
            {
                if (string.IsNullOrEmpty(filter) || entry.ToLower().Contains(filter))
                {
                    cbSelectors.Items.Add(entry);
                }
            }

            // Restore selection if it still exists in the filtered list
            if (currentSelector != null && cbSelectors.Items.Contains(currentSelector))
            {
                cbSelectors.SelectedItem = currentSelector;
            }
            else if (cbSelectors.SelectedIndex < 0)
            {
                cbSelectors.SelectedIndex = 0;
            }
        }

        private void SyncCheckedTemplatesToConfig()
        {
            // We only update config with what is CURRENTLY visible and checked
            // PLUS what was already in config but is currently HIDDEN by filter
            var currentlyVisible = new HashSet<string>();
            for (int i = 0; i < clbTemplates.Items.Count; i++)
            {
                string typeName = clbTemplates.Items[i].ToString();
                currentlyVisible.Add(typeName);
                
                bool isChecked = clbTemplates.GetItemChecked(i);
                bool inConfig = _editingConfig.RecordTypeNames.Any(rt => rt.StartsWith(typeName));

                if (isChecked && !inConfig)
                {
                    var type = _discoveredTemplates.FirstOrDefault(t => t.FullName == typeName);
                    if (type != null) _editingConfig.RecordTypeNames.Add(type.AssemblyQualifiedName);
                }
                else if (!isChecked && inConfig)
                {
                    _editingConfig.RecordTypeNames.RemoveAll(rt => rt.StartsWith(typeName));
                }
            }
        }

        private void LoadTemplatesAndSelectors()
        {
            try
            {
                string dllPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PAMC.Formats.dll");
                if (!System.IO.File.Exists(dllPath)) 
                {
                    Log.Warning("PAMC.Formats.dll not found for discovery at {Path}", dllPath);
                    return;
                }

                Log.Information("Scanning {Path} for templates and selectors...", dllPath);
                var assembly = Assembly.LoadFrom(dllPath);
                var types = assembly.GetTypes().Where(t => t.IsPublic).ToList();

                // Templates
                _discoveredTemplates = types
                    .Where(t => !t.IsAbstract && (t.GetCustomAttributes(typeof(FileHelpers.DelimitedRecordAttribute), true).Any() || t.GetCustomAttributes(typeof(FileHelpers.FixedLengthRecordAttribute), true).Any()))
                    .OrderBy(t => t.FullName)
                    .ToList();

                Log.Information("Discovered {Count} FileHelpers record templates.", _discoveredTemplates.Count);
                foreach (var t in _discoveredTemplates)
                {
                    Log.Debug("Found template: {TypeName}", t.FullName);
                }

                FilterTemplates();

                // Selectors
                _discoveredSelectors.Clear();
                foreach (var type in types)
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(m => m.ReturnType == typeof(Type) && m.GetParameters().Length == 2);
                    foreach (var m in methods)
                    {
                        string entry = string.Format("{0}.{1}, {2}", type.FullName, m.Name, assembly.GetName().Name);
                        _discoveredSelectors.Add(entry);
                        Log.Debug("Found selector method: {Selector}", entry);
                    }
                }
                Log.Information("Discovered {Count} selector methods.", _discoveredSelectors.Count);

                // Run filter again to populate selectors
                FilterTemplates();

                // Set initial selector if editing
                if (!string.IsNullOrEmpty(_editingConfig.SelectorTypeName) && !string.IsNullOrEmpty(_editingConfig.SelectorMethodName))
                {
                    for (int i = 0; i < cbSelectors.Items.Count; i++)
                    {
                        string item = cbSelectors.Items[i].ToString();
                        if (item.Contains(_editingConfig.SelectorMethodName) && item.Contains(_editingConfig.SelectorTypeName.Split(',')[0]))
                        {
                            cbSelectors.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during assembly discovery scanning.");
                MessageBox.Show("Error loading assembly info: " + ex.Message);
            }
        }

        private void UpdateStepUI()
        {
            lblStepTitle.Text = string.Format("Step {0} of {1}", _currentStep, TotalSteps);
            btnBack.Enabled = _currentStep > 1;
            btnNext.Text = _currentStep == TotalSteps ? "Finish" : "Next >";

            panelStep1.Visible = _currentStep == 1;
            panelStep2.Visible = _currentStep == 2;
            panelStep3.Visible = _currentStep == 3;

            switch (_currentStep)
            {
                case 1: 
                    lblInstruction.Text = "Name, Selector (for multi-record), and Templates."; 
                    break;
                case 2: 
                    lblInstruction.Text = "Master columns and Primary Key for grouping."; 
                    panelStep2.Controls.Clear();
                    panelStep2.Controls.Add(clbFields);
                    panelStep2.Controls.Add(cbPrimaryKey);
                    panelStep2.Controls.Add(new KryptonLabel { Text = "Select Primary Key:", Dock = DockStyle.Top });
                    PopulateFieldsForSelection(_editingConfig.MasterColumns.Select(c => c.Name).ToList());
                    
                    // Sync PK combo
                    cbPrimaryKey.Items.Clear();
                    foreach (var item in clbFields.CheckedItems) cbPrimaryKey.Items.Add(item.ToString());
                    var currentPk = _editingConfig.MasterColumns.FirstOrDefault(c => c.IsPrimaryKey)?.Name;
                    if (currentPk != null) cbPrimaryKey.SelectedItem = currentPk;
                    break;
                case 3: 
                    lblInstruction.Text = "Detail columns (Curated Fields)."; 
                    panelStep3.Controls.Clear();
                    panelStep3.Controls.Add(clbFields);
                    PopulateFieldsForSelection(_editingConfig.CuratedFields);
                    break;
            }
        }

        private void PopulateFieldsForSelection(List<string> checkedItems)
        {
            // Ensure config is in sync with UI before reading types
            if (_currentStep == 2 || _currentStep == 3) SyncCheckedTemplatesToConfig();

            if (_editingConfig.RecordTypeNames.Count == 0) return;

            clbFields.Items.Clear();
            var addedFields = new HashSet<string>();
            var allFields = new List<string>();

            foreach (var typeName in _editingConfig.RecordTypeNames)
            {
                var type = Type.GetType(typeName);
                if (type == null) continue;

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var f in fields)
                {
                    if (addedFields.Add(f.Name))
                    {
                        allFields.Add(f.Name);
                    }
                }
            }

            // NI11: Alphabetical sorting for fields
            foreach (var fieldName in allFields.OrderBy(f => f))
            {
                clbFields.Items.Add(fieldName);
                if (checkedItems.Contains(fieldName))
                {
                    clbFields.SetItemChecked(clbFields.Items.Count - 1, true);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentStep == 1)
            {
                SyncCheckedTemplatesToConfig();
                if (string.IsNullOrWhiteSpace(txtName.Text) || _editingConfig.RecordTypeNames.Count == 0)
                {
                    MessageBox.Show("Please enter a name and select at least one template.");
                    return;
                }
                _editingConfig.DisplayName = txtName.Text;

                // Selector
                if (cbSelectors.SelectedIndex > 0)
                {
                    string selected = cbSelectors.SelectedItem.ToString();
                    var parts = selected.Split(',');
                    var typePart = parts[0].Trim();
                    int lastDot = typePart.LastIndexOf('.');
                    string typeNameOnly = typePart.Substring(0, lastDot);
                    string methodName = typePart.Substring(lastDot + 1);

                    _editingConfig.SelectorMethodName = methodName;
                    
                    // Find the type to get its AQN
                    var type = _discoveredTemplates.FirstOrDefault(t => t.FullName == typeNameOnly) 
                               ?? _discoveredTemplates[0].Assembly.GetType(typeNameOnly);
                    _editingConfig.SelectorTypeName = type?.AssemblyQualifiedName ?? typePart + ", " + parts[1].Trim();
                }
                else
                {
                    _editingConfig.SelectorTypeName = null;
                    _editingConfig.SelectorMethodName = null;
                }
            }
            else if (_currentStep == 2)
            {
                if (cbPrimaryKey.SelectedItem == null && clbFields.CheckedItems.Count > 0)
                {
                    MessageBox.Show("Please select a Primary Key for the Master view.");
                    return;
                }

                _editingConfig.MasterColumns.Clear();
                string selectedPk = cbPrimaryKey.SelectedItem?.ToString();

                foreach (var item in clbFields.CheckedItems)
                {
                    string fieldName = item.ToString();
                    _editingConfig.MasterColumns.Add(new ColumnConfig 
                    { 
                        Name = fieldName, 
                        DataType = "System.String",
                        IsPrimaryKey = (fieldName == selectedPk)
                    });
                }
            }
            else if (_currentStep == 3)
            {
                _editingConfig.CuratedFields.Clear();
                foreach (var item in clbFields.CheckedItems)
                {
                    _editingConfig.CuratedFields.Add(item.ToString());
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            if (_currentStep < TotalSteps)
            {
                _currentStep++;
                UpdateStepUI();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (_currentStep > 1)
            {
                _currentStep--;
                UpdateStepUI();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public FormatConfig GetResult()
        {
            return _editingConfig;
        }
    }
}
