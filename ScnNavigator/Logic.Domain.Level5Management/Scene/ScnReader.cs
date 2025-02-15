using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using Logic.Domain.Level5Management.Contract.Scene;
using Logic.Domain.Level5Management.DataClasses.Scene;

namespace Logic.Domain.Level5Management.Scene
{
    internal class ScnReader : IScnReader
    {
        private readonly IBinaryFactory _binaryFactory;

        public ScnReader(IBinaryFactory binaryFactory)
        {
            _binaryFactory = binaryFactory;
        }

        public SceneNavigator Read(Stream input)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding("Shift-JIS");

            using IBinaryReaderX br = _binaryFactory.CreateReader(input, sjis);

            ScnHeader header = ReadHeader(br);

            br.BaseStream.Position += 0x20;
            byte[] presetCounters = ReadPresetValues(br, header.counterCount);
            byte[] presetSelections = ReadPresetValues(br, header.selectionCount);
            byte[] presetQuickTimes = ReadPresetValues(br, header.quickTimeCount);

            string[] flags = ReadStrings(br, header.flagCount);
            string[] counters = ReadStrings(br, header.counterCount);
            string[] selections = ReadStrings(br, header.selectionCount);
            string[] quickTimes = ReadStrings(br, header.quickTimeCount);

            ScnSceneEntry[] sceneEntries = ReadSceneEntries(br, header.sceneEntryCount);

            br.BaseStream.Position = header.sceneNameOffset;
            ScnSceneNameEntry[] sceneNames = ReadSceneNameEntries(br, header.sceneEntryCount);

            br.BaseStream.Position = header.decisionTextOffset;
            IDictionary<long, ScnText> decisionTexts = ReadTexts(br, header.badEndTextOffset);

            br.BaseStream.Position = header.badEndTextOffset;
            IDictionary<long, ScnText> badEndTexts = ReadTexts(br, header.hintTextOffset);

            br.BaseStream.Position = header.hintTextOffset;
            IDictionary<long, ScnText> hintTexts = ReadTexts(br, br.BaseStream.Length);

            return CreateScriptNavigator(sceneEntries, sceneNames, decisionTexts, badEndTexts, hintTexts, flags);
        }

        private ScnHeader ReadHeader(IBinaryReaderX br)
        {
            return new ScnHeader
            {
                magic = br.ReadString(8),

                unk1 = br.ReadInt32(),
                unk2 = br.ReadInt32(),
                unk3 = br.ReadInt16(),
                unk4 = br.ReadInt16(),
                unk5 = br.ReadInt16(),
                unk6 = br.ReadInt16(),

                sceneEntryCount = br.ReadInt32(),
                selectionCount = br.ReadInt32(),
                quickTimeCount = br.ReadInt32(),
                flagCount = br.ReadInt32(),
                counterCount = br.ReadInt32(),

                sceneNameOffset = br.ReadInt32(),
                decisionTextOffset = br.ReadInt32(),
                badEndTextOffset = br.ReadInt32(),
                hintTextOffset = br.ReadInt32()
            };
        }

        private byte[] ReadPresetValues(IBinaryReaderX br, int count)
        {
            br.BaseStream.Position += 8;
            return br.ReadBytes(count);
        }

        private string[] ReadStrings(IBinaryReaderX br, int count)
        {
            var result = new string[count];

            br.BaseStream.Position += 4;
            for (var i = 0; i < count; i++)
            {
                long position = br.BaseStream.Position;
                string value = br.ReadNullTerminatedString();

                if ((br.BaseStream.Position - position) % 8 == 0)
                    br.BaseStream.Position++;
                while ((br.BaseStream.Position - position) % 8 != 0)
                    br.BaseStream.Position++;

                result[i] = value;
            }

            return result;
        }

        private ScnSceneEntry[] ReadSceneEntries(IBinaryReaderX br, int count)
        {
            var result = new ScnSceneEntry[count];

            br.BaseStream.Position += 8;
            for (var i = 0; i < count; i++)
            {
                ScnSceneEntryHeader header = ReadSceneEntryHeader(br);
                object? data = ReadSceneEntryData(br, header.dataType);
                ScnSceneEntryBranchEntry[] branches = ReadSceneBranchEntries(br);

                result[i] = new ScnSceneEntry
                {
                    header = header,
                    data = data,
                    branches = branches
                };
            }

            return result;
        }

