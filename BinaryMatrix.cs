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
        public const int WIDTH = Constants.BINARY_MATRIX_WIDTH;
        public const int HEIGHT = Constants.BINARY_MATRIX_HEIGHT;

        private int width = WIDTH;
        private int height = HEIGHT;
        private int[,] matrix = new int[WIDTH, HEIGHT];

        // covered by unit-tests
        public BinaryMatrix(String filename, 
            int width=Constants.BINARY_MATRIX_WIDTH, int height = Constants.BINARY_MATRIX_HEIGHT)
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
                    if ((color.R - Constants.MAX_BRIGHTNESS) * (color.R - Constants.MAX_BRIGHTNESS) +
                        (color.G - Constants.MAX_BRIGHTNESS) * (color.G - Constants.MAX_BRIGHTNESS) +
                        (color.B - Constants.MAX_BRIGHTNESS) * (color.B - Constants.MAX_BRIGHTNESS) > 
                            Constants.BINARY_MATRIX_TRESHOLD)
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
