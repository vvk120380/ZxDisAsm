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
        
        Color[] ZxColor = { Color.Black, Color.Blue, Color.Red, Color.Magenta, Color.Green, Color.Cyan, Color.Yellow, Color.White };
        Color[] ZxColorLight = { Color.FromArgb(0, 50, 50, 50), Color.LightBlue, Color.FromArgb(0, 255, 59, 59), Color.FromArgb(0, 255, 77, 255), Color.LightGreen, Color.LightCyan, Color.LightYellow, Color.LightGray };

        bool flash = false;

        Progress<ProgessRet> progress;
        Progress<ProgessRetBorder> progressBorder;

        Graphics gForm;

        static bool runZx48 = false;
        Thread myThread;
        Zx48Machine zx48;

        public Form1()
        {
            InitializeComponent();
            progress = new Progress<ProgessRet>(s => { RedrawScreen(gForm, s.videoRam, s.attrRam); });
            progressBorder = new Progress<ProgessRetBorder>(s => { RedrawBorder(gForm, s.border); });
        }

        async private void button1_Click(object sender, EventArgs e)
        {

            if (Worker.run)
            {
                Worker.run = false;
                return;
            }

            zx48 = new Zx48Machine();

            await Task.Factory.StartNew(() => Worker.StartZX(progress, progressBorder, zx48), TaskCreationOptions.DenyChildAttach);
            //RedrawScreen(gForm, zx48.GetVideoRAM(), zx48.GetAttRAM());
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (runZx48)
            {
                runZx48 = false;
                //myThread.Abort();
                while (myThread.IsAlive){};
                return;
            }
            zx48 = new Zx48Machine();
            zx48.BorderEvent += new BorderEventHandler(BorderChange);
            zx48.VideoEvent += new VideoEventHandler(VideoChange);
            runZx48 = true;
            myThread = new Thread(new ParameterizedThreadStart(this.StartZX));
            myThread.Start(zx48);
        }

        public void StartZX(object obj)
        {

            Zx48Machine zx48 = (Zx48Machine)obj;

            zx48.Reset();

            Stopwatch swRedraw = new Stopwatch();
            Stopwatch swInterrupt = new Stopwatch();
            Stopwatch swBorder = new Stopwatch();
            Stopwatch swlonger = new Stopwatch();

            swRedraw.Restart();
            swInterrupt.Restart();
            swBorder.Restart();

            double milliseconds;
            double microseconds;
            double nanoseconds;


            while (runZx48)
            {
                zx48.Execute();

                if (swRedraw.ElapsedMilliseconds > 100)
                {
                    swRedraw.Restart();
                    swlonger.Reset();
                    swlonger.Start();
                    zx48.SendVideoEvent();
                    swlonger.Stop();
                    milliseconds = (swlonger.ElapsedTicks * 1000) / Stopwatch.Frequency;
                    microseconds = (swlonger.ElapsedTicks * 1000000) / Stopwatch.Frequency;
                    nanoseconds  = (swlonger.ElapsedTicks * 1000000000) / Stopwatch.Frequency;
                    Console.WriteLine($"{milliseconds} ms - {microseconds} mks - { nanoseconds} ns");
                    //zx48.SendBorder();
                }

                if (swInterrupt.ElapsedMilliseconds > 20) //50 раз в секунду
                {
                    zx48.Interrupt = true;
                    swInterrupt.Restart();
                }

                if (swBorder.ElapsedMilliseconds > 1)
                {
                    //zx48.SendBorder();
                }
            }

            swRedraw.Stop();
            swInterrupt.Stop();
            swBorder.Stop();
        }


        private void BorderChange(object sender, BorderEventArgs e)
        {
            RedrawBorder(gForm, e.BorderColor);
        }

        private void VideoChange(object sender, VideoEventArgs e)
        {
            RedrawScreen(gForm, e.VideoRAM, e.AttrRAM);
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

        private void RedrawBorder(Graphics g, byte border)
        {
            int scr_size_x = 256;
            int scr_size_y = 192;

            g.FillRectangle(new SolidBrush(ZxColor[border]), 150, 30, scr_size_x + 60, 30);
            g.FillRectangle(new SolidBrush(ZxColor[border]), 150, scr_size_y + 60, scr_size_x + 60, 30);
            g.FillRectangle(new SolidBrush(ZxColor[border]), 150, 60, 30, scr_size_y);
            g.FillRectangle(new SolidBrush(ZxColor[border]), scr_size_x + 180, 60, 30, scr_size_y);
        }

        private void RedrawScreen(Graphics g, byte[] video, byte[] attr)
        {
            Color backcolor = Color.White;
            Color forecolor = Color.Black;
            Color color;

            int scr_size_x = 256;
            int scr_size_y = 192;
            int x = 0;
            int iy = 0;
            int dColor = 0;


            Bitmap bmp = new Bitmap(scr_size_x, scr_size_y, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            unsafe
            {
                byte* data = (byte*)bmpData.Scan0.ToPointer();

                for (int l = 0; l < scr_ypoz.Length; l++)
                {
                    iy = scr_ypoz[l] * bmp.Width * 3;
                    int dZ = (scr_ypoz[l] / 8) * 32;
                    for (int i = 0; i < 32; i++, x++)
                    {
                        dColor = attr[dZ + i];

                        //Установлен бит 6 - повышенная яркость
                        if ((dColor & 0x40) > 0)
                        {
                            forecolor = ZxColorLight[dColor & 0x7];
                            backcolor = ZxColorLight[(dColor >> 3) & 0x7];
                        }
                        else
                        {
                            if ((dColor & 0x80) > 0 && flash)
                            {
                                forecolor = ZxColor[(dColor >> 3) & 0x7];
                                backcolor = ZxColor[dColor & 0x7];
                            }
                            else
                            {
                                forecolor = ZxColor[dColor & 0x7];
                                backcolor = ZxColor[(dColor >> 3) & 0x7];
                            }


                            //Не установлен бит 7 - мерцание
                            //if ((dColor & 0x80) == 0)
                            //{
                            //    forecolor = ZxColor[dColor & 0x7];
                            //    backcolor = ZxColor[(dColor >> 3) & 0x7];
                            //}
                            //else
                            //    if (flash)
                            //    {
                            //        forecolor = ZxColor[(dColor >> 3) & 0x7];
                            //        backcolor = ZxColor[dColor & 0x7];
                            //    }
                            //    else
                            //    {
                            //        forecolor = ZxColor[dColor & 0x7];
                            //        backcolor = ZxColor[(dColor >> 3) & 0x7];
                            //    }
                        }

                        for (int r = 0; r < 8; r++)
                        {
                            color = ((video[x] & (1 << r)) > 0) ? forecolor : backcolor;
                            int ix = ((i * 8) + (7 - r)) * 3;
                            data[iy + ix + 0] = color.B;
                            data[iy + ix + 1] = color.G;
                            data[iy + ix + 2] = color.R;
                        }
                    }
                }

            }
            bmp.UnlockBits(bmpData);
            g.DrawImage(bmp, 180, 60);

            //// Get the address of the first line.
            //IntPtr ptr = bmpData.Scan0;

            //// Declare an array to hold the bytes of the bitmap.
            //int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            //byte[] rgbValues = new byte[bytes];

            //// Copy the RGB values into the array.
            //Marshal.Copy(ptr, rgbValues, 0, bytes);

            //for (int l = 0; l < scr_ypoz.Length; l++)
            //{
            //    iy = scr_ypoz[l] * bmp.Width * 3;
            //    for (int i = 0; i < 32; i++, x++)
            //    {
            //        dColor = attr[(scr_ypoz[l] / 8) * 32 + i];

            //        //Установлен бит 6 - повышенная яркость
            //        if ((dColor & 0x40) > 0)
            //        {
            //            forecolor = ZxColorLight[dColor & 0x7];
            //            backcolor = ZxColorLight[(dColor >> 30) & 0x7];
            //        }
            //        else
            //        {
            //            //Не установлен бит 7 - мерцание
            //            if ((dColor & 0x80) == 0)
            //            {
            //                forecolor = ZxColor[dColor & 0x7];
            //                backcolor = ZxColor[(dColor >> 3) & 0x7];
            //            }
            //            else
            //            {
            //                if (flash)
            //                {
            //                    forecolor = ZxColor[(dColor >> 3) & 0x7];
            //                    backcolor = ZxColor[dColor & 0x7];
            //                }
            //                else
            //                {
            //                    forecolor = ZxColor[dColor & 0x7];
            //                    backcolor = ZxColor[(dColor >> 3) & 0x7];
            //                }
            //            }
            //        }

            //        for (int r = 0; r < 8; r++)
            //        {
            //            color = ((video[x] & (1 << r)) > 0) ? forecolor : backcolor;
            //            int ix = ((i * 8) + (7 - r)) * 3;
            //            rgbValues[iy + ix + 0] = color.B;
            //            rgbValues[iy + ix + 1] = color.G;
            //            rgbValues[iy + ix + 2] = color.R;
            //        }
            //    }
            //}

            //// Copy the RGB values back to the bitmap
            //Marshal.Copy(rgbValues, 0, ptr, bytes);

            //// Unlock the bits.
            //bmp.UnlockBits(bmpData);
            //g.DrawImage(bmp, 180, 60);
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Worker.run) Worker.run = false;
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
                case Keys.ControlKey: zx48.port = 0xFEFE; zx48.key = 0xFE; zx48.keyBuff[0] ^= 0x01; break;
                case Keys.Z: zx48.port = 0xFEFE; zx48.key = 0xFD; zx48.keyBuff[0] ^= 0x02; break;
                case Keys.X: zx48.port = 0xFEFE; zx48.key = 0xFB; zx48.keyBuff[0] ^= 0x04; break;
                case Keys.C: zx48.port = 0xFEFE; zx48.key = 0xF7; zx48.keyBuff[0] ^= 0x08; break;
                case Keys.V: zx48.port = 0xFEFE; zx48.key = 0xEF; zx48.keyBuff[0] ^= 0x10; break;

                case Keys.A: zx48.port = 0xFDFE; zx48.key = 0xFE; zx48.keyBuff[1] ^= 0x01; break;
                case Keys.S: zx48.port = 0xFDFE; zx48.key = 0xFD; zx48.keyBuff[1] ^= 0x02; break;
                case Keys.D: zx48.port = 0xFDFE; zx48.key = 0xFB; zx48.keyBuff[1] ^= 0x04; break;
                case Keys.F: zx48.port = 0xFDFE; zx48.key = 0xF7; zx48.keyBuff[1] ^= 0x08; break;
                case Keys.G: zx48.port = 0xFDFE; zx48.key = 0xEF; zx48.keyBuff[1] ^= 0x10; break;

                case Keys.Q: zx48.port = 0xFBFE; zx48.key = 0xFE; zx48.keyBuff[2] ^= 0x01; break;
                case Keys.W: zx48.port = 0xFBFE; zx48.key = 0xFD; zx48.keyBuff[2] ^= 0x02; break;
                case Keys.E: zx48.port = 0xFBFE; zx48.key = 0xFB; zx48.keyBuff[2] ^= 0x04; break;
                case Keys.R: zx48.port = 0xFBFE; zx48.key = 0xF7; zx48.keyBuff[2] ^= 0x08; break;
                case Keys.T: zx48.port = 0xFBFE; zx48.key = 0xEF; zx48.keyBuff[2] ^= 0x10; break;

                case Keys.D1: zx48.port = 0xF7FE; zx48.key = 0xFE; zx48.keyBuff[3] ^= 0x01; break;
                case Keys.D2: zx48.port = 0xF7FE; zx48.key = 0xFD; zx48.keyBuff[3] ^= 0x02; break;
                case Keys.D3: zx48.port = 0xF7FE; zx48.key = 0xFB; zx48.keyBuff[3] ^= 0x04; break;
                case Keys.D4: zx48.port = 0xF7FE; zx48.key = 0xF7; zx48.keyBuff[3] ^= 0x08; break;
                case Keys.D5: zx48.port = 0xF7FE; zx48.key = 0xEF; zx48.keyBuff[3] ^= 0x10; break;

                case Keys.D0: zx48.port = 0xEFFE; zx48.key = 0xFE; zx48.keyBuff[4] ^= 0x01; break;
                case Keys.D9: zx48.port = 0xEFFE; zx48.key = 0xFD; zx48.keyBuff[4] ^= 0x02; break;
                case Keys.D8: zx48.port = 0xEFFE; zx48.key = 0xFB; zx48.keyBuff[4] ^= 0x04; break;
                case Keys.D7: zx48.port = 0xEFFE; zx48.key = 0xF7; zx48.keyBuff[4] ^= 0x08; break;
                case Keys.D6: zx48.port = 0xEFFE; zx48.key = 0xEF; zx48.keyBuff[4] ^= 0x10; break;

                case Keys.P: zx48.port = 0xDFFE; zx48.key = 0xFE; zx48.keyBuff[5] ^= 0x01; break;
                case Keys.O: zx48.port = 0xDFFE; zx48.key = 0xFD; zx48.keyBuff[5] ^= 0x02; break;
                case Keys.I: zx48.port = 0xDFFE; zx48.key = 0xFB; zx48.keyBuff[5] ^= 0x04; break;
                case Keys.U: zx48.port = 0xDFFE; zx48.key = 0xF7; zx48.keyBuff[5] ^= 0x08; break;
                case Keys.Y: zx48.port = 0xDFFE; zx48.key = 0xEF; zx48.keyBuff[5] ^= 0x10; break;

                case Keys.Enter: zx48.port = 0xBFFE; zx48.key = 0xFE; zx48.keyBuff[6] ^= 0x01; break;
                case Keys.L: zx48.port = 0xBFFE; zx48.key = 0xFD; zx48.keyBuff[6] ^= 0x02; break;
                case Keys.K: zx48.port = 0xBFFE; zx48.key = 0xFB; zx48.keyBuff[6] ^= 0x04; break;
                case Keys.J: zx48.port = 0xBFFE; zx48.key = 0xF7; zx48.keyBuff[6] ^= 0x08; break;
                case Keys.H: zx48.port = 0xBFFE; zx48.key = 0xEF; zx48.keyBuff[6] ^= 0x10; break;

                case Keys.Space: zx48.port = 0x7FFE; zx48.key = 0xFE; zx48.keyBuff[7] ^= 0x01; break;
                case Keys.ShiftKey: zx48.port = 0x7FFE; zx48.key = 0xFD; zx48.keyBuff[7] ^= 0x02; break;
                case Keys.M: zx48.port = 0x7FFE; zx48.key = 0xFB; zx48.keyBuff[7] ^= 0x04; break;
                case Keys.N: zx48.port = 0x7FFE; zx48.key = 0xF7; zx48.keyBuff[7] ^= 0x08; break;
                case Keys.B: zx48.port = 0x7FFE; zx48.key = 0xEF; zx48.keyBuff[7] ^= 0x10; break;

                default:
                    break;
            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey: zx48.port = 0xFEFE; zx48.key = 0xFE; zx48.keyBuff[0] |= 0x01; break;
                case Keys.Z: zx48.port = 0xFEFE; zx48.key = 0xFD; zx48.keyBuff[0] |= 0x02; break;
                case Keys.X: zx48.port = 0xFEFE; zx48.key = 0xFB; zx48.keyBuff[0] |= 0x04; break;
                case Keys.C: zx48.port = 0xFEFE; zx48.key = 0xF7; zx48.keyBuff[0] |= 0x08; break;
                case Keys.V: zx48.port = 0xFEFE; zx48.key = 0xEF; zx48.keyBuff[0] |= 0x10; break;

                case Keys.A: zx48.port = 0xFDFE; zx48.key = 0xFE; zx48.keyBuff[1] |= 0x01; break;
                case Keys.S: zx48.port = 0xFDFE; zx48.key = 0xFD; zx48.keyBuff[1] |= 0x02; break;
                case Keys.D: zx48.port = 0xFDFE; zx48.key = 0xFB; zx48.keyBuff[1] |= 0x04; break;
                case Keys.F: zx48.port = 0xFDFE; zx48.key = 0xF7; zx48.keyBuff[1] |= 0x08; break;
                case Keys.G: zx48.port = 0xFDFE; zx48.key = 0xEF; zx48.keyBuff[1] |= 0x10; break;

                case Keys.Q: zx48.port = 0xFBFE; zx48.key = 0xFE; zx48.keyBuff[2] |= 0x01; break;
                case Keys.W: zx48.port = 0xFBFE; zx48.key = 0xFD; zx48.keyBuff[2] |= 0x02; break;
                case Keys.E: zx48.port = 0xFBFE; zx48.key = 0xFB; zx48.keyBuff[2] |= 0x04; break;
                case Keys.R: zx48.port = 0xFBFE; zx48.key = 0xF7; zx48.keyBuff[2] |= 0x08; break;
                case Keys.T: zx48.port = 0xFBFE; zx48.key = 0xEF; zx48.keyBuff[2] |= 0x10; break;

                case Keys.D1: zx48.port = 0xF7FE; zx48.key = 0xFE; zx48.keyBuff[3] |= 0x01; break;
                case Keys.D2: zx48.port = 0xF7FE; zx48.key = 0xFD; zx48.keyBuff[3] |= 0x02; break;
                case Keys.D3: zx48.port = 0xF7FE; zx48.key = 0xFB; zx48.keyBuff[3] |= 0x04; break;
                case Keys.D4: zx48.port = 0xF7FE; zx48.key = 0xF7; zx48.keyBuff[3] |= 0x08; break;
                case Keys.D5: zx48.port = 0xF7FE; zx48.key = 0xEF; zx48.keyBuff[3] |= 0x10; break;

                case Keys.D0: zx48.port = 0xEFFE; zx48.key = 0xFE; zx48.keyBuff[4] |= 0x01; break;
                case Keys.D9: zx48.port = 0xEFFE; zx48.key = 0xFD; zx48.keyBuff[4] |= 0x02; break;
                case Keys.D8: zx48.port = 0xEFFE; zx48.key = 0xFB; zx48.keyBuff[4] |= 0x04; break;
                case Keys.D7: zx48.port = 0xEFFE; zx48.key = 0xF7; zx48.keyBuff[4] |= 0x08; break;
                case Keys.D6: zx48.port = 0xEFFE; zx48.key = 0xEF; zx48.keyBuff[4] |= 0x10; break;

                case Keys.P: zx48.port = 0xDFFE; zx48.key = 0xFE; zx48.keyBuff[5] |= 0x01; break;
                case Keys.O: zx48.port = 0xDFFE; zx48.key = 0xFD; zx48.keyBuff[5] |= 0x02; break;
                case Keys.I: zx48.port = 0xDFFE; zx48.key = 0xFB; zx48.keyBuff[5] |= 0x04; break;
                case Keys.U: zx48.port = 0xDFFE; zx48.key = 0xF7; zx48.keyBuff[5] |= 0x08; break;
                case Keys.Y: zx48.port = 0xDFFE; zx48.key = 0xEF; zx48.keyBuff[5] |= 0x10; break;

                case Keys.Enter: zx48.port = 0xBFFE; zx48.key = 0xFE; zx48.keyBuff[6] |= 0x01; break;
                case Keys.L: zx48.port = 0xBFFE; zx48.key = 0xFD; zx48.keyBuff[6] |= 0x02; break;
                case Keys.K: zx48.port = 0xBFFE; zx48.key = 0xFB; zx48.keyBuff[6] |= 0x04; break;
                case Keys.J: zx48.port = 0xBFFE; zx48.key = 0xF7; zx48.keyBuff[6] |= 0x08; break;
                case Keys.H: zx48.port = 0xBFFE; zx48.key = 0xEF; zx48.keyBuff[6] |= 0x10; break;

                case Keys.Space: zx48.port = 0x7FFE; zx48.key = 0xFE; zx48.keyBuff[7] |= 0x01; break;
                case Keys.ShiftKey: zx48.port = 0x7FFE; zx48.key = 0xFD; zx48.keyBuff[7] |= 0x02; break;
                case Keys.M: zx48.port = 0x7FFE; zx48.key = 0xFB; zx48.keyBuff[7] |= 0x04; break;
                case Keys.N: zx48.port = 0x7FFE; zx48.key = 0xF7; zx48.keyBuff[7] |= 0x08; break;
                case Keys.B: zx48.port = 0x7FFE; zx48.key = 0xEF; zx48.keyBuff[7] |= 0x10; break;

                default:
                    break;
            }

        }

    }

    public class Worker
    {
        public static bool run;
  

        public static void StartZX(IProgress<ProgessRet> progress, IProgress<ProgessRetBorder> progressBorder, Zx48Machine zx48)
        {
            byte borderOld = 0x00;
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

                if (swInterrupt.ElapsedMilliseconds > 20) //50 раз в секунду
                {
                    zx48.Interrupt = true;                        
                    swInterrupt.Restart();
                }

                if (borderOld != zx48.border)
                {
                    progressBorder.Report(new ProgessRetBorder(zx48.border));
                    borderOld = zx48.border;
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

    public class ProgessRetBorder
    {

        public ProgessRetBorder(byte border)
        {
            this.border = border;
        }

        public byte border;

    }
}
