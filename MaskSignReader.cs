using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

namespace Tagrec_S
{
    class MaskSignReader : ISignReader
    {
        public MaskSignReader()
        {
            LoadDatabase(@"C:\temp\OCR\");
        }

        public string[] signs = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                 "A", "B", "C", "E", "H", "I", "K", "M", "O", "P", "T", "X"};
        public Dictionary<string, BinaryMatrix> signDict = new Dictionary<string, BinaryMatrix>();

        public void LoadDatabase(String folder)
        {
            foreach (var i in signs)
            {
                try
                {
                    BinaryMatrix matrix = new BinaryMatrix(folder + @"\" + i.ToString() + @"\" + i.ToString() + ".txt");
                    signDict.Add(i, matrix);
                }
                catch(Exception /*e*/)
                {
                    // TODO: add logger
                }
            }
        }

        public int Matches(BinaryMatrix bmp1, BinaryMatrix bmp2)
        {
            int answer = 0;
            
            for (int i = 0; i < BinaryMatrix.HEIGHT; i++)
            {
                for (int j = 0; j < BinaryMatrix.WIDTH; j++)
                {
                    if (bmp2.GetPixelValue(i, j) == bmp1.GetPixelValue(i, j))
                    {
                        answer++;
                    }
                }
            }

            return answer;
        }

        public String ReadSign(IImage ipl)
        {
            var size = new Size(120, 200);

            Image<Gray, Byte> gray = new Image<Gray, Byte>(size);

            Image<Gray, Byte> blur = gray.SmoothBlur(5, 5);

            Image<Gray, Byte> binary = blur.ThresholdBinary(new Gray(149), new Gray(255));

            BinaryMatrix sign = new BinaryMatrix(binary.ToBitmap());

            Dictionary<string, int> Matching = new Dictionary<string, int>();

            foreach (var i in signDict)
            {
                Matching.Add(i.Key, Matches(i.Value, sign));
            }

            int max = -1;
            String answer = "";

            foreach (var i in Matching)
            {
                if (i.Value > max)
                {
                    max = i.Value;
                    answer = i.Key;
                }
            }

            return answer;
        }
    }
}
