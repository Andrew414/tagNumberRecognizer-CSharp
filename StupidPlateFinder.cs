﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    class StupidPlateFinder : IPlateFinder
    {
        public List<OpenCvSharp.CvBox2D> FindRectangles(OpenCvSharp.IplImage ipl)
        {
            return new List<OpenCvSharp.CvBox2D>();
        }

    }
}
