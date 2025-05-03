using System;
using System.Runtime.InteropServices;

namespace ii.InfinityEngine.Binary
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuHeaderBinary
    {
        public array4 ftype;
        public array4 fversion;
        public Int32 WindowCount;
        public Int32 ControlOffset;
        public Int32 WindowOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuWindow
    {
        public Int16 WindowId;
        public Int16 Unknown02;
        public Int16 XCoordinate;
        public Int16 YCoordinate;
        public Int16 Width;
        public Int16 Height;
        public Int16 BackgroundFlag;
        public Int16 ControlCount;
        public array8 BackgroundMos;
        public Int16 FirstControlIndex;
        public Int16 Unknown1a;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlTable
    {
        public Int32 ControlsOffset;
        public Int32 Length;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlCommon
    {
        public Int32 ControlId;
        public Int32 XCoordinate;
        public Int32 YCoordinate;
        public Int16 Width;
        public Int16 Height;
        public byte Type;
        public byte Unknownd;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlButton
    {
        public array8 ButtonImageBam;
        public byte AnimationCycle;
        public byte JustificationFlags;
        public byte FrameIndexUnpressed;
        public byte AnchorCoordinateX1;
        public byte FrameIndexPressed;
        public byte AnchorCoordinateX2;
        public byte FrameIndexSelected;
        public byte AnchorCoordinateY1;
        public byte FrameIndexDisabled;
        public byte AnchorCoordinateY2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlSlider
    {
        public array8 BackgroundImageMos;
        public array8 KnobImageMos;
        public Int16 CycleNumber;
        public Int16 FrameIndexUngrabbed;
        public Int16 FrameIndexGrabbed;
        public Int16 KnobXOffset;
        public Int16 KnobYOffset;
        public Int16 KnobJumpWidth;
        public Int16 KnobJumpCount;
        public Int16 Unknown2c;
        public Int16 Unknown2e;
        public Int16 Unknown30;
        public Int16 Unknown32;
    }

    // TextEdit
    // TextArea
    // Label
    // Scrollbar
}