using System;
using Emgu.CV;

namespace AutoNumberRecognizer
{
    public class StupidSignReader : ISignReader
    {
        public String ReadSign(IImage bmp, bool isLetter) { return "A"; }
    }
}
