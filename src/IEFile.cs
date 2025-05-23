﻿namespace ii.InfinityEngine
{
    /// <summary>
    /// Defines common properties an IE file must support
    /// </summary>
    public interface IEFile
    {
        string Filename { get; set; }
        string Checksum { get; set; }
        IEFileType FileType { get; }
        IEFile OriginalFile { get; set; }
    }
}