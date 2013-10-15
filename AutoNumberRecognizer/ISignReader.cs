using System;
using Emgu.CV;

namespace AutoNumberRecognizer
{
    public interface ISignReader
    {
       String ReadSign(IImage ipl, bool isLetter);
    }
}
