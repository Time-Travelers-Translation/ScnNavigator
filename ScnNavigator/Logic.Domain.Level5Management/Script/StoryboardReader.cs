using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses.Script;
using Logic.Domain.Level5Management.Contract.Enums.Script;
using Logic.Domain.Level5Management.Contract.Script;
using Logic.Domain.Level5Management.DataClasses.Script;

namespace Logic.Domain.Level5Management.Script
{
    internal class StoryboardReader : IStoryboardReader
    {
        private readonly IBinaryFactory _binaryFactory;

        public StoryboardReader(IBinaryFactory binaryFactory)
        {
            _binaryFactory = binaryFactory;
        }

        public Storyboard Read(Stream input)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding("Shift-JIS");

            using IBinaryReaderX br = _binaryFactory.CreateReader(input, sjis, true);

            StbHeader header = ReadHeader(br);

            br.BaseStream.Position = header.logicTableOffset;
            StbLogicEntry[] logicEntries = ReadLogicEntries(br, header.logicTableCount);

            br.BaseStream.Position = header.firstLogicOffset;
            StbOperation[] firstLogicOperationSet = ReadOperations(br);

            StbOperation[] mainLogicOperations = Array.Empty<StbOperation>();
            StbOperation[][] logicTableOperations = new StbOperation[header.logicTableCount][];
            for (var i = 0; i < header.logicTableCount; i++)
            {
                br.BaseStream.Position = logicEntries[i].offset;
                int logicOffset = br.ReadInt32() + header.firstLogicOffset;

                br.BaseStream.Position = logicOffset;
                logicTableOperations[i] = ReadOperations(br);

                if (header.mainLogicOffset == logicEntries[i].offset)
                    mainLogicOperations = logicTableOperations[i];
            }

            return CreateStoryboard(br, mainLogicOperations, header.firstLogicOffset);
        }

        private StbHeader ReadHeader(IBinaryReaderX br)
        {
            return new StbHeader
            {
                magic = br.ReadString(4),
                mainLogicOffset = br.ReadInt32(),
                firstLogicOffset = br.ReadInt32(),
                logicTableOffset = br.ReadInt32(),
                logicTableCount = br.ReadInt32()
            };
        }

        private StbLogicEntry[] ReadLogicEntries(IBinaryReaderX br, int count)
        {
            var result = new StbLogicEntry[count];
            for (var i = 0; i < count; i++)
                result[i] = ReadLogicEntry(br);

            return result;
        }

        private StbLogicEntry ReadLogicEntry(IBinaryReaderX br)
        {
            return new StbLogicEntry
            {
                unk1 = br.ReadInt32(),
                offset = br.ReadInt32()
            };
        }

        private StbOperation[] ReadOperations(IBinaryReaderX br)
        {
            var result = new List<StbOperation>();

            StbOperation operation = ReadOperation(br);
            while (operation.opCode != 0)
            {
                result.Add(operation);
                operation = ReadOperation(br);
            }

            return result.ToArray();
        }

        private StbOperation ReadOperation(IBinaryReaderX br)
        {
            return new StbOperation
            {
                opCode = br.ReadUInt32(),
                opCodeInfo = br.ReadUInt32(),
                value = br.ReadUInt32()
            };
        }

        private Storyboard CreateStoryboard(IBinaryReaderX br, StbOperation[] operations, long baseOffset)
        {
            var instructions = new List<StoryboardInstruction>();

            var stack = new Stack<StoryboardValue>();
            for (var i = 0; i < operations.Length; i++)
            {
                switch (operations[i].opCode)
                {
                    case 0x3:
                    case 0x6:
                    case 0xB:
                    case 0x13:
                        StoryboardValue value = CreateStoryboardValue(br, operations[i], baseOffset);
                        stack.Push(value);
                        break;

                    case 0x4:
                    case 0xF:
                    case 0x15:
                        StoryboardInstruction instruction = CreateStoryboardInstruction(operations, i, stack);
                        instructions.Add(instruction);
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown op code {operations[i].opCode}.");
                }
            }

            return new Storyboard
            {
                MainInstructions = instructions.ToArray()
            };
        }

        private StoryboardValue CreateStoryboardValue(IBinaryReaderX br, StbOperation operation, long baseOffset)
        {
            StoryboardValueType type;
            object? value;

            switch (operation.opCode)
            {
                case 0x3:
                    switch (operation.opCodeInfo)
                    {
                        case 0x1:
                            type = StoryboardValueType.UInt;
                            value = operation.value;
                            break;

                        case 0x2:
                            type = StoryboardValueType.Float;
                            value = BitConverter.Int32BitsToSingle(unchecked((int)operation.value));
                            break;

                        case 0x3:
                            type = StoryboardValueType.String;
                            value = ReadString(br, operation.value + baseOffset);
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown op code info {operation.opCodeInfo} for op code {operation.opCode}.");
                    }
                    break;

                case 0x6:
                    type = StoryboardValueType.Boolean;
                    value = true;
                    break;

                case 0xB:
                    type = StoryboardValueType.Null;
                    value = null;
                    break;

                case 0x13:
                    type = StoryboardValueType.OperationOffset;
                    value = operation.value;
                    break;

                default:
                    throw new InvalidOperationException($"Unknown op code {operation.opCode}.");
            }

            return new StoryboardValue
            {
                Type = type,
                Value = value
            };
        }

        private string ReadString(IBinaryReaderX br, long offset)
        {
            br.BaseStream.Position = offset;
            return br.ReadNullTerminatedString();
        }

        private StoryboardInstruction CreateStoryboardInstruction(StbOperation[] operations, int operationIndex, Stack<StoryboardValue> valueStack)
        {
            var arguments = new List<StoryboardValue>();

            StoryboardValue value;
            switch (operations[operationIndex].opCode)
            {
                case 0x4:
                    value = valueStack.Pop();
                    if (value.Type != StoryboardValueType.OperationOffset)
                        throw new InvalidOperationException($"Unexpected value type {value.Type} for macro instruction.");

                    var macroValue = (uint)value.Value!;
                    if (macroValue == 0)
                        arguments.Add(valueStack.Pop());

                    break;

                case 0xF:
                    arguments.Add(valueStack.Pop());
                    break;

                case 0x15:
                    for (var i = 0; i < operations[operationIndex].opCodeInfo - 1;)
                    {
                        value = valueStack.Pop();
                        if (value.Type == StoryboardValueType.Null)
                            continue;

                        arguments.Add(value);
                        i++;
                    }

                    valueStack.Pop();
                    StoryboardValue subroutineType = valueStack.Pop();
                    valueStack.Pop();

                    arguments.Add(subroutineType);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown op code {operations[operationIndex].opCode}.");
            }

            return new StoryboardInstruction
            {
                Operation = CreateStoryboardOperation(operations[operationIndex]),
                Arguments = arguments.ToArray()
            };
        }

        private StoryboardOperation CreateStoryboardOperation(StbOperation operation)
        {
            return new StoryboardOperation
            {
                OpCode = operation.opCode,
                OpCodeInfo = operation.opCodeInfo,
                Value = operation.value
            };
        }
    }
}
