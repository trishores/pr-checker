namespace PrChecker
{
    partial class TextBoxX
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool v_disposing)
        {
            if (v_disposing && (components != null))
            {
                components.Dispose();
                v_hintTextBox.Dispose();
            }
            base.Dispose(v_disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.v_hintTextBox = new PrChecker.HintTextBox();
            this.SuspendLayout();
            // 
            // hintTextBox
            // 
            this.v_hintTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.v_hintTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_hintTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v_hintTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.v_hintTextBox.Hint = null;
            this.v_hintTextBox.Location = new System.Drawing.Point(2, 2);
            this.v_hintTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.v_hintTextBox.Name = "hintTextBox";
            this.v_hintTextBox.Password = false;
            this.v_hintTextBox.Size = new System.Drawing.Size(96, 19);
            this.v_hintTextBox.TabIndex = 2;
            this.v_hintTextBox.WordWrap = false;
            // 
            // TextBoxX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.v_hintTextBox);
            this.Name = "TextBoxX";
            this.Size = new System.Drawing.Size(100, 23);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
