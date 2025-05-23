﻿using System;
using System.Collections.Generic;
using System.IO;
using ii.InfinityEngine.Binary;
using ii.InfinityEngine.Files;
using ii.InfinityEngine.Writers.Interfaces;

namespace ii.InfinityEngine.Writers
{
    public class SplFileBinaryWriter : ISplFileWriter
    {
        const int HeaderSize = 114;
        const int ExtendedHeaderSize = 40;
        const int FeatureBlockSize = 48;

        public TlkFile TlkFile { get; set; }
        public BackupManager BackupManger { get; set; }

        public bool Write(string filename, IEFile file, bool forceSave = false)
        {
            if (file is not SplFile)
                throw new ArgumentException("File is not a valid spl file");

            var splFile = file as SplFile;

            if (!(forceSave) && (HashGenerator.GenerateKey(splFile) == splFile.Checksum))
                return false;

            var splExtendedHeaders = new List<SplExtendedHeaderBinary>();
            var splFeatureBlocks = new List<SplFeatureBlockBinary>();

            foreach (var featureBlock in splFile.splFeatureBlocks)
            {
                var featureBlockBinary = new SplFeatureBlockBinary();
                featureBlockBinary.DiceSides = featureBlock.DiceSides;
                featureBlockBinary.DiceThrown = featureBlock.DiceThrown;
                featureBlockBinary.Duration = featureBlock.Duration;
                featureBlockBinary.Opcode = featureBlock.Opcode;
                featureBlockBinary.Parameter1 = featureBlock.Parameter1;
                featureBlockBinary.Parameter2 = featureBlock.Parameter2;
                featureBlockBinary.Power = featureBlock.Power;
                featureBlockBinary.Probability1 = featureBlock.Probability1;
                featureBlockBinary.Probability2 = featureBlock.Probability2;
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.DispellableAffectedByMagicResistance ? featureBlockBinary.Resistance | Common.Bit0 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.IgnoreMagicResistance ? featureBlockBinary.Resistance | Common.Bit1 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit2 ? featureBlockBinary.Resistance | Common.Bit2 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit3 ? featureBlockBinary.Resistance | Common.Bit3 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit4 ? featureBlockBinary.Resistance | Common.Bit4 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit5 ? featureBlockBinary.Resistance | Common.Bit5 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit6 ? featureBlockBinary.Resistance | Common.Bit6 : featureBlockBinary.Resistance);
                featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit7 ? featureBlockBinary.Resistance | Common.Bit7 : featureBlockBinary.Resistance);
                featureBlockBinary.Resource = featureBlock.Resource;
                featureBlockBinary.SavingThrowBonus = featureBlock.SavingThrowBonus;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Spells ? featureBlockBinary.SavingThrowType | Common.Bit0 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Breath ? featureBlockBinary.SavingThrowType | Common.Bit1 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.ParalyzePoisonDeath ? featureBlockBinary.SavingThrowType | Common.Bit2 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Wands ? featureBlockBinary.SavingThrowType | Common.Bit3 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.PetrifyPolymorph ? featureBlockBinary.SavingThrowType | Common.Bit4 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit5 ? featureBlockBinary.SavingThrowType | Common.Bit5 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit6 ? featureBlockBinary.SavingThrowType | Common.Bit6 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit7 ? featureBlockBinary.SavingThrowType | Common.Bit7 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit8 ? featureBlockBinary.SavingThrowType | Common.Bit8 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit9 ? featureBlockBinary.SavingThrowType | Common.Bit9 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.IgnorePrimaryTarget ? featureBlockBinary.SavingThrowType | Common.Bit10 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.IgnoreSecondaryTarget ? featureBlockBinary.SavingThrowType | Common.Bit11 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit12 ? featureBlockBinary.SavingThrowType | Common.Bit12 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit13 ? featureBlockBinary.SavingThrowType | Common.Bit13 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit14 ? featureBlockBinary.SavingThrowType | Common.Bit14 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit15 ? featureBlockBinary.SavingThrowType | Common.Bit15 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit16 ? featureBlockBinary.SavingThrowType | Common.Bit16 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit17 ? featureBlockBinary.SavingThrowType | Common.Bit17 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit18 ? featureBlockBinary.SavingThrowType | Common.Bit18 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit19 ? featureBlockBinary.SavingThrowType | Common.Bit19 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit20 ? featureBlockBinary.SavingThrowType | Common.Bit20 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit21 ? featureBlockBinary.SavingThrowType | Common.Bit21 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit22 ? featureBlockBinary.SavingThrowType | Common.Bit22 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit23 ? featureBlockBinary.SavingThrowType | Common.Bit23 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.BypassMirrorImage ? featureBlockBinary.SavingThrowType | Common.Bit24 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.IgnoreDifficulty ? featureBlockBinary.SavingThrowType | Common.Bit25 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit26 ? featureBlockBinary.SavingThrowType | Common.Bit26 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit27 ? featureBlockBinary.SavingThrowType | Common.Bit27 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit28 ? featureBlockBinary.SavingThrowType | Common.Bit28 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit29 ? featureBlockBinary.SavingThrowType | Common.Bit29 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit30 ? featureBlockBinary.SavingThrowType | Common.Bit30 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit31 ? featureBlockBinary.SavingThrowType | Common.Bit31 : featureBlockBinary.SavingThrowType;
                featureBlockBinary.TargetType = (byte)featureBlock.TargetType;
                featureBlockBinary.TimingMode = Convert.ToByte(featureBlock.TimingMode);
                featureBlockBinary.Unused2c = featureBlock.Unused2c;
                splFeatureBlocks.Add(featureBlockBinary);
            }

