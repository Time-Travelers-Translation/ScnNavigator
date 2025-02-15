using CrossCutting.Abstract.DataClasses;

namespace Logic.Business.TranslationManagement.Contract
{
    public interface ITutorialTranslationManager
    {
        Task<TextData?> GetTutorialTitle(int index);
        Task<TextData[]?> GetTutorialTexts(int index);
        Task UpdateTutorials(int[] tutorials);
    }
}
