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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlTextEdit
    {
        public array8 BackgroundImageMos1;
        public array8 BackgroundImageMos2;
        public array8 BackgroundImageMos3;
        public array8 CursorImageBam;
        public Int16 CarotAnimationCycle;
        public Int16 CarotAnimationFrame;
        public Int16 XCoordinate;
        public Int16 YCoordinate;
        public Int32 ScrollbarControlId;
        public array8 FontBam;
        public Int16 Unknown42;
        public array32 InitialText;
        public Int16 MaxLength;
        public Int32 TextCase;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlTextArea
    {
        public array8 InitialFontBam;
        public array8 MainFontBam;
        public array4 MainColourRGBA;
        public array4 InitialColourRGBA;
        public array4 BackgroundColourRGBA;
        public Int32 ScrollbarControlId;        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlTextLabel
    {
        public Int32 InitialText;
        public array8 FontBam;
        public array4 TextColourRGBA;
        public array4 BackgroundColourRGBA;
        public Int16 JustificationFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ChuControlTextScrollbar
    {
        public array8 Bam;
        public Int16 Cycle;
        public Int16 FrameIndexUpUnpressed;
        public Int16 FrameIndexUpPressed;
        public Int16 FrameIndexDownUnpressed;
        public Int16 FrameIndexDownPressed;
        public Int16 FrameIndexTrough;
        public Int16 FrameIndexSlider;
        public Int32 ControlId;
    }
}