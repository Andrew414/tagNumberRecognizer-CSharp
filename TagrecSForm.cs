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
using System.IO;
using System.Threading;

namespace Tagrec_S
{
    public partial class TagrecSForm : Form
    {

        private bool bCapturing;
        public List<Bitmap> lstBmpSavedNumbers;
        public String lastNumberSaved = "";

        CaptureProcessor processor;

        public TagrecSForm(String filename = "")
        {
            InitializeComponent();
            processor = new CaptureProcessor(filename);
            if (processor.InitializedCorrectly)
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

            String result = processor.MakeCapture();
            if (result == null)
            {
                return;
            }
            if (result == "")
            {
                Text = "Searching...";
            }
            else
            {
                DateTime now = DateTime.Now;
                lstSavedNumbers.Items.Add(new ListViewItem(now.Year.ToString() + "-" + now.Month.ToString() + "-"
                                                           + now.Day.ToString() + " " + now.Hour + ":" + now.Minute + " " + result, processor.lstBmpSavedNumbers.Count));

                ilsSavedImages.Images.Add(processor.lstBmpSavedNumbers.Last());
                Text = processor.lastNumberSaved;
            }

            pbxCurrentImage.BackgroundImage = processor.bmpSnapshot;
        }

        private void TagrecSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        private void btnSaveSelected_Click(object sender, EventArgs e)
        {
            if (lstSavedNumbers.FocusedItem != null)
            {
                if (sfdSaveSelected.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lstBmpSavedNumbers[lstSavedNumbers.FocusedItem.Index].Save(sfdSaveSelected.FileName);
                }
            }
        }
    }
}

