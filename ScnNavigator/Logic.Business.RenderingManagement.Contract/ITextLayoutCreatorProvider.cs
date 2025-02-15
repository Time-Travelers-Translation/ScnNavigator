using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.Enums;

namespace Logic.Business.RenderingManagement.Contract
{
    [MapException(typeof(RenderingManagementException))]
    public interface ITextLayoutCreatorProvider
    {
        ITextLayoutCreator? GetIntroSubtitleLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetSubtitleLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetNarrationLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetDecisionLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetBadEndTitleLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetBadEndHintLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetSceneTitleLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetRouteNameLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetOutlineLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetTipTitleLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetTipTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextLayoutCreator? GetHelpTitleLayoutCreator(AssetPreference fontPreference);
        ITextLayoutCreator? GetHelpTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextLayoutCreator? GetTutorialTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextLayoutCreator? GetRevealTutorialTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextLayoutCreator? GetTimeLockTutorialTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextLayoutCreator? GetStaffrollTitleLayoutCreator(int y, AssetPreference fontPreference);
        ITextLayoutCreator? GetStaffrollNameLayoutCreator(int y, AssetPreference fontPreference);
    }
}
