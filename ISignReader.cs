﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    interface ISignReader
    {
        String ReadSign(OpenCvSharp.IplImage ipl, bool isLetter);
    }
}
