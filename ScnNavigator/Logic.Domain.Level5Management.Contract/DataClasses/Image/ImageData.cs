using Konnect.Contract.Plugin.File.Image;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Image
{
    public class ImageData
    {
        public FormatVersion Version { get; set; }

        public IImageFile Image { get; set; }
        public IImageFile[] Mipmaps { get; set; }

        public byte[]? LegacyData { get; set; }
    }
}
