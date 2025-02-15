using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class TipPreviewData
    {
        public int Id { get; set; }

        public TextData TipTitle { get; set; }
        public TextData? TranslatedTipTitle { get; set; }

        public TextData TipText { get; set; }
        public TextData? TranslatedTipText { get; set; }
    }
}
