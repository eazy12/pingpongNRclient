using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pingpong
{
    public partial class FormNickname : Form
    {
        private static string NickName = "Player";
        public FormNickname()
        {
            InitializeComponent();
            textBox1.Text = "Player";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static string GetNickName()
        {
            return NickName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            NickName = textBox1.Text;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.ForeColor = System.Drawing.Color.Black;
        }
    }
}
