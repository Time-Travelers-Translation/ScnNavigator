using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.RenderingManagement.Contract.Renderers
{
    [MapException(typeof(RenderingManagementException))]
    public interface IMultiTextRenderer
    {
        void Render(Image<Rgba32> image, string[] texts);
    }
}
