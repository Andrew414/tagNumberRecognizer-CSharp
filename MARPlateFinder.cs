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
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(ipl.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

            IplImage canny = new IplImage(ipl.Size, BitDepth.U8, 1);
            binary.Canny(canny, 20, 50);

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
                (   box.Size.Width > 60 
                    && box.Size.Height > 40
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
