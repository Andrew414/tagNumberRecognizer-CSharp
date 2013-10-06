using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    interface PlateReader
    {
        String ReadPlate(OpenCvSharp.IplImage ipl);
    }
}
