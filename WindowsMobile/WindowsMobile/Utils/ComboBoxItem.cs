using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace WindowsMobile.Utils
{
    class ComboBoxItem
    {
        private string text;
        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public ComboBoxItem(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}
