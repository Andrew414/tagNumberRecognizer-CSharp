using System;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;

namespace AutoNumberRecognizer
{
    public class MARPlateFinder : IPlateFinder
    {
        public MARPlateFinder()
        {
        }


        private Rectangle CvBox2DToRectangle(MCvBox2D box)
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

        private bool CheckPlate(IImage ipl, MCvBox2D box)
        {
            return true;
        }

        public Rectangle FindRectangle(IImage ipl)
        {
            Image<Gray, Byte> gray = ((Image<Bgr, Byte>)ipl).Convert<Gray, Byte>();

            Image<Gray, Byte> blur = gray.SmoothBlur (5, 5);

            Image<Gray, Byte> binary = blur.ThresholdBinary(new Gray(149), new Gray(255));

            Image<Gray, Byte> canny = binary.Canny(0, 100);

            MCvBox2D candidate = new MCvBox2D();
            bool candidateFound = false;
            double candidateSize = 0;

            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contours = canny.FindContours(); contours != null; contours = contours.HNext)
                {
                    Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    // double p = Math.Abs(currentContour.Perimeter);
                    // double s = Math.Abs(currentContour.Area);

                    MCvBox2D box = currentContour.GetMinAreaRect();

                    if(box.size.Width > 50 && 
                       (box.size.Width / box.size.Height) > 4 && 
                       (box.size.Width / box.size.Height) < 4.8 &&
                       Math.Abs(box.angle) < 5)
                    {
                        if (CheckPlate(ipl, box))
                        {
                            // write a comment
                            double newSize = box.size.Width / (Math.Abs(box.angle) + 2.0);
                            if (newSize > candidateSize) 
                            {
                                candidateSize = newSize;
                                candidateFound = true;
                                candidate = box;
                            }
                        }
                    }
                }
            }

            if (candidateFound)
            {
                return new Rectangle(
                (int)(candidate.center.X - (candidate.size.Width / 2)), 
                (int)(candidate.center.Y - (candidate.size.Height / 2)), 
                (int)(candidate.size.Width), (int)(candidate.size.Height));
            }

            return new Rectangle();
        }

    }
}
