using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    interface IPlateReader
    {
        String ReadPlate(OpenCvSharp.IplImage ipl, out List<System.Drawing.Rectangle> rects);
    }
}
