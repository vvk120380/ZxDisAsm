using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;

namespace ZxDisAsm
{
    public partial class Form1 : Form
    {

        Color[] ZxColor = { Color.Black, Color.Blue, Color.Red, Color.Magenta, Color.Green, Color.Cyan, Color.Yellow, Color.White };
        Color[] ZxColorLight = { Color.FromArgb(0, 50, 50, 50), Color.LightBlue, Color.FromArgb(0, 255, 59, 59), Color.FromArgb(0, 255, 77, 255), Color.LightGreen, Color.LightCyan, Color.LightYellow, Color.LightGray };

        bool flash = false;

        Progress<ProgessRet> progress;

        Graphics gForm;

        Zx48Machine zx48;

        public Form1()
        {
            InitializeComponent();

            progress = new Progress<ProgessRet>(s => {
                //Graphics g = this.CreateGraphics();
                RedrawScreen(gForm, s.videoRam, s.attrRam);
            });

        }

        async private void button1_Click(object sender, EventArgs e)
        {

            if (Worker.run)
            {
                Worker.run = false;
                timer_flash.Stop();
                return;
            }

            //zx48.SetROM();

            timer_flash.Start();
            zx48 = new Zx48Machine();
            gForm = this.CreateGraphics();
            await Task.Factory.StartNew(() => Worker.StartZX(progress, zx48), TaskCreationOptions.DenyChildAttach);
            RedrawScreen(gForm, zx48.GetVideoRAM(), zx48.GetAttRAM());
        }
        
        int[] scr_ypoz = {  0,8,16,24,32,40,48,56,
                            1,9,17,25,33,41,49,57,
                            2,10,18,26,34,42,50,58,
                            3,11,19,27,35,43,51,59,
                            4,12,20,28,36,44,52,60,
                            5,13,21,29,37,45,53,61,
                            6,14,22,30,38,46,54,62,
                            7,15,23,31,39,47,55,63,

                            64,72,80,88,96,104,112,120,
                            65,73,81,89,97,105,113,121,
                            66,74,82,90,98,106,114,122,
                            67,75,83,91,99,107,115,123,
                            68,76,84,92,100,108,116,124,
                            69,77,85,93,101,109,117,125,
                            70,78,86,94,102,110,118,126,
                            71,79,87,95,103,111,119,127,

                            128,136,144,152,160,168,176,184,
                            129,137,145,153,161,169,177,185,
                            130,138,146,154,162,170,178,186,
                            131,139,147,155,163,171,179,187,
                            132,140,148,156,164,172,180,188,
                            133,141,149,157,165,173,181,189,
                            134,142,150,158,166,174,182,190,
                            135,143,151,159,167,175,183,191 };

        private void RedrawScreen(Graphics g, byte[] video, byte[] attr)
        {
            Color backcolor = Color.White;
            Color forecolor = Color.Black;
            Color color;

            int scr_size_x = 256;
            int scr_size_y = 192;


            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();

            Bitmap bmp = new Bitmap(scr_size_x, scr_size_y, PixelFormat.Format24bppRgb);
            int x = 0;
 
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int l = 0; l < scr_ypoz.Length; l++)
            {
                int iy = scr_ypoz[l] * bmp.Width * 3;
                for (int i = 0; i < 32; i++, x++)
                {
                    int dColor = attr[ (scr_ypoz[l] / 8)*32 + i];
                    if ((dColor & 0x40) > 0)
                    {
                        forecolor = ZxColorLight[dColor & 0x7];
                        backcolor = ZxColorLight[(dColor >> 30) & 0x7];
                    }
                    else
                    {
                        if ((dColor & 0x80) == 0)
                        {
                            forecolor = ZxColor[dColor & 0x7];
                            backcolor = ZxColor[(dColor >> 3) & 0x7];
                        }
                        else
                        {
                            if (flash)
                            {
                                forecolor = ZxColor[(dColor >> 3) & 0x7];
                                backcolor = ZxColor[dColor & 0x7];
                            }
                            else
                            {
                                forecolor = ZxColor[dColor & 0x7];
                                backcolor = ZxColor[(dColor >> 3) & 0x7];
                            }
                        }
                    }

                    for (int r = 0; r < 8; r++)
                    {
                        color = ((video[x] & (1 << r)) > 0) ? forecolor : backcolor;
                        if ((video[x] & (1 << r)) > 0)
                        {
                            int z = 0;
                        }

                        int ix = ((i * 8) + (7 - r)) * 3;
                        rgbValues[iy + ix] = color.B;
                        rgbValues[iy + ix + 1] = color.G;
                        rgbValues[iy + ix + 2] = color.R;
                    }
                }
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            //stopWatch.Stop();
            //long ms = stopWatch.ElapsedMilliseconds;
            ////Console.WriteLine($" >>>>>>>>>>>  {ms} ms (Create)");
            //stopWatch.Restart();
            g.DrawImage(bmp, 30, 30);
            //stopWatch.Stop();
            //ms = stopWatch.ElapsedMilliseconds;
            //Console.WriteLine($" >>>>>>>>>>>  {ms} ms (Draw)");
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Worker.run) Worker.run = false;
        }

