using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class TutorialPreviewData
    {
        public int Id { get; set; }

        public TextData TutorialTitle { get; set; }
        public TextData? TranslatedTutorialTitle { get; set; }

        public TextData[] TutorialTexts { get; set; }
        public TextData?[]? TranslatedTutorialTexts { get; set; }
    }
}
