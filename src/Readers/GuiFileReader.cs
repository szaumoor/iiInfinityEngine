﻿using System.IO;
using ii.InfinityEngine.Files;
using ii.InfinityEngine.Readers.Interfaces;

namespace ii.InfinityEngine.Readers
{
    public class GuiFileReader : IGuiFileReader
    {
        public GuiFile Read(string filename)
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var f = Read(fs);
            f.Filename = Path.GetFileName(filename);
            return f;
        }

        public GuiFile Read(Stream s)
        {
            using var rdr = new StreamReader(s);
            var file = Parse(rdr);
            rdr.BaseStream.Seek(0, SeekOrigin.Begin);
            file.OriginalFile = Parse(rdr);
            return file;
        }

        private GuiFile Parse(StreamReader rdr)
        {
            var str = rdr.ReadToEnd();
            var file = new GuiFile();
            file.Contents = str;
            file.Checksum = HashGenerator.GenerateKey(file);
            return file;
        }
    }
}