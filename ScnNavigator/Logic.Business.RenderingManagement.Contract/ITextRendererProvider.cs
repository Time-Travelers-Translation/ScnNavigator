using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Renderers;
using Logic.Business.TimeTravelersManagement.Contract.Enums;

namespace Logic.Business.RenderingManagement.Contract
{
    [MapException(typeof(RenderingManagementException))]
    public interface ITextRendererProvider
    {
        ITextRenderer? GetIntroSubtitleRenderer(AssetPreference fontPreference);
        ITextRenderer? GetSubtitleRenderer(AssetPreference fontPreference);
        ITextRenderer? GetNarrationRenderer(AssetPreference fontPreference);
        ITextRenderer? GetUnselectedDecisionRenderer(AssetPreference fontPreference);
        ITextRenderer? GetSelectedDecisionRenderer(AssetPreference fontPreference);
        ITextRenderer? GetDecisionRenderer(AssetPreference fontPreference);
        ITextRenderer? GetBadEndTitleRenderer(AssetPreference fontPreference);
        ITextRenderer? GetBadEndHintRenderer(AssetPreference fontPreference);
        ITextRenderer? GetSceneTitleRenderer(AssetPreference fontPreference);
        ITextRenderer? GetRouteNameRenderer(AssetPreference fontPreference);
        ITextRenderer? GetOutlineRenderer(AssetPreference fontPreference);
        ITextRenderer? GetTipTitleRenderer(AssetPreference fontPreference);
        ITextRenderer? GetTipTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextRenderer? GetHelpTitleRenderer(AssetPreference fontPreference);
        ITextRenderer? GetHelpTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextRenderer? GetTutorialTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextRenderer? GetRevealTutorialTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextRenderer? GetTimeLockTutorialTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference);
        ITextRenderer? GetStaffrollTitleRenderer(int y, AssetPreference fontPreference);
        ITextRenderer? GetStaffrollNameRenderer(int y, AssetPreference fontPreference);
    }
}