            foreach (var extendedHeader in splFile.splExtendedHeader)
            {
                var extendedHeaderBinary = new SplExtendedHeaderBinary();
                extendedHeaderBinary.CastingTime = extendedHeader.CastingTime;
                extendedHeaderBinary.LevelRequired = extendedHeader.LevelRequired;
                extendedHeaderBinary.Location = (Int16)extendedHeader.Location;
                extendedHeaderBinary.MemorisedIcon = extendedHeader.MemorisedIcon;
                extendedHeaderBinary.ProjectileAnimation = extendedHeader.ProjectileAnimation;
                extendedHeaderBinary.Range = extendedHeader.Range;
                extendedHeaderBinary.SpellForm = (byte)extendedHeader.SpellForm;
                extendedHeaderBinary.TargetCount = extendedHeader.TargetCount;
                extendedHeaderBinary.TargetType = (byte)extendedHeader.TargetType;
                extendedHeaderBinary.TimesPerDay = extendedHeader.TimesPerDay;
                extendedHeaderBinary.Unused1 = extendedHeader.Unused1;
                extendedHeaderBinary.Unused16 = extendedHeader.Unused16;
                extendedHeaderBinary.Unused18 = extendedHeader.Unused18;
                extendedHeaderBinary.Unused1a = extendedHeader.Unused1a;
                extendedHeaderBinary.Unused1c = extendedHeader.Unused1c;
                extendedHeaderBinary.Unused22 = extendedHeader.Unused22;
                extendedHeaderBinary.Unused24 = extendedHeader.Unused24;
                extendedHeaderBinary.FeatureBlockCount = extendedHeader.FeatureBlockCount;
                extendedHeaderBinary.FeatureBlockOffset = Convert.ToInt16(extendedHeader.splFeatureBlocks.Count);

                foreach (var featureBlock in extendedHeader.splFeatureBlocks)
                {
                    var featureBlockBinary = new SplFeatureBlockBinary();
                    featureBlockBinary.DiceSides = featureBlock.DiceSides;
                    featureBlockBinary.DiceThrown = featureBlock.DiceThrown;
                    featureBlockBinary.Duration = featureBlock.Duration;
                    featureBlockBinary.Opcode = featureBlock.Opcode;
                    featureBlockBinary.Parameter1 = featureBlock.Parameter1;
                    featureBlockBinary.Parameter2 = featureBlock.Parameter2;
                    featureBlockBinary.Power = featureBlock.Power;
                    featureBlockBinary.Probability1 = featureBlock.Probability1;
                    featureBlockBinary.Probability2 = featureBlock.Probability2;
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.DispellableAffectedByMagicResistance ? featureBlockBinary.Resistance | Common.Bit0 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.IgnoreMagicResistance ? featureBlockBinary.Resistance | Common.Bit1 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit2 ? featureBlockBinary.Resistance | Common.Bit2 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit3 ? featureBlockBinary.Resistance | Common.Bit3 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit4 ? featureBlockBinary.Resistance | Common.Bit4 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit5 ? featureBlockBinary.Resistance | Common.Bit5 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit6 ? featureBlockBinary.Resistance | Common.Bit6 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resistance = (byte)(featureBlock.Resistance.Bit7 ? featureBlockBinary.Resistance | Common.Bit7 : featureBlockBinary.Resistance);
                    featureBlockBinary.Resource = featureBlock.Resource;
                    featureBlockBinary.SavingThrowBonus = featureBlock.SavingThrowBonus;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Spells ? featureBlockBinary.SavingThrowType | Common.Bit0 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Breath ? featureBlockBinary.SavingThrowType | Common.Bit1 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.ParalyzePoisonDeath ? featureBlockBinary.SavingThrowType | Common.Bit2 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Wands ? featureBlockBinary.SavingThrowType | Common.Bit3 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.PetrifyPolymorph ? featureBlockBinary.SavingThrowType | Common.Bit4 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit5 ? featureBlockBinary.SavingThrowType | Common.Bit5 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit6 ? featureBlockBinary.SavingThrowType | Common.Bit6 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit7 ? featureBlockBinary.SavingThrowType | Common.Bit7 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit8 ? featureBlockBinary.SavingThrowType | Common.Bit8 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit9 ? featureBlockBinary.SavingThrowType | Common.Bit9 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.IgnorePrimaryTarget ? featureBlockBinary.SavingThrowType | Common.Bit10 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.IgnoreSecondaryTarget ? featureBlockBinary.SavingThrowType | Common.Bit11 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit12 ? featureBlockBinary.SavingThrowType | Common.Bit12 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit13 ? featureBlockBinary.SavingThrowType | Common.Bit13 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit14 ? featureBlockBinary.SavingThrowType | Common.Bit14 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit15 ? featureBlockBinary.SavingThrowType | Common.Bit15 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit16 ? featureBlockBinary.SavingThrowType | Common.Bit16 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit17 ? featureBlockBinary.SavingThrowType | Common.Bit17 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit18 ? featureBlockBinary.SavingThrowType | Common.Bit18 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit19 ? featureBlockBinary.SavingThrowType | Common.Bit19 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit20 ? featureBlockBinary.SavingThrowType | Common.Bit20 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit21 ? featureBlockBinary.SavingThrowType | Common.Bit21 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit22 ? featureBlockBinary.SavingThrowType | Common.Bit22 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit23 ? featureBlockBinary.SavingThrowType | Common.Bit23 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.BypassMirrorImage ? featureBlockBinary.SavingThrowType | Common.Bit24 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.IgnoreDifficulty ? featureBlockBinary.SavingThrowType | Common.Bit25 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit26 ? featureBlockBinary.SavingThrowType | Common.Bit26 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit27 ? featureBlockBinary.SavingThrowType | Common.Bit27 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit28 ? featureBlockBinary.SavingThrowType | Common.Bit28 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit29 ? featureBlockBinary.SavingThrowType | Common.Bit29 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit30 ? featureBlockBinary.SavingThrowType | Common.Bit30 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.SavingThrowType = featureBlock.SavingThrowType.Bit31 ? featureBlockBinary.SavingThrowType | Common.Bit31 : featureBlockBinary.SavingThrowType;
                    featureBlockBinary.TargetType = (byte)featureBlock.TargetType;
                    featureBlockBinary.TimingMode = Convert.ToByte(featureBlock.TimingMode);
                    featureBlockBinary.Unused2c = featureBlock.Unused2c;
                    splFeatureBlocks.Add(featureBlockBinary);
                }

                splExtendedHeaders.Add(extendedHeaderBinary);
            }

