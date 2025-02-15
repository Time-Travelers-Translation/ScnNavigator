using CrossCutting.Abstract.DataClasses;

namespace Logic.Business.TimeTravelersManagement.Contract.Texts
{
    public interface IStoryTextManager
    {
        TextData? GetTitleText(string sceneName);
        StoryTextData[] GetStoryTexts(string sceneName);
    }
}
