using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using AviFile;

namespace SharpLapse
{
    class ScreenCapture
    {

        int intTop;
        int intleft;
        int intWidth;
        int intHeight;
        Size size;

        Bitmap printscreen;
        Graphics gfx;

       // int currentImg = 0;

        string strPath;

        AviManager aviMan;
        VideoStream videoStream;


        public ScreenCapture(int fps, Screen screen) {


            string x = System.DateTime.Now.ToString();
            x = x.Replace(@"/", ".");
            x = x.Replace(@":", ".");

            strPath = @"/Videos/";

            intTop = screen.Bounds.X;
            intleft = screen.Bounds.Y;
            intWidth = screen.Bounds.Width;
            intHeight = screen.Bounds.Height;
            size = screen.Bounds.Size;

            capture();

            aviMan = new AviManager(x + ".avi", false);
            videoStream = aviMan.AddVideoStream(true, 30, printscreen);
            printscreen.Dispose();

            Console.WriteLine(strPath);
            System.IO.Directory.CreateDirectory(strPath);

            System.Timers.Timer aTimer;
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000 / fps;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (Form1.blRecording) {

                capture();

                try
                {
                    videoStream.AddFrame(printscreen);
                }
                catch (Exception)
                {}
                printscreen.Dispose();
                //printscreen.Save(path + @"\" + currentImg.ToString() + ".png", ImageFormat.Png);
                //currentImg++;

            }
        }

        private void capture() {
            printscreen = new Bitmap(intWidth, intHeight);
            gfx = Graphics.FromImage(printscreen as Image);
            gfx.CopyFromScreen(intTop, intleft, 0, 0, printscreen.Size);
        }

        ~ScreenCapture() {
            aviMan.Close();
        }

    }
}
