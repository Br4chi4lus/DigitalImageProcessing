using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalImageProcessing
{
    class Writer
    {
        private FileStream fs;
        private int offset;
        private int height;
        private int width;
        public Writer(FileStream fs, int offset, int width, int height)
        {
            this.fs = fs;
            this.offset = offset;
            this.height = height;
            this.width = width;
        }
        public void WriteHeader(byte[] header)
        {
            fs.Position = 0;
            fs.Write(header, 0, header.Length);
        }
        public void WriteDIBHeader(byte[] dib)
        {
            fs.Position = 14;
            fs.Write(dib, 0, dib.Length);
        }
        public void WritePixel(Pixel pixel, int x, int y)
        {
            int zeros;
            zeros = (4 - width * 3 % 4) % 4;
            fs.Position = offset + x * 3 + y * width * 3 + zeros * y;
            fs.WriteByte(pixel.B);
            fs.WriteByte(pixel.G);
            fs.WriteByte(pixel.R);
            byte zero = 0;
            for (int i = 0; i < zeros; i++)
            {
                fs.WriteByte(zero);
            }
        }
        public void Close()
        {
            this.fs.Close();
        }
    }
}
