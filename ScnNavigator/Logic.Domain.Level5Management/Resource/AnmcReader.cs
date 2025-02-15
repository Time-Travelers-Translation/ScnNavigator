using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;
using Logic.Domain.Level5Management.Contract.Resource;
using Logic.Domain.Level5Management.DataClasses.Resource;

namespace Logic.Domain.Level5Management.Resource
{
    internal class AnmcReader : IAnmcReader
    {
        private readonly IBinaryFactory _binaryFactory;
        private readonly IStreamFactory _streamFactory;

        public AnmcReader(IBinaryFactory binaryFactory, IStreamFactory streamFactory)
        {
            _binaryFactory = binaryFactory;
            _streamFactory = streamFactory;
        }

        public AnmcData Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, true);

            // Read header
            ResHeader header = ReadHeader(br);

            // Read table entries
            input.Position = header.imageTablesOffset << 2;
            ResTableEntry[] imageTableEntries = ReadTableEntries(br, header.imageTablesCount);
            input.Position = header.layoutDataTablesOffset << 2;
            ResTableEntry[] clusterTableEntries = ReadTableEntries(br, header.layoutDataTablesCount);

            // Read image entries
            input.Position = imageTableEntries[0].offset << 2;
            ResImageEntry[] imageEntries = ReadImageEntries(br, imageTableEntries[0].entryCount);
            input.Position = imageTableEntries[1].offset << 2;
            ResImageArea[] imageAreas = ReadImageAreas(br, imageTableEntries[1].entryCount);

            // Read layout data entries
            input.Position = clusterTableEntries[0].offset << 2;
            ResRootEntry[] rootEntries = ReadRootEntries(br, clusterTableEntries[0].entryCount);
            input.Position = clusterTableEntries[1].offset << 2;
            ResCompositeEntry[] compositeEntries = ReadCompositeEntries(br, clusterTableEntries[1].entryCount);
            input.Position = clusterTableEntries[2].offset << 2;
            uint[] hashes = ReadHashes(br, clusterTableEntries[2].entryCount);
            input.Position = clusterTableEntries[3].offset << 2;
            ResDimensionEntry[] pointIndexEntries = ReadDimensionEntries(br, clusterTableEntries[3].entryCount);
            input.Position = clusterTableEntries[4].offset << 2;
            ResUnk1Entry[] unk1Entries = ReadUnk1Entries(br, clusterTableEntries[4].entryCount);
            input.Position = clusterTableEntries[5].offset << 2;
            ResUnk2Entry[] unk2Entries = ReadUnk2Entries(br, clusterTableEntries[5].entryCount);

            long stringOffset = header.stringTablesOffset << 2;
            Stream stringStream = _streamFactory.CreateSubStream(input, stringOffset);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding("Shift-JIS");

            using IBinaryReaderX stringReader = _binaryFactory.CreateReader(stringStream, sjis, true);

            var result = new AnmcData
            {
                ResourceInfos = new Dictionary<string, AnmcIndexData>()
            };

            for (var compositeEntryIndex = 0; compositeEntryIndex < compositeEntries.Length; compositeEntryIndex++)
            {
                stringStream.Position = compositeEntries[compositeEntryIndex].stringPtr.offset;
                string pointIndexName = stringReader.ReadNullTerminatedString();

                uint imageAreaHash = compositeEntries[compositeEntryIndex].imageAreaHash;
                ResImageArea imageArea = imageAreas.First(x => x.stringPtr.crc32 == imageAreaHash);

                uint imageEntryHash = imageArea.imageEntryHash;
                ResImageEntry imageEntry = imageEntries.First(x => x.stringPtr.crc32 == imageEntryHash);

                int imageEntryIndex = Array.IndexOf(imageEntries, imageEntry);

                result.ResourceInfos[pointIndexName] = new AnmcIndexData
                {
                    ImageIndex = imageEntryIndex,
                    PointVectorIndex = compositeEntryIndex
                };
            }

