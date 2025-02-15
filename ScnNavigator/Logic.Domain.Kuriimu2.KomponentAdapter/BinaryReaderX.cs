using System.IO;
using System.Text;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract.DataClasses;

namespace Logic.Domain.Kuriimu2.KomponentAdapter
{
    internal class BinaryReaderX : IBinaryReaderX
    {
        private readonly Komponent.IO.BinaryReaderX _reader;

        public Stream BaseStream => _reader.BaseStream;

        public BitOrder BitOrder
        {
            get => (BitOrder)_reader.BitOrder;
            set => _reader.BitOrder = (Komponent.Contract.Enums.BitOrder)value;
        }

        public ByteOrder ByteOrder
        {
            get => (ByteOrder)_reader.ByteOrder;
            set => _reader.ByteOrder = (Komponent.Contract.Enums.ByteOrder)value;
        }

        public int BlockSize
        {
            get => _reader.BlockSize;
            set => _reader.BlockSize = value;
        }

        public BinaryReaderX(Stream input, Encoding encoding, bool leaveOpen, ByteOrder byteOrder, BitOrder bitOrder, int blockSize)
        {
            _reader = new Komponent.IO.BinaryReaderX(input, encoding, leaveOpen, (Komponent.Contract.Enums.ByteOrder)byteOrder, (Komponent.Contract.Enums.BitOrder)bitOrder, blockSize);
        }

        public void SeekAlignment(int alignment = 0x10)
        {
            _reader.SeekAlignment(alignment);
        }

        public byte[] ReadBytes(int length)
        {
            return _reader.ReadBytes(length);
        }

        public bool PeekBoolean()
        {
            long pos = _reader.BaseStream.Position;
            bool value = _reader.ReadBoolean();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public byte PeekByte()
        {
            long pos = _reader.BaseStream.Position;
            byte value = _reader.ReadByte();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public sbyte PeekSByte()
        {
            long pos = _reader.BaseStream.Position;
            sbyte value = _reader.ReadSByte();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public char PeekChar()
        {
            return (char)_reader.PeekChar();
        }

        public short PeekInt16()
        {
            long pos = _reader.BaseStream.Position;
            short value = _reader.ReadInt16();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public ushort PeekUInt16()
        {
            long pos = _reader.BaseStream.Position;
            ushort value = _reader.ReadUInt16();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public int PeekInt32()
        {
            long pos = _reader.BaseStream.Position;
            int value = _reader.ReadInt32();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public uint PeekUInt32()
        {
            long pos = _reader.BaseStream.Position;
            uint value = _reader.ReadUInt32();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public long PeekInt64()
        {
            long pos = _reader.BaseStream.Position;
            long value = _reader.ReadInt64();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public ulong PeekUInt64()
        {
            long pos = _reader.BaseStream.Position;
            ulong value = _reader.ReadUInt64();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public float PeekSingle()
        {
            long pos = _reader.BaseStream.Position;
            float value = _reader.ReadSingle();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public double PeekDouble()
        {
            long pos = _reader.BaseStream.Position;
            double value = _reader.ReadDouble();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public decimal PeekDecimal()
        {
            long pos = _reader.BaseStream.Position;
            decimal value = _reader.ReadDecimal();
            _reader.BaseStream.Position = pos;
            return value;
        }

        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }

        public char ReadChar()
        {
            return _reader.ReadChar();
        }

        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        public long ReadInt64()
        {
            return _reader.ReadInt64();
        }

        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        public float ReadSingle()
        {
            return _reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public decimal ReadDecimal()
        {
            return _reader.ReadDecimal();
        }

        public string ReadNullTerminatedString()
        {
            return _reader.ReadNullTerminatedString();
        }

        public string ReadString()
        {
            return _reader.ReadString();
        }

        public string ReadString(int length)
        {
            return _reader.ReadString(length);
        }

        public string ReadString(int length, Encoding encoding)
        {
            return _reader.ReadString(length, encoding);
        }

        public long ReadBits(int count)
        {
            return _reader.ReadBits<long>(count);
        }

        public void ResetBitBuffer()
        {
            // HINT: Workaround for now
            PeekByte();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
