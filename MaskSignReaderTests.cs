using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    using NUnit.Framework;
    using System.Drawing;
    using System.IO;
    [TestFixture]
    public class MaskSignReaderTests
    {
        // TODO: remove it, take from MaskSignReader
        public string[] digits = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
        public string[] letters = {"A", "B", "C", "E", "H", "I", "K", "M", "O", "P", "T", "X"};


        [Test]
        public void testMatches()
        {
            Bitmap bitmap = new Bitmap(BinaryMatrix.WIDTH, BinaryMatrix.HEIGHT);
            for (int i = 0; i < BinaryMatrix.WIDTH; ++i)
            {
                for (int j = 0; j < BinaryMatrix.HEIGHT; ++j)
                {
                    if (j % 2 == 0)
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                }
            }

            BinaryMatrix matrix = new BinaryMatrix(bitmap);

            Assert.AreEqual(MaskSignReader.Matches(matrix, matrix), BinaryMatrix.HEIGHT * BinaryMatrix.WIDTH);
        }

        [Test]
        public void testFindBestMatches()
        {
            foreach (var digit in digits)
            {
                if (File.Exists(digit + ".txt"))
                {
                    BinaryMatrix matrix = new BinaryMatrix(digit + ".txt");
                    MaskSignReader signReader = new MaskSignReader();

                    Assert.AreEqual(signReader.FindBestDigitMatches(matrix), digit);
                }
            }

            foreach (var letter in letters)
            {
                if (File.Exists(letter + ".txt"))
                {
                    BinaryMatrix matrix = new BinaryMatrix(letter + ".txt");
                    MaskSignReader signReader = new MaskSignReader();

                    Assert.AreEqual(signReader.FindBestLetterMatches(matrix), letter);
                }
            }

        }
    }
}
