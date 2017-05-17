using System;
using System.Windows.Forms;

namespace Pingpong
{
    public partial class FormConnection : Form
    {
        private static string Address = "localhost";
        public FormConnection()
        {
            InitializeComponent();
            textBox1.Text = "localhost";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Address = textBox1.Text;
        }

        public static string GetAddress()
        {
            return Address;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.ForeColor = System.Drawing.Color.Black;
        }
    }
}
