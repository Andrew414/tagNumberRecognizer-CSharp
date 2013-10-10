using System;
using System.Collections.Generic;
 using Emgu.CV;

namespace Tagrec_S
{
    interface IPlateReader
    {
        String ReadPlate(IImage ipl, out List<System.Drawing.Rectangle> rects);
    }
}
