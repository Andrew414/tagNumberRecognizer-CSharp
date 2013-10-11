using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;

namespace Tagrec_S
{
    class ContourInfo : IComparable
    {
        public double P;
        public double S;

        public MCvBox2D Box;

        public ContourInfo(double p, double s, MCvBox2D box)
        {
            P = p;
            S = s;
            Box = box;
        }

        public int CompareTo(Object a)
        {
            return Box.center.X.CompareTo((a as ContourInfo).Box.center.X);
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

            double angle = Math.Abs(info.Box.angle);

            if (angle > 40 && angle < 50)
            {
                return false; // little ones;
            }

            if (info.Box.size.Height > 90 && info.Box.size.Height < 110 && angle < 5)
            {
                return true; //Just straight numbers
            }

            if (info.Box.size.Width > 90 && info.Box.size.Width < 110 && angle > 80 && angle < 100)
            {
                return true; //90-rotated ones
            }

            if (angle > 65 && angle < 75 && info.Box.size.Width > 80 && info.Box.size.Width < 100)
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

        public Rectangle ConvertBox2DToRectangle(MCvBox2D box)
        {
            double angle = Math.Abs(box.angle);

            if (angle < 5)
            {
                return new Rectangle(
                    (int)(box.center.X - (box.size.Width / 2)),
                    (int)(box.center.Y - (box.size.Height / 2)),
                    (int)(box.size.Width), (int)(box.size.Height));
            }
            else if (angle > 80 && angle < 100)
            {
                return new Rectangle(
                    (int)(box.center.X - (box.size.Height / 2)),
                    (int)(box.center.Y - (box.size.Width / 2)),
                    (int)(box.size.Height), (int)(box.size.Width));
            }
            else if (angle > 65 && angle < 75)
            {
                return new Rectangle(
                    (int)(box.center.X - (box.size.Height / 2) - 10),
                    (int)(box.center.Y - (box.size.Width / 2) - 5),
                    (int)(box.size.Height) + 5, (int)(box.size.Width) + 10);
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

        public String ReadPlate(IImage iplImage, out List<Rectangle> rectangles)
        {
            var size = new Size (650, 150);
            double scale = iplImage.Size.Height / 150.0;
            Image<Bgr, Byte> ipl = ((Image<Bgr, Byte>)iplImage).Resize (scale, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            Image<Gray, Byte> gray = ipl.Convert<Gray, Byte>();

            Image<Gray, Byte> blur = gray.SmoothBlur(5, 5);

            Image<Gray, Byte> binary = blur.ThresholdBinary(new Gray(149), new Gray(255));

            //binary.ToBitmap().Save("/Users/pavel/Downloads/test.bmp");

            List<ContourInfo> conInfo = new List<ContourInfo> ();

            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contours = binary.FindContours(); contours != null; contours = contours.HNext) 
                {
                    Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    conInfo.Add (new ContourInfo (Math.Abs (currentContour.Perimeter), 
                                             Math.Abs (currentContour.Area), currentContour.GetMinAreaRect ()));
                }
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
                return RecognizeNumber(possibleNumbersAndLetters, ipl).Insert(4, " ").Insert(7, "-");
            }
            else
            {
                return "";
            }
        }

        public String RecognizeNumber(List<ContourInfo> infos, Image<Bgr, Byte> ipl)
        {
            String finalNumber = "";
			// TODO: rewrite
            int begin = 0;
            for (int i = begin; i < begin + DigitsAmountInFirstGroup; ++i)
            {
                Rectangle next = ConvertBox2DToRectangle(infos[i].Box);
                var defaultROI = ipl.ROI;
                ipl.ROI = next;
                Image<Bgr, Byte> justSign = ipl.Clone ();

                ipl.ROI = defaultROI;

                finalNumber += reader.ReadSign(justSign, false);    
            }

            begin = DigitsAmountInFirstGroup;
            for (int i = begin; i < begin + LettersAmountInSecondGroup; ++i)
            {
                Rectangle next = ConvertBox2DToRectangle(infos[i].Box);
                var defaultROI = ipl.ROI;
                ipl.ROI = next;
                Image<Bgr, Byte> justSign = ipl.Clone ();

                ipl.ROI = defaultROI;
                finalNumber += reader.ReadSign(justSign, true);    
            }

            begin += LettersAmountInSecondGroup;

            for (int i = begin; i < begin + DigitsAmountInThirdGroup; ++i)
            {
                Rectangle next = ConvertBox2DToRectangle(infos[i].Box);
                var defaultROI = ipl.ROI;
                ipl.ROI = next;
                Image<Bgr, Byte> justSign = ipl.Clone ();

                ipl.ROI = defaultROI;

                finalNumber += reader.ReadSign(justSign, false);
            }

            return finalNumber;

        }
    }
}
