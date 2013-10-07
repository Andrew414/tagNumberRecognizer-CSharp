using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    class StupidSignReader : ISignReader
    {
        public String ReadSign(OpenCvSharp.IplImage bmp) { return "A"; }
    }
}
