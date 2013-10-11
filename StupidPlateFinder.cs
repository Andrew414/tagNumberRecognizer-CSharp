using Emgu.CV;

namespace Tagrec_S
{
    class StupidPlateFinder : IPlateFinder
    {
        public System.Drawing.Rectangle FindRectangle(IImage ipl)
        {
            return new System.Drawing.Rectangle(0,0,200,50);
        }

        public IImage Transform(IImage ipl) { return ipl; }
    }
}
