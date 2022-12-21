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
            
            if (x == width - 1)
            {
                byte zero = 0;
                for (int i = 0; i < zeros; i++)
                {
                    fs.WriteByte(zero);
                }
            }
        }
        public void Write8BitPixel(byte n, int x, int y)
        {
            int zeros = (4 - width * 3 % 4) % 4;
            fs.Position = offset + y * (width + zeros) + x;
            fs.WriteByte(n);
            if (x == width - 1)
            {
                byte zero = 0;
                for(int i=0;i<zeros;i++)
                {
                    fs.WriteByte(zero);
                }
            }
        }
        public void ChangeHeaderFor8Bit()
        {
            fs.Position = 2;
            int zeros = (4 - width * 3 % 4) % 4;
            int newSize = offset + 256 * 4 + height * (width + zeros);
            fs.Write(BitConverter.GetBytes(newSize));
            fs.Position = 10;
            int newOffset = offset + 256 * 4;
            fs.Write(BitConverter.GetBytes(newOffset));
            fs.Position = 28;
            short newBitCount = 8;
            fs.Write(BitConverter.GetBytes(newBitCount));
            fs.Position = 34;
            int newImageSize = height * (width + zeros);
            fs.Write(BitConverter.GetBytes(newImageSize));
            fs.Position = 46;
            int newColorsUsed = 256;
            fs.Write(BitConverter.GetBytes(newColorsUsed));
            WriteGrayscaleColorTable();
        }
        public void WriteGrayscaleColorTable()
        {
            fs.Position = offset;
            byte[] tmp = new byte[4];
            tmp[3] = 0;
            for(int i = 0; i < 256; ++i)
            {
                tmp[0]=(byte)i;
                tmp[1]=(byte)i;
                tmp[2]=(byte)i;
                fs.Write(tmp, 0, 4);
            }
            offset += 256 * 4;
        }
        public void Close()
        {
            this.fs.Close();
        }
    }
}
