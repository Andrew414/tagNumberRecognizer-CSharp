using Emgu.CV;

namespace AutoNumberRecognizer
{
    public interface IPlateFinder
    {
        System.Drawing.Rectangle FindRectangle(IImage ipl);
    }
}
