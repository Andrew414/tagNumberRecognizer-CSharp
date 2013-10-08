using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;
using NLog;

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
        public static Logger log;
        public void LoadDatabase(String folder)
        {
            foreach (var i in signs)
            {
                try
                {
                    BinaryMatrix matrix = new BinaryMatrix(folder + @"\" + i.ToString() + @"\" + i.ToString() + ".txt");
                    signDict.Add(i, matrix);
                }
                catch(Exception e)
                {
                    //TODO: add logger
                    //log = LogManager.GetCurrentClassLogger();
                    //log.Error("No such sign. Update your database. Exception message: " + e.Message);
                }
            }
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

        public String FindBestMatches(BinaryMatrix matrix)
        {
            int max = -1;
            String answer = "";

            foreach (var i in signDict)
            {
                if (Matches(i.Value, matrix) > max)
                {
                    max = Matches(i.Value, matrix);
                    answer = i.Key;
                }
            }

            return answer;

        }

        public String ReadSign(IplImage ipl)
        {
            IplImage resized = new IplImage(new CvSize(120, 200), ipl.Depth, ipl.NChannels);
            ipl.Resize(resized);

            IplImage gray = new IplImage(resized.Size, BitDepth.U8, 1);
            resized.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(resized.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, 5, 5);

            IplImage binary = new IplImage(resized.Size, BitDepth.U8, 1);
            blur.Threshold(binary, 0, 255, ThresholdType.Otsu);

            BinaryMatrix sign = new BinaryMatrix(binary.ToBitmap());

            return FindBestMatches(sign);
        }
    }
}
