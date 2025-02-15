using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TranslationManagement.Contract.Exceptions;

namespace Logic.Business.TranslationManagement.Contract
{
    [MapException(typeof(TranslationManagementException))]
    public interface IHintTranslationManager
    {
        Task<TextData?> GetSceneHint(string sceneName);
        Task UpdateSceneHint(string[] sceneNames);
    }
}
