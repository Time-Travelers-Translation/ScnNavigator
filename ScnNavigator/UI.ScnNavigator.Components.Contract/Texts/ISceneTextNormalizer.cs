using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.Texts
{
    public interface ISceneTextNormalizer
    {
        string? Normalize(StoryTextData? originalText, StoryTextData? translatedText, TextData? translatedSpeaker);
    }
}
