using Autocomplete;
using DataProj;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace UI_example
{
    public partial class Form1 : Form
    {
        IAutocomplete ac;
        public char[] options;

        public Form1()
        {
            ac = new Autocomplete.Autocomplete(Data.Cities.Select(s => s.ToCharArray()).ToArray());

            InitializeComponent();

            textBox3.Lines = ac.AllItems.Select(s => new string(s)).ToArray();
            setOptions(ac.InitialOptions);
        }

        private void textBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectedText = textBox2.SelectedText;

            if (selectedText.Length > 1 && selectedText.Equals("__"))
                selectedText = " ";

            if (selectedText.Length != 1)
                return;

            textBox1.AppendText(selectedText);

            filter(selectedText[0]);
        }

        private void filter(char c)
        {
            textBox3.Lines = ac.Filter(c, out char[] options).Select(s => new string(s)).ToArray();

            if (options == null)
                textBox2.Text = string.Empty;
            else
                setOptions(options);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length-1);

            filter('\b');
        }

        private void setOptions(char[] options)
        {
            textBox2.Lines = options.Select(o => o == ' ' ? "__" : o.ToString()).Distinct().ToArray();
        }
    }
}
