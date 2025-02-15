using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Paths;

namespace Logic.Business.TimeTravelersManagement.Paths
{
    internal class BasePathProvider : IBasePathProvider
    {
        private readonly TimeTravelersManagementConfiguration _config;

        public BasePathProvider(TimeTravelersManagementConfiguration config)
        {
            _config = config;
        }

        public string GetFullPath(string relativePath, AssetPreference preference)
        {
            switch (preference)
            {
                case AssetPreference.Original:
                    return GetOriginalFullPath(relativePath);

                case AssetPreference.Patch:
                    return GetPatchFullPath(relativePath);

                case AssetPreference.PatchOrOriginal:
                    return GetOriginalOrPatchFullPath(relativePath);

                default:
                    throw new InvalidOperationException($"Unknown asset preference {preference}.");
            }
        }

        private string GetOriginalFullPath(string relativePath)
        {
            return Path.Combine(_config.OriginalPath, relativePath);
        }

        private string GetPatchFullPath(string relativePath)
        {
            return Path.Combine(_config.PatchPath, relativePath);
        }

        private string GetOriginalOrPatchFullPath(string relativePath)
        {
            string fullPatchPath = GetPatchFullPath(relativePath);
            return File.Exists(fullPatchPath) || Directory.Exists(fullPatchPath) ? fullPatchPath : GetOriginalFullPath(relativePath);
        }
    }
}
