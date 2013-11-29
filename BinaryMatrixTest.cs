using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Tagrec_S
{
    using NUnit.Framework;
    [TestFixture]
    public class BinaryMatrixTest
    {
        private String testFileName = "testFile.txt";

        [Test]
        public void dumpToFile()
        {
            using (StreamWriter writer = new StreamWriter(testFileName))
            {

                for (int i = 0; i < BinaryMatrix.WIDTH; ++i)
                {
                    for (int j = 0; j < BinaryMatrix.HEIGHT; ++j)
                    {
                        writer.Write(new Random().Next() % 2 + " ");
                    }
                    writer.WriteLine();
                }
            }
            BinaryMatrix matrix = new BinaryMatrix(testFileName);

            String outFileName = "out" + testFileName;

            matrix.dumpToFile("out" + testFileName);

            BinaryMatrix dumpedMatrix = new BinaryMatrix(outFileName);

            for (int i = 0; i < BinaryMatrix.WIDTH; ++i)
            {
                for (int j = 0; j < BinaryMatrix.HEIGHT; ++j)
                {
                    Assert.AreEqual(dumpedMatrix.GetPixelValue(i, j), matrix.GetPixelValue(i, j));
                }
            }
            //File.Delete(outFileName);
            //File.Delete(testFileName);
        }

        [Test]
        public void initFromFile()
        {

            using (StreamWriter writer = new StreamWriter(testFileName))
            {

                for (int i = 0; i < BinaryMatrix.WIDTH; ++i)
                {
                    for (int j = 0; j < BinaryMatrix.HEIGHT; ++j)
                    {
                        if (j % 2 == 0)
                        {
                            writer.Write("1 ");
                        }
                        else
                        {
                            writer.Write("0 ");
                        }
                    }
                    writer.WriteLine();
                }
            }

            BinaryMatrix matrix = new BinaryMatrix(testFileName);
            for (int i = 0; i < BinaryMatrix.WIDTH; ++i)
            {
                for (int j = 0; j < BinaryMatrix.HEIGHT; ++j)
                {
                    if (j % 2 == 0)
                    {
                        Assert.AreEqual(1, matrix.GetPixelValue(i, j));
                    }
                    else
                    {
                        Assert.AreEqual(0, matrix.GetPixelValue(i, j));
                    }
                }
            }
            
            File.Delete(testFileName);
        }

        [Test]
        public void initFromBitmap()
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
            for (int i = 0; i < BinaryMatrix.WIDTH; ++i)
            {
                for (int j = 0; j < BinaryMatrix.HEIGHT; ++j)
                {
                    if (j % 2 == 0)
                    {
                        Assert.AreEqual(1, matrix.GetPixelValue(i, j));
                    }
                    else
                    {
                        Assert.AreEqual(0, matrix.GetPixelValue(i, j));
                    }
                }
            }

        }
    }
}