            return result;
        }

        private ResHeader ReadHeader(IBinaryReaderX reader)
        {
            return new ResHeader
            {
                magic = reader.ReadString(8),

                stringTablesOffset = reader.ReadInt16(),
                stringTablesCount = reader.ReadInt16(),

                imageTablesOffset = reader.ReadInt16(),
                imageTablesCount = reader.ReadInt16(),

                layoutDataTablesOffset = reader.ReadInt16(),
                layoutDataTablesCount = reader.ReadInt16(),
            };
        }

        private ResTableEntry[] ReadTableEntries(IBinaryReaderX reader, int count)
        {
            var result = new ResTableEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadTableEntry(reader);

            return result;
        }

        private ResTableEntry ReadTableEntry(IBinaryReaderX reader)
        {
            return new ResTableEntry
            {
                offset = reader.ReadInt16(),
                entryCount = reader.ReadInt16(),
                unk1 = reader.ReadInt16(),
                entrySize = reader.ReadInt16()
            };
        }

        private ResImageEntry[] ReadImageEntries(IBinaryReaderX reader, int count)
        {
            var result = new ResImageEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadImageEntry(reader);

            return result;
        }

        private ResImageEntry ReadImageEntry(IBinaryReaderX reader)
        {
            return new ResImageEntry
            {
                stringPtr = ReadStringPointer(reader),
                unk1 = reader.ReadInt32(),
                unk2 = reader.ReadInt32(),
                unk3 = reader.ReadInt32()
            };
        }

        private ResImageArea[] ReadImageAreas(IBinaryReaderX reader, int count)
        {
            var result = new ResImageArea[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadImageArea(reader);

            return result;
        }

        private ResImageArea ReadImageArea(IBinaryReaderX reader)
        {
            return new ResImageArea
            {
                stringPtr = ReadStringPointer(reader),
                unk1 = reader.ReadInt32(),
                unk2 = reader.ReadUInt32(),
                imageEntryHash = reader.ReadUInt32(),
                unk3 = reader.ReadBytes(0xCC)
            };
        }

        private ResRootEntry[] ReadRootEntries(IBinaryReaderX reader, int count)
        {
            var result = new ResRootEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadRootEntry(reader);

            return result;
        }

        private ResRootEntry ReadRootEntry(IBinaryReaderX reader)
        {
            return new ResRootEntry
            {
                stringPtr = ReadStringPointer(reader)
            };
        }

        private ResCompositeEntry[] ReadCompositeEntries(IBinaryReaderX reader, int count)
        {
            var result = new ResCompositeEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadCompositeEntry(reader);

            return result;
        }

        private uint[] ReadHashes(IBinaryReaderX reader, int count)
        {
            var result = new uint[count];

            for (var i = 0; i < count; i++)
                result[i] = reader.ReadUInt32();

            return result;
        }

        private ResCompositeEntry ReadCompositeEntry(IBinaryReaderX reader)
        {
            return new ResCompositeEntry
            {
                stringPtr = ReadStringPointer(reader),
                unk1 = reader.ReadUInt32(),
                imageAreaHash = reader.ReadUInt32(),
                rootHash = reader.ReadUInt32(),
                unk4 = reader.ReadUInt32(),
                origin = new PointF(reader.ReadSingle(), reader.ReadSingle()),
                size = new SizeF(reader.ReadSingle(), reader.ReadSingle()),
                unk5 = reader.ReadUInt32(),
                unk6 = reader.ReadUInt32(),
                unk7 = reader.ReadUInt32(),
                unk8 = reader.ReadUInt32(),
                unk9 = reader.ReadUInt32()
            };
        }

        private ResDimensionEntry[] ReadDimensionEntries(IBinaryReaderX reader, int count)
        {
            var result = new ResDimensionEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadDimensionEntry(reader);

            return result;
        }

        private ResDimensionEntry ReadDimensionEntry(IBinaryReaderX reader)
        {
            return new ResDimensionEntry
            {
                stringPtr = ReadStringPointer(reader),
                unk1 = reader.ReadSingle(),
                origin = new PointF(reader.ReadSingle(), reader.ReadSingle()),
                size = new SizeF(reader.ReadSingle(), reader.ReadSingle()),
                unk2 = reader.ReadInt32()
            };
        }

        private ResUnk1Entry[] ReadUnk1Entries(IBinaryReaderX reader, int count)
        {
            var result = new ResUnk1Entry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadUnk1Entry(reader);

            return result;
        }

        private ResUnk1Entry ReadUnk1Entry(IBinaryReaderX reader)
        {
            return new ResUnk1Entry
            {
                parentHash = reader.ReadUInt32(),
                unk1 = reader.ReadInt32(),
                unk2 = reader.ReadInt32(),
                unk3 = reader.ReadInt32(),
                unk4 = reader.ReadInt32()
            };
        }

        private ResUnk2Entry[] ReadUnk2Entries(IBinaryReaderX reader, int count)
        {
            var result = new ResUnk2Entry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadUnk2Entry(reader);

            return result;
        }

        private ResUnk2Entry ReadUnk2Entry(IBinaryReaderX reader)
        {
            return new ResUnk2Entry
            {
                stringPtr = ReadStringPointer(reader),
                unk1 = reader.ReadSingle(),
                unk2 = reader.ReadSingle(),
                unk3 = reader.ReadSingle(),
                unk4 = reader.ReadSingle(),
                unk5 = reader.ReadSingle(),
                unk6 = reader.ReadSingle(),
                unk7 = reader.ReadSingle(),
                unk8 = reader.ReadSingle(),
                unk9 = reader.ReadSingle(),
                unk10 = reader.ReadInt32()
            };
        }

        private ResStringPointer ReadStringPointer(IBinaryReaderX reader)
        {
            return new ResStringPointer
            {
                crc32 = reader.ReadUInt32(),
                offset = reader.ReadInt16(),
                unk1 = reader.ReadInt16()
            };
        }
    }
}
