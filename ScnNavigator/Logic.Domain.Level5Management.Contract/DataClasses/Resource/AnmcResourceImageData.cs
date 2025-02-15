using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using SixLabors.ImageSharp;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Resource
{
    public class AnmcResourceImageData
    {
        public ImageData Image { get; set; }

        public PointF Location { get; set; }
        public SizeF Size { get; set; }

        public PointF UvLocation { get; set; }
        public SizeF UvSize { get; set; }
    }
}
