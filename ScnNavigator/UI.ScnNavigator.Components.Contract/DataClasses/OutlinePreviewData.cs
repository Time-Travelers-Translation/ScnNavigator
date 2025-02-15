using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class OutlinePreviewData
    {
        public string Route { get; set; }
        public TextData[] Texts { get; set; }
        public TextData[]? TranslatedTexts { get; set; }
    }
}
