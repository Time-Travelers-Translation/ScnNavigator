using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.Enums;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Font
{
    public class FontImageData
    {
        public PlatformType Platform { get; set; }
        public FontData Font { get; set; }
        public ImageData[] Images { get; set; }
    }
}
