using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpLapse
{
    public partial class Form1 : Form
    {

        public static bool blRecording = false;
        public static bool blCountDown = false;
        private bool blPaused;


        Thread thrStartRecording;

        ScreenCapture screenCapture;

        public delegate void UpdateTextCallback(string text);
        public delegate string GetTextCallback();

        private List<Screen> lstScreens = new List<Screen>();


        public Form1()
        {
            InitializeComponent();

            foreach (Screen screen in Screen.AllScreens)
            {
                lstScreens.Add(screen);
                comboBox1.Items.Add(screen.DeviceName);
            }
        }

        private void startRecording()
        {

            Screen selectedScreen = null;

            string cmbScreen = lblStatus.Invoke(new GetTextCallback(getcmb1Text), new object[] { }) as string;


            foreach (Screen screen in lstScreens)
            {
                if (screen.DeviceName == cmbScreen)
                {
                    selectedScreen = screen;
                }
            }

            screenCapture = new ScreenCapture(Convert.ToInt32(txbFrameRate.Text), selectedScreen);

            float fltDelay = Convert.ToInt32(txbTime.Text);
            blCountDown = true;
            for (; fltDelay > 0; fltDelay -= 0.10f)
            {
                lblStatus.Invoke(new UpdateTextCallback(updateLblStatus), new string[] {"Waiting: " + Math.Round(fltDelay, 1).ToString(".0#")});
                Thread.Sleep(100);
            }
            lblStatus.Invoke(new UpdateTextCallback(updateLblStatus), new string[] { "Recording!" });
            blRecording = true;
            blCountDown = false;


            if (selectedScreen == null) {
                selectedScreen = lstScreens[0];
            }

        }

        private void updateLblStatus(string text)
        {
            lblStatus.Text = text;
        }

        private string getcmb1Text() {
            return comboBox1.Text;
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!blRecording && !blCountDown)
            {
                thrStartRecording = new Thread(() => startRecording());
                thrStartRecording.Start();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (blRecording)
            {
                lblStatus.Text = "Stopped";
                screenCapture = null;
                blRecording = false;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (blRecording)
            {
                blRecording = false;
                lblStatus.Text = "Paused";
                blPaused = true;
            }
            else if(blPaused)
            {
                blRecording = true;
                lblStatus.Text = "Recording";
                blPaused = false;
            }
        }
    }
}
