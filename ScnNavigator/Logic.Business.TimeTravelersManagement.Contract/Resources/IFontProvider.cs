using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;

namespace Logic.Business.TimeTravelersManagement.Contract.Resources
{
    public interface IFontProvider
    {
        FontImageData? GetMainFont(AssetPreference preference);
        FontImageData? GetSubtitleFont(AssetPreference preference);
        FontImageData? GetTitleFont(AssetPreference preference);
        FontImageData? GetRouteFont(AssetPreference preference);
        FontImageData? GetStaffrollFont(AssetPreference preference);
    }
}