            var header = new SplHeaderBinary();

            header.Flags = splFile.Flags.Bit0 ? header.Flags | Common.Bit0 : header.Flags;
            header.Flags = splFile.Flags.Bit1 ? header.Flags | Common.Bit1 : header.Flags;
            header.Flags = splFile.Flags.Bit2 ? header.Flags | Common.Bit2 : header.Flags;
            header.Flags = splFile.Flags.Bit3 ? header.Flags | Common.Bit3 : header.Flags;
            header.Flags = splFile.Flags.Bit4 ? header.Flags | Common.Bit4 : header.Flags;
            header.Flags = splFile.Flags.Bit5 ? header.Flags | Common.Bit5 : header.Flags;
            header.Flags = splFile.Flags.Bit6 ? header.Flags | Common.Bit6 : header.Flags;
            header.Flags = splFile.Flags.Bit7 ? header.Flags | Common.Bit7 : header.Flags;
            header.Flags = splFile.Flags.Bit8 ? header.Flags | Common.Bit8 : header.Flags;
            header.Flags = splFile.Flags.BreaksSanctuaryInvisibility ? header.Flags | Common.Bit9 : header.Flags;
            header.Flags = splFile.Flags.Hostile ? header.Flags | Common.Bit10 : header.Flags;
            header.Flags = splFile.Flags.NoLOSRequired ? header.Flags | Common.Bit11 : header.Flags;
            header.Flags = splFile.Flags.AllowSpotting ? header.Flags | Common.Bit12 : header.Flags;
            header.Flags = splFile.Flags.OutdoorsOnly ? header.Flags | Common.Bit13 : header.Flags;
            header.Flags = splFile.Flags.IgnoreWildSurgeDeadMagic ? header.Flags | Common.Bit14 : header.Flags;
            header.Flags = splFile.Flags.IgnoreWildSurge ? header.Flags | Common.Bit15 : header.Flags;
            header.Flags = splFile.Flags.NonCombatAbility ? header.Flags | Common.Bit16 : header.Flags;
            header.Flags = splFile.Flags.Bit17 ? header.Flags | Common.Bit17 : header.Flags;
            header.Flags = splFile.Flags.Bit18 ? header.Flags | Common.Bit18 : header.Flags;
            header.Flags = splFile.Flags.Bit19 ? header.Flags | Common.Bit19 : header.Flags;
            header.Flags = splFile.Flags.Bit20 ? header.Flags | Common.Bit20 : header.Flags;
            header.Flags = splFile.Flags.Bit21 ? header.Flags | Common.Bit21 : header.Flags;
            header.Flags = splFile.Flags.Bit22 ? header.Flags | Common.Bit22 : header.Flags;
            header.Flags = splFile.Flags.Bit23 ? header.Flags | Common.Bit23 : header.Flags;
            header.Flags = splFile.Flags.CanTargetInvisible ? header.Flags | Common.Bit24 : header.Flags;
            header.Flags = splFile.Flags.CastableWhenSilenced ? header.Flags | Common.Bit25 : header.Flags;
            header.Flags = splFile.Flags.Bit26 ? header.Flags | Common.Bit26 : header.Flags;
            header.Flags = splFile.Flags.Bit27 ? header.Flags | Common.Bit27 : header.Flags;
            header.Flags = splFile.Flags.Bit28 ? header.Flags | Common.Bit28 : header.Flags;
            header.Flags = splFile.Flags.Bit29 ? header.Flags | Common.Bit29 : header.Flags;
            header.Flags = splFile.Flags.Bit30 ? header.Flags | Common.Bit30 : header.Flags;
            header.Flags = splFile.Flags.Bit31 ? header.Flags | Common.Bit31 : header.Flags;

