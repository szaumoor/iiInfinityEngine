﻿using System.IO;
using iiInfinityEngine.Core.Files;
using iiInfinityEngine.Core.Writers.Interfaces;
using System;

namespace iiInfinityEngine.Core.Writers
{
    public class MenuFileWriter : IMenuFileWriter
    {
        public BackupManager BackupManger { get; set; }

        public bool Write(string filename, IEFile file, bool forceSave = false)
        {
            if (file is not MenuFile)
                throw new ArgumentException("File is not a valid menu file");

            var menuFile = file as MenuFile;

            if (!(forceSave) && (HashGenerator.GenerateKey(menuFile) == menuFile.Checksum))
                return false;

            BackupManger?.BackupFile(file, file.Filename, file.FileType, this);

            File.WriteAllText(filename, menuFile.Contents);
            return true;
        }
    }
}