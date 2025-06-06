﻿using System.Collections.Generic;
using System.IO;
using ii.InfinityEngine.Binary;
using ii.InfinityEngine.Files;
using System;
using System.Numerics;
using System.ComponentModel;

namespace ii.InfinityEngine.Readers
{
    public class AreFileBinaryReader : IAreFileReader
    {
        public TlkFile TlkFile { get; set; }

        public AreFile Read(string filename)
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var f = Read(fs);
            f.Filename = Path.GetFileName(filename);
            return f;
        }

        public AreFile Read(Stream s)
        {
            using var br = new BinaryReader(s);
            var areFile = ParseFile(br);
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            areFile.OriginalFile = ParseFile(br);
            return areFile;
        }

        private AreFile ParseFile(BinaryReader br)
        {
            var header = (AreHeaderBinary)Common.ReadStruct(br, typeof(AreHeaderBinary));

            if (header.ftype.ToString() != "AREA")
                return new AreFile();

            List<AreActorBinary> actors = [];
            List<AreRegionBinary> regions = [];
            List<AreSpawnPointBinary> spawns = [];
            List<AreEntranceBinary> entrances = [];
            List<AreContainerBinary> containers = [];
            List<AreItemBinary> items = [];
            List<AreAmbientBinary> ambients = [];
            List<AreVariableBinary> variables = [];
            List<AreDoorBinary> doors = [];
            List<AreAnimationBinary> animations = [];
            List<AreNoteBinary> notes = [];
            List<AreTiledObjectBinary> tiledObjects = [];
            List<AreProjectileBinary> projectiles = [];
            List<AreSongBinary> songs = [];
            List<AreInterruptionBinary> interruptions = [];
            List<bool> exploredArea = [];
            List<Int32> vertices = [];

            br.BaseStream.Seek(header.ActorOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.ActorCount; i++)
            {
                var actor = (AreActorBinary)Common.ReadStruct(br, typeof(AreActorBinary));
                actors.Add(actor);
            }

            br.BaseStream.Seek(header.RegionOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.RegionCount; i++)
            {
                var region = (AreRegionBinary)Common.ReadStruct(br, typeof(AreRegionBinary));
                regions.Add(region);
            }

            br.BaseStream.Seek(header.SpawnPointOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.SpawnPointCount; i++)
            {
                var spawn = (AreSpawnPointBinary)Common.ReadStruct(br, typeof(AreSpawnPointBinary));
                spawns.Add(spawn);
            }

            br.BaseStream.Seek(header.EntrancesOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.EntrancesCount; i++)
            {
                var entrance = (AreEntranceBinary)Common.ReadStruct(br, typeof(AreEntranceBinary));
                entrances.Add(entrance);
            }

            br.BaseStream.Seek(header.ContainerOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.ContainerCount; i++)
            {
                var container = (AreContainerBinary)Common.ReadStruct(br, typeof(AreContainerBinary));
                containers.Add(container);
            }

            br.BaseStream.Seek(header.ItemOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.ItemCount; i++)
            {
                var item = (AreItemBinary)Common.ReadStruct(br, typeof(AreItemBinary));
                items.Add(item);
            }

            br.BaseStream.Seek(header.AmbientOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.AmbientCount; i++)
            {
                var ambient = (AreAmbientBinary)Common.ReadStruct(br, typeof(AreAmbientBinary));
                ambients.Add(ambient);
            }

            br.BaseStream.Seek(header.VariableOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.VariableCount; i++)
            {
                var variable = (AreVariableBinary)Common.ReadStruct(br, typeof(AreVariableBinary));
                variables.Add(variable);
            }

            br.BaseStream.Seek(header.DoorOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.DoorCount; i++)
            {
                var door = (AreDoorBinary)Common.ReadStruct(br, typeof(AreDoorBinary));
                doors.Add(door);
            }

            br.BaseStream.Seek(header.AnimationOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.AnimationCount; i++)
            {
                var animation = (AreAnimationBinary)Common.ReadStruct(br, typeof(AreAnimationBinary));
                animations.Add(animation);
            }

            br.BaseStream.Seek(header.AutomatOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.AutomatCount; i++)
            {
                var note = (AreNoteBinary)Common.ReadStruct(br, typeof(AreNoteBinary));
                notes.Add(note);
            }

            br.BaseStream.Seek(header.TiledObjectOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.TiledObjectCount; i++)
            {
                var tiledObject = (AreTiledObjectBinary)Common.ReadStruct(br, typeof(AreTiledObjectBinary));
                tiledObjects.Add(tiledObject);
            }

            br.BaseStream.Seek(header.ProjectileOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.ProjectileCount; i++)
            {
                var projectile = (AreProjectileBinary)Common.ReadStruct(br, typeof(AreProjectileBinary));
                projectiles.Add(projectile);
            }

            br.BaseStream.Seek(header.SongEntryOffset, SeekOrigin.Begin);
            for (int i = 0; i < 1; i++) // ?
            {
                var song = (AreSongBinary)Common.ReadStruct(br, typeof(AreSongBinary));
                songs.Add(song);
            }

            br.BaseStream.Seek(header.RestInterruptionOffset, SeekOrigin.Begin);
            for (int i = 0; i < 1; i++) // ?
            {
                var interruption = (AreInterruptionBinary)Common.ReadStruct(br, typeof(AreInterruptionBinary));
                interruptions.Add(interruption);
            }

            // Assume that we'll always have a multiple of a byte
            br.BaseStream.Seek(header.ExploredBitmaskOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.ExploredBitmaskSize / 8; i++) // ?
            {
                var exploration = (byte)Common.ReadStruct(br, typeof(byte));
                bool explored = false;
                if ((exploration & Common.Bit0) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit1) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit2) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit3) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit4) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit5) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit6) != 0)
                    explored = true;
                exploredArea.Add(explored);

                if ((exploration & Common.Bit7) != 0)
                    explored = true;
                exploredArea.Add(explored);
            }

            br.BaseStream.Seek(header.VertexOffset, SeekOrigin.Begin);
            for (int i = 0; i < header.VertexCount; i++)
            {
                var vertex = (Int32)Common.ReadStruct(br, typeof(Int32));
                vertices.Add(vertex);
            }

            var areFile = new AreFile();
            areFile.AreaFlags.SaveAllowed = (header.AreaFlags & Common.Bit0) != 0;
            areFile.AreaFlags.TutorialArea = (header.AreaFlags & Common.Bit1) != 0;
            areFile.AreaFlags.DeadMagicZone = (header.AreaFlags & Common.Bit2) != 0;
            areFile.AreaFlags.Dream = (header.AreaFlags & Common.Bit3) != 0;
            areFile.AreaFlags.Player1DeathDoesNotEndGame = (header.AreaFlags & Common.Bit4) != 0;
            areFile.AreaFlags.RestingNotAllowed = (header.AreaFlags & Common.Bit5) != 0;
            areFile.AreaFlags.TravelNotAllowed = (header.AreaFlags & Common.Bit6) != 0;
            areFile.AreaFlags.Bit07 = (header.AreaFlags & Common.Bit7) != 0;
            areFile.AreaFlags.Bit08 = (header.AreaFlags & Common.Bit8) != 0;
            areFile.AreaFlags.Bit09 = (header.AreaFlags & Common.Bit9) != 0;
            areFile.AreaFlags.Bit10 = (header.AreaFlags & Common.Bit10) != 0;
            areFile.AreaFlags.Bit11 = (header.AreaFlags & Common.Bit11) != 0;
            areFile.AreaFlags.Bit12 = (header.AreaFlags & Common.Bit12) != 0;
            areFile.AreaFlags.Bit13 = (header.AreaFlags & Common.Bit13) != 0;
            areFile.AreaFlags.Bit14 = (header.AreaFlags & Common.Bit14) != 0;
            areFile.AreaFlags.Bit15 = (header.AreaFlags & Common.Bit15) != 0;
            areFile.AreaFlags.Bit16 = (header.AreaFlags & Common.Bit16) != 0;
            areFile.AreaFlags.Bit17 = (header.AreaFlags & Common.Bit17) != 0;
            areFile.AreaFlags.Bit18 = (header.AreaFlags & Common.Bit18) != 0;
            areFile.AreaFlags.Bit19 = (header.AreaFlags & Common.Bit19) != 0;
            areFile.AreaFlags.Bit20 = (header.AreaFlags & Common.Bit20) != 0;
            areFile.AreaFlags.Bit21 = (header.AreaFlags & Common.Bit21) != 0;
            areFile.AreaFlags.Bit22 = (header.AreaFlags & Common.Bit22) != 0;
            areFile.AreaFlags.Bit23 = (header.AreaFlags & Common.Bit23) != 0;
            areFile.AreaFlags.Bit24 = (header.AreaFlags & Common.Bit24) != 0;
            areFile.AreaFlags.Bit25 = (header.AreaFlags & Common.Bit25) != 0;
            areFile.AreaFlags.Bit26 = (header.AreaFlags & Common.Bit26) != 0;
            areFile.AreaFlags.Bit27 = (header.AreaFlags & Common.Bit27) != 0;
            areFile.AreaFlags.Bit28 = (header.AreaFlags & Common.Bit28) != 0;
            areFile.AreaFlags.Bit29 = (header.AreaFlags & Common.Bit29) != 0;
            areFile.AreaFlags.Bit30 = (header.AreaFlags & Common.Bit30) != 0;
            areFile.AreaFlags.Bit31 = (header.AreaFlags & Common.Bit31) != 0;

            areFile.AreaTypeFlags.Outdoor = (header.AreaTypeFlags & Common.Bit0) != 0;
            areFile.AreaTypeFlags.DayNight = (header.AreaTypeFlags & Common.Bit1) != 0;
            areFile.AreaTypeFlags.Weather = (header.AreaTypeFlags & Common.Bit2) != 0;
            areFile.AreaTypeFlags.City = (header.AreaTypeFlags & Common.Bit3) != 0;
            areFile.AreaTypeFlags.Forest = (header.AreaTypeFlags & Common.Bit4) != 0;
            areFile.AreaTypeFlags.Dungeon = (header.AreaTypeFlags & Common.Bit5) != 0;
            areFile.AreaTypeFlags.ExtendedNight = (header.AreaTypeFlags & Common.Bit6) != 0;
            areFile.AreaTypeFlags.CanRestIndoors = (header.AreaTypeFlags & Common.Bit7) != 0;
            areFile.AreaTypeFlags.Bit08 = (header.AreaTypeFlags & Common.Bit8) != 0;
            areFile.AreaTypeFlags.Bit09 = (header.AreaTypeFlags & Common.Bit9) != 0;
            areFile.AreaTypeFlags.Bit10 = (header.AreaTypeFlags & Common.Bit10) != 0;
            areFile.AreaTypeFlags.Bit11 = (header.AreaTypeFlags & Common.Bit11) != 0;
            areFile.AreaTypeFlags.Bit12 = (header.AreaTypeFlags & Common.Bit12) != 0;
            areFile.AreaTypeFlags.Bit13 = (header.AreaTypeFlags & Common.Bit13) != 0;
            areFile.AreaTypeFlags.Bit14 = (header.AreaTypeFlags & Common.Bit14) != 0;
            areFile.AreaTypeFlags.Bit15 = (header.AreaTypeFlags & Common.Bit15) != 0;

            areFile.AreaToTheNorthFlags.PartyRequired = (header.AreaToTheNorthFlags & Common.Bit0) != 0;
            areFile.AreaToTheNorthFlags.PartyEnabled = (header.AreaToTheNorthFlags & Common.Bit1) != 0;
            areFile.AreaToTheNorthFlags.Bit02 = (header.AreaToTheNorthFlags & Common.Bit2) != 0;
            areFile.AreaToTheNorthFlags.Bit03 = (header.AreaToTheNorthFlags & Common.Bit3) != 0;
            areFile.AreaToTheNorthFlags.Bit04 = (header.AreaToTheNorthFlags & Common.Bit4) != 0;
            areFile.AreaToTheNorthFlags.Bit05 = (header.AreaToTheNorthFlags & Common.Bit5) != 0;
            areFile.AreaToTheNorthFlags.Bit06 = (header.AreaToTheNorthFlags & Common.Bit6) != 0;
            areFile.AreaToTheNorthFlags.Bit07 = (header.AreaToTheNorthFlags & Common.Bit7) != 0;
            areFile.AreaToTheNorthFlags.Bit08 = (header.AreaToTheNorthFlags & Common.Bit8) != 0;
            areFile.AreaToTheNorthFlags.Bit09 = (header.AreaToTheNorthFlags & Common.Bit9) != 0;
            areFile.AreaToTheNorthFlags.Bit10 = (header.AreaToTheNorthFlags & Common.Bit10) != 0;
            areFile.AreaToTheNorthFlags.Bit11 = (header.AreaToTheNorthFlags & Common.Bit11) != 0;
            areFile.AreaToTheNorthFlags.Bit12 = (header.AreaToTheNorthFlags & Common.Bit12) != 0;
            areFile.AreaToTheNorthFlags.Bit13 = (header.AreaToTheNorthFlags & Common.Bit13) != 0;
            areFile.AreaToTheNorthFlags.Bit14 = (header.AreaToTheNorthFlags & Common.Bit14) != 0;
            areFile.AreaToTheNorthFlags.Bit15 = (header.AreaToTheNorthFlags & Common.Bit15) != 0;
            areFile.AreaToTheNorthFlags.Bit16 = (header.AreaToTheNorthFlags & Common.Bit16) != 0;
            areFile.AreaToTheNorthFlags.Bit17 = (header.AreaToTheNorthFlags & Common.Bit17) != 0;
            areFile.AreaToTheNorthFlags.Bit18 = (header.AreaToTheNorthFlags & Common.Bit18) != 0;
            areFile.AreaToTheNorthFlags.Bit19 = (header.AreaToTheNorthFlags & Common.Bit19) != 0;
            areFile.AreaToTheNorthFlags.Bit20 = (header.AreaToTheNorthFlags & Common.Bit20) != 0;
            areFile.AreaToTheNorthFlags.Bit21 = (header.AreaToTheNorthFlags & Common.Bit21) != 0;
            areFile.AreaToTheNorthFlags.Bit22 = (header.AreaToTheNorthFlags & Common.Bit22) != 0;
            areFile.AreaToTheNorthFlags.Bit23 = (header.AreaToTheNorthFlags & Common.Bit23) != 0;
            areFile.AreaToTheNorthFlags.Bit24 = (header.AreaToTheNorthFlags & Common.Bit24) != 0;
            areFile.AreaToTheNorthFlags.Bit25 = (header.AreaToTheNorthFlags & Common.Bit25) != 0;
            areFile.AreaToTheNorthFlags.Bit26 = (header.AreaToTheNorthFlags & Common.Bit26) != 0;
            areFile.AreaToTheNorthFlags.Bit27 = (header.AreaToTheNorthFlags & Common.Bit27) != 0;
            areFile.AreaToTheNorthFlags.Bit28 = (header.AreaToTheNorthFlags & Common.Bit28) != 0;
            areFile.AreaToTheNorthFlags.Bit29 = (header.AreaToTheNorthFlags & Common.Bit29) != 0;
            areFile.AreaToTheNorthFlags.Bit30 = (header.AreaToTheNorthFlags & Common.Bit30) != 0;
            areFile.AreaToTheNorthFlags.Bit31 = (header.AreaToTheNorthFlags & Common.Bit31) != 0;

            areFile.AreaToTheEastFlags.PartyRequired = (header.AreaToTheEastFlags & Common.Bit0) != 0;
            areFile.AreaToTheEastFlags.PartyEnabled = (header.AreaToTheEastFlags & Common.Bit1) != 0;
            areFile.AreaToTheEastFlags.Bit02 = (header.AreaToTheEastFlags & Common.Bit2) != 0;
            areFile.AreaToTheEastFlags.Bit03 = (header.AreaToTheEastFlags & Common.Bit3) != 0;
            areFile.AreaToTheEastFlags.Bit04 = (header.AreaToTheEastFlags & Common.Bit4) != 0;
            areFile.AreaToTheEastFlags.Bit05 = (header.AreaToTheEastFlags & Common.Bit5) != 0;
            areFile.AreaToTheEastFlags.Bit06 = (header.AreaToTheEastFlags & Common.Bit6) != 0;
            areFile.AreaToTheEastFlags.Bit07 = (header.AreaToTheEastFlags & Common.Bit7) != 0;
            areFile.AreaToTheEastFlags.Bit08 = (header.AreaToTheEastFlags & Common.Bit8) != 0;
            areFile.AreaToTheEastFlags.Bit09 = (header.AreaToTheEastFlags & Common.Bit9) != 0;
            areFile.AreaToTheEastFlags.Bit10 = (header.AreaToTheEastFlags & Common.Bit10) != 0;
            areFile.AreaToTheEastFlags.Bit11 = (header.AreaToTheEastFlags & Common.Bit11) != 0;
            areFile.AreaToTheEastFlags.Bit12 = (header.AreaToTheEastFlags & Common.Bit12) != 0;
            areFile.AreaToTheEastFlags.Bit13 = (header.AreaToTheEastFlags & Common.Bit13) != 0;
            areFile.AreaToTheEastFlags.Bit14 = (header.AreaToTheEastFlags & Common.Bit14) != 0;
            areFile.AreaToTheEastFlags.Bit15 = (header.AreaToTheEastFlags & Common.Bit15) != 0;
            areFile.AreaToTheEastFlags.Bit16 = (header.AreaToTheEastFlags & Common.Bit16) != 0;
            areFile.AreaToTheEastFlags.Bit17 = (header.AreaToTheEastFlags & Common.Bit17) != 0;
            areFile.AreaToTheEastFlags.Bit18 = (header.AreaToTheEastFlags & Common.Bit18) != 0;
            areFile.AreaToTheEastFlags.Bit19 = (header.AreaToTheEastFlags & Common.Bit19) != 0;
            areFile.AreaToTheEastFlags.Bit20 = (header.AreaToTheEastFlags & Common.Bit20) != 0;
            areFile.AreaToTheEastFlags.Bit21 = (header.AreaToTheEastFlags & Common.Bit21) != 0;
            areFile.AreaToTheEastFlags.Bit22 = (header.AreaToTheEastFlags & Common.Bit22) != 0;
            areFile.AreaToTheEastFlags.Bit23 = (header.AreaToTheEastFlags & Common.Bit23) != 0;
            areFile.AreaToTheEastFlags.Bit24 = (header.AreaToTheEastFlags & Common.Bit24) != 0;
            areFile.AreaToTheEastFlags.Bit25 = (header.AreaToTheEastFlags & Common.Bit25) != 0;
            areFile.AreaToTheEastFlags.Bit26 = (header.AreaToTheEastFlags & Common.Bit26) != 0;
            areFile.AreaToTheEastFlags.Bit27 = (header.AreaToTheEastFlags & Common.Bit27) != 0;
            areFile.AreaToTheEastFlags.Bit28 = (header.AreaToTheEastFlags & Common.Bit28) != 0;
            areFile.AreaToTheEastFlags.Bit29 = (header.AreaToTheEastFlags & Common.Bit29) != 0;
            areFile.AreaToTheEastFlags.Bit30 = (header.AreaToTheEastFlags & Common.Bit30) != 0;
            areFile.AreaToTheEastFlags.Bit31 = (header.AreaToTheEastFlags & Common.Bit31) != 0;

            areFile.AreaToTheSouthFlags.PartyRequired = (header.AreaToTheSouthFlags & Common.Bit0) != 0;
            areFile.AreaToTheSouthFlags.PartyEnabled = (header.AreaToTheSouthFlags & Common.Bit1) != 0;
            areFile.AreaToTheSouthFlags.Bit02 = (header.AreaToTheSouthFlags & Common.Bit2) != 0;
            areFile.AreaToTheSouthFlags.Bit03 = (header.AreaToTheSouthFlags & Common.Bit3) != 0;
            areFile.AreaToTheSouthFlags.Bit04 = (header.AreaToTheSouthFlags & Common.Bit4) != 0;
            areFile.AreaToTheSouthFlags.Bit05 = (header.AreaToTheSouthFlags & Common.Bit5) != 0;
            areFile.AreaToTheSouthFlags.Bit06 = (header.AreaToTheSouthFlags & Common.Bit6) != 0;
            areFile.AreaToTheSouthFlags.Bit07 = (header.AreaToTheSouthFlags & Common.Bit7) != 0;
            areFile.AreaToTheSouthFlags.Bit08 = (header.AreaToTheSouthFlags & Common.Bit8) != 0;
            areFile.AreaToTheSouthFlags.Bit09 = (header.AreaToTheSouthFlags & Common.Bit9) != 0;
            areFile.AreaToTheSouthFlags.Bit10 = (header.AreaToTheSouthFlags & Common.Bit10) != 0;
            areFile.AreaToTheSouthFlags.Bit11 = (header.AreaToTheSouthFlags & Common.Bit11) != 0;
            areFile.AreaToTheSouthFlags.Bit12 = (header.AreaToTheSouthFlags & Common.Bit12) != 0;
            areFile.AreaToTheSouthFlags.Bit13 = (header.AreaToTheSouthFlags & Common.Bit13) != 0;
            areFile.AreaToTheSouthFlags.Bit14 = (header.AreaToTheSouthFlags & Common.Bit14) != 0;
            areFile.AreaToTheSouthFlags.Bit15 = (header.AreaToTheSouthFlags & Common.Bit15) != 0;
            areFile.AreaToTheSouthFlags.Bit16 = (header.AreaToTheSouthFlags & Common.Bit16) != 0;
            areFile.AreaToTheSouthFlags.Bit17 = (header.AreaToTheSouthFlags & Common.Bit17) != 0;
            areFile.AreaToTheSouthFlags.Bit18 = (header.AreaToTheSouthFlags & Common.Bit18) != 0;
            areFile.AreaToTheSouthFlags.Bit19 = (header.AreaToTheSouthFlags & Common.Bit19) != 0;
            areFile.AreaToTheSouthFlags.Bit20 = (header.AreaToTheSouthFlags & Common.Bit20) != 0;
            areFile.AreaToTheSouthFlags.Bit21 = (header.AreaToTheSouthFlags & Common.Bit21) != 0;
            areFile.AreaToTheSouthFlags.Bit22 = (header.AreaToTheSouthFlags & Common.Bit22) != 0;
            areFile.AreaToTheSouthFlags.Bit23 = (header.AreaToTheSouthFlags & Common.Bit23) != 0;
            areFile.AreaToTheSouthFlags.Bit24 = (header.AreaToTheSouthFlags & Common.Bit24) != 0;
            areFile.AreaToTheSouthFlags.Bit25 = (header.AreaToTheSouthFlags & Common.Bit25) != 0;
            areFile.AreaToTheSouthFlags.Bit26 = (header.AreaToTheSouthFlags & Common.Bit26) != 0;
            areFile.AreaToTheSouthFlags.Bit27 = (header.AreaToTheSouthFlags & Common.Bit27) != 0;
            areFile.AreaToTheSouthFlags.Bit28 = (header.AreaToTheSouthFlags & Common.Bit28) != 0;
            areFile.AreaToTheSouthFlags.Bit29 = (header.AreaToTheSouthFlags & Common.Bit29) != 0;
            areFile.AreaToTheSouthFlags.Bit30 = (header.AreaToTheSouthFlags & Common.Bit30) != 0;
            areFile.AreaToTheSouthFlags.Bit31 = (header.AreaToTheSouthFlags & Common.Bit31) != 0;

            areFile.AreaToTheWestFlags.PartyRequired = (header.AreaToTheWestFlags & Common.Bit0) != 0;
            areFile.AreaToTheWestFlags.PartyEnabled = (header.AreaToTheWestFlags & Common.Bit1) != 0;
            areFile.AreaToTheWestFlags.Bit02 = (header.AreaToTheWestFlags & Common.Bit2) != 0;
            areFile.AreaToTheWestFlags.Bit03 = (header.AreaToTheWestFlags & Common.Bit3) != 0;
            areFile.AreaToTheWestFlags.Bit04 = (header.AreaToTheWestFlags & Common.Bit4) != 0;
            areFile.AreaToTheWestFlags.Bit05 = (header.AreaToTheWestFlags & Common.Bit5) != 0;
            areFile.AreaToTheWestFlags.Bit06 = (header.AreaToTheWestFlags & Common.Bit6) != 0;
            areFile.AreaToTheWestFlags.Bit07 = (header.AreaToTheWestFlags & Common.Bit7) != 0;
            areFile.AreaToTheWestFlags.Bit08 = (header.AreaToTheWestFlags & Common.Bit8) != 0;
            areFile.AreaToTheWestFlags.Bit09 = (header.AreaToTheWestFlags & Common.Bit9) != 0;
            areFile.AreaToTheWestFlags.Bit10 = (header.AreaToTheWestFlags & Common.Bit10) != 0;
            areFile.AreaToTheWestFlags.Bit11 = (header.AreaToTheWestFlags & Common.Bit11) != 0;
            areFile.AreaToTheWestFlags.Bit12 = (header.AreaToTheWestFlags & Common.Bit12) != 0;
            areFile.AreaToTheWestFlags.Bit13 = (header.AreaToTheWestFlags & Common.Bit13) != 0;
            areFile.AreaToTheWestFlags.Bit14 = (header.AreaToTheWestFlags & Common.Bit14) != 0;
            areFile.AreaToTheWestFlags.Bit15 = (header.AreaToTheWestFlags & Common.Bit15) != 0;
            areFile.AreaToTheWestFlags.Bit16 = (header.AreaToTheWestFlags & Common.Bit16) != 0;
            areFile.AreaToTheWestFlags.Bit17 = (header.AreaToTheWestFlags & Common.Bit17) != 0;
            areFile.AreaToTheWestFlags.Bit18 = (header.AreaToTheWestFlags & Common.Bit18) != 0;
            areFile.AreaToTheWestFlags.Bit19 = (header.AreaToTheWestFlags & Common.Bit19) != 0;
            areFile.AreaToTheWestFlags.Bit20 = (header.AreaToTheWestFlags & Common.Bit20) != 0;
            areFile.AreaToTheWestFlags.Bit21 = (header.AreaToTheWestFlags & Common.Bit21) != 0;
            areFile.AreaToTheWestFlags.Bit22 = (header.AreaToTheWestFlags & Common.Bit22) != 0;
            areFile.AreaToTheWestFlags.Bit23 = (header.AreaToTheWestFlags & Common.Bit23) != 0;
            areFile.AreaToTheWestFlags.Bit24 = (header.AreaToTheWestFlags & Common.Bit24) != 0;
            areFile.AreaToTheWestFlags.Bit25 = (header.AreaToTheWestFlags & Common.Bit25) != 0;
            areFile.AreaToTheWestFlags.Bit26 = (header.AreaToTheWestFlags & Common.Bit26) != 0;
            areFile.AreaToTheWestFlags.Bit27 = (header.AreaToTheWestFlags & Common.Bit27) != 0;
            areFile.AreaToTheWestFlags.Bit28 = (header.AreaToTheWestFlags & Common.Bit28) != 0;
            areFile.AreaToTheWestFlags.Bit29 = (header.AreaToTheWestFlags & Common.Bit29) != 0;
            areFile.AreaToTheWestFlags.Bit30 = (header.AreaToTheWestFlags & Common.Bit30) != 0;
            areFile.AreaToTheWestFlags.Bit31 = (header.AreaToTheWestFlags & Common.Bit31) != 0;

            areFile.AreaScript = header.AreaScript;
            areFile.AreaToTheEast = header.AreaToTheEast;
            areFile.AreaToTheNorth = header.AreaToTheNorth;
            areFile.AreaToTheSouth = header.AreaToTheSouth;
            areFile.AreaToTheWest = header.AreaToTheWest;
            areFile.AreaWed = header.AreaWED;
            areFile.LastSaved = header.LastSaved;
            areFile.RestMovieDay = header.RestMovieDay;
            areFile.RestMovieNight = header.RestMovieNight;
            areFile.OverlayTransparency = header.OverlayTransparency;
            areFile.TiledObjectFlagOffset = header.TiledObjectFlagOffset;
            areFile.TiledObjectFlagCount = header.TiledObjectFlagCount;
            areFile.WeatherProbabilityFog = header.WeatherProbabilityFog;
            areFile.WeatherProbabilityLightning = header.WeatherProbabilityLightning;
            areFile.WeatherProbabilityRain = header.WeatherProbabilityRain;
            areFile.WeatherProbabilitySnow = header.WeatherProbabilitySnow;

            foreach (var actor in actors)
            {
                var areActor2 = new AreActor2();
                areActor2.ActorFlags.CreAttached = (actor.Flags & Common.Bit0) != 0;
                areActor2.ActorFlags.HasSeenParty = (actor.Flags & Common.Bit1) != 0;
                areActor2.ActorFlags.Invulnerable = (actor.Flags & Common.Bit2) != 0;
                areActor2.ActorFlags.OverrideScriptName = (actor.Flags & Common.Bit3) != 0;
                areActor2.ActorFlags.Bit04 = (actor.Flags & Common.Bit4) != 0;
                areActor2.ActorFlags.Bit05 = (actor.Flags & Common.Bit5) != 0;
                areActor2.ActorFlags.Bit06 = (actor.Flags & Common.Bit6) != 0;
                areActor2.ActorFlags.Bit07 = (actor.Flags & Common.Bit7) != 0;
                areActor2.ActorFlags.Bit08 = (actor.Flags & Common.Bit8) != 0;
                areActor2.ActorFlags.Bit09 = (actor.Flags & Common.Bit9) != 0;
                areActor2.ActorFlags.Bit10 = (actor.Flags & Common.Bit10) != 0;
                areActor2.ActorFlags.Bit11 = (actor.Flags & Common.Bit11) != 0;
                areActor2.ActorFlags.Bit12 = (actor.Flags & Common.Bit12) != 0;
                areActor2.ActorFlags.Bit13 = (actor.Flags & Common.Bit13) != 0;
                areActor2.ActorFlags.Bit14 = (actor.Flags & Common.Bit14) != 0;
                areActor2.ActorFlags.Bit15 = (actor.Flags & Common.Bit15) != 0;
                areActor2.ActorFlags.Bit16 = (actor.Flags & Common.Bit16) != 0;
                areActor2.ActorFlags.Bit17 = (actor.Flags & Common.Bit17) != 0;
                areActor2.ActorFlags.Bit18 = (actor.Flags & Common.Bit18) != 0;
                areActor2.ActorFlags.Bit19 = (actor.Flags & Common.Bit19) != 0;
                areActor2.ActorFlags.Bit20 = (actor.Flags & Common.Bit20) != 0;
                areActor2.ActorFlags.Bit21 = (actor.Flags & Common.Bit21) != 0;
                areActor2.ActorFlags.Bit22 = (actor.Flags & Common.Bit22) != 0;
                areActor2.ActorFlags.Bit23 = (actor.Flags & Common.Bit23) != 0;
                areActor2.ActorFlags.Bit24 = (actor.Flags & Common.Bit24) != 0;
                areActor2.ActorFlags.Bit25 = (actor.Flags & Common.Bit25) != 0;
                areActor2.ActorFlags.Bit26 = (actor.Flags & Common.Bit26) != 0;
                areActor2.ActorFlags.Bit27 = (actor.Flags & Common.Bit27) != 0;
                areActor2.ActorFlags.Bit28 = (actor.Flags & Common.Bit28) != 0;
                areActor2.ActorFlags.Bit29 = (actor.Flags & Common.Bit29) != 0;
                areActor2.ActorFlags.Bit30 = (actor.Flags & Common.Bit30) != 0;
                areActor2.ActorFlags.Bit31 = (actor.Flags & Common.Bit31) != 0;
                areActor2.ActorAnimation = actor.ActorAnimation;
                areActor2.ActorAppearenceSchedule.Am1 = (actor.ActorAppearenceSchedule & Common.Bit0) != 0;
                areActor2.ActorAppearenceSchedule.Am2 = (actor.ActorAppearenceSchedule & Common.Bit1) != 0;
                areActor2.ActorAppearenceSchedule.Am3 = (actor.ActorAppearenceSchedule & Common.Bit2) != 0;
                areActor2.ActorAppearenceSchedule.Am4 = (actor.ActorAppearenceSchedule & Common.Bit3) != 0;
                areActor2.ActorAppearenceSchedule.Am5 = (actor.ActorAppearenceSchedule & Common.Bit4) != 0;
                areActor2.ActorAppearenceSchedule.Am6 = (actor.ActorAppearenceSchedule & Common.Bit5) != 0;
                areActor2.ActorAppearenceSchedule.Am7 = (actor.ActorAppearenceSchedule & Common.Bit6) != 0;
                areActor2.ActorAppearenceSchedule.Am8 = (actor.ActorAppearenceSchedule & Common.Bit7) != 0;
                areActor2.ActorAppearenceSchedule.Am9 = (actor.ActorAppearenceSchedule & Common.Bit8) != 0;
                areActor2.ActorAppearenceSchedule.Am10 = (actor.ActorAppearenceSchedule & Common.Bit9) != 0;
                areActor2.ActorAppearenceSchedule.Am11 = (actor.ActorAppearenceSchedule & Common.Bit10) != 0;
                areActor2.ActorAppearenceSchedule.Am12 = (actor.ActorAppearenceSchedule & Common.Bit11) != 0;
                areActor2.ActorAppearenceSchedule.Pm1 = (actor.ActorAppearenceSchedule & Common.Bit12) != 0;
                areActor2.ActorAppearenceSchedule.Pm2 = (actor.ActorAppearenceSchedule & Common.Bit13) != 0;
                areActor2.ActorAppearenceSchedule.Pm3 = (actor.ActorAppearenceSchedule & Common.Bit14) != 0;
                areActor2.ActorAppearenceSchedule.Pm4 = (actor.ActorAppearenceSchedule & Common.Bit15) != 0;
                areActor2.ActorAppearenceSchedule.Pm5 = (actor.ActorAppearenceSchedule & Common.Bit16) != 0;
                areActor2.ActorAppearenceSchedule.Pm6 = (actor.ActorAppearenceSchedule & Common.Bit17) != 0;
                areActor2.ActorAppearenceSchedule.Pm7 = (actor.ActorAppearenceSchedule & Common.Bit18) != 0;
                areActor2.ActorAppearenceSchedule.Pm8 = (actor.ActorAppearenceSchedule & Common.Bit19) != 0;
                areActor2.ActorAppearenceSchedule.Pm9 = (actor.ActorAppearenceSchedule & Common.Bit20) != 0;
                areActor2.ActorAppearenceSchedule.Pm10 = (actor.ActorAppearenceSchedule & Common.Bit21) != 0;
                areActor2.ActorAppearenceSchedule.Pm11 = (actor.ActorAppearenceSchedule & Common.Bit22) != 0;
                areActor2.ActorAppearenceSchedule.Pm12 = (actor.ActorAppearenceSchedule & Common.Bit23) != 0;
                areActor2.ActorAppearenceSchedule.Bit24 = (actor.ActorAppearenceSchedule & Common.Bit24) != 0;
                areActor2.ActorAppearenceSchedule.Bit25 = (actor.ActorAppearenceSchedule & Common.Bit25) != 0;
                areActor2.ActorAppearenceSchedule.Bit26 = (actor.ActorAppearenceSchedule & Common.Bit26) != 0;
                areActor2.ActorAppearenceSchedule.Bit27 = (actor.ActorAppearenceSchedule & Common.Bit27) != 0;
                areActor2.ActorAppearenceSchedule.Bit28 = (actor.ActorAppearenceSchedule & Common.Bit28) != 0;
                areActor2.ActorAppearenceSchedule.Bit29 = (actor.ActorAppearenceSchedule & Common.Bit29) != 0;
                areActor2.ActorAppearenceSchedule.Bit30 = (actor.ActorAppearenceSchedule & Common.Bit30) != 0;
                areActor2.ActorAppearenceSchedule.Bit31 = (actor.ActorAppearenceSchedule & Common.Bit31) != 0;
                areActor2.ActorOrientation = actor.ActorOrientation;
                areActor2.ActorRemovalTimer = actor.ActorRemovalTimer;
                areActor2.CREFile = actor.CREFile;
                areActor2.CreOffset = actor.CreOffset;
                areActor2.CreSize = actor.CreSize;
                areActor2.CurrentXCoordinate = actor.CurrentXCoordinate;
                areActor2.CurrentYCoordinate = actor.CurrentYCoordinate;
                areActor2.DestinationXCoordinate = actor.DestinationXCoordinate;
                areActor2.DestinationYCoordinate = actor.DestinationYCoordinate;
                areActor2.FilenameInitialCharacter = actor.FilenameInitialCharacter;
                areActor2.HasBeenSpawned = actor.HasBeenSpawned;
                areActor2.MovementRestrictionDistance = actor.MovementRestrictionDistance;
                areActor2.MovementRestrictionDistanceMoveToObject = actor.MovementRestrictionDistanceMoveToObject;
                areActor2.Name = actor.Name;
                areActor2.NumTimesTalkedTo = actor.NumTimesTalkedTo;
                areActor2.ScriptClass = actor.ScriptClass;
                areActor2.ScriptDefault = actor.ScriptDefault;
                areActor2.ScriptGeneral = actor.ScriptGeneral;
                areActor2.ScriptOverride = actor.ScriptOverride;
                areActor2.ScriptRace = actor.ScriptRace;
                areActor2.ScriptSpecific = actor.ScriptSpecific;
                areActor2.Unknownef = actor.Unknownef;
                areActor2.Unknown90 = actor.Unknown90;
                areActor2.Unknown36 = actor.Unknown36;
                areFile.actors.Add(areActor2);
            }

            foreach (var region in regions)
            {
                var region2 = new AreRegion2();
                region2.Flags.KeyRequired = (region.Flags & Common.Bit0) != 0;
                region2.Flags.ResetTrap = (region.Flags & Common.Bit1) != 0;
                region2.Flags.PartyRequired = (region.Flags & Common.Bit2) != 0;
                region2.Flags.Detectable = (region.Flags & Common.Bit3) != 0;
                region2.Flags.EnemiesActivates = (region.Flags & Common.Bit4) != 0;
                region2.Flags.TutorialTrigger = (region.Flags & Common.Bit5) != 0;
                region2.Flags.NpcActivates = (region.Flags & Common.Bit6) != 0;
                region2.Flags.SilentTrigger = (region.Flags & Common.Bit7) != 0;
                region2.Flags.Deactivated = (region.Flags & Common.Bit8) != 0;
                region2.Flags.NPCCannotPass = (region.Flags & Common.Bit9) != 0;
                region2.Flags.AlternativePoint = (region.Flags & Common.Bit10) != 0;
                region2.Flags.DoorClosed = (region.Flags & Common.Bit11) != 0;
                region2.Flags.Bit12 = (region.Flags & Common.Bit12) != 0;
                region2.Flags.Bit13 = (region.Flags & Common.Bit13) != 0;
                region2.Flags.Bit14 = (region.Flags & Common.Bit14) != 0;
                region2.Flags.Bit15 = (region.Flags & Common.Bit15) != 0;
                region2.Flags.Bit16 = (region.Flags & Common.Bit16) != 0;
                region2.Flags.Bit17 = (region.Flags & Common.Bit17) != 0;
                region2.Flags.Bit18 = (region.Flags & Common.Bit18) != 0;
                region2.Flags.Bit19 = (region.Flags & Common.Bit19) != 0;
                region2.Flags.Bit20 = (region.Flags & Common.Bit20) != 0;
                region2.Flags.Bit21 = (region.Flags & Common.Bit21) != 0;
                region2.Flags.Bit22 = (region.Flags & Common.Bit22) != 0;
                region2.Flags.Bit23 = (region.Flags & Common.Bit23) != 0;
                region2.Flags.Bit24 = (region.Flags & Common.Bit24) != 0;
                region2.Flags.Bit25 = (region.Flags & Common.Bit25) != 0;
                region2.Flags.Bit26 = (region.Flags & Common.Bit26) != 0;
                region2.Flags.Bit27 = (region.Flags & Common.Bit27) != 0;
                region2.Flags.Bit28 = (region.Flags & Common.Bit28) != 0;
                region2.Flags.Bit29 = (region.Flags & Common.Bit29) != 0;
                region2.Flags.Bit30 = (region.Flags & Common.Bit30) != 0;
                region2.Flags.Bit31 = (region.Flags & Common.Bit31) != 0;

                region2.AlternativeUsePointXCoordinate = region.AlternativeUsePointXCoordinate;
                region2.AlternativeUsePointYCoordinate = region.AlternativeUsePointYCoordinate;
                region2.BoundingBoxBottom = region.BoundingBoxBottom;
                region2.BoundingBoxLeft = region.BoundingBoxLeft;
                region2.BoundingBoxRight = region.BoundingBoxRight;
                region2.BoundingBoxTop = region.BoundingBoxTop;
                region2.Cursor = region.Cursor;
                region2.DestinationArea = region.DestinationArea;
                region2.DestinationEntrance = region.DestinationEntrance;
                region2.DialogFile = region.DialogFile;
                region2.DialogName = Common.ReadString(region.DialogName, TlkFile);
                region2.InformationText = Common.ReadString(region.InformationText, TlkFile);
                region2.IsTrap = region.IsTrap;
                region2.KeyItem = region.KeyItem;
                region2.Name = region.Name;
                region2.RegionScript = region.RegionScript;
                region2.RegionType = (RegionType)region.RegionType;
                region2.Sound = region.Sound;
                region2.TalkLocationXCoordinate = region.TalkLocationXCoordinate;
                region2.TalkLocationYCoordinate = region.TalkLocationYCoordinate;
                region2.TrapDetected = region.TrapDetected;
                region2.TrapDetectionDifficulty = region.TrapDetectionDifficulty;
                region2.TrapLaunchXCoordinate = region.TrapLaunchXCoordinate;
                region2.TrapLaunchYCoordinate = region.TrapLaunchYCoordinate;
                region2.TrapRemovalDifficulty = region.TrapRemovalDifficulty;
                region2.TriggerValue = region.TriggerValue;
                region2.Unknown88 = region.Unknown88;
                region2.Unknown8c = region.Unknown8c;
                region2.VertexCount = region.VertexCount;//xx
                region2.VertexIndex = region.VertexIndex;//xx
                areFile.regions.Add(region2);
            }

            foreach (var spawn in spawns)
            {
                var spawn2 = new AreSpawnPoint2();
                spawn2.ActorRemovalTime = spawn.ActorRemovalTime;
                spawn2.BaseCreatureNumberToSpawn = spawn.BaseCreatureNumberToSpawn;
                spawn2.CreatureSpawnCount = spawn.CreatureSpawnCount;
                spawn2.Enabled = spawn.Enabled;
                spawn2.Frequency = spawn.Frequency;
                spawn2.MaximumCreaturesToSpawn = spawn.MaximumCreaturesToSpawn;
                spawn2.MovementRestrictionDistance = spawn.MovementRestrictionDistance;
                spawn2.MovementRestrictionDistanceToObject = spawn.MovementRestrictionDistanceToObject;
                spawn2.Name = spawn.Name;
                spawn2.ProbabilityDay = spawn.ProbabilityDay;
                spawn2.ProbabilityNight = spawn.ProbabilityNight;
                spawn2.Resref1 = spawn.Resref1;
                spawn2.Resref2 = spawn.Resref2;
                spawn2.Resref3 = spawn.Resref3;
                spawn2.Resref4 = spawn.Resref4;
                spawn2.Resref5 = spawn.Resref5;
                spawn2.Resref6 = spawn.Resref6;
                spawn2.Resref7 = spawn.Resref7;
                spawn2.Resref8 = spawn.Resref8;
                spawn2.Resref9 = spawn.Resref9;
                spawn2.Resref10 = spawn.Resref10;
                spawn2.SpawnMethod.SpawnUntilPaused = (spawn.SpawnMethod & Common.Bit0) != 0;
                spawn2.SpawnMethod.DisableAfterSpawn = (spawn.SpawnMethod & Common.Bit1) != 0;
                spawn2.SpawnMethod.SpawnPaused = (spawn.SpawnMethod & Common.Bit2) != 0;
                spawn2.SpawnMethod.Bit3 = (spawn.SpawnMethod & Common.Bit3) != 0;
                spawn2.SpawnMethod.Bit4 = (spawn.SpawnMethod & Common.Bit4) != 0;
                spawn2.SpawnMethod.Bit5 = (spawn.SpawnMethod & Common.Bit5) != 0;
                spawn2.SpawnMethod.Bit6 = (spawn.SpawnMethod & Common.Bit6) != 0;
                spawn2.SpawnMethod.Bit7 = (spawn.SpawnMethod & Common.Bit7) != 0;
                spawn2.SpawnMethod.Bit8 = (spawn.SpawnMethod & Common.Bit8) != 0;
                spawn2.SpawnMethod.Bit9 = (spawn.SpawnMethod & Common.Bit9) != 0;
                spawn2.SpawnMethod.Bit10 = (spawn.SpawnMethod & Common.Bit10) != 0;
                spawn2.SpawnMethod.Bit11 = (spawn.SpawnMethod & Common.Bit11) != 0;
                spawn2.SpawnMethod.Bit12 = (spawn.SpawnMethod & Common.Bit12) != 0;
                spawn2.SpawnMethod.Bit13 = (spawn.SpawnMethod & Common.Bit13) != 0;
                spawn2.SpawnMethod.Bit14 = (spawn.SpawnMethod & Common.Bit14) != 0;
                spawn2.SpawnMethod.Bit15 = (spawn.SpawnMethod & Common.Bit15) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am1 = (spawn.SpawnPointAppearenceSchedule & Common.Bit0) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am2 = (spawn.SpawnPointAppearenceSchedule & Common.Bit1) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am3 = (spawn.SpawnPointAppearenceSchedule & Common.Bit2) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am4 = (spawn.SpawnPointAppearenceSchedule & Common.Bit3) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am5 = (spawn.SpawnPointAppearenceSchedule & Common.Bit4) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am6 = (spawn.SpawnPointAppearenceSchedule & Common.Bit5) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am7 = (spawn.SpawnPointAppearenceSchedule & Common.Bit6) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am8 = (spawn.SpawnPointAppearenceSchedule & Common.Bit7) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am9 = (spawn.SpawnPointAppearenceSchedule & Common.Bit8) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am10 = (spawn.SpawnPointAppearenceSchedule & Common.Bit9) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am11 = (spawn.SpawnPointAppearenceSchedule & Common.Bit10) != 0;
                spawn2.SpawnPointAppearenceSchedule.Am12 = (spawn.SpawnPointAppearenceSchedule & Common.Bit11) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm1 = (spawn.SpawnPointAppearenceSchedule & Common.Bit12) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm2 = (spawn.SpawnPointAppearenceSchedule & Common.Bit13) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm3 = (spawn.SpawnPointAppearenceSchedule & Common.Bit14) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm4 = (spawn.SpawnPointAppearenceSchedule & Common.Bit15) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm5 = (spawn.SpawnPointAppearenceSchedule & Common.Bit16) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm6 = (spawn.SpawnPointAppearenceSchedule & Common.Bit17) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm7 = (spawn.SpawnPointAppearenceSchedule & Common.Bit18) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm8 = (spawn.SpawnPointAppearenceSchedule & Common.Bit19) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm9 = (spawn.SpawnPointAppearenceSchedule & Common.Bit20) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm10 = (spawn.SpawnPointAppearenceSchedule & Common.Bit21) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm11 = (spawn.SpawnPointAppearenceSchedule & Common.Bit22) != 0;
                spawn2.SpawnPointAppearenceSchedule.Pm12 = (spawn.SpawnPointAppearenceSchedule & Common.Bit23) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit24 = (spawn.SpawnPointAppearenceSchedule & Common.Bit24) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit25 = (spawn.SpawnPointAppearenceSchedule & Common.Bit25) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit26 = (spawn.SpawnPointAppearenceSchedule & Common.Bit26) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit27 = (spawn.SpawnPointAppearenceSchedule & Common.Bit27) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit28 = (spawn.SpawnPointAppearenceSchedule & Common.Bit28) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit29 = (spawn.SpawnPointAppearenceSchedule & Common.Bit29) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit30 = (spawn.SpawnPointAppearenceSchedule & Common.Bit30) != 0;
                spawn2.SpawnPointAppearenceSchedule.Bit31 = (spawn.SpawnPointAppearenceSchedule & Common.Bit31) != 0;
                spawn2.Unknowna2 = spawn.Unknowna2;
                spawn2.XCoordinate = spawn.XCoordinate;
                spawn2.YCoordinate = spawn.YCoordinate;
                spawn2.SpawnFrequency = spawn.SpawnFrequency;
                spawn2.Countdown = spawn.Countdown;
                spawn2.SpawnWeight1 = spawn.SpawnWeight1;
                spawn2.SpawnWeight2 = spawn.SpawnWeight2;
                spawn2.SpawnWeight3 = spawn.SpawnWeight3;
                spawn2.SpawnWeight4 = spawn.SpawnWeight4;
                spawn2.SpawnWeight5 = spawn.SpawnWeight5;
                spawn2.SpawnWeight6 = spawn.SpawnWeight6;
                spawn2.SpawnWeight7 = spawn.SpawnWeight7;
                spawn2.SpawnWeight8 = spawn.SpawnWeight8;
                spawn2.SpawnWeight9 = spawn.SpawnWeight9;
                spawn2.SpawnWeight10 = spawn.SpawnWeight10;
                spawn2.Unknowna2 = spawn.Unknowna2;
                areFile.spawns.Add(spawn2);
            }

            foreach (var entrance in entrances)
            {
                var entrance2 = new AreEntrance2();
                entrance2.Name = entrance.Name;
                entrance2.Orientation = entrance.Orientation;
                entrance2.Unknown26 = entrance.Unknown26;
                entrance2.XCoordinate = entrance.XCoordinate;
                entrance2.YCoordinate = entrance.YCoordinate;
                areFile.entrances.Add(entrance2);
            }

            foreach (var container in containers)
            {
                var container2 = new AreContainer2();

                container2.Flags.Locked = (container.Flags & Common.Bit0) != 0;
                container2.Flags.DisabledIfNoOwner = (container.Flags & Common.Bit1) != 0;
                container2.Flags.MagicalLock = (container.Flags & Common.Bit2) != 0;
                container2.Flags.TrapResets = (container.Flags & Common.Bit3) != 0;
                container2.Flags.RemoveOnly = (container.Flags & Common.Bit4) != 0;
                container2.Flags.Disabled = (container.Flags & Common.Bit5) != 0;
                container2.Flags.DoNotClear = (container.Flags & Common.Bit6) != 0;
                container2.Flags.Bit07 = (container.Flags & Common.Bit7) != 0;
                container2.Flags.Bit08 = (container.Flags & Common.Bit8) != 0;
                container2.Flags.Bit09 = (container.Flags & Common.Bit9) != 0;
                container2.Flags.Bit10 = (container.Flags & Common.Bit10) != 0;
                container2.Flags.Bit11 = (container.Flags & Common.Bit11) != 0;
                container2.Flags.Bit12 = (container.Flags & Common.Bit12) != 0;
                container2.Flags.Bit13 = (container.Flags & Common.Bit13) != 0;
                container2.Flags.Bit14 = (container.Flags & Common.Bit14) != 0;
                container2.Flags.Bit15 = (container.Flags & Common.Bit15) != 0;
                container2.Flags.Bit16 = (container.Flags & Common.Bit16) != 0;
                container2.Flags.Bit17 = (container.Flags & Common.Bit17) != 0;
                container2.Flags.Bit18 = (container.Flags & Common.Bit18) != 0;
                container2.Flags.Bit19 = (container.Flags & Common.Bit19) != 0;
                container2.Flags.Bit20 = (container.Flags & Common.Bit20) != 0;
                container2.Flags.Bit21 = (container.Flags & Common.Bit21) != 0;
                container2.Flags.Bit22 = (container.Flags & Common.Bit22) != 0;
                container2.Flags.Bit23 = (container.Flags & Common.Bit23) != 0;
                container2.Flags.Bit24 = (container.Flags & Common.Bit24) != 0;
                container2.Flags.Bit25 = (container.Flags & Common.Bit25) != 0;
                container2.Flags.Bit26 = (container.Flags & Common.Bit26) != 0;
                container2.Flags.Bit27 = (container.Flags & Common.Bit27) != 0;
                container2.Flags.Bit28 = (container.Flags & Common.Bit28) != 0;
                container2.Flags.Bit29 = (container.Flags & Common.Bit29) != 0;
                container2.Flags.Bit30 = (container.Flags & Common.Bit30) != 0;
                container2.Flags.Bit31 = (container.Flags & Common.Bit31) != 0;

                container2.BoundingBoxBottom = container.BoundingBoxBottom;
                container2.BoundingBoxLeft = container.BoundingBoxLeft;
                container2.BoundingBoxRight = container.BoundingBoxRight;
                container2.BoundingBoxTop = container.BoundingBoxTop;
                container2.ContainerType = (ContainerType)container.ContainerType;
                container2.IsTrap = container.IsTrap;
                container2.KeyItem = container.KeyItem;
                container2.LockDifficulty = container.LockDifficulty;
                container2.LockpickString = Common.ReadString(container.LockpickString, TlkFile);
                container2.Name = container.Name;
                container2.TrapDetected = container.TrapDetected;
                container2.TrapDetectionDifficulty = container.TrapDetectionDifficulty;
                container2.TrapLaunchXCoordinate = container.TrapLaunchXCoordinate;
                container2.TrapLaunchYCoordinate = container.TrapLaunchYCoordinate;
                container2.TrapRemovalDifficulty = container.TrapRemovalDifficulty;
                container2.TrapScript = container.TrapScript;
                container2.TriggerRange = container.TriggerRange;
                container2.Owner = container.Owner;
                container2.BreakDifficulty = container.BreakDifficulty;
                container2.VertexCount = container.VertexCount;//xx
                container2.VertexIndex = container.VertexIndex;//xx
                container2.XCoordinate = container.XCoordinate;
                container2.YCoordinate = container.YCoordinate;

                for (int i = 0; i < container.ItemCount; i++)
                {
                    var item = new AreItem2();
                    item.Charges1 = items[container.ItemIndex + 0].Charges1;
                    item.Charges2 = items[container.ItemIndex + 0].Charges2;
                    item.Charges3 = items[container.ItemIndex + 0].Charges3;
                    item.ExpirationTime = items[container.ItemIndex + 0].ExpirationTime;
                    item.Flags.Identified = (items[container.ItemIndex + 0].Flags & Common.Bit0) != 0;
                    item.Flags.Unstealable = (items[container.ItemIndex + 0].Flags & Common.Bit1) != 0;
                    item.Flags.Stolen = (items[container.ItemIndex + 0].Flags & Common.Bit2) != 0;
                    item.Flags.Unstealable = (items[container.ItemIndex + 0].Flags & Common.Bit3) != 0;
                    item.Flags.Bit04 = (items[container.ItemIndex + 0].Flags & Common.Bit4) != 0;
                    item.Flags.Bit05 = (items[container.ItemIndex + 0].Flags & Common.Bit5) != 0;
                    item.Flags.Bit06 = (items[container.ItemIndex + 0].Flags & Common.Bit6) != 0;
                    item.Flags.Bit07 = (items[container.ItemIndex + 0].Flags & Common.Bit7) != 0;
                    item.Flags.Bit08 = (items[container.ItemIndex + 0].Flags & Common.Bit8) != 0;
                    item.Flags.Bit09 = (items[container.ItemIndex + 0].Flags & Common.Bit9) != 0;
                    item.Flags.Bit10 = (items[container.ItemIndex + 0].Flags & Common.Bit10) != 0;
                    item.Flags.Bit11 = (items[container.ItemIndex + 0].Flags & Common.Bit11) != 0;
                    item.Flags.Bit12 = (items[container.ItemIndex + 0].Flags & Common.Bit12) != 0;
                    item.Flags.Bit13 = (items[container.ItemIndex + 0].Flags & Common.Bit13) != 0;
                    item.Flags.Bit14 = (items[container.ItemIndex + 0].Flags & Common.Bit14) != 0;
                    item.Flags.Bit15 = (items[container.ItemIndex + 0].Flags & Common.Bit15) != 0;
                    item.Flags.Bit16 = (items[container.ItemIndex + 0].Flags & Common.Bit16) != 0;
                    item.Flags.Bit17 = (items[container.ItemIndex + 0].Flags & Common.Bit17) != 0;
                    item.Flags.Bit18 = (items[container.ItemIndex + 0].Flags & Common.Bit18) != 0;
                    item.Flags.Bit19 = (items[container.ItemIndex + 0].Flags & Common.Bit19) != 0;
                    item.Flags.Bit20 = (items[container.ItemIndex + 0].Flags & Common.Bit20) != 0;
                    item.Flags.Bit21 = (items[container.ItemIndex + 0].Flags & Common.Bit21) != 0;
                    item.Flags.Bit22 = (items[container.ItemIndex + 0].Flags & Common.Bit22) != 0;
                    item.Flags.Bit23 = (items[container.ItemIndex + 0].Flags & Common.Bit23) != 0;
                    item.Flags.Bit24 = (items[container.ItemIndex + 0].Flags & Common.Bit24) != 0;
                    item.Flags.Bit25 = (items[container.ItemIndex + 0].Flags & Common.Bit25) != 0;
                    item.Flags.Bit26 = (items[container.ItemIndex + 0].Flags & Common.Bit26) != 0;
                    item.Flags.Bit27 = (items[container.ItemIndex + 0].Flags & Common.Bit27) != 0;
                    item.Flags.Bit28 = (items[container.ItemIndex + 0].Flags & Common.Bit28) != 0;
                    item.Flags.Bit29 = (items[container.ItemIndex + 0].Flags & Common.Bit29) != 0;
                    item.Flags.Bit30 = (items[container.ItemIndex + 0].Flags & Common.Bit30) != 0;
                    item.Flags.Bit31 = (items[container.ItemIndex + 0].Flags & Common.Bit31) != 0;
                    item.ItemResref = items[container.ItemIndex + 0].ItemResref;
                    container2.items.Add(item);
                }

                areFile.containers.Add(container2);
            }
            /*
            foreach (var item in items)
            {
                var item2 = new AreItem2();
                item2.Charges1 = item.Charges1;
                item2.Charges2 = item.Charges2;
                item2.Charges3 = item.Charges3;
                item2.ExpirationTime = item.ExpirationTime;
                item2.Flags = item.Flags;//xx AreaItemFlags
                item2.ItemResref = item.ItemResref.ToString();
                areFile.items.Add(item2);
            }
            */
            foreach (var ambient in ambients)
            {
                var ambient2 = new AreAmbient2();
                ambient2.AmbientAppearenceSchedule.Am1 = (ambient.AmbientAppearenceSchedule & Common.Bit0) != 0;
                ambient2.AmbientAppearenceSchedule.Am2 = (ambient.AmbientAppearenceSchedule & Common.Bit1) != 0;
                ambient2.AmbientAppearenceSchedule.Am3 = (ambient.AmbientAppearenceSchedule & Common.Bit2) != 0;
                ambient2.AmbientAppearenceSchedule.Am4 = (ambient.AmbientAppearenceSchedule & Common.Bit3) != 0;
                ambient2.AmbientAppearenceSchedule.Am5 = (ambient.AmbientAppearenceSchedule & Common.Bit4) != 0;
                ambient2.AmbientAppearenceSchedule.Am6 = (ambient.AmbientAppearenceSchedule & Common.Bit5) != 0;
                ambient2.AmbientAppearenceSchedule.Am7 = (ambient.AmbientAppearenceSchedule & Common.Bit6) != 0;
                ambient2.AmbientAppearenceSchedule.Am8 = (ambient.AmbientAppearenceSchedule & Common.Bit7) != 0;
                ambient2.AmbientAppearenceSchedule.Am9 = (ambient.AmbientAppearenceSchedule & Common.Bit8) != 0;
                ambient2.AmbientAppearenceSchedule.Am10 = (ambient.AmbientAppearenceSchedule & Common.Bit9) != 0;
                ambient2.AmbientAppearenceSchedule.Am11 = (ambient.AmbientAppearenceSchedule & Common.Bit10) != 0;
                ambient2.AmbientAppearenceSchedule.Am12 = (ambient.AmbientAppearenceSchedule & Common.Bit11) != 0;
                ambient2.AmbientAppearenceSchedule.Pm1 = (ambient.AmbientAppearenceSchedule & Common.Bit12) != 0;
                ambient2.AmbientAppearenceSchedule.Pm2 = (ambient.AmbientAppearenceSchedule & Common.Bit13) != 0;
                ambient2.AmbientAppearenceSchedule.Pm3 = (ambient.AmbientAppearenceSchedule & Common.Bit14) != 0;
                ambient2.AmbientAppearenceSchedule.Pm4 = (ambient.AmbientAppearenceSchedule & Common.Bit15) != 0;
                ambient2.AmbientAppearenceSchedule.Pm5 = (ambient.AmbientAppearenceSchedule & Common.Bit16) != 0;
                ambient2.AmbientAppearenceSchedule.Pm6 = (ambient.AmbientAppearenceSchedule & Common.Bit17) != 0;
                ambient2.AmbientAppearenceSchedule.Pm7 = (ambient.AmbientAppearenceSchedule & Common.Bit18) != 0;
                ambient2.AmbientAppearenceSchedule.Pm8 = (ambient.AmbientAppearenceSchedule & Common.Bit19) != 0;
                ambient2.AmbientAppearenceSchedule.Pm9 = (ambient.AmbientAppearenceSchedule & Common.Bit20) != 0;
                ambient2.AmbientAppearenceSchedule.Pm10 = (ambient.AmbientAppearenceSchedule & Common.Bit21) != 0;
                ambient2.AmbientAppearenceSchedule.Pm11 = (ambient.AmbientAppearenceSchedule & Common.Bit22) != 0;
                ambient2.AmbientAppearenceSchedule.Pm12 = (ambient.AmbientAppearenceSchedule & Common.Bit23) != 0;
                ambient2.AmbientAppearenceSchedule.Bit24 = (ambient.AmbientAppearenceSchedule & Common.Bit24) != 0;
                ambient2.AmbientAppearenceSchedule.Bit25 = (ambient.AmbientAppearenceSchedule & Common.Bit25) != 0;
                ambient2.AmbientAppearenceSchedule.Bit26 = (ambient.AmbientAppearenceSchedule & Common.Bit26) != 0;
                ambient2.AmbientAppearenceSchedule.Bit27 = (ambient.AmbientAppearenceSchedule & Common.Bit27) != 0;
                ambient2.AmbientAppearenceSchedule.Bit28 = (ambient.AmbientAppearenceSchedule & Common.Bit28) != 0;
                ambient2.AmbientAppearenceSchedule.Bit29 = (ambient.AmbientAppearenceSchedule & Common.Bit29) != 0;
                ambient2.AmbientAppearenceSchedule.Bit30 = (ambient.AmbientAppearenceSchedule & Common.Bit30) != 0;
                ambient2.AmbientAppearenceSchedule.Bit31 = (ambient.AmbientAppearenceSchedule & Common.Bit31) != 0;
                ambient2.Flags.Enabled = (ambient.Flags & Common.Bit0) != 0;
                ambient2.Flags.Looping = (ambient.Flags & Common.Bit1) != 0;
                ambient2.Flags.IgnoreRadius = (ambient.Flags & Common.Bit2) != 0;
                ambient2.Flags.PlayInRandomOrder = (ambient.Flags & Common.Bit3) != 0;
                ambient2.Flags.HighMemoryAmbient = (ambient.Flags & Common.Bit4) != 0;
                ambient2.Flags.Bit5 = (ambient.Flags & Common.Bit5) != 0;
                ambient2.Flags.Bit6 = (ambient.Flags & Common.Bit6) != 0;
                ambient2.Flags.Bit7 = (ambient.Flags & Common.Bit7) != 0;
                ambient2.Flags.Bit8 = (ambient.Flags & Common.Bit8) != 0;
                ambient2.Flags.Bit9 = (ambient.Flags & Common.Bit9) != 0;
                ambient2.Flags.Bit10 = (ambient.Flags & Common.Bit10) != 0;
                ambient2.Flags.Bit11 = (ambient.Flags & Common.Bit11) != 0;
                ambient2.Flags.Bit12 = (ambient.Flags & Common.Bit12) != 0;
                ambient2.Flags.Bit13 = (ambient.Flags & Common.Bit13) != 0;
                ambient2.Flags.Bit14 = (ambient.Flags & Common.Bit14) != 0;
                ambient2.Flags.Bit15 = (ambient.Flags & Common.Bit15) != 0;
                ambient2.Flags.Bit16 = (ambient.Flags & Common.Bit16) != 0;
                ambient2.Flags.Bit17 = (ambient.Flags & Common.Bit17) != 0;
                ambient2.Flags.Bit18 = (ambient.Flags & Common.Bit18) != 0;
                ambient2.Flags.Bit19 = (ambient.Flags & Common.Bit19) != 0;
                ambient2.Flags.Bit20 = (ambient.Flags & Common.Bit20) != 0;
                ambient2.Flags.Bit21 = (ambient.Flags & Common.Bit21) != 0;
                ambient2.Flags.Bit22 = (ambient.Flags & Common.Bit22) != 0;
                ambient2.Flags.Bit23 = (ambient.Flags & Common.Bit23) != 0;
                ambient2.Flags.Bit24 = (ambient.Flags & Common.Bit24) != 0;
                ambient2.Flags.Bit25 = (ambient.Flags & Common.Bit25) != 0;
                ambient2.Flags.Bit26 = (ambient.Flags & Common.Bit26) != 0;
                ambient2.Flags.Bit27 = (ambient.Flags & Common.Bit27) != 0;
                ambient2.Flags.Bit28 = (ambient.Flags & Common.Bit28) != 0;
                ambient2.Flags.Bit29 = (ambient.Flags & Common.Bit29) != 0;
                ambient2.Flags.Bit30 = (ambient.Flags & Common.Bit30) != 0;
                ambient2.Flags.Bit31 = (ambient.Flags & Common.Bit31) != 0;
                ambient2.FrequencyBase = ambient.FrequencyBase;
                ambient2.FrequencyVariation = ambient.FrequencyVariation;
                ambient2.Height = ambient.Height;
                ambient2.Name = ambient.Name;
                ambient2.Radius = ambient.Radius;
                ambient2.Resref1 = ambient.Resref1;
                ambient2.Resref2 = ambient.Resref2;
                ambient2.Resref3 = ambient.Resref3;
                ambient2.Resref4 = ambient.Resref4;
                ambient2.Resref5 = ambient.Resref5;
                ambient2.Resref6 = ambient.Resref6;
                ambient2.Resref7 = ambient.Resref7;
                ambient2.Resref8 = ambient.Resref8;
                ambient2.Resref9 = ambient.Resref9;
                ambient2.Resref10 = ambient.Resref10;
                //ambient2.ResRefCount = ambient.ResRefCount;//xx
                ambient2.PitchVariance = ambient.PitchVariance;
                ambient2.VolumeVariance = ambient.VolumeVariance;
                ambient2.Unknown82 = ambient.Unknown82;
                ambient2.Unknown94 = ambient.Unknownc0;
                ambient2.Volume = ambient.Volume;
                ambient2.XCoordinate = ambient.XCoordinate;
                ambient2.YCoordinate = ambient.YCoordinate;
                areFile.ambients.Add(ambient2);
            }

            foreach (var variable in variables)
            {
                var variable2 = new AreVariable2();
                variable2.Name = variable.Name;
                variable2.Type = variable.Type;
                variable2.ResourceType = variable.ResourceType;
                variable2.ValueDword = variable.ValueDword;
                variable2.ValueInt = variable.ValueInt;
                variable2.ValueDouble = variable.ValueDouble;
                variable2.ScriptName = variable.ScriptName;
                areFile.variables.Add(variable2);
            }

            foreach (var door in doors)
            {
                var door2 = new AreDoor2();
                door2.ClosedBoundingBoxBottom = door.ClosedBoundingBoxBottom;
                door2.ClosedBoundingBoxLeft = door.ClosedBoundingBoxLeft;
                door2.ClosedBoundingBoxRight = door.ClosedBoundingBoxRight;
                door2.ClosedBoundingBoxTop = door.ClosedBoundingBoxTop;
                door2.ClosedVertexBlockCount = door.ClosedVertexBlockCount;//xx
                door2.ClosedVertexBlockIndex = door.ClosedVertexBlockIndex;//xx
                door2.ClosedVertexCount = door.ClosedVertexCount;//xx
                door2.ClosedVertexIndex = door.ClosedVertexIndex;//xx
                door2.Cursor = door.Cursor;
                door2.DialogName = Common.ReadString(door.DialogName, TlkFile);
                door2.DialogResref = door.DialogResref;
                door2.DoorCloseSound = door.DoorCloseSound;
                door2.DoorId = door.DoorId;
                door2.DoorOpenSound = door.DoorOpenSound;
                door2.DoorScript = door.DoorScript;
                door2.DoorState1X = door.DoorState1X;
                door2.DoorState1Y = door.DoorState1Y;
                door2.DoorState2X = door.DoorState2X;
                door2.DoorState2Y = door.DoorState2Y;
                door2.Flags.DoorOpen = (door.Flags & Common.Bit0) != 0;
                door2.Flags.DoorLocked = (door.Flags & Common.Bit1) != 0;
                door2.Flags.ResetTrap = (door.Flags & Common.Bit2) != 0;
                door2.Flags.DetectableTrap = (door.Flags & Common.Bit3) != 0;
                door2.Flags.DoorForced = (door.Flags & Common.Bit4) != 0;
                door2.Flags.CannotClose = (door.Flags & Common.Bit5) != 0;
                door2.Flags.Linked = (door.Flags & Common.Bit6) != 0;
                door2.Flags.DoorHidden = (door.Flags & Common.Bit7) != 0;
                door2.Flags.DoorFound = (door.Flags & Common.Bit8) != 0;
                door2.Flags.DoNotBlockLos = (door.Flags & Common.Bit9) != 0;
                door2.Flags.RemoveKey = (door.Flags & Common.Bit10) != 0;
                door2.Flags.IgnoreObstaclesWhenClosing = (door.Flags & Common.Bit11) != 0;
                door2.Flags.Bit12 = (door.Flags & Common.Bit12) != 0;
                door2.Flags.Bit13 = (door.Flags & Common.Bit13) != 0;
                door2.Flags.Bit14 = (door.Flags & Common.Bit14) != 0;
                door2.Flags.Bit15 = (door.Flags & Common.Bit15) != 0;
                door2.Flags.Bit16 = (door.Flags & Common.Bit16) != 0;
                door2.Flags.Bit17 = (door.Flags & Common.Bit17) != 0;
                door2.Flags.Bit18 = (door.Flags & Common.Bit18) != 0;
                door2.Flags.Bit19 = (door.Flags & Common.Bit19) != 0;
                door2.Flags.Bit20 = (door.Flags & Common.Bit20) != 0;
                door2.Flags.Bit21 = (door.Flags & Common.Bit21) != 0;
                door2.Flags.Bit22 = (door.Flags & Common.Bit22) != 0;
                door2.Flags.Bit23 = (door.Flags & Common.Bit23) != 0;
                door2.Flags.Bit24 = (door.Flags & Common.Bit24) != 0;
                door2.Flags.Bit25 = (door.Flags & Common.Bit25) != 0;
                door2.Flags.Bit26 = (door.Flags & Common.Bit26) != 0;
                door2.Flags.Bit27 = (door.Flags & Common.Bit27) != 0;
                door2.Flags.Bit28 = (door.Flags & Common.Bit28) != 0;
                door2.Flags.Bit29 = (door.Flags & Common.Bit29) != 0;
                door2.Flags.Bit30 = (door.Flags & Common.Bit30) != 0;
                door2.Flags.Bit31 = (door.Flags & Common.Bit31) != 0;
                door2.IsTrap = door.IsTrap;
                door2.KeyItem = door.KeyItem;
                door2.LockDifficulty = door.LockDifficulty;
                door2.LockpickString = Common.ReadString(door.LockpickString, TlkFile);
                door2.Name = door.Name;
                door2.OpenBoundingBoxBottom = door.OpenBoundingBoxBottom;
                door2.OpenBoundingBoxLeft = door.OpenBoundingBoxLeft;
                door2.OpenBoundingBoxRight = door.OpenBoundingBoxRight;
                door2.OpenBoundingBoxTop = door.OpenBoundingBoxTop;
                door2.OpenVertexBlockCount = door.OpenVertexBlockCount;//xx
                door2.OpenVertexBlockIndex = door.OpenVertexBlockIndex;//xx
                door2.OpenVertexCount = door.OpenVertexCount;//xx
                door2.OpenVertexIndex = door.OpenVertexIndex;//xx
                door2.SecretDoorDetectionDifficulty = door.SecretDoorDetectionDifficulty;
                door2.TrapDetected = door.TrapDetected;
                door2.TrapDetectionDifficulty = door.TrapDetectionDifficulty;
                door2.TrapLaunchXCoordinate = door.TrapLaunchXCoordinate;
                door2.TrapLaunchYCoordinate = door.TrapLaunchYCoordinate;
                door2.TrapRemovalDifficulty = door.TrapRemovalDifficulty;
                door2.TravelTriggerName = door.TravelTriggerName;
                door2.Unknownc0 = door.Unknownc0;
                door2.Hitpoints = door.Hitpoints;
                door2.ArmourClass = door.ArmourClass;
                areFile.doors.Add(door2);
            }

            foreach (var animation in animations)
            {
                var animation2 = new AreAnimation2();
                animation2.AnimationAppearenceSchedule.Am1 = (animation.AnimationAppearenceSchedule & Common.Bit0) != 0;
                animation2.AnimationAppearenceSchedule.Am2 = (animation.AnimationAppearenceSchedule & Common.Bit1) != 0;
                animation2.AnimationAppearenceSchedule.Am3 = (animation.AnimationAppearenceSchedule & Common.Bit2) != 0;
                animation2.AnimationAppearenceSchedule.Am4 = (animation.AnimationAppearenceSchedule & Common.Bit3) != 0;
                animation2.AnimationAppearenceSchedule.Am5 = (animation.AnimationAppearenceSchedule & Common.Bit4) != 0;
                animation2.AnimationAppearenceSchedule.Am6 = (animation.AnimationAppearenceSchedule & Common.Bit5) != 0;
                animation2.AnimationAppearenceSchedule.Am7 = (animation.AnimationAppearenceSchedule & Common.Bit6) != 0;
                animation2.AnimationAppearenceSchedule.Am8 = (animation.AnimationAppearenceSchedule & Common.Bit7) != 0;
                animation2.AnimationAppearenceSchedule.Am9 = (animation.AnimationAppearenceSchedule & Common.Bit8) != 0;
                animation2.AnimationAppearenceSchedule.Am10 = (animation.AnimationAppearenceSchedule & Common.Bit9) != 0;
                animation2.AnimationAppearenceSchedule.Am11 = (animation.AnimationAppearenceSchedule & Common.Bit10) != 0;
                animation2.AnimationAppearenceSchedule.Am12 = (animation.AnimationAppearenceSchedule & Common.Bit11) != 0;
                animation2.AnimationAppearenceSchedule.Pm1 = (animation.AnimationAppearenceSchedule & Common.Bit12) != 0;
                animation2.AnimationAppearenceSchedule.Pm2 = (animation.AnimationAppearenceSchedule & Common.Bit13) != 0;
                animation2.AnimationAppearenceSchedule.Pm3 = (animation.AnimationAppearenceSchedule & Common.Bit14) != 0;
                animation2.AnimationAppearenceSchedule.Pm4 = (animation.AnimationAppearenceSchedule & Common.Bit15) != 0;
                animation2.AnimationAppearenceSchedule.Pm5 = (animation.AnimationAppearenceSchedule & Common.Bit16) != 0;
                animation2.AnimationAppearenceSchedule.Pm6 = (animation.AnimationAppearenceSchedule & Common.Bit17) != 0;
                animation2.AnimationAppearenceSchedule.Pm7 = (animation.AnimationAppearenceSchedule & Common.Bit18) != 0;
                animation2.AnimationAppearenceSchedule.Pm8 = (animation.AnimationAppearenceSchedule & Common.Bit19) != 0;
                animation2.AnimationAppearenceSchedule.Pm9 = (animation.AnimationAppearenceSchedule & Common.Bit20) != 0;
                animation2.AnimationAppearenceSchedule.Pm10 = (animation.AnimationAppearenceSchedule & Common.Bit21) != 0;
                animation2.AnimationAppearenceSchedule.Pm11 = (animation.AnimationAppearenceSchedule & Common.Bit22) != 0;
                animation2.AnimationAppearenceSchedule.Pm12 = (animation.AnimationAppearenceSchedule & Common.Bit23) != 0;
                animation2.AnimationAppearenceSchedule.Bit24 = (animation.AnimationAppearenceSchedule & Common.Bit24) != 0;
                animation2.AnimationAppearenceSchedule.Bit25 = (animation.AnimationAppearenceSchedule & Common.Bit25) != 0;
                animation2.AnimationAppearenceSchedule.Bit26 = (animation.AnimationAppearenceSchedule & Common.Bit26) != 0;
                animation2.AnimationAppearenceSchedule.Bit27 = (animation.AnimationAppearenceSchedule & Common.Bit27) != 0;
                animation2.AnimationAppearenceSchedule.Bit28 = (animation.AnimationAppearenceSchedule & Common.Bit28) != 0;
                animation2.AnimationAppearenceSchedule.Bit29 = (animation.AnimationAppearenceSchedule & Common.Bit29) != 0;
                animation2.AnimationAppearenceSchedule.Bit30 = (animation.AnimationAppearenceSchedule & Common.Bit30) != 0;
                animation2.AnimationAppearenceSchedule.Bit31 = (animation.AnimationAppearenceSchedule & Common.Bit31) != 0;
                animation2.BamAnimation = animation.BamAnimation;
                animation2.BamFrame = animation.BamFrame;
                animation2.BamSequence = animation.BamSequence;
                animation2.Flags.Enabled = (animation.Flags & Common.Bit0) != 0;
                animation2.Flags.TransparentBlack = (animation.Flags & Common.Bit1) != 0;
                animation2.Flags.NotLightSource = (animation.Flags & Common.Bit2) != 0;
                animation2.Flags.PartialAnimation = (animation.Flags & Common.Bit3) != 0;
                animation2.Flags.SynchronizedDraw = (animation.Flags & Common.Bit4) != 0;
                animation2.Flags.RandomStartFrame = (animation.Flags & Common.Bit5) != 0;
                animation2.Flags.NotCoveredByWall = (animation.Flags & Common.Bit6) != 0;
                animation2.Flags.DisableOnSlowMachines = (animation.Flags & Common.Bit7) != 0;
                animation2.Flags.DrawAsBackground = (animation.Flags & Common.Bit8) != 0;
                animation2.Flags.PlayAllFrames = (animation.Flags & Common.Bit9) != 0;
                animation2.Flags.UsePaletteBitmap = (animation.Flags & Common.Bit10) != 0;
                animation2.Flags.MirrorYAxis = (animation.Flags & Common.Bit11) != 0;
                animation2.Flags.DoNotRemoveInCombat = (animation.Flags & Common.Bit12) != 0;
                animation2.Flags.WbmResref = (animation.Flags & Common.Bit13) != 0;
                animation2.Flags.DrawStenciled = (animation.Flags & Common.Bit14) != 0;
                animation2.Flags.PvrzResref = (animation.Flags & Common.Bit15) != 0;
                animation2.Flags.Bit16 = (animation.Flags & Common.Bit16) != 0;
                animation2.Flags.Bit17 = (animation.Flags & Common.Bit17) != 0;
                animation2.Flags.Bit18 = (animation.Flags & Common.Bit18) != 0;
                animation2.Flags.Bit19 = (animation.Flags & Common.Bit19) != 0;
                animation2.Flags.Bit20 = (animation.Flags & Common.Bit20) != 0;
                animation2.Flags.Bit21 = (animation.Flags & Common.Bit21) != 0;
                animation2.Flags.Bit22 = (animation.Flags & Common.Bit22) != 0;
                animation2.Flags.Bit23 = (animation.Flags & Common.Bit23) != 0;
                animation2.Flags.Bit24 = (animation.Flags & Common.Bit24) != 0;
                animation2.Flags.Bit25 = (animation.Flags & Common.Bit25) != 0;
                animation2.Flags.Bit26 = (animation.Flags & Common.Bit26) != 0;
                animation2.Flags.Bit27 = (animation.Flags & Common.Bit27) != 0;
                animation2.Flags.Bit28 = (animation.Flags & Common.Bit28) != 0;
                animation2.Flags.Bit29 = (animation.Flags & Common.Bit29) != 0;
                animation2.Flags.Bit30 = (animation.Flags & Common.Bit30) != 0;
                animation2.Flags.Bit31 = (animation.Flags & Common.Bit31) != 0;
                animation2.Height = animation.Height;
                animation2.LoopChance = animation.LoopChance;
                animation2.Name = animation.Name;
                animation2.Palette = animation.Palette;
                animation2.SkipCycles = animation.SkipCycles;
                animation2.StartFrame = animation.StartFrame;
                animation2.Transparency = animation.Transparency;
                animation2.XCoordinate = animation.XCoordinate;
                animation2.YCoordinate = animation.YCoordinate;
                animation2.WidthPVRZ = animation.WidthPVRZ;
                animation2.HeightPVRZ = animation.HeightPVRZ;
                areFile.animations.Add(animation2);
            }

            foreach (var note in notes)
            {
                var note2 = new AreNote2();
                note2.Colour = (NoteColor)note.Colour;
                note2.Location = note.Location;
                note2.Text = Common.ReadString(note.Text, TlkFile);
                note2.Unknown10 = note.Unknown10;
                note2.XCoordinate = note.XCoordinate;
                note2.YCoordinate = note.YCoordinate;
                areFile.notes.Add(note2);
            }

            foreach (var tiledObject in tiledObjects)
            {
                var tiledObject2 = new AreTiledObject2();
                tiledObject2.ClosedSearchCount = tiledObject.ClosedSearchCount;
                tiledObject2.ClosedSearchOffset = tiledObject.ClosedSearchOffset;
                tiledObject2.Name = tiledObject.Name;
                tiledObject2.OpenSearchCount = tiledObject.OpenSearchCount;
                tiledObject2.OpenSearchOffset = tiledObject.OpenSearchOffset;
                tiledObject2.Unknown3c = tiledObject.Unknown3c;
                tiledObject2.Flags.InSecondaryState = (tiledObject.Flags & Common.Bit0) != 0;
                tiledObject2.Flags.CanBeSeenThrough = (tiledObject.Flags & Common.Bit1) != 0;
                tiledObject2.Flags.Bit2 = (tiledObject.Flags & Common.Bit2) != 0;
                tiledObject2.Flags.Bit3 = (tiledObject.Flags & Common.Bit3) != 0;
                tiledObject2.Flags.Bit4 = (tiledObject.Flags & Common.Bit4) != 0;
                tiledObject2.Flags.Bit5 = (tiledObject.Flags & Common.Bit5) != 0;
                tiledObject2.Flags.Bit6 = (tiledObject.Flags & Common.Bit6) != 0;
                tiledObject2.Flags.Bit7 = (tiledObject.Flags & Common.Bit7) != 0;
                tiledObject2.Flags.Bit8 = (tiledObject.Flags & Common.Bit8) != 0;
                tiledObject2.Flags.Bit9 = (tiledObject.Flags & Common.Bit9) != 0;
                tiledObject2.Flags.Bit10 = (tiledObject.Flags & Common.Bit10) != 0;
                tiledObject2.Flags.Bit11 = (tiledObject.Flags & Common.Bit11) != 0;
                tiledObject2.Flags.Bit12 = (tiledObject.Flags & Common.Bit12) != 0;
                tiledObject2.Flags.Bit13 = (tiledObject.Flags & Common.Bit13) != 0;
                tiledObject2.Flags.Bit14 = (tiledObject.Flags & Common.Bit14) != 0;
                tiledObject2.Flags.Bit15 = (tiledObject.Flags & Common.Bit15) != 0;
                tiledObject2.Flags.Bit16 = (tiledObject.Flags & Common.Bit16) != 0;
                tiledObject2.Flags.Bit17 = (tiledObject.Flags & Common.Bit17) != 0;
                tiledObject2.Flags.Bit18 = (tiledObject.Flags & Common.Bit18) != 0;
                tiledObject2.Flags.Bit19 = (tiledObject.Flags & Common.Bit19) != 0;
                tiledObject2.Flags.Bit20 = (tiledObject.Flags & Common.Bit20) != 0;
                tiledObject2.Flags.Bit21 = (tiledObject.Flags & Common.Bit21) != 0;
                tiledObject2.Flags.Bit22 = (tiledObject.Flags & Common.Bit22) != 0;
                tiledObject2.Flags.Bit23 = (tiledObject.Flags & Common.Bit23) != 0;
                tiledObject2.Flags.Bit24 = (tiledObject.Flags & Common.Bit24) != 0;
                tiledObject2.Flags.Bit25 = (tiledObject.Flags & Common.Bit25) != 0;
                tiledObject2.Flags.Bit26 = (tiledObject.Flags & Common.Bit26) != 0;
                tiledObject2.Flags.Bit27 = (tiledObject.Flags & Common.Bit27) != 0;
                tiledObject2.Flags.Bit28 = (tiledObject.Flags & Common.Bit28) != 0;
                tiledObject2.Flags.Bit29 = (tiledObject.Flags & Common.Bit29) != 0;
                tiledObject2.Flags.Bit30 = (tiledObject.Flags & Common.Bit30) != 0;
                tiledObject2.Flags.Bit31 = (tiledObject.Flags & Common.Bit31) != 0;
                tiledObject2.TileId = tiledObject.TileId;
                areFile.tiledObjects.Add(tiledObject2);
            }

            foreach (var projectile in projectiles)
            {
                var projectile2 = new AreProjectile2();
                projectile2.EATarget = projectile.EATarget;
                projectile2.EffectOffset = projectile.EffectOffset;
                projectile2.EffectSize = projectile.EffectSize;
                projectile2.MissileId = projectile.MissileId;
                projectile2.PartyOwnerIndex = projectile.PartyOwnerIndex;
                projectile2.Resref = projectile.Resref;
                projectile2.TickUntilTriggerCheck = projectile.TickUntilTriggerCheck;
                projectile2.TriggersRemaining = projectile.TriggersRemaining;
                projectile2.XCoordinate = projectile.XCoordinate;
                projectile2.YCoordinate = projectile.YCoordinate;
                projectile2.ZCoordinate = projectile.ZCoordinate;
                areFile.projectiles.Add(projectile2);
            }

            foreach (var song in songs)
            {
                var song2 = new AreSong2();
                song2.BattleSong = song.BattleSong;
                song2.DayAmbient1Wav = song.DayAmbient1Wav;
                song2.DayAmbient2Wav = song.DayAmbient2Wav;
                song2.DayAmbientVolume = song.DayAmbientVolume;
                song2.DaySong = song.DaySong;
                song2.LoseSong = song.LoseSong;
                song2.NightAmbient1Wav = song.NightAmbient1Wav;
                song2.NightAmbient2Wav = song.NightAmbient2Wav;
                song2.NightAmbientVolume = song.NightAmbientVolume;
                song2.NightSong = song.NightSong;
                song2.NightSong = song.Reverb;
                song2.AltMusic1 = song.AltMusic1;
                song2.AltMusic2 = song.AltMusic2;
                song2.AltMusic3 = song.AltMusic3;
                song2.AltMusic4 = song.AltMusic4;
                song2.AltMusic5 = song.AltMusic5;
                song2.Unknown54 = song.Unknown54;
                song2.WinSong = song.WinSong;
                areFile.songs.Add(song2);
            }

            foreach (var interruption in interruptions)
            {
                var interruption2 = new AreInterruption2();
                interruption2.CreatureCount = interruption.CreatureCount;
                interruption2.DayProbability = interruption.DayProbability;
                interruption2.Difficulty = interruption.Difficulty;
                interruption2.Enabled = interruption.Enabled;
                interruption2.MaximumCreaturesToSpawn = interruption.MaximumCreaturesToSpawn;
                interruption2.Name = interruption.Name;
                interruption2.NightProbability = interruption.NightProbability;
                interruption2.RemovalTime = interruption.RemovalTime;
                interruption2.ResRef1 = interruption.ResRef1;
                interruption2.ResRef2 = interruption.ResRef2;
                interruption2.ResRef3 = interruption.ResRef3;
                interruption2.ResRef4 = interruption.ResRef4;
                interruption2.ResRef5 = interruption.ResRef5;
                interruption2.ResRef6 = interruption.ResRef6;
                interruption2.ResRef7 = interruption.ResRef7;
                interruption2.ResRef8 = interruption.ResRef8;
                interruption2.ResRef9 = interruption.ResRef9;
                interruption2.ResRef10 = interruption.ResRef10;
                interruption2.RestrictionDistance = interruption.RestrictionDistance;
                interruption2.RestrictionDistanceToObject = interruption.RestrictionDistanceToObject;                
                interruption2.Text1 = Common.ReadString(interruption.Text1, TlkFile);
                interruption2.Text2 = Common.ReadString(interruption.Text2, TlkFile);
                interruption2.Text3 = Common.ReadString(interruption.Text3, TlkFile);
                interruption2.Text4 = Common.ReadString(interruption.Text4, TlkFile);
                interruption2.Text5 = Common.ReadString(interruption.Text5, TlkFile);
                interruption2.Text6 = Common.ReadString(interruption.Text6, TlkFile);
                interruption2.Text7 = Common.ReadString(interruption.Text7, TlkFile);
                interruption2.Text8 = Common.ReadString(interruption.Text8, TlkFile);
                interruption2.Text9 = Common.ReadString(interruption.Text9, TlkFile);
                interruption2.Text10 = Common.ReadString(interruption.Text10, TlkFile);
                interruption2.Unknownac = interruption.Unknownac;
                areFile.interruptions.Add(interruption2);
            }

            foreach (var exploration in exploredArea)
            {
                areFile.exploration.Add(exploration);
            }

            foreach (var vertex in vertices)
            {
                areFile.vertices.Add(vertex);
            }

            areFile.Checksum = HashGenerator.GenerateKey(areFile);
            return areFile;
        }
    }
}