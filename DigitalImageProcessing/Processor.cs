using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using Path = System.IO.Path;
namespace DigitalImageProcessing
{
    class Processor
    {
        private Reader reader;
        private Writer writer;
        private Method method;
        private String newFileName;
        public Processor(String name, String newName, Method method, String path)
        {
            this.method = method;
            reader = new Reader(File.OpenRead(name));
            newFileName = path + "\\" + newName + ".bmp";
            writer = new Writer(File.Create(newFileName), reader.GetOffset(), reader.GetWidth(), reader.GetHeight());
        }
        public void ProcessImage()
        {
            long start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            int divider = 1;
            int numberOfMatrices = 1;
            int matrixSize = 3;
            int[,] matrices;
            if (reader.GetBitCount() != 24)
            {
                reader.Close();
                writer.Close();
                File.Delete(newFileName);
                Error error = new Error();
                error.ShowDialog();
                return;
            }

            switch (method)
            {
                case Method.Roberts:
                    numberOfMatrices = 2;                                                                                       //all matrices are upside down
                    matrixSize = 2;                                                                                             //because image is upside down
                    matrices = new int[2, 4] { { 0, -1, 1, 0 }, { -1, 0, 0, 1 } };                                              //now we dont have to care about it anymore
                    break;
                case Method.Prewitt:
                    numberOfMatrices = 2;
                    matrixSize = 3;
                    matrices = new int[2, 9] { { -1, 0, 1, -1, 0, 1, -1, 0, 1 }, { -1, -1, -1, 0, 0, 0, 1, 1, 1 } };
                    break;
                case Method.Lowpass:
                    divider = 9;
                    numberOfMatrices = 1;
                    matrixSize = 3;
                    matrices = new int[1, 9] { { 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
                    break;
                case Method.Highpass:
                    divider = 1;
                    numberOfMatrices = 1;
                    matrixSize = 3;
                    matrices = new int[1, 9] { { 0, -1, 0, -1, 5, -1, 0, -1, 0 } };
                    break;
                default:
                    divider = 12;
                    numberOfMatrices = 1;
                    matrixSize = 3;
                    matrices = new int[1, 9] { { 1, 1, 1, 1, 4, 1, 1, 1, 1 } };
                    break;
            }
            writer.WriteHeader(reader.ReadHeader());
            writer.WriteDIBHeader(reader.ReadDIBHeader());
            for (int i = 0; i < reader.GetHeight(); ++i)
            {
                for (int j = 0; j < reader.GetWidth(); ++j)
                {
                    Pixel pixel = CalculatePixel(numberOfMatrices, matrixSize, matrices, divider, j, i);
                    writer.WritePixel(pixel, j, i);
                }
            }
            writer.Close();
            reader.Close();
            long end = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            double diff = (end - start)/1000;
            Done done = new Done(diff);
            done.ShowDialog();
        }
        public Pixel CalculatePixel(int numberOfMatrices, int matrixSize, int[,] matrices, int divider, int x, int y)
        {
            byte[] tmp = new byte[3];
            int x1, y1, middleIndex;
            middleIndex = matrixSize * matrixSize / 2;
            int[] sumB = new int[numberOfMatrices];
            int[] sumG = new int[numberOfMatrices];
            int[] sumR = new int[numberOfMatrices];
            for (int i = 0; i < numberOfMatrices; ++i)
            {
                sumB[i] = 0;
                sumG[i] = 0;
                sumR[i] = 0;
            }
            for (int i = 0; i < matrixSize * matrixSize; ++i)
            {
                for (int j = 0; j < numberOfMatrices; ++j)
                {
                    x1 = x - (middleIndex % matrixSize - i % matrixSize);
                    y1 = y - (middleIndex / matrixSize - i / matrixSize);
                    Pixel pxl = reader.ReadPixel(x1, y1);
                    sumB[j] += pxl.B * matrices[j, i];
                    sumG[j] += pxl.G * matrices[j, i];
                    sumR[j] += pxl.R * matrices[j, i];
                }
            }
            for (int i = 0; i < numberOfMatrices; ++i)
            {
                sumB[i] /= divider;
                sumG[i] /= divider;
                sumR[i] /= divider;
            }
            if (numberOfMatrices != 1)
            {
                int sB = 0, sG = 0, sR = 0;
                for (int i = 0; i < numberOfMatrices; ++i)
                {
                    sB += sumB[i] * sumB[i];
                    sG += sumG[i] * sumG[i];
                    sR += sumR[i] * sumR[i];

                }
                sB = ((int)Math.Sqrt(sB));
                sG = ((int)Math.Sqrt(sG));
                sR = ((int)Math.Sqrt(sR));
                if (sB > 255)
                {
                    tmp[2] = 255;
                }
                else
                {
                    tmp[2] = (byte)sB;
                }
                if (sG > 255)
                {
                    tmp[1] = 255;
                }
                else
                {
                    tmp[1] = (byte)sG;
                }
                if (sR > 255)
                {
                    tmp[0] = 255;
                }
                else
                {
                    tmp[0] = (byte)sR;
                }

            }
            else
            {
                if (sumB[0] < 0)
                {
                    sumB[0] = 0;
                }
                else if (sumB[0] > 255)
                {
                    sumB[0] = 255;
                }
                if (sumG[0] < 0)
                {
                    sumG[0] = 0;
                }
                else if (sumG[0] > 255)
                {
                    sumG[0] = 255;
                }
                if (sumR[0] < 0)
                {
                    sumR[0] = 0;
                }
                else if (sumR[0] > 255)
                {
                    sumR[0] = 255;
                }
                tmp[0] = (byte)sumB[0];
                tmp[1] = (byte)sumG[0];
                tmp[2] = (byte)sumR[0];
            }
            Pixel pixel = new Pixel(tmp[2], tmp[1], tmp[0]);
            return pixel;
        }
    }
}
