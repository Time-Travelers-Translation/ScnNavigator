using CrossCutting.Core.Contract.Aspects;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UI.ScnNavigator.Resources.Contract.Exceptions;

namespace UI.ScnNavigator.Resources.Contract
{
    [MapException(typeof(ScnNavigatorResourcesException))]
    public interface IScreenResourceProvider
    {
        Image<Rgba32> GetSubtitleScreen();
        Image<Rgba32> GetNarrationScreen();
        Image<Rgba32> GetDecisionScreen();
        Image<Rgba32> GetBadEndTitleScreen();
        Image<Rgba32> GetBadEndHintScreen();
        Image<Rgba32> GetSceneTitleScreen();
        Image<Rgba32> GetOutlineScreen();
        Image<Rgba32> GetTipScreen();
        Image<Rgba32> GetHelpScreen();
        Image<Rgba32> GetTutorialScreen();
        Image<Rgba32> GetRevealTutorialScreen();
        Image<Rgba32> GetTimeLockTutorialScreen();
        Image<Rgba32> GetStaffrollScreen();
    }
}
