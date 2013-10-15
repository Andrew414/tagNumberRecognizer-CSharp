using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using AutoNumberRecognizer;

namespace Tagrec_S
{
    public class CaptureProcessor : IDisposable
    {
        public bool InitializedCorrectly = false;

        private IPlateFinder finder;
        private IPlateReader reader;
        private Capture cvCapture;
        public string lastNumberSaved = "";
        public List<Bitmap> lstBmpSavedNumbers;
        public Bitmap bmpSnapshot;

        public CaptureProcessor (string filename)
        {
            InitCapture (filename);
        }

        private void InitCapture(string filename)
        {
            lstBmpSavedNumbers = new List<Bitmap>();

            if (filename != "")
            {
                cvCapture = new Capture (filename);
            }
            else
            {
                cvCapture = new Capture(CaptureType.ANY);
            }

            if(cvCapture != null)
            {
                InitializedCorrectly = true;
            }

            finder = new MARPlateFinder ();
            reader = new NLPlateReader();
        }

        public void MakeCapture()
        {
            Color BorderColor;

            Image<Bgr, Byte> snapshot = cvCapture.QueryFrame();
            bmpSnapshot = snapshot.ToBitmap();
            //Bitmap bmpSnapshot = finder.Transform(snapshot).ToBitmap();
            Rectangle numberRectangle = finder.FindRectangle(snapshot);

            if (numberRectangle != new Rectangle())
            {
                var defaultRoi = snapshot.ROI;
                snapshot.ROI = numberRectangle;

                var justNumber = snapshot.Clone ();

                snapshot.ROI = defaultRoi;

                List<Rectangle> numbers;
                String carNumber = reader.ReadPlate(justNumber, out numbers);

                if (carNumber != "")
                {
                    ProcessRecognizedNumber();

                    BorderColor = Color.Green;

                    if (carNumber != lastNumberSaved)
                    {
                        lstBmpSavedNumbers.Add(bmpSnapshot);
                        lastNumberSaved = carNumber;
                    }
                }
                else 
                {
                    BorderColor = Color.Red;
                    lastNumberSaved = "";
                }

                int BorderSize = 5;

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

                //bmpSnapshot = justNumber.ToBitmap();
            }
        }

        private void ProcessRecognizedNumber()
        {

        }

        private void SafeSetPixel(ref Bitmap bmp, int x, int y, Color color)
        {
            if (x >= 0 && x < bmp.Width && y >= 0 && y < bmp.Height)
            {
                bmp.SetPixel(x, y, color);
            }
        }


        #region IDisposable implementation

        public void Dispose ()
        {
            cvCapture = null;
        }

        #endregion
    }
}

