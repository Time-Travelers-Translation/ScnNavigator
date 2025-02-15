using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TranslationManagement.Contract.Exceptions;

namespace Logic.Business.TranslationManagement.Contract
{
    [MapException(typeof(TranslationManagementException))]
    public interface ITitleTranslationManager
    {
        Task<TextData?> GetSceneTitle(string sceneName);
        Task UpdateSceneTitle(string sceneName);
        Task UpdateSceneTitle(string[] sceneNames);
    }
}
