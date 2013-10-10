using System;
using Emgu.CV;

namespace Tagrec_S
{
    interface ISignReader
    {
        String ReadSign(IImage ipl);
    }
}
