using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class TtpCallPreviewData
    {
        public TextData[] Texts { get; set; }
        public TextData[]? TranslatedTexts { get; set; }
    }
}
