using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZxDisAsm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Z80Core z80Core = new Z80Core();

            z80Core.B = 0x01;
            z80Core.C = 0x02;
            z80Core.D = 0x03;
            z80Core.E = 0x04;
            z80Core.H = 0x05;
            z80Core.L = 0x06;
            z80Core.A = 0x07;
            z80Core.F = 0x08;
            z80Core.IXH = 0x09;
            z80Core.IXL = 0x0A;
            z80Core.IYH = 0x0B;
            z80Core.IYL = 0x0C;
        }
    }
}
