using System;
using System.Collections.Generic;

using Emgu.CV;

namespace Tagrec_S
{
    class StupidPlateReader : IPlateReader
    {
        public String ReadPlate(IImage ipl, out List<System.Drawing.Rectangle> rects)
        {
            rects = null;
            return "4142 AB-1"; 
        } 
    }
}
