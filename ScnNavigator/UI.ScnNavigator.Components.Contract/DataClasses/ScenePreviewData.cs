using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class ScenePreviewData
    {
        public string SceneName { get; set; }

        public TextData? TranslatedRoute { get; set; }

        public TextData? Title { get; set; }
        public TextData? TranslatedTitle { get; set; }

        public StoryTextData?[] Texts { get; set; }
        public StoryTextData?[]? TranslatedTexts { get; set; }

        public TextData?[]? TranslatedSpeakers { get; set; }
    }
}
