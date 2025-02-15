using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class StaffRollPreviewData
    {
        public StaffrollTextData?[] Texts { get; set; }
        public StaffrollTextData?[]? TranslatedTexts { get; set; }
    }
}
