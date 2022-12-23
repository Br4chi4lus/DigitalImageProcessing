using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using Path = System.IO.Path;
using System.Windows.Threading;
using System.Windows;

namespace DigitalImageProcessing
{
    class Processor
    {
        private MainWindow mainWindow;
        private Reader reader;
        private Writer writer;
        private Method method;
        private String newFileName;
        private Pixel[][]? pixels;
        private int mSize;
        private double sValue;
        private int[] matrix1;
        public Processor(MainWindow mw, String name, String newName, Method method, String path, int mSize, double sValue, int[] matrix1)
        {
            this.matrix1 = matrix1;
            this.mainWindow = mw;
            this.method = method;
            reader = new Reader(File.OpenRead(name));
            if (Path.GetFileName(name).Equals(newName + ".bmp"))
            {
                newName = newName + "(1)";
            }
            newFileName = path + "\\" + newName + ".bmp";
            writer = new Writer(File.Create(newFileName), reader.GetOffset(), reader.GetWidth(), reader.GetHeight());
            this.mSize = mSize;
            this.sValue = sValue;
        }
        public void ProcessImage()
        {
            long start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            int divider = 1;
            int numberOfMatrices = 1;
            int matrixSize = 3;
            int[][] matrices = null;
            double[] matrix = null; 
            CallInfoWindowDel callInfo;
            if (reader.IsWindowsBitmap() == false)
            {
                reader.Close();
                writer.Close();
                File.Delete(newFileName);
                callInfo = MainWindow.OpenInfoWindow;
                App.Current.Dispatcher.BeginInvoke(new Action(() => callInfo(Status.Error2, 0)));
                return;
            }
            if (reader.GetBitCount() != 24)
            {
                reader.Close();
                writer.Close();
                File.Delete(newFileName);
                callInfo = MainWindow.OpenInfoWindow;
                App.Current.Dispatcher.BeginInvoke(new Action(() => callInfo(Status.Error, 0)));
                return;
            }
            switch (method)
            {
                case Method.Roberts:
                    numberOfMatrices = 2;                                                                                       //all matrices are upside down
                    matrixSize = 2;                                                                                             //because image is upside down
                    matrices = new int[2][];                                                                                    //now we dont have to care about it anymore
                    matrices[0] = new int[4] { 0, -1, 1, 0 };
                    matrices[0] = new int[4] { -1, 0, 0, 1 };                                               
                    break;
                case Method.Prewitt:
                    numberOfMatrices = 2;
                    matrixSize = 3;
                    matrices = new int[2][];
                    matrices[0] = new int[9] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
                    matrices[1] = new int[9] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
                    break;
                case Method.Sobel:
                    numberOfMatrices = 2;
                    matrixSize = 3;
                    matrices = new int[2][];
                    matrices[0] = new int[9] { 1, 0, -1, 2, 0, -2, 1, 0, -1 };
                    matrices[1] = new int[9] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
                    break;
                case Method.Scharr:
                    numberOfMatrices = 2;
                    matrixSize = 3;
                    matrices = new int[2][];
                    matrices[0] = new int[9] { -3, 0, 3, -10, 0, 10, -3, 0, 3 };
                    matrices[1] = new int[9] { -3, -10, -3, 0, 0, 0, 3, 10, 3 };
                    break;
                case Method.Lowpass:
                    numberOfMatrices = 1;
                    matrixSize = mSize;                   
                    matrices = new int[1][];
                    matrices[0] = matrix1;
                    divider = CalculateDivider(matrices[0], matrixSize);
                    break;
                case Method.Highpass:
                    numberOfMatrices = 1;
                    matrixSize = mSize;
                    matrices = new int[1][];
                    matrices[0] = matrix1;
                    divider = CalculateDivider(matrices[0],matrixSize);
                    break;
                case Method.Median:
                    matrixSize = 3;
                    break;
                case Method.Gauss:
                    matrixSize = mSize;
                    matrix = CalculateGaussMatrix(matrixSize, sValue);
                    break;
                case Method.Grayscale:
                    matrixSize = 1;
                    break;
                case Method.Grayscale2:
                    matrixSize = 1;
                    break;
                default:
                    divider = 12;
                    numberOfMatrices = 1;
                    matrixSize = 3;
                    matrices = new int[1][];
                    matrices[0] = new int[9] { 1, 1, 1, 1, 4, 1, 1, 1, 1 };
                    break;
            }
            pixels = new Pixel[matrixSize][];
            writer.WriteHeader(reader.ReadHeader());
            writer.WriteDIBHeader(reader.ReadDIBHeader());
            if (method == Method.Grayscale2)
            {
                writer.ChangeHeaderFor8Bit();
            }
            CalculateAllPixels(numberOfMatrices, matrixSize, matrices, divider, matrix);
            writer.Close();
            reader.Close();
            long end = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            double diff = (end - start)/1000;
            
            callInfo = MainWindow.OpenInfoWindow;
            App.Current.Dispatcher.BeginInvoke(new Action(()=>callInfo(Status.Done, diff)));
            CallEnableProcessButton callEnable = mainWindow.EnableProcessButton;
            App.Current.Dispatcher.BeginInvoke(new Action(() => callEnable()));
        }
        private void CalculateAllPixels(int numberOfMatrices, int matrixSize, int[][] matrices, int divider, double[] matrix)
        {
            byte pxl;
            for (int i = 0; i < matrixSize; ++i)                                                                                //read initial lines we will swap them after
            {                                                                                                                   //every iteration e.g. with mask 3x3
                pixels[i] = reader.ReadLineOfPixels(i - matrixSize / 2);                                                        // we start with lines -1,0,1
            }
            CallUpdateProgress callUpdate = mainWindow.UpdateProgress;
            for (int i = 0; i < reader.GetHeight(); ++i)
            {
                double progress = i * 100 / reader.GetHeight();
                App.Current.Dispatcher.Invoke(new Action(() => callUpdate(progress)));
                for (int j = 0; j < reader.GetWidth(); ++j)
                {
                    Pixel pixel;
                    if (method == Method.Median)
                    {
                        pixel = CalculateMedianPixel(matrixSize, j, i);
                        writer.WritePixel(pixel, j, i);
                    }
                    else if (method == Method.Gauss)
                    {
                        pixel = CalculateGaussPixel(matrixSize, matrix, j, i);
                        writer.WritePixel(pixel, j, i);
                    }
                    else if (method == Method.Grayscale2)
                    {
                        pxl = CalculateRGBToGrayscalePixel(j);
                        writer.Write8BitPixel(pxl, j, i);
                    }
                    else if (method == Method.Grayscale)
                    {
                        pxl = CalculateRGBToGrayscalePixel(j);
                        pixel = new Pixel(pxl, pxl, pxl);
                        writer.WritePixel(pixel, j, i);
                    }
                    else
                    {
                        pixel = CalculatePixel(numberOfMatrices, matrixSize, matrices, divider, j, i);                          //common method for egde detection and some filters                      
                        writer.WritePixel(pixel, j, i);
                    }

                }
                for (int j = 0; j < matrixSize; ++j)                                                                             //swaping lines and reading a new one
                {
                    if (j == matrixSize - 1)
                    {
                        pixels[j] = reader.ReadLineOfPixels(i + matrixSize / 2);
                    }
                    else
                    {
                        pixels[j] = pixels[j + 1];
                    }
                }
            }
        }
        private Pixel CalculatePixel(int numberOfMatrices, int matrixSize, int[][] matrices, int divider, int x, int y)
        {
            byte[] tmp;
            int x1, middleIndex;
            middleIndex = matrixSize * matrixSize / 2;
            double[] sumB = new double[numberOfMatrices];
            double[] sumG = new double[numberOfMatrices];
            double[] sumR = new double[numberOfMatrices];
            for (int i = 0; i < numberOfMatrices; ++i)
            {
                sumB[i] = 0;
                sumG[i] = 0;
                sumR[i] = 0;
            }
            for (int i = 0; i < matrixSize * matrixSize; ++i)
            {
                x1 = x - (middleIndex % matrixSize - i % matrixSize);
                Pixel pxl;
                if (x1>0 && x1 < reader.GetWidth())
                {
                    pxl = pixels[i / matrixSize][x1];
                }
                else
                {
                    pxl = new Pixel(0, 0, 0);
                }
                for (int j = 0; j < numberOfMatrices; ++j)
                {                   
                    sumB[j] += pxl.B * matrices[j][i];
                    sumG[j] += pxl.G * matrices[j][i];
                    sumR[j] += pxl.R * matrices[j][i];
                }
            }
            for (int i = 0; i < numberOfMatrices; ++i)
            {
                sumB[i] /= divider;
                sumG[i] /= divider;
                sumR[i] /= divider;
            }
            tmp = ConvertRGBValues(numberOfMatrices, sumR, sumG, sumB);
            Pixel pixel = new Pixel(tmp[2], tmp[1], tmp[0]);
            return pixel;
        }
        private byte[] ConvertRGBValues(int numberOfMatrices, double[] sumR, double[] sumG, double[] sumB)
        {
            byte[] tmp = new byte[3];
            if (numberOfMatrices != 1)
            {
                int sB = 0, sG = 0, sR = 0;
                for (int i = 0; i < numberOfMatrices; ++i)
                {
                    sB += (int)sumB[i] * (int)sumB[i];
                    sG += (int)sumG[i] * (int)sumG[i];
                    sR += (int)sumR[i] * (int)sumR[i];

                }
                sB = ((int)Math.Sqrt(sB));
                sG = ((int)Math.Sqrt(sG));
                sR = ((int)Math.Sqrt(sR));
                if (sB > 255)
                {
                    sB = 255;
                }
                tmp[2] = (byte)sB;
                if (sG > 255)
                {
                    sG = 255;
                }
                tmp[1] = (byte)sG;
                if (sR > 255)
                {
                    sR = 255;
                }
                tmp[0] = (byte)sR;
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
            return tmp;
        }
        private Pixel CalculateMedianPixel( int matrixSize, int x, int y)
        {
            int realPixels, x1 ,y1, middleIndex;
            realPixels = matrixSize * matrixSize;
            middleIndex = matrixSize * matrixSize / 2;
            if ((x < 0 + matrixSize / 2 || x > reader.GetWidth() - 1 - matrixSize / 2) && (y < 0 + matrixSize / 2 || y > reader.GetHeight() - 1 - matrixSize / 2))
            {
                realPixels = realPixels - matrixSize - matrixSize + 1;
            }
            else
            {
                if (x < 0 + matrixSize / 2 || x > reader.GetWidth() - 1 - matrixSize / 2)
                {
                    realPixels -= matrixSize;
                }
                if (y < 0 + matrixSize / 2 || y > reader.GetHeight() - 1 - matrixSize / 2)
                {
                    realPixels -= matrixSize;
                }
            }
            
            int[] reds = new int[realPixels];
            int[] greens = new int[realPixels];
            int[] blues = new int[realPixels];
            int counter = 0;
            for (int i = 0; i < matrixSize * matrixSize; ++i)
            {
                x1 = x - (middleIndex % matrixSize - i % matrixSize);
                y1 = y - (middleIndex / matrixSize - i / matrixSize);
                if(x1 > 0 && x1 < reader.GetWidth() && y1 > 0 && y1 < reader.GetHeight())
                {
                    Pixel pxl = pixels[i / matrixSize][x1];
                    reds[counter] = pxl.R;
                    greens[counter] = pxl.G;
                    blues[counter] = pxl.B;
                    ++counter;
                }
            }
            Array.Sort(reds);
            Array.Sort(greens);
            Array.Sort(blues);
            Pixel pixel = new Pixel((byte)reds[realPixels/2], (byte)greens[realPixels / 2], (byte)blues[realPixels / 2]);
            return pixel;
        }
        private double[] CalculateGaussMatrix(int matrixSize, double sigma) 
        {
            double[] matrix = new double[matrixSize*matrixSize];
            double sum = 0;
            for(int i = -matrixSize/2; i < matrixSize / 2 + 1; ++i)
            {
                for (int j = -matrixSize / 2; j < matrixSize / 2 + 1; ++j)
                {
                    matrix[(i + matrixSize / 2) * matrixSize + (j + matrixSize / 2)] = 1 * Math.Exp(-((j * j + i * i) / (2 * sigma * sigma))) / (2 * Math.PI * sigma * sigma);
                    sum += matrix[(i + matrixSize / 2) * matrixSize + (j + matrixSize / 2)];
                }
            }
            
            return matrix;
        }
        private Pixel CalculateGaussPixel(int matrixSize, double[] matrix, int x, int y)
        {
            Pixel pixel;
            double sumB = 0, sumG = 0, sumR = 0;
            int middleIndex = matrixSize * matrixSize / 2;
            int x1;
            for (int i = 0; i < matrixSize * matrixSize; ++i)
            {
                x1 = x - (middleIndex % matrixSize - i % matrixSize);
                Pixel pxl;
                if (x1 > 0 && x1 < reader.GetWidth())
                {
                    pxl = pixels[i / matrixSize][x1];
                }
                else
                {
                    pxl = new Pixel(0, 0, 0);
                }                
                sumB += pxl.B * matrix[i];
                sumG += pxl.G * matrix[i];
                sumR += pxl.R * matrix[i];
            }
            pixel = new Pixel((byte)sumR,(byte)sumG,(byte)sumB);

            return pixel;
        }
        private byte CalculateRGBToGrayscalePixel(int x)
        {
            Pixel tmp;
            byte pixel;
            tmp = pixels[0][x];
            int t = (int)(0.299 * tmp.R + 0.587 * tmp.G + 0.114 * tmp.B);
            pixel = (byte)t;
            return pixel;
        }
        private int CalculateDivider(int[] matrix, int matrixSize)
        {
            int div = 0;
            for(int i = 0; i < matrixSize * matrixSize; ++i)
            {
                div += (int)matrix[i];
            }
            if (div == 0) div = 1;
            return div;
        }
    }
}
