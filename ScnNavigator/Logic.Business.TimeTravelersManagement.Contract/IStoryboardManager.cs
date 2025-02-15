namespace Logic.Business.TimeTravelersManagement.Contract
{
    public interface IStoryboardManager
    {
        string[] GetStoryTextIdentifiers(int chapter);
        string[] GetStoryTextIdentifiers(string sceneName);
    }
}
