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
        public double P; // perimeter
        public double S; // square

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
        // standart belarusian number has 4 digits, next 2 letters and the last digit.
        // 1234 AB-7
        
        public const int DigitsAmountInFirstGroup = Constants.PLATEREADER_NUMBERS_1ST_GROUP;
        public const int LettersAmountInSecondGroup = Constants.PLATEREADER_LETTERS_2ND_GROUP;
        public const int DigitsAmountInThirdGroup = Constants.PLATEREADER_NUMBERS_3RD_GROUP;

        public NLPlateReader(/*TagrecSForm form*/)
        {

        }

        ISignReader reader = new MaskSignReader();

        public bool IsNumberOrLetter(ContourInfo info)
        {

            double angle = Math.Abs(info.Box.Angle);

            if (angle > Constants.READER_LITTLE_ITEMS_ANGLE_FROM && 
                angle < Constants.READER_LITTLE_ITEMS_ANGLE_TO)
            {
                return false; // little ones;
            }

            //valid numbers are divided into 3 groups:
            // straight
            // 90-rotated, sometimes letter A recognized in this way
            // 70-rotated, usually digit 4 and sometimes 7 recognized in this way

            if (info.Box.Size.Height > Constants.READER_NO_ROTATE_HEIGHT_FROM && 
                info.Box.Size.Height < Constants.READER_NO_ROTATE_HEIGHT_TO &&
                angle < Constants.READER_NO_ROTATE_MAX_ANGLE)
            {
                return true; //Just straight numbers
            }

            if (info.Box.Size.Width > Constants.READER_90_ROTATE_HEIGHT_FROM && 
                info.Box.Size.Width < Constants.READER_90_ROTATE_HEIGHT_TO &&
                angle > Constants.READER_90_ROTATE_MIN_ANGLE &&
                angle < Constants.READER_90_ROTATE_MAX_ANGLE)
            {
                return true; //90-rotated ones
            }

            if (angle > Constants.READER_70_ROTATE_MIN_ANGLE && 
                angle < Constants.READER_70_ROTATE_MAX_ANGLE && 
                info.Box.Size.Width > Constants.READER_70_ROTATE_WIDTH_FROM && 
                info.Box.Size.Width < Constants.READER_70_ROTATE_WIDTH_TO)
            {
                return true; // this is 4 or maybe 7
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

            if (angle < Constants.READER_NO_ROTATE_MAX_ANGLE)
            {
                return new Rectangle(
                    (int)(box.Center.X - (box.Size.Width / 2)),
                    (int)(box.Center.Y - (box.Size.Height / 2)),
                    (int)(box.Size.Width), (int)(box.Size.Height));
            }
            else if (angle > Constants.READER_90_ROTATE_MIN_ANGLE && 
                     angle < Constants.READER_90_ROTATE_MAX_ANGLE)
            {
                return new Rectangle(
                    (int)(box.Center.X - (box.Size.Height / 2)),
                    (int)(box.Center.Y - (box.Size.Width / 2)),
                    (int)(box.Size.Height), (int)(box.Size.Width)); // swap W and H
            }
            else if (angle > Constants.READER_70_ROTATE_MIN_ANGLE &&
                     angle < Constants.READER_70_ROTATE_MAX_ANGLE)
            {
                // below the image is also rotated more than for 45, so 
                // it should also be swapper by W and H

                return new Rectangle(
                    (int)(box.Center.X - (box.Size.Height / 2) + Constants.READER_70_DELTA_X),
                    (int)(box.Center.Y - (box.Size.Width / 2) + Constants.READER_70_DELTA_Y),
                    (int)(box.Size.Height) + Constants.READER_70_DELTA_WIDTH,
                    (int)(box.Size.Width) + Constants.READER_70_DELTA_HEIGHT);  
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
            IplImage ipl = new IplImage(new CvSize(
                Constants.READER_DEFAULT_WIDTH, 
                Constants.READER_DEFAULT_HEIGHT), 
                iplImage.Depth, iplImage.NChannels);

            if (iplImage.Size.Width * iplImage.Size.Height == 0)
            {
                rectangles = null;
                return "";
            }
            iplImage.Resize(ipl);

            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur,
                Constants.PLATEREADER_BLUR_SIZE, 
                Constants.PLATEREADER_BLUR_SIZE);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, Constants.PLATEREADER_THRESHOLD_FROM, Constants.PLATEREADER_THRESHOLD_TO, ThresholdType.Otsu);

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
            if (possibleNumbersAndLetters.Count >= Constants.PLATEREADER_NUMBER_LEN)
            {
                return RecognizeNumber(possibleNumbersAndLetters, ipl);
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
