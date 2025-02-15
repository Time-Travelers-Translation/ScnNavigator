using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;

namespace UI.ScnNavigator.Components.Mergers
{
    internal class ScenePreviewDataMerger : IScenePreviewDataMerger
    {
        public ScenePreviewData Merge(
            TextData? originalTitle, StoryTextData[] originalTexts, 
            TextData? translatedTitle, StoryTextData[]? translatedTexts, 
            TextData? routeName)
        {
            // Match up translated texts to original
            if (translatedTexts != null)
            {
                var newTexts = new StoryTextData[translatedTexts.Length];

                for (var i = 0; i < translatedTexts.Length; i++)
                {
                    StoryTextData translatedText = translatedTexts[i];

                    StoryTextData? matchedText = originalTexts.FirstOrDefault(t => t.Name == translatedText.Name && t.Index == translatedText.Index);
                    if (matchedText == null)
                    {
                        matchedText = new StoryTextData
                        {
                            Name = translatedText.Name,
                            Index = translatedText.Index,
                            Speaker = null,
                            Text = "「" + translatedText.Text + "」"
                        };
                    }

                    newTexts[i] = matchedText;
                }

                originalTexts = newTexts;
            }

            return new ScenePreviewData
            {
                TranslatedRoute = routeName,
                Title = originalTitle,
                TranslatedTitle = translatedTitle,
                Texts = originalTexts,
                TranslatedTexts = translatedTexts
            };
        }
    }
}
