using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace Tagrec_S
{
    struct ContourInfo
    {
        public double P;
        public double S;

        public CvBox2D Box;

        public ContourInfo(double p, double s, CvBox2D box)
        {
            P = p;
            S = s;
            Box = box;
        }
    }

    class NLPlateReader : IPlateReader
    {
        public NLPlateReader(TagrecSForm form)
        {
            myForm = form;
        }

        TagrecSForm myForm;

        int before = 20;

        public bool IsNumberOrLetter(ContourInfo info)
        {

            double angle = Math.Abs(info.Box.Angle);

            if (angle > 40 && angle < 50)
            {
                return false; // little ones;
            }

            if (info.Box.Size.Height > 90 && info.Box.Size.Height < 110 && angle < 5)
            {
                return true; //Just straight numbers
            }

            if (info.Box.Size.Width > 90 && info.Box.Size.Width < 110 && angle > 80 && angle < 100)
            {
                return true; //90-rotated ones
            }

            if (angle > 65 && angle < 75 && info.Box.Size.Width > 80 && info.Box.Size.Width < 100)
            {
                return true; // this is 4
            }

            return false;
        }

        private void SafeSetPixel(ref Bitmap bmp, int x, int y, Color color)
        {
            if (x >= 0 && x < bmp.Width && y >= 0 && y < bmp.Height)
            {
                bmp.SetPixel(x, y, color);
            }
        }

        public void DrawBorder(ref Bitmap bmp, ContourInfo info)
        {
            Color BorderColor = Color.Red;

            double angle = Math.Abs(info.Box.Angle);

            Rectangle numberRectangle = new Rectangle();

            if (angle < 5)
            {
                numberRectangle = new Rectangle(
                    (int)(info.Box.Center.X - (info.Box.Size.Width / 2)),
                    (int)(info.Box.Center.Y - (info.Box.Size.Height / 2)),
                    (int)(info.Box.Size.Width), (int)(info.Box.Size.Height));
            }
            else if (angle > 80 && angle < 100)
            {
                numberRectangle = new Rectangle(
                    (int)(info.Box.Center.X - (info.Box.Size.Height / 2)),
                    (int)(info.Box.Center.Y - (info.Box.Size.Width / 2)),
                    (int)(info.Box.Size.Height), (int)(info.Box.Size.Width));
            }
            else if (angle > 65 && angle < 75)
            {
                numberRectangle = new Rectangle(
                    (int)(info.Box.Center.X - (info.Box.Size.Height / 2) - 10),
                    (int)(info.Box.Center.Y - (info.Box.Size.Width / 2) - 5),
                    (int)(info.Box.Size.Height) + 5, (int)(info.Box.Size.Width) + 10);
            }

            int BorderSize = 2;

            for (int i = numberRectangle.Top; i < numberRectangle.Bottom; i++)
            {
                for (int j = 0; j < BorderSize; j++)
                {
                    SafeSetPixel(ref bmp, numberRectangle.Left + j, i, BorderColor);
                    SafeSetPixel(ref bmp, numberRectangle.Right - j, i, BorderColor);
                }
            }

            for (int i = numberRectangle.Left; i < numberRectangle.Right; i++)
            {
                for (int j = 0; j < BorderSize; j++)
                {
                    SafeSetPixel(ref bmp, i, numberRectangle.Top + j, BorderColor);
                    SafeSetPixel(ref bmp, i, numberRectangle.Bottom - j, BorderColor);
                }
            }
        }

        public String ReadPlate(IplImage iplImage)
        {
            IplImage ipl = new IplImage(new CvSize(650, 150), iplImage.Depth, iplImage.NChannels);
            iplImage.Resize(ipl);
            //myForm.Text = ipl.Size.Width.ToString() + "x" + ipl.Size.Height.ToString();

            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

            myForm.pbxCurrentImage.BackgroundImage = binary.ToBitmap();

            CvMemStorage storage = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> contours;

            int count = Cv.FindContours(binary, storage, out contours);

            List<ContourInfo> conInfo = new List<ContourInfo>();

            for (int i = 0; i < count; i++)
            {
                conInfo.Add(new ContourInfo
                    (Math.Abs(Cv.ContourPerimeter(contours)),
                     Math.Abs(Cv.ContourArea(contours)),
                     Cv.MinAreaRect2(contours)));

                contours = contours.HNext;
            }

            List<ContourInfo> possibleNumbersAndLetters = conInfo.Where(IsNumberOrLetter).ToList();

            if (before-- == 0)
            {
                myForm.ilsSavedImages.Images.Add(ipl.ToBitmap());
                myForm.lstSavedNumbers.Items.Add("ALL image");
                myForm.lstBmpSavedNumbers.Add(ipl.ToBitmap());

                foreach (var i in possibleNumbersAndLetters)
                {
                    Rectangle newRect = new Rectangle(
                    (int)(i.Box.Center.X - (i.Box.Size.Width / 2)),
                    (int)(i.Box.Center.Y - (i.Box.Size.Height / 2)),
                    (int)(i.Box.Size.Width), (int)(i.Box.Size.Height));

                    ipl.SetROI(newRect.Left, newRect.Top,
                    newRect.Width, newRect.Height);
                    IplImage justNumber = new IplImage(Cv.GetSize(ipl), ipl.Depth, ipl.NChannels);
                    ipl.Copy(justNumber);
                    ipl.ResetROI();

                    myForm.ilsSavedImages.Images.Add(justNumber.ToBitmap());
                    myForm.lstSavedNumbers.Items.Add("angle=" + i.Box.Angle.ToString()
                        + " sz=" + ((int)(i.Box.Size.Width)).ToString() + "*" + 
                        ((int)(i.Box.Size.Height)).ToString());
                    myForm.lstBmpSavedNumbers.Add(justNumber.ToBitmap());
                }
            }

            Bitmap CoolBitmap = ipl.ToBitmap();

            foreach (var i in possibleNumbersAndLetters)
            {
                DrawBorder(ref CoolBitmap, i);
            }

            //myForm.pbxCurrentImage.BackgroundImage = binary.ToBitmap();
            myForm.pbxCurrentImage.BackgroundImage = CoolBitmap;

            myForm.Text = possibleNumbersAndLetters.Count.ToString();

            return "";
        }
    }
}
