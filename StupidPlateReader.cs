using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    class StupidPlateReader : IPlateReader
    {
        public String ReadPlate(OpenCvSharp.IplImage ipl, out List<System.Drawing.Rectangle> rects)
        {
            rects = null;
            return "4142 AB-1"; 
        } 
    }
}
