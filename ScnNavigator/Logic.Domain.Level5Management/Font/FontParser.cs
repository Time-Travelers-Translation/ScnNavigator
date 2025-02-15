using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Enums;
using Logic.Domain.Level5Management.Contract.Enums.Archive;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.Contract.Image;

namespace Logic.Domain.Level5Management.Font
{
    internal class FontParser : IFontParser
    {
        private readonly IArchiveTypeReader _archiveTypeReader;
        private readonly IArchiveReaderFactory _archiveReaderFactory;
        private readonly IFontTypeReader _fontVersionReader;
        private readonly IFontReaderFactory _fontReaderFactory;
        private readonly IImageParser _imageParser;

        public FontParser(
            IArchiveTypeReader archiveTypeReader, IArchiveReaderFactory archiveReaderFactory,
            IFontTypeReader fontVersionReader, IFontReaderFactory fontReaderFactory,
            IImageParser imageParser)
        {
            _archiveTypeReader = archiveTypeReader;
            _archiveReaderFactory = archiveReaderFactory;
            _fontVersionReader = fontVersionReader;
            _fontReaderFactory = fontReaderFactory;
            _imageParser = imageParser;
        }

        public FontImageData? Parse(Stream input)
        {
            ArchiveData archiveData = ReadFiles(input);

            FontData? fontData = GetFontData(archiveData.Files);
            if (fontData == null)
                return null;

            ImageData[] glyphImages = GetGlyphImages(archiveData.Files);
            PlatformType platform = GetPlatform(fontData, glyphImages);

            return new FontImageData
            {
                Platform = platform,
                Font = fontData,
                Images = glyphImages
            };
        }

        private ArchiveData ReadFiles(Stream input)
        {
            ArchiveType archiveType = _archiveTypeReader.Peek(input);
            IArchiveReader reader = _archiveReaderFactory.Create(archiveType);

            return reader.Read(input);
        }

        private FontData? GetFontData(IList<NamedArchiveEntry> files)
        {
            NamedArchiveEntry? fntFile = files.FirstOrDefault(f => f.Name == "FNT.bin");
            if (fntFile == null)
                return null;

            int fontVersion = _fontVersionReader.Peek(fntFile.Content);
            IFontReader fontReader = _fontReaderFactory.Create(fontVersion);

            return fontReader.Read(fntFile.Content);
        }

        private ImageData[] GetGlyphImages(IList<NamedArchiveEntry> files)
        {
            var result = new List<ImageData>();

            IEnumerable<NamedArchiveEntry> imageFiles = files.Where(f => f.Name.EndsWith(".xi"));
            foreach (NamedArchiveEntry imageFile in imageFiles)
            {
                ImageData glyphImage = _imageParser.Parse(imageFile.Content);

                result.Add(glyphImage);
            }

            return result.ToArray();
        }

        private PlatformType GetPlatform(FontData fontData, ImageData[] glyphImages)
        {
            if (glyphImages.Any(i => i.Version.Platform != fontData.Version.Platform))
                throw new InvalidOperationException("Inconsistent platform indicators in font file.");

            return fontData.Version.Platform;
        }
    }
}
