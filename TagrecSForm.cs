using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AutoNumberRecognizer;

namespace Tagrec_S
{
    public partial class TagrecSForm : Form
    {

        private bool bCapturing;

        private CaptureProcessor capture;

        public TagrecSForm (string filename = "")
        {
            InitializeComponent();

            capture = new CaptureProcessor (filename);

            if (capture.InitializedCorrectly)
            {
                tmrCapture.Enabled = true;
                bCapturing = true;
            }
        }

        private void tmrCapture_Tick(object sender, EventArgs e)
        {
            if (!bCapturing)
            {
                return;
            }

            //Application.DoEvents();

            capture.MakeCapture ();

            if(capture.lastNumberSaved == "")
            {
                Text = "Searching...";
            }            
            else 
            {
                DateTime now = DateTime.Now;
                lstSavedNumbers.Items.Add(new ListViewItem(now.Year.ToString() + "-" + now.Month.ToString() + "-"
                                                           + now.Day.ToString() + " " + now.Hour + ":" + now.Minute + " " + capture.lastNumberSaved, capture.lstBmpSavedNumbers.Count));

                ilsSavedImages.Images.Add(capture.lstBmpSavedNumbers.Last());
                Text = capture.lastNumberSaved;
            }

            pbxCurrentImage.BackgroundImage = capture.bmpSnapshot;

            //Application.DoEvents();
        }

        private void TagrecSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            capture.Dispose ();
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
                pbxCurrentImage.BackgroundImage = capture.lstBmpSavedNumbers[lstSavedNumbers.FocusedItem.Index];
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
        

        private void btnSaveSelected_Click(object sender, EventArgs e)
        {
            if (lstSavedNumbers.FocusedItem != null)
            {
                if (sfdSaveSelected.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    capture.lstBmpSavedNumbers[lstSavedNumbers.FocusedItem.Index].Save(sfdSaveSelected.FileName);
                }
            }
        }
    }
}
