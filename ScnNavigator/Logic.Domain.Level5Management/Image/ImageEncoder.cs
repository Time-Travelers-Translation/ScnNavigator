using System;
using System.Linq;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class ImageEncoder : IImageEncoder
    {
        public RawImageData Encode(ImageData image)
        {
            return new RawImageData
            {
                Version = image.Version,

                BitDepth = image.Image.ImageInfo.BitDepth,
                Format = image.Image.ImageInfo.ImageFormat,
                Width = image.Image.ImageInfo.ImageSize.Width,
                Height = image.Image.ImageInfo.ImageSize.Height,

                PaletteBitDepth = image.Image.ImageInfo.PaletteBitDepth,
                PaletteData = image.Image.ImageInfo.PaletteData,
                PaletteFormat = image.Image.ImageInfo.PaletteFormat,

                LegacyData = image.LegacyData,

                Data = image.Image.ImageInfo.ImageData,
                MipMapData = image.Image.ImageInfo.MipMapData?.ToArray() ?? Array.Empty<byte[]>()
            };
        }
    }
}
