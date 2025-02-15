using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TranslationManagement.Contract.Exceptions;

namespace Logic.Business.TranslationManagement.Contract
{
    [MapException(typeof(TranslationManagementException))]
    public interface ITranslationSettingsProvider
    {
        public bool IsTranslationEnabled();
        void SetTranslationEnabled(bool value);
    }
}
