using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class ImageComposer : IImageComposer
    {
        private readonly IImageWriterFactory _writerFactory;
        private readonly IImageEncoder _imageEncoder;

        public ImageComposer(IImageWriterFactory writerFactory, IImageEncoder imageEncoder)
        {
            _writerFactory = writerFactory;
            _imageEncoder = imageEncoder;
        }

        public void Compose(ImageData data, Stream output)
        {
            RawImageData rawImageData = _imageEncoder.Encode(data);

            IImageWriter imageWriter = _writerFactory.Create(data.Version.Version);
            imageWriter.Write(rawImageData, output);
        }
    }
}
