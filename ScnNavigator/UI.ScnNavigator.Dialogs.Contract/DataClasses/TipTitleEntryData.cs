using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Dialogs.Contract.DataClasses
{
    public class TipTitleEntryData
    {
        public int Id { get; set; }
        public TextData? TipTitle { get; set; }
        public TextData? TranslatedTipTitle { get; set; }
    }
}
