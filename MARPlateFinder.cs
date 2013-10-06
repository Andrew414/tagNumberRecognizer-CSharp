using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;

namespace Tagrec_S
{
    class MARPlateFinder : PlateFinder
    {
        public MARPlateFinder()
        {

        }

        private Rectangle CvBox2DToRectangle(CvBox2D box)
        {
            return new Rectangle();
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

            int count = Cv.FindContours(canny, storage, out contours);

            for (int i = 0; i < count; i++)
            {
                double p = Math.Abs(Cv.ContourPerimeter(contours));
                double s = Math.Abs(Cv.ContourArea(contours));

                CvBox2D box = Cv.MinAreaRect2(contours);

                if 
                (   box.Size.Width > 100 && 
                   (box.Size.Width / box.Size.Height) > 2.5 && 
                   (box.Size.Width / box.Size.Height) < 3.5
                    
                )
                {
                    return new Rectangle(
                        (int)(box.Center.X - (box.Size.Width / 2)), 
                        (int)(box.Center.Y - (box.Size.Height / 2)), 
                        (int)(box.Size.Width), (int)(box.Size.Height));
                }

                contours = contours.HNext;
            }

            return new Rectangle();
        }

        int now = 0;

        public IplImage Transform(IplImage ipl)
        {
            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

            IplImage canny = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Canny(canny, 0, 100);

            IplImage sobel = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Sobel(sobel, 1, 1, ApertureSize.Size3);

            return canny;
            //return ((now++ % 2) == 0) ? blur : gray;
        }
    }
}
