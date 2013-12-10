using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Tagrec_S
{
    class BinaryMatrix
    {
        public const int WIDTH = 120;
        public const int HEIGHT = 200;

        private int width = 120;
        private int height = 200;
        private int[,] matrix = new int[WIDTH, HEIGHT];

        // covered by unit-tests
        public BinaryMatrix(String filename, int width=120, int height = 200)
        {
            using (StreamReader reader = new StreamReader(filename)) {            
                String curLine;
                int lineInd = 0;
                while ((curLine = reader.ReadLine()) != null)
                {
                    var row = curLine.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();
                    for (int i = 0; i < row.Length; ++i)
                    {
                        matrix[lineInd, i] = row[i];
                    }
                    ++lineInd;
                }
            }

        }

        // covered by unit-tests
        public BinaryMatrix(Bitmap bmp)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    // Mother fucker hack
                    if ((color.R - 255) * (color.R - 255) +
                        (color.G - 255) * (color.G - 255) +
                        (color.B - 255) * (color.B - 255) > 20 * 20 *3)
                    {
                        matrix[i, j] = 1;
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }
                }
            }
           
        }

        // covered with unit-tests
        public void dumpToFile(String filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        writer.Write(matrix[i,j] + " ");
                    }
                    writer.WriteLine();
                }
            }
        }

        // covered with unit-tests
        public int GetPixelValue(int x, int y)
        {
            return matrix[x, y];
        }

    }
}
