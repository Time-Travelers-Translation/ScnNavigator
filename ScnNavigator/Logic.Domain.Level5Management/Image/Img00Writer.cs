using System.Collections.Generic;
using System;
using System.IO;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Kuriimu2.KryptographyAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Cryptography;
using System.Text;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Enums;
using Logic.Domain.Level5Management.Contract.Enums.Compression;
using Logic.Domain.Level5Management.InternalContract.Compression;
using Logic.Domain.Level5Management.DataClasses.Image;
using Logic.Domain.Level5Management.InternalContract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class Img00Writer : IImg00Writer
    {
        private readonly IChecksum<uint> _crc32;

        private readonly IBinaryFactory _binaryFactory;
        private readonly ICompressor _compressor;

        public Img00Writer(IBinaryFactory binaryFactory, IChecksumFactory checksumFactory, ICompressor compressor)
        {
            _crc32 = checksumFactory.CreateCrc32();

            _binaryFactory = binaryFactory;
            _compressor = compressor;
        }

        public void Write(RawImageData data, Stream output)
        {
            (int paddedWidth, int paddedHeight) = ((data.Width + 7) & ~7, (data.Height + 7) & ~7);
            int bitDepth = data.Data.Length * 8 / (paddedWidth * paddedHeight);

            // Create tiles
            (Stream dataStream, Stream tileStream) = SplitTiles(data.Data, data.LegacyData, bitDepth);

            // Compress palette data
            Stream? paletteStream = null;
            if (data.PaletteData != null)
                paletteStream = _compressor.Compress(new MemoryStream(data.PaletteData), CompressionType.Huffman4Bit);

            // Compress tile data
            tileStream = _compressor.Compress(tileStream, CompressionType.Huffman4Bit);

            // Compress image data
            dataStream = _compressor.Compress(dataStream, CompressionType.Lz10);

            // Create palette entries
            Img00PaletteEntry[] paletteEntries = CreatePaletteEntries(data, paletteStream);

            // Create image entries
            Img00ImageEntry[] imageEntries = CreateImageEntries(paletteEntries, tileStream, dataStream);

            // Create header
            Img00Header header = CreateHeader(data, paletteEntries, imageEntries);

            // Write header data
            WriteHeaderEntries(header, paletteEntries, imageEntries, output);

            // Write data
            WriteData(header, paletteStream, tileStream, dataStream, output);
        }

        private (Stream imageData, Stream tileData) SplitTiles(byte[] imageData, byte[]? legacyData, int bitDepth)
        {
            int tileByteDepth = 64 * bitDepth / 8;

            var tileStream = new MemoryStream();
            var dataStream = new MemoryStream();

            using IBinaryWriterX tileWriter = _binaryFactory.CreateWriter(tileStream, true);
            using IBinaryWriterX dataWriter = _binaryFactory.CreateWriter(dataStream, true);

            if (legacyData != null)
                tileStream.Write(legacyData, 0, legacyData.Length);

            var tileDictionary = new Dictionary<uint, int>();

            // Add placeholder tile for all 0's
            // TODO: Use Span<byte> overload
            uint zeroTileHash = _crc32.ComputeValue(new MemoryStream(new byte[tileByteDepth]));
            tileDictionary[zeroTileHash] = -1;

            var imageOffset = 0;
            var tileIndex = 0;
            while (imageOffset < imageData.Length)
            {
                Span<byte> tileData = imageData.AsSpan(imageOffset, tileByteDepth);

                // TODO: Use Span<byte> overload
                uint tileHash = _crc32.ComputeValue(new MemoryStream(tileData.ToArray()));
                if (!tileDictionary.ContainsKey(tileHash))
                {
                    // TODO: Use Span<byte> overload
                    dataWriter.Write(tileData.ToArray());

                    tileDictionary[tileHash] = tileIndex++;
                }

                if (legacyData != null)
                    tileWriter.Write(tileDictionary[tileHash]);
                else
                    tileWriter.Write((short)tileDictionary[tileHash]);

                imageOffset += tileByteDepth;
            }

            dataStream.Position = tileStream.Position = 0;
            return (dataStream, tileStream);
        }

        private Img00PaletteEntry[] CreatePaletteEntries(RawImageData imageData, Stream? paletteStream)
        {
            if (imageData.PaletteFormat < 0 || imageData.PaletteData == null || paletteStream == null)
                return Array.Empty<Img00PaletteEntry>();

            return new[]
            {
                new Img00PaletteEntry
                {
                    offset = 0,
                    size = (int)paletteStream.Length,
                    colorCount = (short)(imageData.PaletteData.Length * 8 / imageData.PaletteBitDepth),
                    const0 = 1,
                    format = (byte)imageData.PaletteFormat
                }
            };
        }

        private Img00ImageEntry[] CreateImageEntries(Img00PaletteEntry[] paletteEntries, Stream tileStream, Stream imageStream)
        {
            int tileOffset = paletteEntries.Length <= 0 ? 0 : paletteEntries[^1].offset + paletteEntries[^1].size;
            tileOffset = (tileOffset + 3) & ~3;

            return new[]
            {
                new Img00ImageEntry
                {
                    tileOffset = tileOffset,
                    tileSize = (int)tileStream.Length,
                    dataOffset = (int)((tileOffset + tileStream.Length + 3) & ~3),
                    dataSize = (int)imageStream.Length
                }
            };
        }

        private Img00Header CreateHeader(RawImageData imageData, Img00PaletteEntry[] paletteEntries, Img00ImageEntry[] imageEntries)
        {
            return new Img00Header
            {
                magic = GetMagic(imageData),
                entryOffset = 0x30,
                imageFormat = (byte)imageData.Format,
                const1 = 1,
                imageCount = (byte)(imageData.MipMapData.Length + 1),
                bitDepth = (byte)imageData.BitDepth,
                bytesPerTile = (short)(64 * imageData.BitDepth / 8),
                width = (short)imageData.Width,
                height = (short)imageData.Height,
                paletteInfoOffset = 0x30,
                paletteInfoCount = (ushort)paletteEntries.Length,
                imageInfoOffset = (ushort)(0x30 + paletteEntries.Length * 0x10),
                imageInfoCount = (ushort)imageEntries.Length,
                dataOffset = (ushort)(0x30 + paletteEntries.Length * 0x10 + imageEntries.Length * 0x18),
                platform = GetPlatformVersion(imageData.Version.Platform)
            };
        }

        private void WriteHeaderEntries(Img00Header header, Img00PaletteEntry[] paletteEntries, Img00ImageEntry[] imageEntries, Stream output)
        {
            output.Position = 0;
            WriteHeader(header, output);

            output.Position = header.paletteInfoOffset;
            WritePaletteEntries(paletteEntries, output);

            output.Position = header.imageInfoOffset;
            WriteImageEntries(imageEntries, output);
        }

        private void WriteHeader(Img00Header header, Stream output)
        {
            using IBinaryWriterX writer = _binaryFactory.CreateWriter(output, true);

            writer.WriteString(header.magic, Encoding.ASCII, false, false);
            writer.Write(header.entryOffset);
            writer.Write(header.imageFormat);
            writer.Write(header.const1);
            writer.Write(header.imageCount);
            writer.Write(header.bitDepth);
            writer.Write(header.bytesPerTile);
            writer.Write(header.width);
            writer.Write(header.height);
            writer.Write(header.paletteInfoOffset);
            writer.Write(header.paletteInfoCount);
            writer.Write(header.imageInfoOffset);
            writer.Write(header.imageInfoCount);
            writer.Write(header.dataOffset);
            writer.Write(header.platform);
        }

        private void WritePaletteEntries(Img00PaletteEntry[] paletteEntries, Stream output)
        {
            foreach (Img00PaletteEntry paletteEntry in paletteEntries)
            {
                WritePaletteEntry(paletteEntry, output);
                output.Position += 4;
            }
        }

        private void WritePaletteEntry(Img00PaletteEntry paletteEntry, Stream output)
        {
            using IBinaryWriterX writer = _binaryFactory.CreateWriter(output, true);

            writer.Write(paletteEntry.offset);
            writer.Write(paletteEntry.size);
            writer.Write(paletteEntry.colorCount);
            writer.Write(paletteEntry.const0);
            writer.Write(paletteEntry.format);
        }

        private void WriteImageEntries(Img00ImageEntry[] imageEntries, Stream output)
        {
            foreach (Img00ImageEntry imageEntry in imageEntries)
            {
                WriteImageEntry(imageEntry, output);
                output.Position += 8;
            }
        }

        private void WriteImageEntry(Img00ImageEntry imageEntry, Stream output)
        {
            using IBinaryWriterX writer = _binaryFactory.CreateWriter(output, true);

            writer.Write(imageEntry.tileOffset);
            writer.Write(imageEntry.tileSize);
            writer.Write(imageEntry.dataOffset);
            writer.Write(imageEntry.dataSize);
        }

        private void WriteData(Img00Header header, Stream? paletteStream, Stream tileStream, Stream dataStream, Stream output)
        {
            using IBinaryWriterX writer = _binaryFactory.CreateWriter(output, true);

            output.Position = header.dataOffset;

            if (paletteStream != null)
            {
                paletteStream.CopyTo(output);
                writer.WriteAlignment(4);
            }

            tileStream.CopyTo(output);
            writer.WriteAlignment(4);

            dataStream.CopyTo(output);
            writer.WriteAlignment(4);
        }

        private string GetMagic(RawImageData image)
        {
            char identifier = GetPlatformIdentifier(image.Version.Platform);
            string version = GetPlatformIdentifier(image.Version.Version);

            return $"IMG{identifier}{version}\0\0";
        }

        private int GetPlatformVersion(PlatformType platform)
        {
            switch (platform)
            {
                case PlatformType.Ctr:
                    return 3;

                case PlatformType.Psp:
                    return 1;

                case PlatformType.PsVita:
                    return 0;

                default:
                    throw new InvalidOperationException($"Unknown platform {platform} for image.");
            }
        }

        private char GetPlatformIdentifier(PlatformType platform)
        {
            switch (platform)
            {
                case PlatformType.Ctr:
                    return 'C';

                case PlatformType.Psp:
                    return 'P';

                case PlatformType.PsVita:
                    return 'V';

                default:
                    throw new InvalidOperationException($"Unknown platform {platform} for image.");
            }
        }

        private string GetPlatformIdentifier(int version)
        {
            return $"{version:00}";
        }
    }
}
