using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using OpenCvSharp;

namespace Tagrec_S
{
    class MARPlateFinder : IPlateFinder
    {
        public MARPlateFinder()
        {
        }

        private bool CheckPlate(IplImage ipl, CvBox2D box)
        {
            return true;
        }

        // covered with unit tests
        public static IplImage ConvertImage(IplImage ipl)
        {
            IplImage gray = new IplImage(ipl.Size, BitDepth.U8, 1);
            ipl.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(ipl.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, Constants.FINDER_BLUR_SIZE, Constants.FINDER_BLUR_SIZE);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, Constants.FINDER_MAIN_THRESHOLD, ThresholdType.Otsu);

            IplImage canny = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Canny(canny, Constants.FINDER_CANNY_FROM, Constants.FINDER_CANNY_TO);

            return canny;
        }

        //covered with unit tests
        public List<CvBox2D> FindRectangles(IplImage ipl)
        {

            IplImage canny = ConvertImage(ipl);

            CvMemStorage storage = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> contours;
           
            int count = Cv.FindContours(canny, storage, out contours);

            List<CvBox2D> result = new List<CvBox2D>();
            for (int i = 0; i < count; i++)
            {                
                CvBox2D box = Cv.MinAreaRect2(contours);

                if 
                (   box.Size.Width > Constants.FINDER_MIN_WIDTH 
                    && box.Size.Height > Constants.FINDER_MIN_HEIGHT
                )
                {
                    if (CheckPlate(ipl, box))
                    {
                        result.Add(box);
                    }
                }

                contours = contours.HNext;
            }

            return result;
        }
    }
}
