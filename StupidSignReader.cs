﻿using System;
using Emgu.CV;

namespace Tagrec_S
{
    class StupidSignReader : ISignReader
    {
        public String ReadSign(IImage bmp) { return "A"; }
    }
}
