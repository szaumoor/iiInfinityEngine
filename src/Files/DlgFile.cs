﻿using System;
using System.Collections.Generic;

namespace ii.InfinityEngine.Files
{
    [Serializable]
    public class DlgFile : IEFile
    {
        public List<State2> states = [];

        [NonSerialized]
        private string checksum;
        public string Checksum { get { return checksum; } set { checksum = value; } }
        [NonSerialized]
        private string filename;
        public string Filename { get { return filename; } set { filename = value; } }
        [NonSerialized]
        private readonly IEFileType fileType = IEFileType.Dlg;
        public IEFileType FileType { get { return fileType; } }
        [NonSerialized]
        private IEFile originalFile;
        public IEFile OriginalFile { get { return originalFile; } set { originalFile = value; } }

        public DlgFile()
        {
            Flags = new HeaderFlags();
        }

        public HeaderFlags Flags { get; set; }
    }

    [Serializable]
    public class State2
    {
        public Int32 Weight { get; set; }
        public Int32 StateNumber { get; set; }
        public string Trigger { get; set; }
        public IEString ResponseText { get; set; }
        public string SymbolicName { get; set; }
        public List<Transition2> transitions = [];
    }

    [Serializable]
    public class Transition2
    {
        public string Trigger { get; set; }
        public IEString TransitionText { get; set; }
        public IEString JournalText { get; set; }
        public array8 Dialog { get; set; }
        public Int32 NextState { get; set; }
        public string NextStateSymbolicName { get; set; }
        public string Action { get; set; }

        public bool HasText { get; set; } //TODO:dlg move to separate class and add missing unused bits
        public bool HasTrigger { get; set; }
        public bool HasAction { get; set; }
        public bool TerminateDialog { get; set; }
        public bool HasJouranl { get; set; }
        public bool Interrupt { get; set; }
        public bool AddQuestJournalEntry { get; set; }
        public bool RemoveQuestJournalEntry { get; set; }
        public bool AddQuestCompleteJournalEntry { get; set; }
        public bool ImmediateActionExecution { get; set; }
        public bool ClearActions { get; set; }
    }

    [Serializable]
    public class HeaderFlags
    {
        public bool Enemy { get; set; }
        public bool EscapeArea { get; set; }
        public bool Nothing { get; set; }
        public bool Bit03 { get; set; }
        public bool Bit04 { get; set; }
        public bool Bit05 { get; set; }
        public bool Bit06 { get; set; }
        public bool Bit07 { get; set; }
        public bool Bit08 { get; set; }
        public bool Bit09 { get; set; }
        public bool Bit10 { get; set; }
        public bool Bit11 { get; set; }
        public bool Bit12 { get; set; }
        public bool Bit13 { get; set; }
        public bool Bit14 { get; set; }
        public bool Bit15 { get; set; }
        public bool Bit16 { get; set; }
        public bool Bit17 { get; set; }
        public bool Bit18 { get; set; }
        public bool Bit19 { get; set; }
        public bool Bit20 { get; set; }
        public bool Bit21 { get; set; }
        public bool Bit22 { get; set; }
        public bool Bit23 { get; set; }
        public bool Bit24 { get; set; }
        public bool Bit25 { get; set; }
        public bool Bit26 { get; set; }
        public bool Bit27 { get; set; }
        public bool Bit28 { get; set; }
        public bool Bit29 { get; set; }
        public bool Bit30 { get; set; }
        public bool Bit31 { get; set; }
    }
}