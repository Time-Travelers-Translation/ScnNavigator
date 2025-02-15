using Logic.Business.TimeTravelersManagement.Contract.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.TimeTravelersManagement.Contract.Resources
{
    public interface IResourceProvider
    {
        bool TryGet(string relativePath, AssetPreference preference, out IList<Image<Rgba32>>? textures);
        bool TryGet(string relativePath, string resourceName, AssetPreference preference, out Image<Rgba32>? texture);
    }
}
