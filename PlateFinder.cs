using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    interface PlateFinder
    {
        System.Drawing.Rectangle FindRectangle(OpenCvSharp.IplImage ipl);
        OpenCvSharp.IplImage Transform(OpenCvSharp.IplImage ipl);
    }
}
