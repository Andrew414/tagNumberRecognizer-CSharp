using System;
using System.Collections.Generic;
using System.Drawing;

using OpenCvSharp;

namespace Tagrec_S
{
    public class CaptureProcessor : IDisposable
    {
        public bool InitializedCorrectly = false;

        private IPlateFinder finder;
        private IPlateReader reader;
        private CvCapture cvCapture;
        public String lastNumberSaved;
        public List<Bitmap> lstBmpSavedNumbers;
        public Bitmap bmpSnapshot;

        public CaptureProcessor (string filename)
        {
            InitCapture (filename);
        }

        private void InitCapture(string filename)
        {
            lastNumberSaved = "";
            lstBmpSavedNumbers = new List<Bitmap>();

            if (filename != "")
            {
                cvCapture = new CvCapture (filename);
            }
            else
            {
                cvCapture = new CvCapture(CaptureDevice.Any);
            }

            if(cvCapture != null)
            {
                InitializedCorrectly = true;
            }

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

        public String MakeCapture()
        {
            Color BorderColor;

            IplImage snapshot = cvCapture.QueryFrame();
            if (snapshot == null)
            {
                return null;
            }
            bmpSnapshot = snapshot.ToBitmap();
            List<CvBox2D> rectangles = finder.FindRectangles(snapshot);
            String result = "";
            foreach (CvBox2D boxNumberRectangle in rectangles)
            {
                IplImage justNumber = getNumberAndRotatePerspective(boxNumberRectangle, snapshot);

                List<Rectangle> numbers;
                String carNumber = reader.ReadPlate(justNumber, out numbers);
                justNumber.SaveImage("kill.jpg");
                if (carNumber != "")
                {

                    BorderColor = Color.Green;

                    if (carNumber != lastNumberSaved)
                    {
                        lstBmpSavedNumbers.Add(bmpSnapshot);
                        lastNumberSaved = carNumber;
                        result = carNumber;
                    }
                }
                else 
                {
                    BorderColor = Color.Red;
                    lastNumberSaved = "";
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
            return result;
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

