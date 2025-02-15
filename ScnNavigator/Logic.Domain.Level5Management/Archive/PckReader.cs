using System.Collections.Generic;
using System.IO;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.DataClasses.Archive;

namespace Logic.Domain.Level5Management.Archive
{
    internal class PckReader : IPckReader
    {
        private readonly IStreamFactory _streamFactory;
        private readonly IBinaryFactory _binaryFactory;

        public PckReader(IStreamFactory streamFactory, IBinaryFactory binaryFactory)
        {
            _streamFactory = streamFactory;
            _binaryFactory = binaryFactory;
        }

        public IList<HashArchiveEntry> Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, true);

            int entryCount = br.ReadInt32();
            PckEntry[] entries = ReadEntries(br, entryCount);

            return CreateArchiveEntries(br, entries);
        }

        private PckEntry[] ReadEntries(IBinaryReaderX br, int count)
        {
            var result = new PckEntry[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = new PckEntry
                {
                    hash = br.ReadUInt32(),
                    fileOffset = br.ReadInt32(),
                    fileLength = br.ReadInt32()
                };
            }

            return result;
        }

        private IList<HashArchiveEntry> CreateArchiveEntries(IBinaryReaderX br, PckEntry[] entries)
        {
            var result = new HashArchiveEntry[entries.Length];
            for (var i = 0; i < entries.Length; i++)
                result[i] = CreateArchiveEntry(br, entries[i]);

            return result;
        }

        private HashArchiveEntry CreateArchiveEntry(IBinaryReaderX br, PckEntry entry)
        {
            br.BaseStream.Position = entry.fileOffset;

            short hashIdentifier = br.ReadInt16();
            if (hashIdentifier == 100)
            {
                short hashCount = br.ReadInt16();
                br.BaseStream.Position += hashCount * 4;
            }

            long hashSize = br.BaseStream.Position - entry.fileOffset;
            return new HashArchiveEntry
            {
                Hash = entry.hash,
                Content = _streamFactory.CreateSubStream(br.BaseStream, br.BaseStream.Position, entry.fileLength - hashSize)
            };
        }
    }
}
