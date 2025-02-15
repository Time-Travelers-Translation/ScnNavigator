using CrossCutting.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;

namespace UI.ScnNavigator.Components.Mergers
{
    internal class TipPreviewDataMerger : ITipPreviewDataMerger
    {
        public TipPreviewData Merge(TextData originalTitle, TextData originalText, TextData? translatedTitle, TextData? translatedText)
        {
            var tipData = new TipPreviewData
            {
                TipTitle = originalTitle,
                TipText = originalText,
                TranslatedTipTitle = translatedTitle,
                TranslatedTipText = translatedText
            };

            return tipData;
        }
    }
}