        private void timerflash_Tick(object sender, EventArgs e)
        {
            flash = !flash;
        }

        private void btn_enter_Click(object sender, EventArgs e)
        {
            zx48.port = 0xBFFE;
            zx48.key = 0xFE;
        }

        private void btn_L_Click(object sender, EventArgs e)
        {
            zx48.port = 0xBFFE;
            zx48.key = 0xFD;
        }

        private void btn_K_Click(object sender, EventArgs e)
        {
            zx48.port = 0xBFFE;
            zx48.key = 0xFB;
        }

        private void btn_J_Click(object sender, EventArgs e)
        {
            zx48.port = 0xBFFE;
            zx48.key = 0xF7;
        }

        private void btn_H_Click(object sender, EventArgs e)
        {
            zx48.port = 0xBFFE;
            zx48.key = 0xEF;
        }

        private void btn_P_Click(object sender, EventArgs e)
        {
            zx48.port = 0xDFFE;
            zx48.key = 0xFE;
        }

        private void btn_O_Click(object sender, EventArgs e)
        {
            zx48.port = 0xDFFE;
            zx48.key = 0xFD;
        }

        private void btn_I_Click(object sender, EventArgs e)
        {
            zx48.port = 0xDFFE;
            zx48.key = 0xFB;
        }

        private void btn_U_Click(object sender, EventArgs e)
        {
            zx48.port = 0xDFFE;
            zx48.key = 0xF7;
        }

        private void btn_Y_Click(object sender, EventArgs e)
        {
            zx48.port = 0xDFFE;
            zx48.key = 0xEF;
        }

        private void btn_0_Click(object sender, EventArgs e)
        {
            zx48.port = 0xEFFE;
            zx48.key = 0xFE;
        }

        private void btn_9_Click(object sender, EventArgs e)
        {
            zx48.port = 0xEFFE;
            zx48.key = 0xFD;
        }

        private void btn_8_Click(object sender, EventArgs e)
        {
            zx48.port = 0xEFFE;
            zx48.key = 0xFB;
        }

        private void btn_7_Click(object sender, EventArgs e)
        {
            zx48.port = 0xEFFE;
            zx48.key = 0xF7;
        }

        private void btn_6_Click(object sender, EventArgs e)
        {
            zx48.port = 0xEFFE;
            zx48.key = 0xEF;
        }

        private void btn_break_Click(object sender, EventArgs e)
        {
            zx48.port = 0x7FFE;
            zx48.key = 0xFE;
        }

        private void btn_SymShift_Click(object sender, EventArgs e)
        {
            zx48.port = 0x7FFE;
            zx48.key = 0xFD;
        }

        private void btn_M_Click(object sender, EventArgs e)
        {
            zx48.port = 0x7FFE;
            zx48.key = 0xFB;
        }

        private void btn_N_Click(object sender, EventArgs e)
        {
            zx48.port = 0x7FFE;
            zx48.key = 0xF7;
        }

        private void btn_B_Click(object sender, EventArgs e)
        {
            zx48.port = 0x7FFE;
            zx48.key = 0xEF;
        }


    }

    public class Worker
    {
        public static bool run;
  

        public static void StartZX(IProgress<ProgessRet> progress, Zx48Machine zx48)
        {

            run = true;
            zx48.Reset();

            Stopwatch swRedraw = new Stopwatch();
            Stopwatch swInterrupt = new Stopwatch();
            swRedraw.Restart();
            swInterrupt.Restart();

            while (run)
            {
                zx48.Execute();
                if (swRedraw.ElapsedMilliseconds > 40)
                {
                    progress.Report(new ProgessRet(zx48.GetVideoRAM(), zx48.GetAttRAM()));
                    swRedraw.Restart();
                }

                if (swInterrupt.ElapsedMilliseconds > 50) //50 раз в секунду
                {
                    zx48.Interrupt = true;                        
                    swInterrupt.Restart();
                }
            }
            

        }
    }
    public class ProgessRet
    {

        public ProgessRet(byte[] videoRam, byte[] attrRam)
        {
            this.videoRam = videoRam;
            this.attrRam = attrRam;
        }

        public byte[] videoRam;
        public byte[] attrRam;

    }

}
