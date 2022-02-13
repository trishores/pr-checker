
namespace PrChecker
{
    partial class Ui
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ui));
            this.btnFocus = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPullRequests = new System.Windows.Forms.TabPage();
            this.pbxSpinner = new System.Windows.Forms.PictureBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbPrState = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.cmbRepoType = new System.Windows.Forms.ComboBox();
            this.txtPrereqs = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPullRequests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFocus
            // 
            this.btnFocus.Location = new System.Drawing.Point(10, 10);
            this.btnFocus.Name = "btnFocus";
            this.btnFocus.Size = new System.Drawing.Size(1, 1);
            this.btnFocus.TabIndex = 21;
            this.btnFocus.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPullRequests);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(852, 336);
            this.tabControl.TabIndex = 0;
            // 
            // tabPullRequests
            // 
            this.tabPullRequests.BackColor = System.Drawing.SystemColors.Window;
            this.tabPullRequests.Controls.Add(this.pbxSpinner);
            this.tabPullRequests.Controls.Add(this.btnRun);
            this.tabPullRequests.Controls.Add(this.label5);
            this.tabPullRequests.Controls.Add(this.cmbPrState);
            this.tabPullRequests.Controls.Add(this.label4);
            this.tabPullRequests.Controls.Add(this.label2);
            this.tabPullRequests.Controls.Add(this.dtpFrom);
            this.tabPullRequests.Controls.Add(this.cmbRepoType);
            this.tabPullRequests.Controls.Add(this.txtPrereqs);
            this.tabPullRequests.Location = new System.Drawing.Point(4, 29);
            this.tabPullRequests.Name = "tabPullRequests";
            this.tabPullRequests.Padding = new System.Windows.Forms.Padding(3);
            this.tabPullRequests.Size = new System.Drawing.Size(844, 303);
            this.tabPullRequests.TabIndex = 3;
            this.tabPullRequests.Text = "Pull Requests";
            // 
            // pbxSpinner
            // 
            this.pbxSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxSpinner.Image = global::PrChecker.Properties.Resources.spinner;
            this.pbxSpinner.Location = new System.Drawing.Point(790, 55);
            this.pbxSpinner.Name = "pbxSpinner";
            this.pbxSpinner.Size = new System.Drawing.Size(31, 31);
            this.pbxSpinner.TabIndex = 21;
            this.pbxSpinner.TabStop = false;
            this.pbxSpinner.Visible = false;
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(722, 54);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(100, 33);
            this.btnRun.TabIndex = 20;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(168, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "PR state";
            // 
            // cmbPrState
            // 
            this.cmbPrState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrState.FormattingEnabled = true;
            this.cmbPrState.Location = new System.Drawing.Point(172, 57);
            this.cmbPrState.Name = "cmbPrState";
            this.cmbPrState.Size = new System.Drawing.Size(161, 28);
            this.cmbPrState.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Repo host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(347, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "From";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(351, 57);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(140, 27);
            this.dtpFrom.TabIndex = 3;
            // 
            // cmbRepoType
            // 
            this.cmbRepoType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRepoType.FormattingEnabled = true;
            this.cmbRepoType.Location = new System.Drawing.Point(31, 57);
            this.cmbRepoType.Name = "cmbRepoType";
            this.cmbRepoType.Size = new System.Drawing.Size(122, 28);
            this.cmbRepoType.TabIndex = 2;
            // 
            // txtPrereqs
            // 
            this.txtPrereqs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrereqs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrereqs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPrereqs.Location = new System.Drawing.Point(22, 114);
            this.txtPrereqs.Multiline = true;
            this.txtPrereqs.Name = "txtPrereqs";
            this.txtPrereqs.Size = new System.Drawing.Size(800, 171);
            this.txtPrereqs.TabIndex = 13;
            // 
            // Ui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 336);
            this.Controls.Add(this.btnFocus);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(700, 375);
            this.Name = "Ui";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "PR Checker";
            this.tabControl.ResumeLayout(false);
            this.tabPullRequests.ResumeLayout(false);
            this.tabPullRequests.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSpinner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnFocus;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPullRequests;
        private System.Windows.Forms.ComboBox cmbRepoType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbPrState;
        private System.Windows.Forms.TextBox txtPrereqs;
        private System.Windows.Forms.PictureBox pbxSpinner;
        private System.Windows.Forms.Button btnRun;
    }
}

