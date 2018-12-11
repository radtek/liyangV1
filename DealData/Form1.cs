using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WCAPP;

namespace DealData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.button1.Enabled = false;

            var db = new Context();

            var x = db.Processes.ToList().First();
            MessageBox.Show(x.PartName);

            this.Cursor = Cursors.Default;
            this.button1.Enabled = true;
        }
    }
}
