using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    interface IPlateFinder
    {
        List<OpenCvSharp.CvBox2D> FindRectangles(OpenCvSharp.IplImage ipl);
    }
}