        private ScnSceneEntryHeader ReadSceneEntryHeader(IBinaryReaderX br)
        {
            return new ScnSceneEntryHeader
            {
                sceneId = br.ReadInt16(),
                dataType = br.ReadInt32()
            };
        }

        private object? ReadSceneEntryData(IBinaryReaderX br, int dataType)
        {
            switch (dataType)
            {
                case 0:
                    return null;

                case 1:
                    return new ScnSceneEntryDecisionData
                    {
                        unk1 = br.ReadInt16(),
                        timerIdent = br.ReadByte(),
                        stringCount = br.ReadByte(),
                        unk2 = br.ReadInt32(),
                        decisionTextOffset = br.ReadInt32()
                    };

                case 2:
                    return new ScnSceneEntryQteData
                    {
                        unk1 = br.ReadInt16(),
                        unk2 = br.ReadInt16(),
                        unk3 = br.ReadInt16()
                    };

                case 4:
                    return new ScnSceneEntryBadEndData
                    {
                        unk1 = br.ReadInt16(),
                        unk2 = br.ReadInt16(),
                        titleOffset = br.ReadInt32(),
                        hintOffset = br.ReadInt32()
                    };

                default:
                    throw new InvalidOperationException($"Unknown data type {dataType}.");
            }
        }

        private ScnSceneEntryBranchEntry[] ReadSceneBranchEntries(IBinaryReaderX br)
        {
            br.BaseStream.Position += 4;
            int count = br.ReadInt32();

            var result = new ScnSceneEntryBranchEntry[count];
            for (var i = 0; i < count; i++)
                result[i] = ReadSceneBranchEntry(br);

            return result;
        }

        private ScnSceneEntryBranchEntry ReadSceneBranchEntry(IBinaryReaderX br)
        {
            var entry = new ScnSceneEntryBranchEntry
            {
                sceneId = br.ReadInt16(),
                unk0 = br.ReadInt16(),

                unk1 = br.ReadInt32(),
                unk2 = br.ReadInt32()
            };

            entry.flags = new ScnSceneEntryBranchFlagEntry[entry.unk2];
            for (var i = 0; i < entry.unk2; i++)
            {
                entry.flags[i].unk1 = br.ReadInt16();
                entry.flags[i].flagIndex = br.ReadInt16();
                entry.flags[i].unk2 = br.ReadInt16();
                entry.flags[i].unk3 = br.ReadInt16();
            }

            entry.unk4 = br.ReadInt32();
            entry.unk5 = br.ReadInt32();

            entry.unk6 = new int[entry.unk5];
            for (var i = 0; i < entry.unk5; i++)
                entry.unk6[i] = br.ReadInt32();

            entry.unk7 = br.ReadInt32();
            entry.unk8 = br.ReadInt32();

            entry.unk9 = new int[entry.unk8];
            for (var i = 0; i < entry.unk8; i++)
                entry.unk9[i] = br.ReadInt32();

            return entry;
        }

        private ScnSceneNameEntry[] ReadSceneNameEntries(IBinaryReaderX br, int count)
        {
            var result = new ScnSceneNameEntry[count];
            for (var i = 0; i < count; i++)
                result[i] = ReadSceneNameEntry(br);

            return result;
        }

        private ScnSceneNameEntry ReadSceneNameEntry(IBinaryReaderX br)
        {
            byte[] nameEntries = br.ReadBytes(0x22);

            return new ScnSceneNameEntry
            {
                name = Encoding.ASCII.GetString(nameEntries.TakeWhile(b => b != 0).ToArray()),
                sceneId = br.ReadInt16()
            };
        }

        private IDictionary<long, ScnText> ReadTexts(IBinaryReaderX br, long endOffset)
        {
            var result = new Dictionary<long, ScnText>();

            long baseOffset = br.BaseStream.Position;
            while (br.BaseStream.Position < endOffset)
            {
                result[br.BaseStream.Position - baseOffset] = new ScnText
                {
                    text = br.ReadNullTerminatedString(),
                    nextTextOffset = br.BaseStream.Position - baseOffset
                };
            }

            return result;
        }

