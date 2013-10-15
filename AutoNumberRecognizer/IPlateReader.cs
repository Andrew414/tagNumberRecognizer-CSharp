using System;
using System.Collections.Generic;
 using Emgu.CV;

namespace AutoNumberRecognizer
{
    public interface IPlateReader
    {
        String ReadPlate(IImage ipl, out List<System.Drawing.Rectangle> rects);
    }
}
