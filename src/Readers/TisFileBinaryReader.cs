﻿using ii.InfinityEngine.Binary;
using ii.InfinityEngine.Files;
using ii.InfinityEngine.Readers.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ii.InfinityEngine.Readers
{
    public class TisFileBinaryReader : ITisFileReader
    {
        public bool FromBiff = false;

        public TisFile Read(string filename)
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var f = Read(fs, false, 0, 0, 0);
            f.Filename = Path.GetFileName(filename);
            return f;
        }

        public TisFile Read(Stream s, bool fromBiff, int tileCount, int tileLength, int tileDimension)
        {
            using var br = new BinaryReader(s);
            var tisFile = ParseFile(br, fromBiff, tileCount, tileLength, tileDimension);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            tisFile.OriginalFile = ParseFile(br, fromBiff, tileCount, tileLength, tileDimension);
            return tisFile;
        }

        private TisFile ParseFile(BinaryReader br, bool fromBiff, int tileCount, int tileLength, int tileDimension)
        {
            TisHeaderBinary header;
            if (tileCount == 0)
            {
                header = (TisHeaderBinary)Common.ReadStruct(br, typeof(TisHeaderBinary));

                if (header.ftype.ToString() != "TIS ")
                    return new TisFile();
            }
            else
            {
                header.TileCount = tileCount;
                header.TileLength = tileLength;
                header.TileDimension = tileDimension;
                header.TileOffset = 0;
            }

            var streamReader = br;
            var palette = new List<TisPaletteBinary>();
            var tileDatas = new List<byte>();

            var tisFile = new TisFile();
            tisFile.TileCount = header.TileCount;
            tisFile.TileDimension = header.TileDimension;
            tisFile.TileLength = header.TileLength;

            streamReader.BaseStream.Seek(header.TileOffset, SeekOrigin.Begin);
            for (int i = 0; i <= header.TileCount; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    var paletteEntry = (TisPaletteBinary)Common.ReadStruct(streamReader, typeof(TisPaletteBinary));
                    palette.Add(paletteEntry);
                }
                var bytes = streamReader.ReadBytes(header.TileDimension * header.TileDimension);
                tileDatas.AddRange(bytes);
            }

            int width = header.TileCount;// 20;
            int height = 1;// 15;
            var bitmap = new Bitmap(header.TileDimension * width, header.TileDimension * height);

            var bData = bitmap.LockBits(new Rectangle(0, 0, header.TileDimension * width, header.TileDimension * height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var size = bData.Stride * bData.Height;
            var xdata = new byte[size];
            Marshal.Copy(bData.Scan0, xdata, 0, size);
            var mcnt = 0;

            for (int y = 0; y < header.TileDimension * height; y++)
            {
                for (int x = 0; x < header.TileDimension * width; x++)
                {
                    var xCoord = x;
                    var yCoord = y;

                    var tile = (x / header.TileDimension) + ((y / header.TileDimension) * width);
                    var pixel = tileDatas[(tile * header.TileDimension * header.TileDimension) + ((y % header.TileDimension) * header.TileDimension) + (x % header.TileDimension)];
                    mcnt = ((yCoord * header.TileDimension * width) + xCoord) * 4;
                    xdata[mcnt] = palette[(tile * 256) + pixel].Blue;
                    xdata[mcnt + 1] = palette[(tile * 256) + pixel].Green;
                    xdata[mcnt + 2] = palette[(tile * 256) + pixel].Red;
                    xdata[mcnt + 3] = palette[(tile * 256) + pixel].Alpha;
                }
            }

            Marshal.Copy(xdata, 0, bData.Scan0, xdata.Length);
            bitmap.UnlockBits(bData);


            tisFile.Checksum = HashGenerator.GenerateKey(tisFile);
            return tisFile;
        }
    }
}