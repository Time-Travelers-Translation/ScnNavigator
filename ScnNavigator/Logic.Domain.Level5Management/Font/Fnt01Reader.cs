using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.Enums;
using Logic.Domain.Level5Management.DataClasses.Font;
using Logic.Domain.Level5Management.InternalContract.Compression;
using Logic.Domain.Level5Management.InternalContract.Font;

namespace Logic.Domain.Level5Management.Font
{
    internal class Fnt01Reader : IFnt01Reader
    {
        private readonly IBinaryFactory _binaryFactory;
        private readonly IStreamFactory _streamFactory;
        private readonly IDecompressor _decompressor;

        public Fnt01Reader(IBinaryFactory binaryFactory, IStreamFactory streamFactory, IDecompressor decompressor)
        {
            _binaryFactory = binaryFactory;
            _streamFactory = streamFactory;
            _decompressor = decompressor;
        }

        public FontData Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, true);

            Fnt01Header header = ReadHeader(br);

            Fnt01CharSize[] charactersSizes = ReadCharSizes(br, header);
            Fnt01CharInfo[] largeCharacterInfos = ReadCharInfos(br, header.largeCharOffset << 2, header.largeCharCount);
            Fnt01CharInfo[] smallCharacterInfos = ReadCharInfos(br, header.smallCharOffset << 2, header.smallCharCount);

            IDictionary<ushort, GlyphData> largeGlyphs = CreateGlyphs(charactersSizes, largeCharacterInfos);
            IDictionary<ushort, GlyphData> smallGlyphs = CreateGlyphs(charactersSizes, smallCharacterInfos);

            return CreateFontData(header, largeGlyphs, smallGlyphs);
        }

        private Fnt01Header ReadHeader(IBinaryReaderX br)
        {
            return new Fnt01Header
            {
                magic = br.ReadString(8),
                const1 = br.ReadInt32(),
                largeCharHeight = br.ReadInt16(),
                smallCharHeight = br.ReadInt16(),
                largeEscapeCharacterIndex = br.ReadUInt16(),
                smallEscapeCharacterIndex = br.ReadUInt16(),
                zero0 = br.ReadInt64(),

                charSizeOffset = br.ReadInt16(),
                charSizeCount = br.ReadInt16(),
                largeCharOffset = br.ReadInt16(),
                largeCharCount = br.ReadInt16(),
                smallCharOffset = br.ReadInt16(),
                smallCharCount = br.ReadInt16(),
            };
        }

        private Fnt01CharSize[] ReadCharSizes(IBinaryReaderX br, Fnt01Header header)
        {
            using Stream compressedCharSizes = _streamFactory.CreateSubStream(br.BaseStream, header.charSizeOffset << 2);
            using Stream decompressedCharSizes = _decompressor.Decompress(compressedCharSizes, 0);

            using IBinaryReaderX sizeReader = _binaryFactory.CreateReader(decompressedCharSizes);

            var result = new Fnt01CharSize[header.charSizeCount];
            for (var i = 0; i < header.charSizeCount; i++)
                result[i] = ReadCharSize(sizeReader);

            return result;
        }

        private Fnt01CharSize ReadCharSize(IBinaryReaderX br)
        {
            return new Fnt01CharSize
            {
                offsetX = br.ReadSByte(),
                offsetY = br.ReadSByte(),
                glyphWidth = br.ReadByte(),
                glyphHeight = br.ReadByte()
            };
        }

        private Fnt01CharInfo[] ReadCharInfos(IBinaryReaderX br, int charInfoOffset, int charInfoCount)
        {
            using Stream compressedCharInfos = _streamFactory.CreateSubStream(br.BaseStream, charInfoOffset);
            using Stream decompressedCharInfos = _decompressor.Decompress(compressedCharInfos, 0);

            using IBinaryReaderX infoReader = _binaryFactory.CreateReader(decompressedCharInfos);

            var result = new Fnt01CharInfo[charInfoCount];
            for (var i = 0; i < charInfoCount; i++)
                result[i] = ReadCharInfo(infoReader);

            return result;
        }

        private Fnt01CharInfo ReadCharInfo(IBinaryReaderX br)
        {
            return new Fnt01CharInfo
            {
                charCode = br.ReadUInt16(),
                charSizeInfo = br.ReadUInt16(),
                imageInfo = br.ReadUInt32()
            };
        }

        private IDictionary<ushort, GlyphData> CreateGlyphs(Fnt01CharSize[] characterSizes, Fnt01CharInfo[] characterInfos)
        {
            var result = new Dictionary<ushort, GlyphData>();
            for (var i = 0; i < characterInfos.Length; i++)
                result[characterInfos[i].charCode] = CreateGlyph(characterSizes[characterInfos[i].charSizeInfo & 0x3FF], characterInfos[i]);

            return result;
        }

        private GlyphData CreateGlyph(Fnt01CharSize characterSize, Fnt01CharInfo characterInfo)
        {
            return new GlyphData
            {
                CodePoint = characterInfo.charCode,
                Width = characterInfo.charSizeInfo >> 10,
                Location = new GlyphLocationData
                {
                    Y = (int)(characterInfo.imageInfo >> 18),
                    X = (int)((characterInfo.imageInfo >> 4) & 0x3FFF),
                    Index = (int)(characterInfo.imageInfo & 0xF)
                },
                Description = new GlyphDescriptionData
                {
                    X = characterSize.offsetX,
                    Y = characterSize.offsetY,
                    Width = characterSize.glyphWidth,
                    Height = characterSize.glyphHeight
                }
            };
        }

        private FontData CreateFontData(Fnt01Header header, IDictionary<ushort, GlyphData> largeGlyphs, IDictionary<ushort, GlyphData> smallGlyphs)
        {
            return new FontData
            {
                Version = new FormatVersion
                {
                    Platform = GetPlatform(header),
                    Version = GetVersion(header)
                },
                LargeFont = new FontGlyphData
                {
                    FallbackCharacter = largeGlyphs.Keys.ElementAtOrDefault(header.largeEscapeCharacterIndex),
                    MaxHeight = header.largeCharHeight,
                    Glyphs = largeGlyphs
                },
                SmallFont = new FontGlyphData
                {
                    FallbackCharacter = smallGlyphs.Keys.ElementAtOrDefault(header.smallEscapeCharacterIndex),
                    MaxHeight = header.smallCharHeight,
                    Glyphs = smallGlyphs
                }
            };
        }

        private PlatformType GetPlatform(Fnt01Header header)
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
                    throw new InvalidOperationException($"Unknown platform identifier '{header.magic[3]}' in font.");
            }
        }

        private int GetVersion(Fnt01Header header)
        {
            return int.Parse(header.magic[4..6]);
        }
    }
}
