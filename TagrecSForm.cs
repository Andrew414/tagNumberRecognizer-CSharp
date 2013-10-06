using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.Threading;

namespace Tagrec_S
{
    public partial class TagrecSForm : Form
    {

        private CvCapture cvCapture;
        private bool bCapturing;
        private List<Bitmap> lstBmpSavedNumbers;


        public TagrecSForm()
        {
            InitializeComponent();
            InitCapture();
        }

        public void InitCapture()
        {
            cvCapture = Cv.CreateCameraCapture(CaptureDevice.Any);

            if (cvCapture != null)
            {
                tmrCapture.Enabled = true;
                bCapturing = true;
            }

            lstBmpSavedNumbers = new List<Bitmap>();
        }

        private void tmrCapture_Tick(object sender, EventArgs e)
        {
            if (!bCapturing)
            {
                return;
            }

            Application.DoEvents();

            IplImage snapshot = cvCapture.QueryFrame();
            Bitmap bmpSnapshot = snapshot.ToBitmap();
            pbxCurrentImage.BackgroundImage = bmpSnapshot;

            
            if (true)
            {
                ilsSavedImages.Images.Add(bmpSnapshot);
                lstSavedNumbers.Items.Add(new ListViewItem("2013-10-06 15:23 " + "8739 IK-I 5", lstBmpSavedNumbers.Count));
                lstBmpSavedNumbers.Add(bmpSnapshot);
            }

            Application.DoEvents();
        }

        private void TagrecSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cvCapture = null;
        }

        private void StartCaptureTimer()
        {
            tmrCapture.Enabled = true;
        }

        private void StopCaptureTimer()
        {
            tmrCapture.Enabled = false;
        }

        private void StartStopCaptureTimer()
        {
            if (tmrCapture.Enabled) 
            { 
                StopCaptureTimer(); 
            } 
            else 
            {
                StartCaptureTimer();
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            StartStopCaptureTimer();
            bCapturing = tmrCapture.Enabled;
            btnStartStop.Text = bCapturing ? "Stop Capturing" : "Start Capturing";
        }

        private void lstSavedNumbers_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopCaptureTimer();
            bCapturing = false;
            btnStartStop.Text = "Start Capturing";


            if (lstSavedNumbers.FocusedItem != null)
            {
                pbxCurrentImage.BackgroundImage = lstBmpSavedNumbers[lstSavedNumbers.FocusedItem.Index];
            }
        }

        private void lstSavedNumbers_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (!bCapturing)
            {
                return;
            }

            StartStopCaptureTimer();
        }
    }
}
