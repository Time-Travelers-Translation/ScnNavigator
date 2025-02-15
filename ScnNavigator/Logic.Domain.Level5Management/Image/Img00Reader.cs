using System;
using System.IO;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Enums;
using Logic.Domain.Level5Management.DataClasses.Image;
using Logic.Domain.Level5Management.InternalContract.Compression;
using Logic.Domain.Level5Management.InternalContract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class Img00Reader : IImg00Reader
    {
        private readonly IDecompressor _decompressor;
        private readonly IBinaryFactory _binaryFactory;
        private readonly IStreamFactory _streamFactory;

        public Img00Reader(IDecompressor decompressor, IBinaryFactory binaryFactory, IStreamFactory streamFactory)
        {
            _decompressor = decompressor;
            _binaryFactory = binaryFactory;
            _streamFactory = streamFactory;
        }

        public RawImageData Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, false);

            // Header
            Img00Header header = ReadHeader(br);

            // Palette entries
            input.Position = header.paletteInfoOffset;
            Img00PaletteEntry[] paletteEntries = ReadPaletteEntries(br, header.paletteInfoCount);

            // Image entries
            input.Position = header.imageInfoOffset;
            Img00ImageEntry[] imageEntries = ReadImageEntries(br, header.imageInfoCount);

            // Get palette
            byte[]? paletteData = null;
            if (paletteEntries.Length > 0)
                paletteData = DecompressPalette(br, header, paletteEntries[0]);

            // Get tile table
            Stream tileDataStream = DecompressTiles(br, header, imageEntries[0]);

            // Get image data
            Stream imageDataStream = DecompressImageData(br, header, imageEntries[0]);

            // Combine tiles to full image data
            (byte[] imageData, byte[]? legacyData) = CombineTiles(tileDataStream, imageDataStream, header.bitDepth);

            // Split image data and mip map data
            var images = new byte[header.imageCount][];

            var dataOffset = 0;
            (int width, int height) = ((header.width + 7) & ~7, (header.height + 7) & ~7);
            for (var i = 0; i < header.imageCount; i++)
            {
                images[i] = new byte[width * height * header.bitDepth / 8];
                Array.Copy(imageData, dataOffset, images[i], 0, images[i].Length);

                (width, height) = (width >> 1, height >> 1);
                dataOffset += images[i].Length;
            }

            var rawImageData = new RawImageData
            {
                Version = new FormatVersion
                {
                    Platform = GetPlatform(header),
                    Version = GetVersion(header)
                },

                BitDepth = header.bitDepth,
                Format = header.imageFormat,

                Width = header.width,
                Height = header.height,

                LegacyData = legacyData,

                Data = images[0],
                MipMapData = images[1..],
                PaletteData = paletteData
            };

            if (paletteData != null)
            {
                rawImageData.PaletteBitDepth = paletteData.Length / paletteEntries[0].colorCount * 8;
                rawImageData.PaletteFormat = paletteEntries[0].format;
                rawImageData.PaletteData = paletteData;
            }

            return rawImageData;
        }

        private Img00Header ReadHeader(IBinaryReaderX br)
        {
            return new Img00Header
            {
                magic = br.ReadString(8),
                entryOffset = br.ReadInt16(),
                imageFormat = br.ReadByte(),
                const1 = br.ReadByte(),
                imageCount = br.ReadByte(),
                bitDepth = br.ReadByte(),
                bytesPerTile = br.ReadInt16(),
                width = br.ReadInt16(),
                height = br.ReadInt16(),
                paletteInfoOffset = br.ReadUInt16(),
                paletteInfoCount = br.ReadUInt16(),
                imageInfoOffset = br.ReadUInt16(),
                imageInfoCount = br.ReadUInt16(),
                dataOffset = br.ReadInt32(),
                platform = br.ReadInt32()
            };
        }

        private Img00PaletteEntry[] ReadPaletteEntries(IBinaryReaderX br, int count)
        {
            var result = new Img00PaletteEntry[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = ReadPaletteEntry(br);
                br.BaseStream.Position += 4;
            }

            return result;
        }

        private Img00PaletteEntry ReadPaletteEntry(IBinaryReaderX br)
        {
            return new Img00PaletteEntry
            {
                offset = br.ReadInt32(),
                size = br.ReadInt32(),
                colorCount = br.ReadInt16(),
                const0 = br.ReadByte(),
                format = br.ReadByte()
            };
        }

        private Img00ImageEntry[] ReadImageEntries(IBinaryReaderX br, int count)
        {
            var result = new Img00ImageEntry[count];

            for (var i = 0; i < count; i++)
            {
                result[i] = ReadImageEntry(br);
                br.BaseStream.Position += 8;
            }

            return result;
        }

        private Img00ImageEntry ReadImageEntry(IBinaryReaderX br)
        {
            return new Img00ImageEntry
            {
                tileOffset = br.ReadInt32(),
                tileSize = br.ReadInt32(),
                dataOffset = br.ReadInt32(),
                dataSize = br.ReadInt32()
            };
        }

        private byte[] DecompressPalette(IBinaryReaderX br, Img00Header header, Img00PaletteEntry paletteEntry)
        {
            using Stream compressedPaletteStream = _streamFactory.CreateSubStream(br.BaseStream, header.dataOffset + paletteEntry.offset, paletteEntry.size);
            using var output = new MemoryStream();

            _decompressor.Decompress(compressedPaletteStream, output, 0);

            return output.ToArray();
        }

        private Stream DecompressTiles(IBinaryReaderX br, Img00Header header, Img00ImageEntry imageEntry)
        {
            using Stream compressedTileStream = _streamFactory.CreateSubStream(br.BaseStream, header.dataOffset + imageEntry.tileOffset, imageEntry.tileSize);
            return _decompressor.Decompress(compressedTileStream, 0);
        }

        private Stream DecompressImageData(IBinaryReaderX br, Img00Header header, Img00ImageEntry imageEntry)
        {
            using Stream compressedImageData = _streamFactory.CreateSubStream(br.BaseStream, header.dataOffset + imageEntry.dataOffset, imageEntry.dataSize);
            return _decompressor.Decompress(compressedImageData, 0);
        }

        private (byte[], byte[]?) CombineTiles(Stream tileDataStream, Stream imageDataStream, int bitDepth)
        {
            using IBinaryReaderX tileDataReader = _binaryFactory.CreateReader(tileDataStream, true);

            int tileByteDepth = 64 * bitDepth / 8;
            var entryByteLength = 2;
            var readEntry = new Func<IBinaryReaderX, int>(br => br.ReadInt16());

            // Read legacy head
            byte[]? tileLegacy = null;

            ushort legacyIndicator = tileDataReader.PeekUInt16();
            if (legacyIndicator == 0x453)
            {
                tileLegacy = tileDataReader.ReadBytes(8);
                entryByteLength = 4;
                readEntry = br => br.ReadInt32();
            }

            long tileEntryCount = tileDataStream.Length / entryByteLength;
            var result = new byte[tileEntryCount * tileByteDepth];

            var tile = new byte[tileByteDepth];
            for (var i = 0; i < tileEntryCount; i++)
            {
                int entry = readEntry(tileDataReader);
                if (entry < 0)
                    continue;

                imageDataStream.Position = entry * tileByteDepth;
                _ = imageDataStream.Read(tile, 0, tileByteDepth);

                Array.Copy(tile, 0, result, i * tileByteDepth, tileByteDepth);
            }

            return (result, tileLegacy);
        }

        private PlatformType GetPlatform(Img00Header header)
        {
            switch (header.magic[3])
            {
                case 'C':
                    return PlatformType.Ctr;

                case 'P':
                    return PlatformType.Psp;

                case 'V':
                    return PlatformType.PsVita;

                default:
                    throw new InvalidOperationException($"Unknown platform identifier '{header.magic[3]}' in image.");
            }
        }

        private int GetVersion(Img00Header header)
        {
            return int.Parse(header.magic[4..6]);
        }
    }
}
