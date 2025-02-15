using CrossCutting.Abstract.DataClasses;

namespace UI.ScnNavigator.Components.Contract.DataClasses
{
    public class PreviewData<TTextData>
        where TTextData : TextData
    {
        public required TTextData?[] OriginalTexts { get; init; }
        public required TTextData?[]? TranslatedTexts { get; init; }
    }
}
