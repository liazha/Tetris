using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            
        }

        public void UpdateRank()
        {
            string rankFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/rank.txt");
            if (File.Exists(rankFile))
            {
                string rank = File.ReadAllText(rankFile);
                label3.Text = rank;
            }
        }

        public void UpdateLabel(string score)
        {
            label4.Text = score;
        }
    }
}
