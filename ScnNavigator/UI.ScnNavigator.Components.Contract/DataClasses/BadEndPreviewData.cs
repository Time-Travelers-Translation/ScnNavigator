using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class BadEndPreviewData
    {
        public string Name { get; set; }
        public TextData HintText { get; set; }
        public TextData TitleText { get; set; }
        public TextData? TranslatedHintText { get; set; }
        public TextData? TranslatedTitleText { get; set; }
    }
}
