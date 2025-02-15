using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class DecisionPreviewData
    {
        public string Name { get; set; }
        public TextData[] Texts { get; set; }
        public TextData[]? TranslatedTexts { get; set; }
    }
}
