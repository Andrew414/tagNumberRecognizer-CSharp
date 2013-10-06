using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    class StupidPlateFinder : PlateFinder
    {
        public System.Drawing.Rectangle FindRectangle(OpenCvSharp.IplImage ipl)
        {
            return new System.Drawing.Rectangle(0,0,200,50);
        }

        public OpenCvSharp.IplImage Transform(OpenCvSharp.IplImage ipl) { return ipl; }
    }
}
