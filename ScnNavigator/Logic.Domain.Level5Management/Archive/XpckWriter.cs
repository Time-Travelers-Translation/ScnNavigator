using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Kuriimu2.KryptographyAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.Cryptography;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.Contract.Enums.Compression;
using Logic.Domain.Level5Management.DataClasses.Archive;
using Logic.Domain.Level5Management.InternalContract.Compression;

namespace Logic.Domain.Level5Management.Archive
{
    internal class XpckWriter : IXpckWriter
    {
        private readonly IBinaryFactory _binaryFactory;
        private readonly ICompressor _compressor;
        private readonly IChecksum<uint> _crc32;

        private const int HeaderSize_ = 0x14;
        private const int EntrySize_ = 0xC;

        public XpckWriter(IBinaryFactory binaryFactory, ICompressor compressor, IChecksumFactory checksumFactory)
        {
            _binaryFactory = binaryFactory;
            _compressor = compressor;
            _crc32 = checksumFactory.CreateCrc32();
        }

        public void Write(ArchiveData archiveData, Stream output)
        {
            using IBinaryWriterX bw = _binaryFactory.CreateWriter(output, true);

            // Write strings
            IDictionary<string, long> stringOffsets = CacheStrings(archiveData.Files);
            Stream stringStream = WriteStrings(stringOffsets);

            long stringOffset = HeaderSize_ + archiveData.Files.Count * EntrySize_;

            output.Position = stringOffset;
            stringStream.CopyTo(output);

            // Write files
            long dataOffset = (stringOffset + stringStream.Length + 3) & ~3;

            IList<XpckEntry> fileEntries = WriteFiles(archiveData.Files, output, dataOffset, stringOffsets);

            // Write entries
            output.Position = HeaderSize_;
            WriteEntries(fileEntries, bw);

            // Write header
            XpckHeader header = CreateHeader(fileEntries, archiveData.Type, dataOffset, dataOffset - stringOffset, output.Length - dataOffset);

            output.Position = 0;
            WriteHeader(header, bw);

            output.Position = 0;
        }

        private IDictionary<string, long> CacheStrings(IList<NamedArchiveEntry> entries)
        {
            var result = new Dictionary<string, long>();

            var offset = 0;
            foreach (NamedArchiveEntry entry in entries)
            {
                if (result.ContainsKey(entry.Name))
                    continue;

                result[entry.Name] = offset;
                offset += Encoding.ASCII.GetByteCount(entry.Name) + 1;
            }

            return result;
        }

        private Stream WriteStrings(IDictionary<string, long> stringOffsets)
        {
            var result = new MemoryStream();

            using IBinaryWriterX bw = _binaryFactory.CreateWriter(result, false);

            foreach (string key in stringOffsets.Keys)
            {
                long offset = stringOffsets[key];

                result.Position = offset;
                bw.WriteString(key, Encoding.ASCII, false);
            }

            result.Position = 0;
            Stream compressedResult = _compressor.Compress(result, CompressionType.Lz10);

            return compressedResult;
        }

        private IList<XpckEntry> WriteFiles(IList<NamedArchiveEntry> entries, Stream output, long dataOffset, IDictionary<string, long> stringOffsets)
        {
            var result = new XpckEntry[entries.Count];

            long localDataOffset = dataOffset;
            for (var i = 0; i < entries.Count; i++)
            {
                result[i] = new XpckEntry
                {
                    hash = _crc32.ComputeValue(entries[i].Name),
                    nameOffset = (ushort)stringOffsets[entries[i].Name],

                    fileOffsetUpper = (byte)((localDataOffset - dataOffset) >> 18),
                    fileOffsetLower = (ushort)((localDataOffset - dataOffset) >> 2),

                    fileSizeUpper = (byte)(entries[i].Content.Length >> 16),
                    fileSizeLower = (ushort)entries[i].Content.Length
                };

                output.Position = localDataOffset;

                entries[i].Content.Position = 0;
                entries[i].Content.CopyTo(output);

                long lengthRemainder = entries[i].Content.Length % 4;
                if (lengthRemainder > 0)
                {
                    output.Write(new byte[lengthRemainder]);
                    localDataOffset += entries[i].Content.Length + lengthRemainder;
                }
                else
                {
                    output.Write(new byte[4]);
                    localDataOffset += entries[i].Content.Length + 4;
                }
            }

            return result;
        }

        private void WriteEntries(IList<XpckEntry> entries, IBinaryWriterX writer)
        {
            foreach (XpckEntry entry in entries)
                WriteEntry(entry, writer);
        }

        private void WriteEntry(XpckEntry entry, IBinaryWriterX writer)
        {
            writer.Write(entry.hash);
            writer.Write(entry.nameOffset);

            writer.Write(entry.fileOffsetLower);
            writer.Write(entry.fileSizeLower);

            writer.Write(entry.fileOffsetUpper);
            writer.Write(entry.fileSizeUpper);
        }

        private XpckHeader CreateHeader(IList<XpckEntry> entries, byte type, long dataOffset, long nameSize, long dataSize)
        {
            return new XpckHeader
            {
                magic = "XPCK",
                fileCountAndType = (ushort)((entries.Count & 0xFFF) | (type << 0xC)),

                infoOffset = HeaderSize_ >> 2,
                nameTableOffset = (ushort)((HeaderSize_ + entries.Count * EntrySize_) >> 2),
                dataOffset = (ushort)(dataOffset >> 2),

                infoSize = (ushort)((entries.Count * EntrySize_) >> 2),
                nameTableSize = (ushort)(nameSize >> 2),
                dataSize = (ushort)(dataSize >> 2)
            };
        }

        private void WriteHeader(XpckHeader header, IBinaryWriterX writer)
        {
            writer.WriteString(header.magic, Encoding.ASCII, false, false);
            writer.Write(header.fileCountAndType);

            writer.Write(header.infoOffset);
            writer.Write(header.nameTableOffset);
            writer.Write(header.dataOffset);

            writer.Write(header.infoSize);
            writer.Write(header.nameTableSize);
            writer.Write(header.dataSize);
        }
    }
}
