using System.Text;
using Logic.Business.TimeTravelersManagement.Contract;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Business.TimeTravelersManagement.InternalContract;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;
using TtpCallDecisionEntry = Logic.Business.TimeTravelersManagement.InternalContract.TtpCallDecisionEntry;

namespace Logic.Business.TimeTravelersManagement
{
    internal class TtpCallReader : ITtpCallReader
    {
        private readonly IBinaryFactory _binaryFacory;

        public TtpCallReader(IBinaryFactory binaryFactory)
        {
            _binaryFacory = binaryFactory;
        }

        public TtpCallData Read(Stream input)
        {
            return Read(input, StringEncoding.Utf8);
        }

        public TtpCallData Read(Stream input, StringEncoding encoding)
        {
            using IBinaryReaderX br = _binaryFacory.CreateReader(input, true);

            TtpCallHeader header = ReadHeader(br);

            TtpCallData result = CreateData(header.name.Trim('\0'), header.sectionCount, encoding);

            input.Position = header.sectionOffset;
            TtpCallSectionEntry[] sections = ReadSectionEntries(br, header.sectionCount);

            for (var i = 0; i < header.sectionCount; i++)
            {
                TtpCallSectionData sectionData = CreateSectionData(sections[i]);
                result.Sections[i] = sectionData;

                input.Position = sections[i].entryOffset;
                TtpCallBlockEntry[] blockEntries = ReadBlockEntries(br, sections[i].entryCount);

                for (var j = 0; j < sections[i].entryCount; j++)
                {
                    TtpCallBlockData blockData = CreateBlockData(blockEntries[j]);
                    sectionData.Blocks[j] = blockData;

                    input.Position = blockEntries[j].entryOffset;
                    TtpCallWardEntry[] wardEntries = ReadWardEntries(br, blockEntries[j].entryCount);

                    input.Position = blockEntries[j].decisionEntryOffset;
                    TtpCallDecisionEntry[] decisionEntries = ReadDecisionEntries(br, blockEntries[j].decisionCount);

                    for (var h = 0; h < blockEntries[j].entryCount; h++)
                        blockData.Wards[h] = CreateWardData(br, wardEntries[h], encoding);

                    for (var h = 0; h < blockEntries[j].decisionCount; h++)
                        blockData.Decisions[h] = CreateDecisionEntry(br, decisionEntries[h], encoding);
                }
            }

            return result;
        }

        private TtpCallHeader ReadHeader(IBinaryReaderX br)
        {
            return new TtpCallHeader
            {
                magic = br.ReadString(8),
                sectionCount = br.ReadInt32(),
                sectionOffset = br.ReadInt32(),
                name = br.ReadString(16)
            };
        }

        private TtpCallSectionEntry[] ReadSectionEntries(IBinaryReaderX br, int count)
        {
            var result = new TtpCallSectionEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadSectionEntry(br);

            return result;
        }

        private TtpCallSectionEntry ReadSectionEntry(IBinaryReaderX br)
        {
            return new TtpCallSectionEntry
            {
                id = br.ReadInt16(),
                entryCount = br.ReadInt16(),
                entryOffset = br.ReadInt32()
            };
        }

        private TtpCallBlockEntry[] ReadBlockEntries(IBinaryReaderX br, int count)
        {
            var result = new TtpCallBlockEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadBlockEntry(br);

            return result;
        }

        private TtpCallBlockEntry ReadBlockEntry(IBinaryReaderX br)
        {
            return new TtpCallBlockEntry
            {
                entryCount = br.ReadInt32(),
                decisionCount = br.ReadInt32(),
                entryOffset = br.ReadInt32(),
                decisionEntryOffset = br.ReadInt32()
            };
        }

        private TtpCallWardEntry[] ReadWardEntries(IBinaryReaderX br, int count)
        {
            var result = new TtpCallWardEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadWardEntry(br);

            return result;
        }

