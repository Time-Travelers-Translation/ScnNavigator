using Logic.Domain.Level5Management.Contract.DataClasses.Resource;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Domain.Level5Management.Contract.Resource
{
    public interface IAnmcResourceParser
    {
        Image<Rgba32> Parse(AnmcResourceData resourceData);
    }
}
