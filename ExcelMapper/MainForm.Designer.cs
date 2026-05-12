namespace ExcelMapper
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
            this.grpSqlSource = new System.Windows.Forms.GroupBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.cmbSources = new System.Windows.Forms.ComboBox();
            this.lblFields = new System.Windows.Forms.Label();
            this.lstFields = new System.Windows.Forms.ListBox();
            this.grpExcelTarget = new System.Windows.Forms.GroupBox();
            this.lblExcel = new System.Windows.Forms.Label();
            this.txtExcelPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblTargetHeaders = new System.Windows.Forms.Label();
            this.lstExcelHeaders = new System.Windows.Forms.ListBox();
            this.grpMapping = new System.Windows.Forms.GroupBox();
            this.dgvMapping = new System.Windows.Forms.DataGridView();
            this.colExcelHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSqlExpression = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpSqlSource.SuspendLayout();
            this.grpExcelTarget.SuspendLayout();
            this.grpMapping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapping)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSqlSource
            // 
            this.grpSqlSource.Controls.Add(this.lblSource);
            this.grpSqlSource.Controls.Add(this.cmbSources);
            this.grpSqlSource.Controls.Add(this.lblFields);
            this.grpSqlSource.Controls.Add(this.lstFields);
            this.grpSqlSource.Location = new System.Drawing.Point(12, 12);
            this.grpSqlSource.Name = "grpSqlSource";
            this.grpSqlSource.Size = new System.Drawing.Size(320, 742);
            this.grpSqlSource.TabIndex = 0;
            this.grpSqlSource.TabStop = false;
            this.grpSqlSource.Text = "1. SQL Data Source";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(6, 25);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(78, 13);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Select Source:";
            // 
            // cmbSources
            // 
            this.cmbSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSources.FormattingEnabled = true;
            this.cmbSources.Location = new System.Drawing.Point(90, 22);
            this.cmbSources.Name = "cmbSources";
            this.cmbSources.Size = new System.Drawing.Size(224, 21);
            this.cmbSources.TabIndex = 1;
            this.cmbSources.SelectedIndexChanged += new System.EventHandler(this.cmbSources_SelectedIndexChanged);
            // 
            // lblFields
            // 
            this.lblFields.AutoSize = true;
            this.lblFields.Location = new System.Drawing.Point(6, 60);
            this.lblFields.Name = "lblFields";
            this.lblFields.Size = new System.Drawing.Size(206, 13);
            this.lblFields.TabIndex = 2;
            this.lblFields.Text = "Available SQL Fields (Double-click to add):";
            // 
            // lstFields
            // 
            this.lstFields.FormattingEnabled = true;
            this.lstFields.Location = new System.Drawing.Point(9, 76);
            this.lstFields.Name = "lstFields";
            this.lstFields.Size = new System.Drawing.Size(305, 654);
            this.lstFields.TabIndex = 3;
            this.lstFields.DoubleClick += new System.EventHandler(this.lstFields_DoubleClick);
            // 
            // grpExcelTarget
            // 
            this.grpExcelTarget.Controls.Add(this.lblExcel);
            this.grpExcelTarget.Controls.Add(this.txtExcelPath);
            this.grpExcelTarget.Controls.Add(this.btnBrowse);
            this.grpExcelTarget.Controls.Add(this.lblTargetHeaders);
            this.grpExcelTarget.Controls.Add(this.lstExcelHeaders);
            this.grpExcelTarget.Location = new System.Drawing.Point(338, 12);
            this.grpExcelTarget.Name = "grpExcelTarget";
            this.grpExcelTarget.Size = new System.Drawing.Size(320, 742);
            this.grpExcelTarget.TabIndex = 1;
            this.grpExcelTarget.TabStop = false;
            this.grpExcelTarget.Text = "2. Excel Template Target";
            // 
            // lblExcel
            // 
            this.lblExcel.AutoSize = true;
            this.lblExcel.Location = new System.Drawing.Point(6, 25);
            this.lblExcel.Name = "lblExcel";
            this.lblExcel.Size = new System.Drawing.Size(55, 13);
            this.lblExcel.TabIndex = 4;
            this.lblExcel.Text = "Excel File:";
            // 
            // txtExcelPath
            // 
            this.txtExcelPath.Location = new System.Drawing.Point(67, 22);
            this.txtExcelPath.Name = "txtExcelPath";
            this.txtExcelPath.ReadOnly = true;
            this.txtExcelPath.Size = new System.Drawing.Size(166, 20);
            this.txtExcelPath.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(239, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblTargetHeaders
            // 
            this.lblTargetHeaders.AutoSize = true;
            this.lblTargetHeaders.Location = new System.Drawing.Point(6, 60);
            this.lblTargetHeaders.Name = "lblTargetHeaders";
            this.lblTargetHeaders.Size = new System.Drawing.Size(183, 13);
            this.lblTargetHeaders.TabIndex = 7;
            this.lblTargetHeaders.Text = "Excel Headers (Double-click to map):";
            // 
            // lstExcelHeaders
            // 
            this.lstExcelHeaders.FormattingEnabled = true;
            this.lstExcelHeaders.Location = new System.Drawing.Point(9, 76);
            this.lstExcelHeaders.Name = "lstExcelHeaders";
            this.lstExcelHeaders.Size = new System.Drawing.Size(305, 654);
            this.lstExcelHeaders.TabIndex = 8;
            this.lstExcelHeaders.DoubleClick += new System.EventHandler(this.lstExcelHeaders_DoubleClick);
            // 
            // grpMapping
            // 
            this.grpMapping.Controls.Add(this.dgvMapping);
            this.grpMapping.Controls.Add(this.btnGenerate);
            this.grpMapping.Controls.Add(this.txtOutput);
            this.grpMapping.Controls.Add(this.lblOutput);
            this.grpMapping.Location = new System.Drawing.Point(664, 12);
            this.grpMapping.Name = "grpMapping";
            this.grpMapping.Size = new System.Drawing.Size(808, 742);
            this.grpMapping.TabIndex = 2;
            this.grpMapping.TabStop = false;
            this.grpMapping.Text = "3. Mapping Configuration";
            // 
            // dgvMapping
            // 
            this.dgvMapping.AllowUserToAddRows = false;
            this.dgvMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMapping.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colExcelHeader,
            this.colSqlExpression});
            this.dgvMapping.Location = new System.Drawing.Point(6, 25);
            this.dgvMapping.Name = "dgvMapping";
            this.dgvMapping.Size = new System.Drawing.Size(796, 435);
            this.dgvMapping.TabIndex = 10;
            // 
            // colExcelHeader
            // 
            this.colExcelHeader.HeaderText = "Excel Header";
            this.colExcelHeader.Name = "colExcelHeader";
            this.colExcelHeader.ReadOnly = true;
            this.colExcelHeader.Width = 200;
            // 
            // colSqlExpression
            // 
            this.colSqlExpression.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSqlExpression.HeaderText = "SQL Expression (Wrap fields in {})";
            this.colSqlExpression.Name = "colSqlExpression";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(6, 466);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(180, 30);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "Generate Mapping DSL";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(6, 515);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(796, 221);
            this.txtOutput.TabIndex = 12;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(6, 499);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(84, 13);
            this.lblOutput.TabIndex = 13;
            this.lblOutput.Text = "Generated DSL:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 759);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1484, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 17);
            this.lblStatus.Text = "Ready";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 781);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grpMapping);
            this.Controls.Add(this.grpExcelTarget);
            this.Controls.Add(this.grpSqlSource);
            this.Name = "MainForm";
            this.Text = "Mapping Configuration Generator";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.grpSqlSource.ResumeLayout(false);
            this.grpSqlSource.PerformLayout();
            this.grpExcelTarget.ResumeLayout(false);
            this.grpExcelTarget.PerformLayout();
            this.grpMapping.ResumeLayout(false);
            this.grpMapping.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMapping)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.ComboBox cmbSources;
        private System.Windows.Forms.Label lblFields;
        private System.Windows.Forms.ListBox lstFields;
        private System.Windows.Forms.Label lblExcel;
        private System.Windows.Forms.TextBox txtExcelPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblTargetHeaders;
        private System.Windows.Forms.ListBox lstExcelHeaders;
        private System.Windows.Forms.Label lblMapping;
        private System.Windows.Forms.DataGridView dgvMapping;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExcelHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSqlExpression;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.GroupBox grpSqlSource;
        private System.Windows.Forms.GroupBox grpExcelTarget;
        private System.Windows.Forms.GroupBox grpMapping;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}

