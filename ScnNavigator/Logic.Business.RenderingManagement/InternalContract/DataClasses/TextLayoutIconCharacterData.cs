using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.RenderingManagement.InternalContract.DataClasses
{
    internal class TextLayoutIconCharacterData : TextLayoutCharacterData
    {
        public Image<Rgba32>? Image { get; set; }
    }
}
