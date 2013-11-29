using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagrec_S
{
    using NUnit.Framework;
    using OpenCvSharp;
    using System.Drawing;
    using System.IO;
    [TestFixture]
    public class MaskSignReaderTests
    {
        // TODO: remove it, take from MaskSignReader
        public string[] digits = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
        public string[] letters = {"A", "B", "C", "E", "H", "I", "K", "M", "O", "P", "T", "X"};

        static bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

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

        private String imageFileName = "tests\\convertImageTest.jpg";
        private String imageFileNameResult = "tests\\MaskSignReaderConvertImageTestResult.jpg";

        [Test]
        public void testConvertImages()
        {
            IplImage image = MaskSignReader.ConvertImage(new IplImage(imageFileName));
            String testedFileName = new Random().Next() + ".jpg";
            image.ToBitmap().Save(testedFileName);
            Assert.True(FileEquals(testedFileName, imageFileNameResult));
            File.Delete(testedFileName);
        }

        [Test]
        public void testReadSign()
        {
            MaskSignReader reader = new MaskSignReader();
            Assert.True(reader.ReadSign(new IplImage("tests\\A.jpg"), true) == "A");
            Assert.True(reader.ReadSign(new IplImage("tests\\C.jpg"), true) == "C");
            Assert.True(reader.ReadSign(new IplImage("tests\\0.jpg"), false) == "0");
            Assert.True(reader.ReadSign(new IplImage("tests\\2.jpg"), false) == "2");
        }
    }
}