        private SceneNavigator CreateScriptNavigator(ScnSceneEntry[] sceneEntries, ScnSceneNameEntry[] sceneNames,
            IDictionary<long, ScnText> decisionTexts, IDictionary<long, ScnText> badEndTexts, IDictionary<long, ScnText> hintTexts,
            string[] flags)
        {
            IDictionary<short, string> nameDictionary = sceneNames.ToDictionary(x => x.sceneId, y => y.name);

            return new SceneNavigator
            {
                Scenes = CreateSceneEntries(sceneEntries, nameDictionary, decisionTexts, badEndTexts, hintTexts, flags)
            };
        }

        private SceneEntry[] CreateSceneEntries(ScnSceneEntry[] sceneEntries, IDictionary<short, string> sceneNames,
            IDictionary<long, ScnText> decisionTexts, IDictionary<long, ScnText> badEndTexts, IDictionary<long, ScnText> hintTexts,
            string[] flags)
        {
            var result = new SceneEntry[sceneEntries.Length];
            var sceneDictionary = new Dictionary<short, SceneEntry>();

            for (var i = 0; i < sceneEntries.Length; i++)
            {
                result[i] = CreateSceneEntry(sceneEntries[i], sceneNames, decisionTexts, badEndTexts, hintTexts);
                sceneDictionary[sceneEntries[i].header.sceneId] = result[i];
            }

            for (var i = 0; i < sceneEntries.Length; i++)
                result[i].Branches = CreateBranchEntries(sceneEntries[i], sceneDictionary, flags);

            return result;
        }

        private SceneEntryBranch?[] CreateBranchEntries(ScnSceneEntry sceneEntry, IDictionary<short, SceneEntry> sceneEntries, string[] flags)
        {
            var result = new SceneEntryBranch?[sceneEntry.branches.Length];
            for (var i = 0; i < sceneEntry.branches.Length; i++)
                result[i] = CreateBranchEntry(sceneEntry.branches[i], sceneEntries, flags);

            return result;
        }

        private SceneEntryBranch? CreateBranchEntry(ScnSceneEntryBranchEntry sceneBranch, IDictionary<short, SceneEntry> sceneEntries, string[] flags)
        {
            if (!sceneEntries.TryGetValue(sceneBranch.sceneId, out SceneEntry? branchSceneEntry))
                return null;

            var requiredFlags = new string[sceneBranch.flags.Length];
            for (var i = 0; i < requiredFlags.Length; i++)
                requiredFlags[i] = flags[sceneBranch.flags[i].flagIndex];

            return new SceneEntryBranch
            {
                Scene = branchSceneEntry,
                RequiredFlags = requiredFlags
            };
        }

        private SceneEntry CreateSceneEntry(ScnSceneEntry sceneEntry,
            IDictionary<short, string> sceneNames, IDictionary<long, ScnText> decisionTexts,
            IDictionary<long, ScnText> badEndTexts, IDictionary<long, ScnText> hintTexts)
        {
            return new SceneEntry
            {
                Id = sceneEntry.header.sceneId,
                Name = sceneNames[sceneEntry.header.sceneId],
                Data = CreateSceneEntryData(sceneEntry.data, decisionTexts, badEndTexts, hintTexts)
            };
        }

        private SceneEntryData CreateSceneEntryData(object? sceneData, IDictionary<long, ScnText> decisionTexts,
            IDictionary<long, ScnText> badEndTexts, IDictionary<long, ScnText> hintTexts)
        {
            if (sceneData == null)
                return new SceneEntryEmptyData();

            switch (sceneData)
            {
                case ScnSceneEntryDecisionData decisionData:
                    long decisionTextOffset = decisionData.decisionTextOffset;

                    var decisions = new string[decisionData.stringCount];
                    for (var i = 0; i < decisionData.stringCount; i++)
                    {
                        decisions[i] = decisionTexts[decisionTextOffset].text;
                        decisionTextOffset = decisionTexts[decisionTextOffset].nextTextOffset;
                    }

                    return new SceneEntryDecisionData
                    {
                        Seconds = decisionData.timerIdent,
                        Decisions = decisions
                    };

                case ScnSceneEntryQteData:
                    return new SceneEntryQteData();

                case ScnSceneEntryBadEndData badEndData:
                    return new SceneEntryBadEndData
                    {
                        Title = badEndTexts[badEndData.titleOffset].text,
                        Hint = hintTexts[badEndData.hintOffset].text
                    };

                default:
                    throw new InvalidOperationException($"Unknown data type {sceneData.GetType().Name}.");
            }
        }
    }
}
