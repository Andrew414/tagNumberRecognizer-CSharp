using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;

namespace Tagrec_S
{
    class MARPlateFinder : IPlateFinder
    {
        public MARPlateFinder()
        {
        }


        private Rectangle CvBox2DToRectangle(CvBox2D box)
        {
            return new Rectangle();
        }

        private bool IsPixelRed(Color color)
        {
            return (color.R > 210 && color.G < 200 && color.B < 200);
        }

        private bool IsPixelGreen(Color color)
        {
            return (color.G > 210 && color.R < 200);
        }

        private bool IsPixelWhite(Color color)
        {
            return (color.R > 200 && color.G > 200 && color.B > 200);
        }

        private bool IsPixelBlack(Color color)
        {
            return (color.R + color.G + color.B < 500);
        }

        private bool CheckPlate(IplImage ipl, CvBox2D box)
        {
            return true;
        }

        public Rectangle FindRectangle(IplImage ipl)
        {
            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

            IplImage canny = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Canny(canny, 0, 100);


            CvMemStorage storage = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> contours;

            int count = Cv.FindContours(binary, storage, out contours);

            CvBox2D candidate = new CvBox2D();
            bool candidateFound = false;
            double candidateSize = 0;

            for (int i = 0; i < count; i++)
            {
                double p = Math.Abs(Cv.ContourPerimeter(contours));
                double s = Math.Abs(Cv.ContourArea(contours));

                CvBox2D box = Cv.MinAreaRect2(contours);

                if 
                (   box.Size.Width > 50 && 
                   (box.Size.Width / box.Size.Height) > 4 && 
                   (box.Size.Width / box.Size.Height) < 4.8 &&
                   Math.Abs(box.Angle) < 30
                )
                {
                    if (CheckPlate(ipl, box))
                    {
                        // Определяет хорошесть прямоугольника Хорошесть прямо пропорциональна его размеру (длине). 
                        // И обратно пропорциональна его повороту.
                        double newSize = box.Size.Width / (Math.Abs(box.Angle) + 2.0);
                        if (newSize > candidateSize) 
                        {
                            candidateSize = newSize;
                            candidateFound = true;
                            candidate = box;
                        }
                    }
                }

                contours = contours.HNext;
            }

            if (candidateFound)
            {
                return new Rectangle(
                (int)(candidate.Center.X - (candidate.Size.Width / 1.9)), 
                (int)(candidate.Center.Y - (candidate.Size.Height / 1.9)), 
                (int)(candidate.Size.Width * 1.1), (int)(candidate.Size.Height * 1.1));
            }

            return new Rectangle();
        }

        public IplImage Transform(IplImage ipl)
        {
            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage contrasted = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.EqualizeHist(contrasted);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

            IplImage canny = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Canny(canny, 0, 100);

            IplImage sobel = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Sobel(sobel, 1, 1, ApertureSize.Size3);

            return contrasted;
        }
    }
}
