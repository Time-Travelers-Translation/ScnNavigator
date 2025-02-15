using Logic.Domain.Level5Management.Contract.DataClasses.Resource;
using Logic.Domain.Level5Management.Contract.Resource;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace Logic.Domain.Level5Management.Resource
{
    internal class AnmcResourceParser : IAnmcResourceParser
    {
        public Image<Rgba32> Parse(AnmcResourceData resourceData)
        {
            var image = new Image<Rgba32>((int)resourceData.Size.Width, (int)resourceData.Size.Height);

            foreach (AnmcResourceImageData part in resourceData.Parts)
            {
                var uvLocation = new Point((int)part.UvLocation.X, (int)part.UvLocation.Y);
                var uvSize = new Size((int)part.UvSize.Width, (int)part.UvSize.Height);
                var uvRectangle = new Rectangle(uvLocation, uvSize);

                var partX = (int)Math.Abs(resourceData.Location.X - part.Location.X);
                var partY = (int)Math.Abs(resourceData.Location.Y - part.Location.Y);

                var drawLocation = new Point(partX, partY);
                var drawSize = new Size((int)part.Size.Width, (int)part.Size.Height);

                Image<Rgba32> partImage = part.Image.Image.GetImage().Clone(x => x.Crop(uvRectangle).Resize(drawSize));
                image.Mutate(x => x.DrawImage(partImage, drawLocation, 1f));
            }

            return image;
        }
    }
}
