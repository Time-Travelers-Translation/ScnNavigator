using System.Collections.Generic;
using System.IO;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.Contract.Image;

namespace Logic.Domain.Level5Management.Font
{
    internal class FontComposer : IFontComposer
    {
        private readonly IFontWriterFactory _fontWriterFactory;
        private readonly IImageComposer _imageComposer;
        private readonly IXpckWriter _xpckWriter;

        public FontComposer(IFontWriterFactory fontWriterFactory, IImageComposer imageComposer, IXpckWriter xpckWriter)
        {
            _fontWriterFactory = fontWriterFactory;
            _imageComposer = imageComposer;
            _xpckWriter = xpckWriter;
        }

        public void Compose(FontImageData data, Stream output)
        {
            var archiveData = new ArchiveData
            {
                Type = 1,
                Files = new List<NamedArchiveEntry>(2)
            };

            AddImages(archiveData.Files, data);
            AddFont(archiveData.Files, data);

            _xpckWriter.Write(archiveData, output);
        }

        private void AddFont(IList<NamedArchiveEntry> files, FontImageData data)
        {
            var fontStream = new MemoryStream();

            IFontWriter fontWriter = _fontWriterFactory.Create(data.Font.Version.Version);
            fontWriter.Write(data.Font, fontStream);

            fontStream.Position = 0;
            files.Add(new NamedArchiveEntry
            {
                Name = "FNT.bin",
                Content = fontStream
            });
        }

        private void AddImages(IList<NamedArchiveEntry> files, FontImageData fontData)
        {
            var index = 0;
            foreach (ImageData image in fontData.Images)
            {
                var imageStream = new MemoryStream();

                _imageComposer.Compose(image, imageStream);

                imageStream.Position = 0;
                files.Add(new NamedArchiveEntry
                {
                    Name = $"{index++:000}.xi",
                    Content = imageStream
                });
            }
        }
    }
}
