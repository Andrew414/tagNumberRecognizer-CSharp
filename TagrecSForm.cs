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

        private CvCapture cvCapture;
        private bool bCapturing;
        public List<Bitmap> lstBmpSavedNumbers;
        public String lastNumberSaved = "";

        IPlateFinder finder;
        IPlateReader reader;

        public TagrecSForm(String filename = "")
        {
            InitializeComponent();
            InitCapture(filename);
        }

        public void InitCapture(String filename)
        {
            if (filename == "")
            {
                cvCapture = Cv.CreateCameraCapture(CaptureDevice.Any);
            }
            else
            {
                cvCapture = Cv.CreateFileCapture(filename);
            }
            if (cvCapture != null)
            {
                tmrCapture.Enabled = true;
                bCapturing = true;
            }

            lstBmpSavedNumbers = new List<Bitmap>();

            finder = new MARPlateFinder ();
            reader = new NLPlateReader();
        }

        public static IplImage getNumberAndRotatePerspective(CvBox2D box, IplImage image)
        {
            CvPoint2D32f[] dstPnt = new CvPoint2D32f[4];

            float leftX = box.Center.X - box.Size.Width / (float)2;
            float bottomY = box.Center.Y - box.Size.Height / (float)2;

            dstPnt[1] = new CvPoint2D32f(0, 0);
            dstPnt[2] = new CvPoint2D32f(box.Size.Width, 0);
            dstPnt[0] = new CvPoint2D32f(0, box.Size.Height);
            dstPnt[3] = new CvPoint2D32f(box.Size.Width, box.Size.Height);
            
            CvBox2D newBox = box;
            newBox.Center.X -= leftX;
            newBox.Center.Y -= bottomY;

            CvMat mapMatrix = Cv.GetPerspectiveTransform(newBox.BoxPoints(), dstPnt);

            image.SetROI((int)leftX, (int)bottomY, (int)box.Size.Width, (int)box.Size.Height);
            IplImage justNumber = new IplImage(Cv.GetSize(image), image.Depth, image.NChannels);
            image.Copy(justNumber);
            image.ResetROI();

            IplImage dstImage = justNumber.Clone();

            Cv.WarpPerspective(justNumber, dstImage, mapMatrix);

            return dstImage;
        }

        private void SafeSetPixel(ref Bitmap bmp, int x, int y, Color color)
        {
            if (x >= 0 && x < bmp.Width && y >= 0 && y < bmp.Height)
            {
                bmp.SetPixel(x, y, color);
            }
        }

        private void ProcessRecognizedNumber()
        {

        }

        public static IplImage getJustNumberFromImage(IplImage snapshot, Rectangle numberRectangle)
        {
            snapshot.SetROI(numberRectangle.Left, numberRectangle.Top,
                numberRectangle.Width, numberRectangle.Height);
            IplImage justNumber = new IplImage(Cv.GetSize(snapshot), snapshot.Depth, snapshot.NChannels);
            snapshot.Copy(justNumber);
            snapshot.ResetROI();

            return justNumber;
        }

        private void tmrCapture_Tick(object sender, EventArgs e)
        {
            if (!bCapturing)
            {
                return;
            }

            Application.DoEvents();
            IplImage snapshot = cvCapture.QueryFrame();

            if (snapshot == null)
            {
                return;
            }
            Bitmap bmpSnapshot = snapshot.ToBitmap();
            List<CvBox2D> rectangles = finder.FindRectangles(snapshot);

            foreach (CvBox2D boxNumberRectangle in rectangles)
            {
                
                IplImage justNumber = getNumberAndRotatePerspective(boxNumberRectangle, snapshot);
                IplImage notRotated = getJustNumberFromImage(snapshot, NLPlateReader.ConvertBox2DToRectangle(boxNumberRectangle));

                List<Rectangle> numbers;
                String carNumber = reader.ReadPlate(justNumber, out numbers);

                Color BorderColor;

                if (carNumber != "")
                {
                    ProcessRecognizedNumber();
                    BorderColor = Color.Green;

                    if (carNumber != lastNumberSaved)
                    {
                        ilsSavedImages.Images.Add(bmpSnapshot);
                        DateTime now = DateTime.Now;
                        lstSavedNumbers.Items.Add(new ListViewItem(now.Year.ToString() + "-" + now.Month.ToString() + "-"
                         + now.Day.ToString() + " " + now.Hour + ":" + now.Minute + " " + carNumber, lstBmpSavedNumbers.Count));
                        lstBmpSavedNumbers.Add(bmpSnapshot);

                        lastNumberSaved = carNumber;
                    }

                    this.Text = carNumber;
                }
                else
                {
                    BorderColor = Color.Red;
                    this.Text = "Searching...";
                }

                int BorderSize = 5;

                Rectangle numberRectangle = NLPlateReader.ConvertBox2DToRectangle(boxNumberRectangle);
                for (int i = numberRectangle.Top; i < numberRectangle.Bottom; i++)
                {
                    for (int j = 0; j < BorderSize; j++)
                    {
                        SafeSetPixel(ref bmpSnapshot, numberRectangle.Left + j, i, BorderColor);
                        SafeSetPixel(ref bmpSnapshot, numberRectangle.Right - j, i, BorderColor);
                    }
                }

                for (int i = numberRectangle.Left; i < numberRectangle.Right; i++)
                {
                    for (int j = 0; j < BorderSize; j++)
                    {
                        SafeSetPixel(ref bmpSnapshot, i, numberRectangle.Top + j, BorderColor);
                        SafeSetPixel(ref bmpSnapshot, i, numberRectangle.Bottom - j, BorderColor);
                    }
                }

            }
            
            if (rectangles.Count == 0)
            {
                this.Text = "Searching...";
            }

            pbxCurrentImage.BackgroundImage = bmpSnapshot;

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
