using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tagrec_S
{
    using NUnit.Framework;
    using OpenCvSharp;
    using System.Xml.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    [TestFixture]
    class MarPlateFinderTest
    {

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

        private String imageFileName = "tests\\convertImageTest.jpg";
        private String imageFileNameResult = "tests\\convertImageTestResult.jpg";

        [Test]
        public void ConvertImageTest()
        {
            IplImage image = MARPlateFinder.ConvertImage(new IplImage(imageFileName));
            String testedFileName = new Random().Next() + ".jpg";
            image.ToBitmap().Save(testedFileName);
            Assert.True(FileEquals(testedFileName, imageFileNameResult));          
            File.Delete(testedFileName);
        }

        private String listFileName = "tests\\file.list";
        
        //TODO: add more examples
        [Test]
        public void FindRectanglesTest()
        {
            FileStream fs = new FileStream(listFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter bf = new BinaryFormatter();
            List<CvBox2D> realList = (List<CvBox2D>)bf.Deserialize(fs);
            fs.Close();

            IPlateFinder finder = new MARPlateFinder();
            List<CvBox2D> list = finder.FindRectangles(new IplImage(imageFileName));

            Assert.True(list.Count() == realList.Count());

            for (int i = 0; i < list.Count(); ++i)
            {
                Assert.True(list[i].Equals(realList[i]));
            }            
        }

    }
}
