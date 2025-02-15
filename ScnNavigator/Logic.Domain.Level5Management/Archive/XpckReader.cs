using System.Collections.Generic;
using System.IO;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.DataClasses.Archive;
using Logic.Domain.Level5Management.InternalContract.Compression;

namespace Logic.Domain.Level5Management.Archive
{
    internal class XpckReader : IXpckReader
    {
        private readonly IBinaryFactory _binaryFactory;
        private readonly IStreamFactory _streamFactory;
        private readonly IDecompressor _decompressor;

        public XpckReader(IBinaryFactory binaryFactory, IStreamFactory streamFactory, IDecompressor decompressor)
        {
            _binaryFactory = binaryFactory;
            _streamFactory = streamFactory;
            _decompressor = decompressor;
        }

        public ArchiveData Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, true);

            XpckHeader header = ReadHeader(br);

            br.BaseStream.Position = header.infoOffset << 2;
            XpckEntry[] entries = ReadEntries(br, header.fileCountAndType & 0xFFF);

            Stream compressedNameStream = _streamFactory.CreateSubStream(input, header.nameTableOffset << 2, header.nameTableSize << 2);
            Stream nameStream = _decompressor.Decompress(compressedNameStream, 0);

            using IBinaryReaderX nameReader = _binaryFactory.CreateReader(nameStream);
            IList<NamedArchiveEntry> files = CreateFileEntries(br, header.dataOffset << 2, entries, nameReader);

            return new ArchiveData
            {
                Type = (byte)(header.fileCountAndType >> 0xC),
                Files = files
            };
        }

        private XpckHeader ReadHeader(IBinaryReaderX br)
        {
            return new XpckHeader
            {
                magic = br.ReadString(4),
                fileCountAndType = br.ReadUInt16(),

                infoOffset = br.ReadUInt16(),
                nameTableOffset = br.ReadUInt16(),
                dataOffset = br.ReadUInt16(),
                infoSize = br.ReadUInt16(),
                nameTableSize = br.ReadUInt16(),
                dataSize = br.ReadUInt32()
            };
        }

        private XpckEntry[] ReadEntries(IBinaryReaderX br, int entryCount)
        {
            var result = new XpckEntry[entryCount];
            for (var i = 0; i < entryCount; i++)
                result[i] = ReadEntry(br);

            return result;
        }

        private XpckEntry ReadEntry(IBinaryReaderX br)
        {
            return new XpckEntry
            {
                hash = br.ReadUInt32(),
                nameOffset = br.ReadUInt16(),

                fileOffsetLower = br.ReadUInt16(),
                fileSizeLower = br.ReadUInt16(),
                fileOffsetUpper = br.ReadByte(),
                fileSizeUpper = br.ReadByte(),
            };
        }

        private IList<NamedArchiveEntry> CreateFileEntries(IBinaryReaderX br, int dataOffset, XpckEntry[] entries, IBinaryReaderX nameReader)
        {
            var result = new NamedArchiveEntry[entries.Length];
            for (var i = 0; i < entries.Length; i++)
                result[i] = CreateFileEntry(br, dataOffset, entries[i], nameReader);

            return result;
        }

        private NamedArchiveEntry CreateFileEntry(IBinaryReaderX br, int dataOffset, XpckEntry entry, IBinaryReaderX nameReader)
        {
            nameReader.BaseStream.Position = entry.nameOffset;
            string name = nameReader.ReadNullTerminatedString();

            int fileOffset = ((entry.fileOffsetUpper << 16) | entry.fileOffsetLower) << 2;
            int fileSize = (entry.fileSizeUpper << 16) | entry.fileSizeLower;

            Stream fileContent = _streamFactory.CreateSubStream(br.BaseStream, dataOffset + fileOffset, fileSize);
            if (name == "RES.bin")
                fileContent = _decompressor.Decompress(fileContent, 0);

            return new NamedArchiveEntry
            {
                Name = name,
                Content = fileContent
            };
        }
    }
}
