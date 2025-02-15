using CrossCutting.Abstract.DataClasses;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    public class TranslatedTextData<TTextData>
        where TTextData : TextData
    {
        public int Row { get; set; }
        public TTextData Text { get; set; }
    }
}
