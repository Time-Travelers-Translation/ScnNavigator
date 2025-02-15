using CrossCutting.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.Texts;

namespace UI.ScnNavigator.Components.Texts
{
    internal class SceneTextNormalizer : ISceneTextNormalizer
    {
        public string? Normalize(StoryTextData? originalTextData, StoryTextData? translatedTextData, TextData? translatedSpeaker)
        {
            string? originalText = originalTextData?.Text;
            string? translatedText = translatedTextData?.Text;

            if (string.IsNullOrEmpty(translatedText) || string.IsNullOrEmpty(originalText))
                return originalText;

            if (IsNarration(originalText) && !IsNarration(translatedText))
            {
                string trimmedText = translatedText.Trim('＊');
                return $"＊＊{trimmedText}";
            }

            if (IsMentalDialogue(originalText) && !IsMentalDialogue(translatedText))
            {
                string trimmedText = translatedText.Trim('＊');
                return $"＊{trimmedText}";
            }

            if (IsSpeech(originalText) && !IsSpeech(translatedText))
            {
                string? originalSpeaker = originalTextData?.Speaker;
                string? translatedSpeakerText = translatedSpeaker?.Text;

                string speechText = translatedText.Trim('「', '」', '“', '”');

                return $"{translatedSpeakerText ?? originalSpeaker}: “{speechText}”";
            }

            return translatedText;
        }

        private bool IsMentalDialogue(string text)
        {
            return text.StartsWith("＊");
        }

        private bool IsNarration(string text)
        {
            return text.StartsWith("＊＊");
        }

        private bool IsSpeech(string text)
        {
            return text.Contains('「') || text.Contains('“');
        }
    }
}
