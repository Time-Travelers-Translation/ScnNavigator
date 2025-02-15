using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TranslationManagement.Contract.Exceptions;

namespace Logic.Business.TranslationManagement.Contract
{
    [MapException(typeof(TranslationManagementException))]
    public interface IHelpTranslationManager
    {
        Task<TextData?> GetHelpTitle(int index);
        Task<TextData[]?> GetHelpTexts(int index);
        Task UpdateHelps(int[] helps);
    }
}
