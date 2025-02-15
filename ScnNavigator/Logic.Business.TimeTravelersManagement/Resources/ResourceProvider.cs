using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;
using Logic.Domain.Level5Management.Contract.Resource;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.TimeTravelersManagement.Resources
{
    internal class ResourceProvider : IResourceProvider
    {
        private readonly IAnmcResourceReader _resourceReader;
        private readonly IAnmcResourceParser _resourceParser;
        private readonly IBasePathProvider _basePathProvider;
        private readonly IGamePathProvider _gamePathProvider;

        private readonly IDictionary<string, IDictionary<string, Image<Rgba32>>> _cachedTextures;

        public ResourceProvider(IAnmcResourceReader resourceReader, IAnmcResourceParser resourceParser, 
            IBasePathProvider basePathProvider, IGamePathProvider gamePathProvider)
        {
            _resourceReader = resourceReader;
            _resourceParser = resourceParser;
            _basePathProvider = basePathProvider;
            _gamePathProvider = gamePathProvider;

            _cachedTextures = new Dictionary<string, IDictionary<string, Image<Rgba32>>>();
        }

        public bool TryGet(string relativePath, AssetPreference preference, out IList<Image<Rgba32>>? textures)
        {
            string normalizedPath = NormalizePath(relativePath);
            string fullPath = _basePathProvider.GetFullPath(normalizedPath, preference);

            textures = null;

            if (!_cachedTextures.TryGetValue(fullPath, out IDictionary<string, Image<Rgba32>>? resourceTextures))
            {
                if (!TryReadResource(fullPath, out IList<AnmcResourceData>? resources))
                    return false;

                resourceTextures = _cachedTextures[fullPath] = new Dictionary<string, Image<Rgba32>>();

                foreach (AnmcResourceData resource in resources!)
                    resourceTextures[resource.Name] = _resourceParser.Parse(resource);
            }

            textures = resourceTextures.Values.ToArray();

            return true;
        }

        public bool TryGet(string relativePath, string name, AssetPreference preference, out Image<Rgba32>? texture)
        {
            string normalizedPath = NormalizePath(relativePath);
            string fullPath = _basePathProvider.GetFullPath(normalizedPath, preference);

            texture = null;

            if (!_cachedTextures.TryGetValue(fullPath, out IDictionary<string, Image<Rgba32>>? resourceTextures))
            {
                if (!TryParseResource(fullPath, out resourceTextures))
                    return false;

                _cachedTextures[fullPath] = resourceTextures!;
            }

            return resourceTextures!.TryGetValue(name, out texture);
        }

        private bool TryParseResource(string fullPath, out IDictionary<string, Image<Rgba32>>? resourceTextures)
        {
            resourceTextures = null;

            if (!TryReadResource(fullPath, out IList<AnmcResourceData>? resources))
                return false;

            resourceTextures = new Dictionary<string, Image<Rgba32>>();

            foreach (AnmcResourceData resource in resources!)
                resourceTextures[resource.Name] = _resourceParser.Parse(resource);

            return true;
        }

        private bool TryReadResource(string fullPath, out IList<AnmcResourceData>? resources)
        {
            resources = null;

            if (!File.Exists(fullPath))
                return false;

            using Stream fileStream = File.OpenRead(fullPath);

            resources = _resourceReader.Read(fileStream);
            if (resources == null)
                return false;

            return true;
        }

        private string NormalizePath(string relativePath)
        {
            if (relativePath.StartsWith("#/"))
                relativePath = relativePath[2..];

            return Path.Combine(_gamePathProvider.GetPlatformPath(), relativePath);
        }
    }
}
