using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class HelpPreviewData
    {
        public int Id { get; set; }

        public TextData HelpTitle { get; set; }
        public TextData? TranslatedHelpTitle { get; set; }

        public TextData[] HelpTexts { get; set; }
        public TextData?[]? TranslatedHelpTexts { get; set; }
    }
}
