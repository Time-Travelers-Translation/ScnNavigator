using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TranslationManagement.Contract.Exceptions;

namespace Logic.Business.TranslationManagement.Contract
{
    [MapException(typeof(TranslationManagementException))]
    public interface ITipTranslationManager
    {
        Task<TextData?> GetTipTitle(int index);
        Task<TextData?> GetTipText(int index);
        Task UpdateTips(int[] indexes);
    }
}
