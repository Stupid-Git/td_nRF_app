using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rtr500ble_Command_test
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            numericUpDown1.Value = Form1.mode;
            numericUpDown2.Value = Form1.range;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Location = new Point(this.Owner.Location.X + (this.Owner.Width - this.Width) / 2, this.Owner.Location.Y + (this.Owner.Height - this.Height) / 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.advance = true;

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.advance = false;

            Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value == 8) numericUpDown1.Value = 2;
            if (numericUpDown1.Value == 10) numericUpDown1.Value = 0;

            if (numericUpDown1.Value > 2) numericUpDown1.Value = 9;
            if (numericUpDown1.Value < 0) numericUpDown1.Value = 9;

            Form1.mode = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Form1.range = (int)numericUpDown2.Value;
        }
    }
}
