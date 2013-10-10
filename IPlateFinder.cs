using Emgu.CV;

namespace Tagrec_S
{
    interface IPlateFinder
    {
        System.Drawing.Rectangle FindRectangle(IImage ipl);
        IImage Transform(IImage ipl);
    }
}
