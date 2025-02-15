using Logic.Business.TimeTravelersManagement.Contract;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Domain.Level5Management.Contract.DataClasses.Script;
using Logic.Domain.Level5Management.Contract.Script;

namespace Logic.Business.TimeTravelersManagement
{
    internal class StoryboardManager : IStoryboardManager
    {
        private readonly IStoryboardReader _storyboardReader;
        private readonly IBasePathProvider _basePathProvider;
        private readonly IGamePathProvider _pathProvider;

        private readonly IDictionary<string, Storyboard> _storyboardLookup;

        private readonly IDictionary<string, string> _scenePathLookup;
        private readonly IDictionary<int, IList<string>> _chapterScenesLookup;

        public StoryboardManager(IStoryboardReader storyboardReader, IBasePathProvider basePathProvider, IGamePathProvider pathProvider)
        {
            _storyboardReader = storyboardReader;
            _basePathProvider = basePathProvider;
            _pathProvider = pathProvider;

            _storyboardLookup = new Dictionary<string, Storyboard>();

            _scenePathLookup = InitializeScenePaths();
            _chapterScenesLookup = InitializeChapterScenes();
        }

        public string[] GetStoryTextIdentifiers(int chapter)
        {
            if (!_chapterScenesLookup.TryGetValue(chapter, out IList<string>? scenes))
                return Array.Empty<string>();

            var result = new List<string>();

            foreach (string scene in scenes)
            {
                string[] sceneIdentifiers = GetStoryTextIdentifiers(scene);
                result.AddRange(sceneIdentifiers);
            }

            return result.ToArray();
        }

        public string[] GetStoryTextIdentifiers(string sceneName)
        {
            if (!_storyboardLookup.TryGetValue(sceneName, out Storyboard? storyboard))
            {
                storyboard = ReadStoryboard(sceneName);
                if (storyboard == null)
                    return Array.Empty<string>();

                _storyboardLookup[sceneName] = storyboard;
            }

            var result = new List<string>();

            IEnumerable<StoryboardInstruction> invocations = storyboard.MainInstructions.Where(x => x.Operation.OpCode == 0x15);
            foreach (StoryboardInstruction invocation in invocations)
            {
                var invocationType = (uint)invocation.Arguments[^1].Value!;
                if (invocationType == 651)
                    result.Add((string)invocation.Arguments[^2].Value!);
            }

            return result.ToArray();
        }

        private Storyboard? ReadStoryboard(string sceneName)
        {
            if (!_scenePathLookup.TryGetValue(sceneName, out string? stbFile))
                return null;

            using Stream stbStream = File.OpenRead(stbFile);
            return _storyboardReader.Read(stbStream);
        }

        private IDictionary<string, string> InitializeScenePaths()
        {
            var result = new Dictionary<string, string>();

            string stbFolderPath = _pathProvider.GetStoryBoardFolderPath();
            string originalStbPath = _basePathProvider.GetFullPath(stbFolderPath, AssetPreference.Original);

            string[] originalStbFiles = Directory.GetFiles(originalStbPath, "*.stb", SearchOption.AllDirectories);
            foreach (string originalStbFile in originalStbFiles)
            {
                string stbSubPath = Path.GetRelativePath(originalStbPath, originalStbFile);
                string stbFile = _basePathProvider.GetFullPath(Path.Combine(stbFolderPath, stbSubPath), AssetPreference.PatchOrOriginal);

                string stbName = Path.GetFileNameWithoutExtension(originalStbFile);
                result[stbName] = stbFile;
            }

            return result;
        }

        private IDictionary<int, IList<string>> InitializeChapterScenes()
        {
            var result = new Dictionary<int, IList<string>>();

            foreach (string scene in _scenePathLookup.Keys)
            {
                if (!int.TryParse(scene[1..3], out int chapter))
                    continue;

                if (!result.TryGetValue(chapter, out IList<string>? chapterScenes))
                    result[chapter] = chapterScenes = new List<string>();

                chapterScenes.Add(scene);
            }

            return result;
        }
    }
}
