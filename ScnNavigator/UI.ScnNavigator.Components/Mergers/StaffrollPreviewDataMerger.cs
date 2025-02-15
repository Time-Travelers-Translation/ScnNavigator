using CrossCutting.Abstract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;

namespace UI.ScnNavigator.Components.Mergers
{
    internal class StaffrollPreviewDataMerger : IStaffrollPreviewDataMerger
    {
        public StaffRollPreviewData Merge(EventText[] originalTexts, StaffrollTextData[]? translatedTexts)
        {
            if (translatedTexts == null)
                return CreatePreviewData(originalTexts);

            var originalLookup = originalTexts.ToDictionary(x => x.Hash, x => x);

            var originalData = new List<StaffrollTextData?>();
            var translatedData = new List<StaffrollTextData?>();

            var translatedIndex = 0;

            while (translatedIndex < translatedTexts.Length)
            {
                StaffrollTextData translatedText = translatedTexts[translatedIndex++];
                StaffrollTextData? originalStaffText =
                    originalLookup.TryGetValue(translatedText.Hash, out EventText? originalText)
                        ? CreateStaffrollData(originalText)
                        : null;

                originalData.Add(originalStaffText);
                translatedData.Add(translatedText);
            }

            return new StaffRollPreviewData
            {
                Texts = originalData.ToArray(),
                TranslatedTexts = translatedData.ToArray()
            };
        }

        private StaffRollPreviewData CreatePreviewData(EventText[] originalTexts)
        {
            return new StaffRollPreviewData
            {
                Texts = originalTexts.Select(CreateStaffrollData).ToArray()
            };
        }

        private StaffrollTextData CreateStaffrollData(EventText text)
        {
            return new StaffrollTextData
            {
                Text = text.Text,
                Flag = text.SubId,
                Hash = text.Hash
            };
        }
    }
}
