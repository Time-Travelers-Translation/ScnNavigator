using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TranslationManagement.Contract.Exceptions;

namespace Logic.Business.TranslationManagement.Contract
{
    [MapException(typeof(TranslationManagementException))]
    public interface IStoryTranslationManager
    {
        Task<IDictionary<string, StoryTextData[]>> GetStoryTexts(int chapter);
        Task<StoryTextData[]?> GetStoryTexts(string sceneName);
        Task UpdateStoryText(string sceneName);
        Task UpdateStoryText(string[] sceneNames);
    }
}