        private TtpCallWardEntry ReadWardEntry(IBinaryReaderX br)
        {
            return new TtpCallWardEntry
            {
                stringOffset = br.ReadInt32(),
                voice = br.ReadUInt32(),
                flags2 = br.ReadUInt16(),
                flags3 = br.ReadUInt16(),
                nextSectionId = br.ReadInt16(),
                flags4 = br.ReadUInt16()
            };
        }

        private TtpCallDecisionEntry[] ReadDecisionEntries(IBinaryReaderX br, int count)
        {
            var result = new TtpCallDecisionEntry[count];

            for (var i = 0; i < count; i++)
                result[i] = ReadDecisionEntry(br);

            return result;
        }

        private TtpCallDecisionEntry ReadDecisionEntry(IBinaryReaderX br)
        {
            var result = new TtpCallDecisionEntry
            {
                stringOffset = br.ReadInt32(),
                flags = br.ReadInt32(),
                nextSectionId = br.ReadInt32()
            };
            br.BaseStream.Position += 4;

            return result;
        }

        private TtpCallData CreateData(string name, int count, StringEncoding encoding)
        {
            return new TtpCallData
            {
                Name = name,
                Sections = new TtpCallSectionData[count],
                Encoding = encoding
            };
        }

        private TtpCallSectionData CreateSectionData(TtpCallSectionEntry sectionEntry)
        {
            return new TtpCallSectionData
            {
                Id = sectionEntry.id,
                Blocks = new TtpCallBlockData[sectionEntry.entryCount]
            };
        }

        private TtpCallBlockData CreateBlockData(TtpCallBlockEntry blockEntry)
        {
            return new TtpCallBlockData
            {
                Wards = new TtpCallWardData[blockEntry.entryCount],
                Decisions = new Contract.DataClasses.TtpCallDecisionEntry[blockEntry.decisionCount]
            };
        }

        private TtpCallWardData CreateWardData(IBinaryReaderX br, TtpCallWardEntry wardEntry, StringEncoding encoding)
        {
            br.BaseStream.Position = wardEntry.stringOffset;
            string text = ReadString(br, encoding);

            return new TtpCallWardData
            {
                Text = text,
                Voice = CreateVoiceData(wardEntry.voice),
                Flags2 = wardEntry.flags2,
                Flags3 = wardEntry.flags3,
                NextSectionId = wardEntry.nextSectionId,
                Flags4 = wardEntry.flags4
            };
        }

        private TtpCallVoiceData? CreateVoiceData(uint voice)
        {
            if (voice is 0xFFFFFFFF or 0)
                return null;

            return new TtpCallVoiceData
            {
                IsSp = (voice & 0x80000000) != 0,
                Type = (byte)((voice & 0x7F000000) >> 24),
                Number = (byte)((voice & 0x00FF0000) >> 16),
                Id = (byte)((voice & 0x0000FF00) >> 8),
                SubId = (byte)voice
            };
        }

        private Contract.DataClasses.TtpCallDecisionEntry CreateDecisionEntry(IBinaryReaderX br, TtpCallDecisionEntry entry, StringEncoding encoding)
        {
            br.BaseStream.Position = entry.stringOffset;
            string text = ReadString(br, encoding);

            return new Contract.DataClasses.TtpCallDecisionEntry
            {
                Text = text,
                Flags = entry.flags,
                NextSectionId = entry.nextSectionId
            };
        }

        private string ReadString(IBinaryReaderX br, StringEncoding encoding)
        {
            var result = new List<byte>();

            byte byteValue = br.ReadByte();
            while (byteValue != 0)
            {
                result.Add(byteValue);
                byteValue = br.ReadByte();
            }

            switch (encoding)
            {
                case StringEncoding.Sjis:
                    return Encoding.GetEncoding("Shift-JIS").GetString(result.ToArray());

                case StringEncoding.Utf8:
                    return Encoding.UTF8.GetString(result.ToArray());

                default:
                    throw new InvalidOperationException($"Unknown encoding {encoding}.");
            }
        }
    }
}
