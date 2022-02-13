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
    public class HintTextBox : TextBox
    {
        private string _hint;
        private string _prevValue = string.Empty;
        public event EventHandler ValueChanged;

        public HintTextBox()
        {
            // defaults:
            BorderStyle = BorderStyle.None;
            base.AutoSize = false;  // avoid truncated text when borderstyle=none.
        }

        protected override void OnTextChanged(EventArgs v_e)
        {
            base.OnTextChanged(v_e);

            // required if text is changed programmatically as OnFocus events won't fire:
            if (Enabled && !base.Focused && string.IsNullOrWhiteSpace(base.Text))   // v_hint should not display if focused.
            {
                base.Text = _hint;
            }

            ForeColor = Text == _hint ? Color.FromArgb(150, 150, 150) : SystemColors.ControlText;
            if (!_prevValue.Equals(Value))
            {
                _prevValue = Value;
                var v_handler = ValueChanged;
                v_handler?.Invoke(this, null);    // generate ValueChanged event.
            }
        }

        [Localizable(true)]
        public bool Password
        {
            get
            {
                return PasswordChar != 0;
            }
            set
            {
                PasswordChar = value ? '\u25CF' : (char)0;
            }
        }

        [Localizable(true)]
        public string Hint
        {
            get
            {
                return _hint;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(Text) || Text == _hint)
                {
                    _hint = value;
                    if (Enabled) Text = value;
                }
                else
                {
                    _hint = value;
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs v_e)
        {
            base.OnVisibleChanged(v_e);
            //if (!Enabled) Clear();
        }

        protected override void OnEnabledChanged(EventArgs v_e)
        {
            base.OnEnabledChanged(v_e);
            if (!Enabled)
            {
                if (Text == _hint)
                {
                    Clear();
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Text))
                {
                    Text = _hint;
                }
            }
        }

        internal new void Clear()
        {
            base.Clear();
            if (Enabled && !base.Focused)   // v_hint should not display if focused.
            {
                base.Text = _hint;
            }
        }

        internal void ClearHint()
        {
            Hint = "";
        }

        internal string Value
        {
            get
            {
                return Text == Hint ? "" : Text.Trim();
            }
        }

        protected override void OnGotFocus(EventArgs v_e)
        {
            base.OnGotFocus(v_e);

            if (Text == _hint)
            {
                Text = "";
                ForeColor = SystemColors.ControlText;
            }
        }

        protected override void OnLostFocus(EventArgs v_e)
        {
            base.OnLostFocus(v_e);

            if (string.IsNullOrWhiteSpace(Text))
            {
                Text = _hint;
                ForeColor = Color.FromArgb(150, 150, 150);
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
                return base.Focused;
            }
        }
    }
}