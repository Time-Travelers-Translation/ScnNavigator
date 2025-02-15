using CrossCutting.Core.Contract.Aspects;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UI.ScnNavigator.Resources.Contract.Exceptions;

namespace UI.ScnNavigator.Resources.Contract
{
    [MapException(typeof(ScnNavigatorResourcesException))]
    public interface IImageResourceProvider
    {
        Image<Rgba32>? GetDecisionResource();
        Image<Rgba32>? GetDecisionBackgroundResource();
        Image<Rgba32>? GetDecisionArrowResource();
        Image<Rgba32>? GetNarrationResource();
        Image<Rgba32>? GetHintButtonResource();
        Image<Rgba32>? GetBackButtonResource();
        Image<Rgba32>? GetHintCaptionResource();
        Image<Rgba32>? GetSceneTitleBackgroundResource();
        Image<Rgba32>? GetOutlineBackgroundResource();
        Image<Rgba32>? GetTipBackgroundResource();
        Image<Rgba32>? GetTipTitleResource();
        Image<Rgba32>? GetTipIconResource();
        Image<Rgba32>? GetHelpBackgroundResource();
        Image<Rgba32>? GetTutorialBackgroundResource();
        Image<Rgba32>? GetRevealTutorialBackgroundResource();
        Image<Rgba32>? GetTimeLockTutorialBackgroundResource();
        Image<Rgba32>? GetTimeLockResource();
    }
}
