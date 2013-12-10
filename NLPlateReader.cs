using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace Tagrec_S
{
    class ContourInfo : IComparable
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

        public int CompareTo(Object a)
        {
            return Box.Center.X.CompareTo((a as ContourInfo).Box.Center.X);
        }
    }


    class NLPlateReader : IPlateReader
    {
        public const int DigitsAmountInFirstGroup = 4;
        public const int LettersAmountInSecondGroup = 2;
        public const int DigitsAmountInThirdGroup = 1;

        public NLPlateReader(/*TagrecSForm form*/)
        {

        }

        ISignReader reader = new MaskSignReader();

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

        public static Rectangle ConvertBox2DToRectangle(CvBox2D box)
        {
            double angle = Math.Abs(box.Angle);

            if (angle < 5)
            {
                return new Rectangle(
                    (int)(box.Center.X - (box.Size.Width / 2)),
                    (int)(box.Center.Y - (box.Size.Height / 2)),
                    (int)(box.Size.Width), (int)(box.Size.Height));
            }
            else if (angle > 80 && angle < 100)
            {
                return new Rectangle(
                    (int)(box.Center.X - (box.Size.Height / 2)),
                    (int)(box.Center.Y - (box.Size.Width / 2)),
                    (int)(box.Size.Height), (int)(box.Size.Width));
            }
            else if (angle > 65 && angle < 75)
            {
                return new Rectangle(
                    (int)(box.Center.X - (box.Size.Height / 2) - 10),
                    (int)(box.Center.Y - (box.Size.Width / 2) - 5),
                    (int)(box.Size.Height) + 5, (int)(box.Size.Width) + 10);
            }
            else
            {
                return new Rectangle();
            }
        }

        public void DrawBorder(ref Bitmap bmp, ContourInfo info)
        {
            Color BorderColor = Color.Red;

            Rectangle numberRectangle = ConvertBox2DToRectangle(info.Box);

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

        public String ReadPlate(IplImage iplImage, out List<Rectangle> rectangles)
        {
            IplImage ipl = new IplImage(new CvSize(650, 150), iplImage.Depth, iplImage.NChannels);
            if (iplImage.Size.Width * iplImage.Size.Height == 0)
            {
                rectangles = null;
                return "";
            }
            iplImage.Resize(ipl);

            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

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
            possibleNumbersAndLetters.Sort();

            Bitmap CoolBitmap = ipl.ToBitmap();

            foreach (var i in possibleNumbersAndLetters)
            {
                DrawBorder(ref CoolBitmap, i);
            }

            rectangles = null;
            if (possibleNumbersAndLetters.Count == 7)
            {
                return RecognizeNumber(possibleNumbersAndLetters, ipl).Insert(4, " ");
            }
            else
            {
                return "";
            }
        }

        public String RecognizeNumber(List<ContourInfo> infos, IplImage ipl)
        {
            String finalNumber = "";

            int begin = 0;
            for (int i = begin; i < begin + DigitsAmountInFirstGroup; ++i)
            {
                Rectangle next = ConvertBox2DToRectangle(infos[i].Box);
                ipl.SetROI(next.Left, next.Top,
                    next.Width, next.Height);
                IplImage justSign = new IplImage(Cv.GetSize(ipl), ipl.Depth, ipl.NChannels);
                ipl.Copy(justSign);
                ipl.ResetROI();

                finalNumber += reader.ReadSign(justSign, false);
            }

            begin = DigitsAmountInFirstGroup;
            for (int i = begin; i < begin + LettersAmountInSecondGroup; ++i)
            {
                Rectangle next = ConvertBox2DToRectangle(infos[i].Box);
                ipl.SetROI(next.Left, next.Top,
                    next.Width, next.Height);
                IplImage justSign = new IplImage(Cv.GetSize(ipl), ipl.Depth, ipl.NChannels);
                ipl.Copy(justSign);
                ipl.ResetROI();

                finalNumber += reader.ReadSign(justSign, true);
            }

            begin += LettersAmountInSecondGroup;

            for (int i = begin; i < begin + DigitsAmountInThirdGroup; ++i)
            {
                Rectangle next = ConvertBox2DToRectangle(infos[i].Box);
                ipl.SetROI(next.Left, next.Top,
                    next.Width, next.Height);
                IplImage justSign = new IplImage(Cv.GetSize(ipl), ipl.Depth, ipl.NChannels);
                ipl.Copy(justSign);
                ipl.ResetROI();

                finalNumber += reader.ReadSign(justSign, false);
            }

            return finalNumber;

        }
    }
}
