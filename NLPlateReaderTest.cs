using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenCvSharp;
using System.Drawing;
using System.IO;
   

namespace Tagrec_S
{
    [TestFixture]
    class NLPlateReaderTest
    {
        NLPlateReader reader = new NLPlateReader();

        [Test]
        public void testAnglesAndSizes()
        {
            CvBox2D info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 100), 48);
            ContourInfo cinfo = new ContourInfo(0, 0, info);

            Assert.AreEqual(false, reader.IsNumberOrLetter(cinfo));

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 100), 90);
            cinfo = new ContourInfo(0, 0, info);

            Assert.AreEqual(true, reader.IsNumberOrLetter(cinfo));

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(95, 100), 72);
            cinfo = new ContourInfo(0, 0, info);

            Assert.AreEqual(true, reader.IsNumberOrLetter(cinfo));

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 100), -1);
            cinfo = new ContourInfo(0, 0, info);

            Assert.AreEqual(true, reader.IsNumberOrLetter(cinfo));

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(50, 50), 90);
            cinfo = new ContourInfo(0, 0, info);

            Assert.AreEqual(false, reader.IsNumberOrLetter(cinfo));
        }

        [Test]
        public void testRectConversion()
        {
            CvBox2D info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 100), 48);
            Rectangle rect = NLPlateReader.ConvertBox2DToRectangle(info);

            Assert.AreEqual(rect.Width, rect.Height);
            Assert.AreEqual(rect.Width, 0);

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 80), 90);
            rect = NLPlateReader.ConvertBox2DToRectangle(info);

            Assert.AreEqual(rect.Width, 80);
            Assert.AreEqual(rect.Height, 100);

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 80), -3);
            rect = NLPlateReader.ConvertBox2DToRectangle(info);

            Assert.AreEqual(rect.Width, 100);
            Assert.AreEqual(rect.Height, 80);

            info = new CvBox2D(new CvPoint2D32f(0, 0), new CvSize2D32f(100, 80), 70);
            rect = NLPlateReader.ConvertBox2DToRectangle(info);

            Assert.AreNotEqual(rect.Width, 100);
            Assert.AreNotEqual(rect.Width, 80);

            Assert.AreNotEqual(rect.Height, 80);
            Assert.AreNotEqual(rect.Height, 100);

            Assert.AreNotEqual(rect.Height, 0);
            Assert.AreNotEqual(rect.Height, 0);
        }

        [Test]
        public void testBorders()
        {
            int size = 50;
            Bitmap bmp = new Bitmap(size, size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    bmp.SetPixel(i, j, Color.FromArgb(0,0,255));
                }
            }

            reader.DrawBorder(ref bmp, new ContourInfo(0, 0, new CvBox2D(new CvPoint2D32f(size / 2, size / 8), new CvSize2D32f(size / 2, size / 8), 0)));

            int nonyellowsup = 0;
            int nonyellowsdown = 0;

            for (int i = 0; i < size/2; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    nonyellowsup += (bmp.GetPixel(j, i).R > 0 || bmp.GetPixel(j, i).G > 0) ? 1 : 0;
                }
            }

            for (int i = size / 2; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    nonyellowsup += (bmp.GetPixel(j, i).R > 0 || bmp.GetPixel(j, i).G > 0) ? 1 : 0;
                }
            }

            Assert.AreEqual(0, nonyellowsdown);
            Assert.AreNotEqual(0, nonyellowsup);
        }

        [Test]
        public void testReadPlate()
        {
            IplImage good = IplImage.FromFile("./tests/nlgood.jpg");
            IplImage bad = IplImage.FromFile("./tests/nlbad.jpg");

            List<Rectangle> dontcare;

            Assert.AreEqual("1234AB7", reader.ReadPlate(good, out dontcare));
            Assert.AreEqual("", reader.ReadPlate(bad, out dontcare));
        }
    }
}
