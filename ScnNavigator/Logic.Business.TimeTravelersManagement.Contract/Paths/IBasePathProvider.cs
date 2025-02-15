using Logic.Business.TimeTravelersManagement.Contract.Enums;

namespace Logic.Business.TimeTravelersManagement.Contract.Paths
{
    public interface IBasePathProvider
    {
        string GetFullPath(string relativePath, AssetPreference preference);
    }
}
