using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.InternalContract.Mergers
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface IScenePreviewDataMerger
    {
        ScenePreviewData Merge(TextData? originalTitle, StoryTextData[] originalTexts, 
            TextData? translatedTitle, StoryTextData[]? translatedTexts, 
            TextData? translatedRoute);
    }
}
