using Emgu.CV;

namespace AutoNumberRecognizer
{
    public class StupidPlateFinder : IPlateFinder
    {
        public System.Drawing.Rectangle FindRectangle(IImage ipl)
        {
            return new System.Drawing.Rectangle(0,0,200,50);
        }

    }
}
