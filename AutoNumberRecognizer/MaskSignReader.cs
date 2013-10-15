using System.Collections.Generic;
using System.Drawing;
using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AutoNumberRecognizer
{
    public class MaskSignReader : ISignReader
    {
        public MaskSignReader()
        {
            LoadDatabase(@"C:\temp\OCR\");
        }
        
        public string[] digits = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
        public string[] letters = {"A", "B", "C", "E", "H", "I", "K", "M", "O", "P", "T", "X"};

        public Dictionary<string, BinaryMatrix> digitsDict = new Dictionary<string, BinaryMatrix>();
        public Dictionary<string, BinaryMatrix> lettersDict = new Dictionary<string, BinaryMatrix>();

        private void LoadToDict(Dictionary<string, BinaryMatrix> dict, string[] symbols, string folder)
        {
            foreach (var i in symbols)
            {
                try
                {
                    BinaryMatrix matrix = new BinaryMatrix(folder + @"\" + i.ToString() + @"\" + i.ToString() + ".txt");
                    dict.Add(i, matrix);
                }
                catch(Exception /*e*/)
                {
                    //TODO: add logger
                    //log = LogManager.GetCurrentClassLogger();
                    //log.Error("No such sign. Update your database. Exception message: " + e.Message);
                }

            }
        }

        public void LoadDatabase(String folder)
        {
    
            LoadToDict(digitsDict, digits, folder);
            LoadToDict(lettersDict, letters, folder);
        }

        public static int Matches(BinaryMatrix bmp1, BinaryMatrix bmp2)
        {
            int answer = 0;
            
            for (int i = 0; i < BinaryMatrix.WIDTH; i++)
            {
                for (int j = 0; j < BinaryMatrix.HEIGHT; j++)
                {
                    if (bmp2.GetPixelValue(i, j) == bmp1.GetPixelValue(i, j))
                    {
                        answer++;
                    }
                }
            }
            return answer;
        }

        public String FindBestLetterMatches(BinaryMatrix matrix)
        {
            int max = -1;
            String answer = "";
            matrix.dumpToFile("zzzXXX.txt");
            foreach (var i in lettersDict)
            {
                if (Matches(i.Value, matrix) > max)
                {
                    max = Matches(i.Value, matrix);
                    answer = i.Key;
                }
            }

            return answer;

        }

        public String FindBestDigitMatches(BinaryMatrix matrix)
        {
            int max = -1;
            String answer = "";

            foreach (var i in digitsDict)
            {
                if (Matches(i.Value, matrix) > max)
                {
                    max = Matches(i.Value, matrix);
                    answer = i.Key;
                }
            }

            return answer;

        }

        public String ReadSign(IImage ipl, bool isLetter)
        {
            Image<Gray, Byte> gray = ((Image<Bgr, Byte>)ipl).Resize (120, 200, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).Convert<Gray, Byte>();//new Image<Gray, Byte>(size);

            Image<Gray, Byte> blur = gray.SmoothBlur(5, 5);

            Image<Gray, Byte> binary = blur.ThresholdBinary(new Gray(149), new Gray(255));


            binary.ToBitmap ().Save ("/Users/pavel/Downloads/number_test.bmp");

            BinaryMatrix sign = new BinaryMatrix(binary.ToBitmap());

            if (isLetter) {
                return FindBestLetterMatches(sign);
            }

            return FindBestDigitMatches(sign);
        }
    }
}
