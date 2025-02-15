using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.InternalContract.Mergers
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface ITipPreviewDataMerger
    {
        TipPreviewData Merge(TextData originalTitle, TextData originalText, TextData? translatedTitle, TextData? translatedTexts);
    }
}
