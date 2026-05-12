namespace Filereader
{
    partial class ConfigWizardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblStepTitle = new Krypton.Toolkit.KryptonLabel();
            this.lblInstruction = new Krypton.Toolkit.KryptonLabel();
            this.btnNext = new Krypton.Toolkit.KryptonButton();
            this.btnBack = new Krypton.Toolkit.KryptonButton();
            this.btnCancel = new Krypton.Toolkit.KryptonButton();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelStep1 = new System.Windows.Forms.Panel();
            this.panelStep2 = new System.Windows.Forms.Panel();
            this.panelStep3 = new System.Windows.Forms.Panel();
            this.kryptonSeparator1 = new Krypton.Toolkit.KryptonSeparator();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStepTitle
            // 
            this.lblStepTitle.LabelStyle = Krypton.Toolkit.LabelStyle.TitlePanel;
            this.lblStepTitle.Location = new System.Drawing.Point(12, 12);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(110, 29);
            this.lblStepTitle.TabIndex = 0;
            this.lblStepTitle.Values.Text = "Step 1 of 3";
            // 
            // lblInstruction
            // 
            this.lblInstruction.Location = new System.Drawing.Point(12, 47);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(262, 20);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Values.Text = "Select a Template and give it a Name.";
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(906, 685);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(90, 25);
            this.btnNext.TabIndex = 2;
            this.btnNext.Values.Text = "Next >";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(810, 685);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(90, 25);
            this.btnBack.TabIndex = 3;
            this.btnBack.Values.Text = "< Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(12, 685);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 25);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Values.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panelContent
            // 
            this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContent.Controls.Add(this.panelStep1);
            this.panelContent.Controls.Add(this.panelStep2);
            this.panelContent.Controls.Add(this.panelStep3);
            this.panelContent.Location = new System.Drawing.Point(12, 85);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(984, 585);
            this.panelContent.TabIndex = 5;
            // 
            // panelStep1
            // 
            this.panelStep1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStep1.Location = new System.Drawing.Point(0, 0);
            this.panelStep1.Name = "panelStep1";
            this.panelStep1.Size = new System.Drawing.Size(982, 583);
            this.panelStep1.TabIndex = 0;
            // 
            // panelStep2
            // 
            this.panelStep2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStep2.Location = new System.Drawing.Point(0, 0);
            this.panelStep2.Name = "panelStep2";
            this.panelStep2.Size = new System.Drawing.Size(982, 583);
            this.panelStep2.TabIndex = 1;
            this.panelStep2.Visible = false;
            // 
            // panelStep3
            // 
            this.panelStep3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStep3.Location = new System.Drawing.Point(0, 0);
            this.panelStep3.Name = "panelStep3";
            this.panelStep3.Size = new System.Drawing.Size(982, 583);
            this.panelStep3.TabIndex = 2;
            this.panelStep3.Visible = false;
            // 
            // ConfigWizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 722);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lblStepTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigWizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Format Configuration Wizard";
            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Krypton.Toolkit.KryptonLabel lblStepTitle;
        private Krypton.Toolkit.KryptonLabel lblInstruction;
        private Krypton.Toolkit.KryptonButton btnNext;
        private Krypton.Toolkit.KryptonButton btnBack;
        private Krypton.Toolkit.KryptonButton btnCancel;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelStep1;
        private System.Windows.Forms.Panel panelStep2;
        private System.Windows.Forms.Panel panelStep3;
        private Krypton.Toolkit.KryptonSeparator kryptonSeparator1;
    }
}
