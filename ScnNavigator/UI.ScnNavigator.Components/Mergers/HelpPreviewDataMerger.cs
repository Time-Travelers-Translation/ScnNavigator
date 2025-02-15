using CrossCutting.Abstract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;

namespace UI.ScnNavigator.Components.Mergers
{
    internal class HelpPreviewDataMerger : IHelpPreviewDataMerger
    {
        public HelpPreviewData Merge(string originalTitle, EventText[] originalTexts, TextData? translatedTitle, TextData[]? translatedTexts)
        {
            if (translatedTexts == null && translatedTitle == null)
                return CreatePreviewData(originalTitle, originalTexts);

            int originalCount = originalTexts.Length;
            int translatedCount = translatedTexts?.Length ?? 0;
            int maxCount = Math.Max(originalCount, translatedCount);

            var originalData = new TextData[maxCount];
            for (var i = 0; i < originalCount; i++)
                originalData[i] = CreateTextData(originalTexts[i]);

            var translatedData = new TextData?[maxCount];
            for (var i = 0; i < translatedCount; i++)
                translatedData[i] = translatedTexts?[i];

            return new HelpPreviewData
            {
                HelpTitle = CreateTextData(originalTitle),
                HelpTexts = originalData,
                TranslatedHelpTitle = translatedTitle,
                TranslatedHelpTexts = translatedData
            };
        }

        private HelpPreviewData CreatePreviewData(string originalTitle, EventText[] originalTexts)
        {
            return new HelpPreviewData
            {
                HelpTitle = CreateTextData(originalTitle),
                HelpTexts = originalTexts.Select(CreateTextData).ToArray()
            };
        }

        private TextData CreateTextData(EventText text)
        {
            return new TextData
            {
                Text = text.Text
            };
        }

        private TextData CreateTextData(string text)
        {
            return new TextData
            {
                Text = text
            };
        }
    }
}
