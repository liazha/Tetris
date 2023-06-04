using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        public Russia MyRussia;

        private void button1_Click(object sender, EventArgs e)
        {
            MyRussia.Label_Level.Text = comboBox1.SelectedItem.ToString();
            this.Close();
        }
    }
}
