/*
 * Copyright (C) 2021 Tris Shores
 * Open source software. Licensed under the MIT license: https://opensource.org/licenses/MIT
*/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PrChecker
{
    public partial class TextBoxX : UserControl
    {
        private HintTextBox v_hintTextBox;

        public TextBoxX()
        {
            InitializeComponent();

            // defaults:
            AutoScaleMode = AutoScaleMode.None;

            // events:
            VisibleChanged += TextBoxX_VisibleChanged;
        }

        private void TextBoxX_VisibleChanged(object v_sender, EventArgs v_e)
        {
            if (Enabled && !ReadOnly)
            //if (Enabled)
            {
                if (BorderColor == SystemColors.Control) BorderColor = SystemColors.Window;
            }
            else BorderColor = SystemColors.Control;
        }

        protected override void OnEnabledChanged(EventArgs v_e)
        {
            base.OnEnabledChanged(v_e);

            if (Enabled && !ReadOnly)
            //if (Enabled)
            {
                BorderColor = SystemColors.Window;
            }
            else BorderColor = SystemColors.Control;
        }

        internal new event EventHandler TextChanged
        {
            add { v_hintTextBox.TextChanged += value; }
            remove { v_hintTextBox.TextChanged -= value; }
        }

        internal event EventHandler ValueChanged
        {
            add { v_hintTextBox.ValueChanged += value; }
            remove { v_hintTextBox.ValueChanged -= value; }
        }

        internal new event EventHandler Click
        {
            add { v_hintTextBox.Click += value; }
            remove { v_hintTextBox.Click -= value; }
        }

        internal new event KeyPressEventHandler KeyPress
        {
            add { v_hintTextBox.KeyPress += value; }
            remove { v_hintTextBox.KeyPress -= value; }
        }

        internal new event KeyEventHandler KeyDown
        {
            add { v_hintTextBox.KeyDown += value; }
            remove { v_hintTextBox.KeyDown -= value; }
        }

        protected override void SetBoundsCore(int v_x, int v_y, int v_width, int v_height, BoundsSpecified v_specified)
        {
            base.SetBoundsCore(v_x, v_y, v_width, Multiline ? v_height : 29, v_specified);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public new Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
            }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public CharacterCasing CharacterCasing
        {
            get { return v_hintTextBox.CharacterCasing; }
            set { v_hintTextBox.CharacterCasing = value; }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(false)]
        public override Color BackColor
        {
            get { return base.BackColor; }
        }

        [Description("Only applies when Multiline is enabled."), Category("Data")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public bool WordWrap
        {
            get
            {
                return v_hintTextBox.WordWrap;
            }
            set
            {
                v_hintTextBox.WordWrap = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return v_hintTextBox.TextAlign;
            }
            set
            {
                v_hintTextBox.TextAlign = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override Color ForeColor
        {
            get
            {
                return v_hintTextBox.ForeColor;
            }
            set
            {
                v_hintTextBox.ForeColor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override Font Font
        {
            get
            {
                return v_hintTextBox.Font;
            }
            set
            {
                v_hintTextBox.Font = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public bool Multiline
        {
            get
            {
                return v_hintTextBox.Multiline;
            }
            set
            {
                v_hintTextBox.Multiline = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public new string Text
        {
            get
            {
                return v_hintTextBox.Value;
            }
            set
            {
                v_hintTextBox.Text = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public int SelectionStart
        {
            get
            {
                return v_hintTextBox.SelectionStart;
            }
            set
            {
                v_hintTextBox.SelectionStart = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public int SelectionLength
        {
            get
            {
                return v_hintTextBox.SelectionLength;
            }
            set
            {
                v_hintTextBox.SelectionLength = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public string SelectedText
        {
            get
            {
                return v_hintTextBox.SelectedText;
            }
            set
            {
                v_hintTextBox.SelectedText = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return v_hintTextBox.ContextMenuStrip;
            }
            set
            {
                v_hintTextBox.ContextMenuStrip = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override bool Focused
        {
            get
            {
                return v_hintTextBox.Focused;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public string Value
        {
            get
            {
                return v_hintTextBox.Value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public string Hint
        {
            get
            {
                return v_hintTextBox.Hint;
            }
            set
            {
                v_hintTextBox.Hint = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public ScrollBars ScrollBars
        {
            get
            {
                return v_hintTextBox.ScrollBars;
            }
            set
            {
                v_hintTextBox.ScrollBars = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public int MaxLength
        {
            get
            {
                return v_hintTextBox.MaxLength;
            }
            set
            {
                v_hintTextBox.MaxLength = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public bool ReadOnly
        {
            get
            {
                return v_hintTextBox.ReadOnly;
            }
            set
            {
                v_hintTextBox.ReadOnly = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public bool Password
        {
            get
            {
                return v_hintTextBox.Password;
            }
            set
            {
                v_hintTextBox.Password = value;
            }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return v_hintTextBox.AutoCompleteCustomSource; }
            set { v_hintTextBox.AutoCompleteCustomSource = value; }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return v_hintTextBox.AutoCompleteMode; }
            set { v_hintTextBox.AutoCompleteMode = value; }
        }

        [DefaultValue(typeof(SystemColors), "Window")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return v_hintTextBox.AutoCompleteSource; }
            set { v_hintTextBox.AutoCompleteSource = value; }
        }

        internal void m_Clear()
        {
            v_hintTextBox.Clear();
        }

        internal void m_ClearHint()
        {
            v_hintTextBox.ClearHint();
        }

        internal void m_AppendText(string v_str)
        {
            v_hintTextBox.AppendText(v_str);
        }

        public void m_PasteText(string text)
        {
            v_hintTextBox.Paste(text);
        }
    }
}
