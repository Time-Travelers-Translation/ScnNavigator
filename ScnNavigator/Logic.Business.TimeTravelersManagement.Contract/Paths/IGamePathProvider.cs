namespace Logic.Business.TimeTravelersManagement.Contract.Paths
{
    public interface IGamePathProvider
    {
        string GetPlatformPath();
        string GetFlowFilePath();
        string GetEventPckFilePath(string identifier);
        string GetFontFilePath(string name);
        string GetStoryBoardFolderPath();
        string GetTipTitleFilePath();
        string GetTipTextFilePath(int tipIndex);
    }
}
