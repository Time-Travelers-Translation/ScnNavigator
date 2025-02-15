using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class ImageParser : IImageParser
    {
        private readonly IImageVersionReader _versionReader;
        private readonly IImageReaderFactory _readerFactory;
        private readonly IImageDecoder _imageDecoder;

        public ImageParser(IImageVersionReader versionReader, IImageReaderFactory readerFactory, IImageDecoder imageDecoder)
        {
            _versionReader = versionReader;
            _readerFactory = readerFactory;
            _imageDecoder = imageDecoder;
        }

        public ImageData Parse(Stream input)
        {
            int imageVersion = _versionReader.Peek(input);

            IImageReader imageReader = _readerFactory.Create(imageVersion);
            RawImageData imageData = imageReader.Read(input);

            return _imageDecoder.Decode(imageData);
        }
    }
}