            header.ftype = new array4() { character1 = 'S', character2 = 'P', character3 = 'L', character4 = ' ' };
            header.fversion = new array4() { character1 = 'V', character2 = '1', character3 = ' ', character4 = ' ' };
            header.CastingGraphic = splFile.CastingGraphic;
            header.CompletionSound = splFile.CompletionSound;
            header.ExclusionFlags = splFile.ExclusionFlags;

            header.ExtendedHeaderCount = Convert.ToInt16(splExtendedHeaders.Count);
            header.ExtendedHeaderOffset = HeaderSize;
            header.FeatureBlockCastingCount = Convert.ToInt16(splFile.splFeatureBlocks.Count);
            header.FeatureBlockCastingIndex = 0;
            header.FeatureBlockOffset = HeaderSize + (ExtendedHeaderSize * splExtendedHeaders.Count);
            header.IdentifiedDescription = Common.WriteString(splFile.IdentifiedDescription, TlkFile);
            header.IdentifiedName = Common.WriteString(splFile.IdentifiedName, TlkFile);
            header.PrimaryType = splFile.PrimaryType;
            header.SecondaryType = splFile.SecondaryType;
            header.SpellBookIcon = splFile.SpellBookIcon;
            header.SpellLevel = splFile.SpellLevel;
            header.SpellType = Convert.ToInt16(splFile.SpellType);
            header.UnidentifiedDescription = Common.WriteString(splFile.UnidentifiedDescription, TlkFile);
            header.UnidentifiedName = Common.WriteString(splFile.UnidentifiedName, TlkFile);
            header.Unused24 = splFile.Unknown24;
            header.Unused24 = splFile.Unknown26;
            header.Unused28 = splFile.Unused28;
            header.Unused29 = splFile.Unused29;
            header.Unused2a = splFile.Unused2a;
            header.Unused2b = splFile.Unused2b;
            header.Unused2c = splFile.Unused2c;
            header.Unused2d = splFile.Unused2d;
            header.Unused2e = splFile.Unused2e;
            header.Unused2f = splFile.Unused2f;
            header.Unused30 = splFile.Unused30;
            header.Unused32 = splFile.Unused32;
            header.Unused38 = splFile.Unused38;
            header.Unused42 = splFile.Unused42;
            header.Unused44 = splFile.Unused44;
            header.Unused4c = splFile.Unused4c;
            header.Unused58 = splFile.Unused58;
            header.Unused60 = splFile.Unused60;

            using var s = new MemoryStream();
            using var bw = new BinaryWriter(s);
            var headerAsBytes = Common.WriteStruct(header);

            bw.Write(headerAsBytes);

            foreach (var extendedHeader in splExtendedHeaders)
            {
                var extendedHeaderAsBytes = Common.WriteStruct(extendedHeader);
                bw.Write(extendedHeaderAsBytes);
            }

            foreach (var featureBlock in splFeatureBlocks)
            {
                var featureBlockAsBytes = Common.WriteStruct(featureBlock);
                bw.Write(featureBlockAsBytes);
            }

            BackupManger?.BackupFile(file, file.Filename, file.FileType, this);

            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            bw.BaseStream.Position = 0;
            bw.BaseStream.CopyTo(fs);
            fs.Flush(flushToDisk: true);
            return true;
        }
    }
}