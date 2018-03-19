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
using System.Threading;


namespace ZxDisAsm
{
    public partial class Form1 : Form
    {

        Color borderColor;
        bool canRedraw = true;

        //public byte[] keyBuff = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        const int dXOffset = 32;
        const int dYOffset = 32;
        const int scr_size_x = 320;
        const int scr_size_y = 256;

        Color[] ZxColor = { Color.Black, Color.Blue, Color.Red, Color.Magenta, Color.Green, Color.Cyan, Color.Yellow, Color.White };
        Color[] ZxColorLight = { Color.Black, Color.Blue, Color.Red, Color.Magenta, Color.Green, Color.Cyan, Color.Yellow, Color.White };//{ Color.FromArgb(0, 50, 50, 50), Color.LightBlue, Color.FromArgb(0, 255, 59, 59), Color.FromArgb(0, 255, 77, 255), Color.LightGreen, Color.LightCyan, Color.LightYellow, Color.LightGray };

        bool flash = false;

        //Progress<ProgessRet> progress;
        //Progress<ProgessRetBorder> progressBorder;

        Graphics gForm;

        static bool runZx48 = false;
        Thread myThread;
        Zx48Machine zx48;

        public Form1()
        {
            InitializeComponent();
            //progress = new Progress<ProgessRet>(s => { RedrawScreen(gForm, s.videoRam, s.attrRam); });
            //progressBorder = new Progress<ProgessRetBorder>(s => { RedrawBorder(gForm, s.border); });
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //if (Worker.run)
            //{
            //    Worker.run = false;
            //    return;
            //}

            //zx48 = new Zx48Machine();

            //await Task.Factory.StartNew(() => Worker.StartZX(progress, progressBorder, zx48), TaskCreationOptions.DenyChildAttach);
            //RedrawScreen(gForm, zx48.GetVideoRAM(), zx48.GetAttRAM());
        }

        private void StartZx_Click(object sender, EventArgs e)
        {

            if (runZx48)
            {
                runZx48 = false;
                while (myThread.IsAlive){};
                return;
            }
            zx48 = new Zx48Machine();
            zx48.BorderEvent += new BorderEventHandler(BorderChange);
            zx48.VideoEvent += new VideoEventHandler(VideoChange);
            zx48.KeybEvent += new KeyboardEventHandler(KeyboardChange);

            runZx48 = true;
            myThread = new Thread(new ParameterizedThreadStart(this.StartZX));
            myThread.Priority = ThreadPriority.Highest;
            myThread.IsBackground = false;
            myThread.Start(zx48);         
        }

        public void StartZX(object obj)
        {

            Zx48Machine zx48 = (Zx48Machine)obj;

            zx48.Reset();

            Stopwatch swRedraw = new Stopwatch();
            Stopwatch swInterrupt = new Stopwatch();
            Stopwatch swSecond = new Stopwatch();

            swRedraw.Restart();
            swInterrupt.Restart();
            swSecond.Restart();

            double milliseconds;
            double microseconds;
            double nanoseconds;

            long elapsedTicks = 0;
            long elapsedTicksStart = 0;
            long execTicks = 0;
            long execTicksStart = 0;
            int  runCount = 0;
            int  execCount = 0;

            while (runZx48)
            {
                execCount++;
                execTicksStart = swInterrupt.ElapsedTicks;
                zx48.Execute();
                execTicks += (swInterrupt.ElapsedTicks - execTicksStart);

                if (swRedraw.ElapsedMilliseconds > 20)
                {
                    swRedraw.Restart();
                    runCount++;
                    elapsedTicksStart = swInterrupt.ElapsedTicks;
                    zx48.SendVideoEvent();
                    elapsedTicks += (swInterrupt.ElapsedTicks - elapsedTicksStart);
                }

                if (swInterrupt.ElapsedMilliseconds > 20) //50 раз в секунду
                {
                    zx48.Interrupt = true;
                    swInterrupt.Restart();
                }


                if (swSecond.ElapsedMilliseconds > 1000)
                {
                    swSecond.Restart();
                    elapsedTicks /= runCount;
                    milliseconds = (elapsedTicks * 1000) / Stopwatch.Frequency;
                    microseconds = (elapsedTicks * 1000000) / Stopwatch.Frequency;
                    nanoseconds = (elapsedTicks * 1000000000) / Stopwatch.Frequency;
                    Console.WriteLine($"Redraw {milliseconds} ms - {microseconds} mks - { nanoseconds} ns - Count = {runCount}");
                    runCount = 0;
                    elapsedTicks = 0;

                    double db = (double)execTicks /(double)execCount;
                    milliseconds = (long)((db * 1000) / Stopwatch.Frequency);
                    microseconds = (long)((db * 1000000) / Stopwatch.Frequency);
                    nanoseconds = (long)((db * 1000000000) / Stopwatch.Frequency);
                    Console.WriteLine($"Execute {milliseconds} ms - {microseconds} mks - { nanoseconds} ns - Count = {execCount}");
                    execCount = 0;
                    execTicks = 0;


                }

            }

            swSecond.Stop();
            swRedraw.Stop();
            swInterrupt.Stop();
        }


