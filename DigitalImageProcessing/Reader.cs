﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalImageProcessing
{
    class Reader
    {
        private FileStream fs;
        private int offset;
        private int height;
        private int width;
        public Reader(FileStream fs)
        {
            this.fs = fs;
            byte[] tmp = new byte[4];
            fs.Position = 10;
            fs.Read(tmp, 0, 4);
            offset = BitConverter.ToInt32(tmp, 0);
            fs.Position = 18;
            fs.Read(tmp, 0, 4);
            width = BitConverter.ToInt32(tmp, 0);
            fs.Position = 22;
            fs.Read(tmp, 0, 4);
            height = BitConverter.ToInt32(tmp, 0);
        }
        public byte[] ReadHeader()
        {
            byte[] header = new byte[14];
            fs.Position = 0;
            fs.Read(header, 0, 14);
            return header;
        }
        public bool IsWindowsBitmap()
        {
            byte[] tmp = new byte[2];
            fs.Position = 0;
            fs.Read(tmp, 0, 2);
            short t = BitConverter.ToInt16(tmp, 0);
            short bm = 0x4d42;                                          //hex for MB
            if (bm == t)
            {
                return true;
            }
            return false;
        }
        public byte[] ReadDIBHeader()
        {
            byte[] header = new byte[offset - 14];
            fs.Position = 14;
            fs.Read(header, 0, offset - 14);
            return header;
        }
        public int GetOffset()
        {
            return offset;
        }
        public int GetWidth()
        {

            return width;
        }
        public int GetHeight()
        {
            return height;
        }
        public Pixel ReadPixel(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                return new Pixel(0, 0, 0);
            }
            else
            {
                byte[] tmp = new byte[3];
                fs.Position = width * y * 3 + y * ((4 - width * 3 % 4) % 4) + x * 3 + offset;
                fs.Read(tmp, 0, 3);
                return new Pixel(tmp[2], tmp[1], tmp[0]);
            }
        }
        public int GetBitCount()
        {
            fs.Position = 28;
            byte[] tmp = new byte[2];
            fs.Read(tmp, 0, 2);
            int bitCount = BitConverter.ToInt16(tmp, 0);
            return bitCount;
        }
        public void Close()
        {
            this.fs.Close();
        }
    }
}
