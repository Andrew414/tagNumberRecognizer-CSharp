using OpenCvSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System;
using System.IO;

namespace Tagrec_S
{
    class MaskSignReader : ISignReader
    {
        public MaskSignReader()
        {
            try
            {
                StreamReader reader = new StreamReader("configdb.cfg");
                LoadDatabase(reader.ReadLine());
            }
            catch(Exception)
            {
                LoadDatabase("./OCR");
            }
        }
        
        public string[] digits = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
        public string[] letters = {"A", "B", "C", "E", "H", "I", "K", "M", "O", "P", "T", "X"};

        public Dictionary<string, BinaryMatrix> digitsDict = new Dictionary<string, BinaryMatrix>();
        public Dictionary<string, BinaryMatrix> lettersDict = new Dictionary<string, BinaryMatrix>();

        private void LoadToDict(Dictionary<string, BinaryMatrix> dict, string[] symbols, string folder)
        {
            foreach (var i in symbols)
            {
                //try
                //{
                    BinaryMatrix matrix = new BinaryMatrix(folder + @"/" + i.ToString() + @"/" + i.ToString() + ".txt");
                    dict.Add(i, matrix);
                //}
                //catch(Exception /*e*/)
                //{
                    //TODO: add logger
                    //log = LogManager.GetCurrentClassLogger();
                    //log.Error("No such sign. Update your database. Exception message: " + e.Message);
                //}

            }
        }

        public void LoadDatabase(String folder)
        {
    
            LoadToDict(digitsDict, digits, folder);
            LoadToDict(lettersDict, letters, folder);
        }

        //covered by unit-tests
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

        // covered by unit-tests
        public String FindBestLetterMatches(BinaryMatrix matrix)
        {
            int max = -1;
            String answer = "";

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

        // covered by unit-tests
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

        // convert by unit-tests
        public static IplImage ConvertImage(IplImage ipl)
        {
            IplImage resized = new IplImage(new CvSize(Constants.BINARY_MATRIX_WIDTH, Constants.BINARY_MATRIX_HEIGHT),
                ipl.Depth, ipl.NChannels);
            ipl.Resize(resized);

            IplImage gray = new IplImage(resized.Size, BitDepth.U8, 1);
            resized.CvtColor(gray, ColorConversion.BgrToGray);

            IplImage blur = new IplImage(resized.Size, BitDepth.U8, 1);
            gray.Smooth(blur, SmoothType.Blur, Constants.SIGN_READER_BLUR_SIZE, Constants.SIGN_READER_BLUR_SIZE);

            IplImage binary = new IplImage(resized.Size, BitDepth.U8, 1);
            blur.Threshold(binary, Constants.SIGN_READER_THRESHOLD_FROM, Constants.SIGN_READER_THRESHOLD_TO, ThresholdType.Otsu);

            return binary;
        }

        // convert by unit-tests
        public String ReadSign(IplImage ipl, bool isLetter)
        {
            IplImage binary = ConvertImage(ipl);
            BinaryMatrix sign = new BinaryMatrix(binary.ToBitmap());

            if (isLetter) {
                return FindBestLetterMatches(sign);
            }

            return FindBestDigitMatches(sign);
        }
    }
}