        private void BorderChange(BorderEventArgs e)
        {
            borderColor = ZxColor[e.BorderColor];
        }

        private void VideoChange(VideoEventArgs e)
        {
            if (!canRedraw) return;
            canRedraw = false;
            RedrawScreen(gForm, e.VideoRAM, e.AttrRAM);            
        }

        private byte KeyboardChange(ushort addr)
        {
            byte retByte = 0xFF;
            switch (addr)
            {
                //case 0xFEFE: retByte = keyBuff[0]; break;
                //case 0xFDFE: retByte = keyBuff[1]; break;
                //case 0xFBFE: retByte = keyBuff[2]; break;
                //case 0xF7FE: retByte = keyBuff[3]; break;
                //case 0xEFFE: retByte = keyBuff[4]; break;
                //case 0xDFFE: retByte = keyBuff[5]; break;
                //case 0xBFFE: retByte = keyBuff[6]; break;
                //case 0x7FFE: retByte = keyBuff[7]; break;
                default: { retByte = 0xFF; break; }
            }
            return retByte;
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
            int dColor = 0;


            Bitmap bmp = new Bitmap(scr_size_x, scr_size_y, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            unsafe
            {
                byte* data = (byte*)bmpData.Scan0.ToPointer();

                for (int m = 0; m < scr_size_y; m ++)
                    for (int t = 0; t < scr_size_x; t++)
                    {
                        data[(m * bmp.Width + t) * 3 + 0] = borderColor.B;
                        data[(m * bmp.Width + t) * 3 + 1] = borderColor.G;
                        data[(m * bmp.Width + t) * 3 + 2] = borderColor.R;
                    }

                for (int l = 0, x = 0, iy = 0; l < scr_ypoz.Length; l++)
                {
                    iy = (scr_ypoz[l] + dYOffset) * bmp.Width * 3;
                    int dZ = (scr_ypoz[l] / 8) * 32;
                    for (int i = 0; i < 32; i++, x++)
                    {
                        dColor = attr[dZ + i];

                        //Установлен бит 6 - повышенная яркость
                        if ((dColor & 0x40) > 0)
                        {
                            forecolor = ZxColorLight[dColor & 0x07];
                            backcolor = ZxColorLight[(dColor >> 3) & 0x07];
                        }
                        else
                        {
                            if ((dColor & 0x80) > 0 && flash)
                            {
                                forecolor = ZxColor[(dColor >> 3) & 0x07];
                                backcolor = ZxColor[dColor & 0x07];
                            }
                            else
                            {
                                forecolor = ZxColor[dColor & 0x07];
                                backcolor = ZxColor[(dColor >> 3) & 0x07];
                            }
                        }

                        int dx = (i * 8 + dXOffset) * 3;
                        byte vd = video[x];
                        int ix = 0;
                        for (int r = 0; r < 8; r++)
                        {
                            color = ((vd & (1 << r)) > 0) ? forecolor : backcolor;
                            ix = dx + (7 - r) * 3;
                            data[iy + ix + 0] = color.B; data[iy + ix + 1] = color.G; data[iy + ix + 2] = color.R;
                        }
                    }
                }

            }
            bmp.UnlockBits(bmpData);
            g.DrawImage(bmp, 0, dYOffset);
            canRedraw = true;
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (runZx48)
            {
                runZx48 = false;
                while (myThread.IsAlive) { };
                return;
            }
        }

        private void timerflash_Tick(object sender, EventArgs e)
        {
            flash = !flash;
            if (myThread != null && myThread.IsAlive)
                tsLabel.Image = ZxDisAsm.Properties.Resources.Status_off;
            else
                tsLabel.Image = ZxDisAsm.Properties.Resources.Status_on;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gForm = this.CreateGraphics();
            timer_flash.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.ControlKey: zx48.keyBuff[0] ^= 0x01; break;
                case Keys.Control: zx48.keyBuff[0] ^= 0x01; break;
                case Keys.Z: zx48.keyBuff[0] ^= 0x02; break;
                case Keys.X: zx48.keyBuff[0] ^= 0x04; break;
                case Keys.C: zx48.keyBuff[0] ^= 0x08; break;
                case Keys.V: zx48.keyBuff[0] ^= 0x10; break;

                case Keys.A: zx48.keyBuff[1] ^= 0x01; break;
                case Keys.S: zx48.keyBuff[1] ^= 0x02; break;
                case Keys.D: zx48.keyBuff[1] ^= 0x04; break;
                case Keys.F: zx48.keyBuff[1] ^= 0x08; break;
                case Keys.G: zx48.keyBuff[1] ^= 0x10; break;

                case Keys.Q: zx48.keyBuff[2] ^= 0x01; break;
                case Keys.W: zx48.keyBuff[2] ^= 0x02; break;
                case Keys.E: zx48.keyBuff[2] ^= 0x04; break;
                case Keys.R: zx48.keyBuff[2] ^= 0x08; break;
                case Keys.T: zx48.keyBuff[2] ^= 0x10; break;

                case Keys.D1: zx48.keyBuff[3] ^= 0x01; break;
                case Keys.D2: zx48.keyBuff[3] ^= 0x02; break;
                case Keys.D3: zx48.keyBuff[3] ^= 0x04; break;
                case Keys.D4: zx48.keyBuff[3] ^= 0x08; break;
                case Keys.D5: zx48.keyBuff[3] ^= 0x10; break;

                case Keys.D0: zx48.keyBuff[4] ^= 0x01; break;
                case Keys.D9: zx48.keyBuff[4] ^= 0x02; break;
                case Keys.D8: zx48.keyBuff[4] ^= 0x04; break;
                case Keys.D7: zx48.keyBuff[4] ^= 0x08; break;
                case Keys.D6: zx48.keyBuff[4] ^= 0x10; break;

                case Keys.P: zx48.keyBuff[5] ^= 0x01; break;
                case Keys.O: zx48.keyBuff[5] ^= 0x02; break;
                case Keys.I: zx48.keyBuff[5] ^= 0x04; break;
                case Keys.U: zx48.keyBuff[5] ^= 0x08; break;
                case Keys.Y: zx48.keyBuff[5] ^= 0x10; break;

                case Keys.Enter: zx48.keyBuff[6] ^= 0x01; break;
                case Keys.L: zx48.keyBuff[6] ^= 0x02; break;
                case Keys.K: zx48.keyBuff[6] ^= 0x04; break;
                case Keys.J: zx48.keyBuff[6] ^= 0x08; break;
                case Keys.H: zx48.keyBuff[6] ^= 0x10; break;

                case Keys.Space: zx48.keyBuff[7] ^= 0x01; break;
                case Keys.ShiftKey: zx48.keyBuff[7] ^= 0x02; break;
                case Keys.Shift: zx48.keyBuff[7] ^= 0x02; break;
                case Keys.M: zx48.keyBuff[7] ^= 0x04; break;
                case Keys.N: zx48.keyBuff[7] ^= 0x08; break;
                case Keys.B: zx48.keyBuff[7] ^= 0x10; break;

                default:
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.ControlKey: zx48.keyBuff[0] |= 0x01; break;
                case Keys.Control: zx48.keyBuff[0] |= 0x01; break;
                case Keys.Z: zx48.keyBuff[0] |= 0x02; break;
                case Keys.X: zx48.keyBuff[0] |= 0x04; break;
                case Keys.C: zx48.keyBuff[0] |= 0x08; break;
                case Keys.V: zx48.keyBuff[0] |= 0x10; break;

                case Keys.A: zx48.keyBuff[1] |= 0x01; break;
                case Keys.S: zx48.keyBuff[1] |= 0x02; break;
                case Keys.D: zx48.keyBuff[1] |= 0x04; break;
                case Keys.F: zx48.keyBuff[1] |= 0x08; break;
                case Keys.G: zx48.keyBuff[1] |= 0x10; break;

                case Keys.Q: zx48.keyBuff[2] |= 0x01; break;
                case Keys.W: zx48.keyBuff[2] |= 0x02; break;
                case Keys.E: zx48.keyBuff[2] |= 0x04; break;
                case Keys.R: zx48.keyBuff[2] |= 0x08; break;
                case Keys.T: zx48.keyBuff[2] |= 0x10; break;

                case Keys.D1: zx48.keyBuff[3] |= 0x01; break;
                case Keys.D2: zx48.keyBuff[3] |= 0x02; break;
                case Keys.D3: zx48.keyBuff[3] |= 0x04; break;
                case Keys.D4: zx48.keyBuff[3] |= 0x08; break;
                case Keys.D5: zx48.keyBuff[3] |= 0x10; break;

                case Keys.D0: zx48.keyBuff[4] |= 0x01; break;
                case Keys.D9: zx48.keyBuff[4] |= 0x02; break;
                case Keys.D8: zx48.keyBuff[4] |= 0x04; break;
                case Keys.D7: zx48.keyBuff[4] |= 0x08; break;
                case Keys.D6: zx48.keyBuff[4] |= 0x10; break;

                case Keys.P: zx48.keyBuff[5] |= 0x01; break;
                case Keys.O: zx48.keyBuff[5] |= 0x02; break;
                case Keys.I: zx48.keyBuff[5] |= 0x04; break;
                case Keys.U: zx48.keyBuff[5] |= 0x08; break;
                case Keys.Y: zx48.keyBuff[5] |= 0x10; break;

                case Keys.Enter: zx48.keyBuff[6] |= 0x01; break;
                case Keys.L: zx48.keyBuff[6] |= 0x02; break;
                case Keys.K: zx48.keyBuff[6] |= 0x04; break;
                case Keys.J: zx48.keyBuff[6] |= 0x08; break;
                case Keys.H: zx48.keyBuff[6] |= 0x10; break;

                case Keys.Space: zx48.keyBuff[7] |= 0x01; break;
                case Keys.ShiftKey: zx48.keyBuff[7] |= 0x02; break;
                case Keys.Shift: zx48.keyBuff[7] |= 0x02; break;
                case Keys.M: zx48.keyBuff[7] |= 0x04; break;
                case Keys.N: zx48.keyBuff[7] |= 0x08; break;
                case Keys.B: zx48.keyBuff[7] |= 0x10; break;

                default:
                    break;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            String path = @"c:\Coding\ZxDisAsm\ZxDisAsm\Games\123.sna";
            Z80Reader zx80Reader = new Z80Reader();
            zx80Reader.Read(path);

            zx48.SetMempory(zx80Reader.Memory, zx80Reader.Header);
        }

    }

}
