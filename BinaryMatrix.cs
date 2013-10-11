using System;
using System.Linq;
using System.IO;
using System.Drawing;

namespace Tagrec_S
{
    class BinaryMatrix
    {
        public const int WIDTH = 120;
        public const int HEIGHT = 200;

        private int[,] matrix = new int[WIDTH, HEIGHT];

        public BinaryMatrix(String filename)
        {
            
            using (StreamReader reader = new StreamReader(filename)) {            
                String curLine;
                int lineInd = 0;
                while ((curLine = reader.ReadLine()) != null)
                {
                    var row = curLine.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
                    for (int i = 0; i < row.Length; ++i)
                    {
                        matrix[lineInd, i] = row[i];
                    }
                    ++lineInd;
                }
            }

        }

        public BinaryMatrix(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    // Mother fucker hack
                    if (bmp.GetPixel(i, j).R != 255)
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

        public void dumpToFile(String filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int i = 0; i < WIDTH; ++i)
                {
                    for (int j = 0; j < HEIGHT; ++j)
                    {
                        writer.Write(matrix[i,j] + " ");
                    }
                    writer.WriteLine();
                }
            }
        }

        public int GetPixelValue(int x, int y)
        {
            return matrix[x, y];
        }


    }
}
