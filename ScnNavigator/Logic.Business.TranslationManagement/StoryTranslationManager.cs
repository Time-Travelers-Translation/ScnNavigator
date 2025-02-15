using CrossCutting.Abstract.DataClasses;
using Logic.Business.TranslationManagement.Contract;
using Logic.Business.TranslationManagement.InternalContract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Business.TranslationManagement
{
    internal class StoryTranslationManager : IStoryTranslationManager
    {
        private static readonly int[] ChapterEndRows =
        {
            1005,
            1642,
            2106,
            1891,
            1868,
            849,
            305
        };

        private readonly ISheetManager _sheetManager;

        private IDictionary<string, IList<TranslatedTextData<StoryTextData>>>? _sceneStoryLookup;
        private IDictionary<int, HashSet<string>>? _chapterScenesLookup;

        public StoryTranslationManager(TranslationManagementConfiguration config, IGoogleApiConnector apiConnector)
        {
            _sheetManager = apiConnector.CreateSheetManager(config.SheetId);
        }

        public async Task<IDictionary<string, StoryTextData[]>> GetStoryTexts(int chapter)
        {
            var result = new Dictionary<string, StoryTextData[]>();

            if (_chapterScenesLookup == null || !_chapterScenesLookup.ContainsKey(chapter))
                await PopulateChapterData(chapter);

            if (_chapterScenesLookup == null || !_chapterScenesLookup.TryGetValue(chapter, out HashSet<string>? scenes))
                return result;

            foreach (string scene in scenes!)
            {
                StoryTextData[]? sceneTexts = await GetStoryTexts(scene);
                if (sceneTexts == null)
                    continue;

                result[scene] = sceneTexts;
            }

            return result;
        }

        public async Task<StoryTextData[]?> GetStoryTexts(string sceneName)
        {
            string identifier = sceneName[1..3];
            if (!int.TryParse(identifier, out int chapter))
                return null;

            if (_chapterScenesLookup != null && _chapterScenesLookup.ContainsKey(chapter))
            {
                if (_sceneStoryLookup != null && _sceneStoryLookup.TryGetValue(sceneName, out IList<TranslatedTextData<StoryTextData>>? texts))
                    return texts.Select(x => x.Text).ToArray();

                return null;
            }

            await PopulateChapterData(chapter);

            if (_sceneStoryLookup == null || !_sceneStoryLookup.TryGetValue(sceneName, out IList<TranslatedTextData<StoryTextData>>? translatedTexts))
                return null;

            return translatedTexts.Select(x => x.Text).ToArray();
        }

        public Task UpdateStoryText(string sceneName)
        {
            return UpdateStoryText(new[] { sceneName });
        }

        public async Task UpdateStoryText(string[] sceneNames)
        {
            if (_sceneStoryLookup == null)
                return;

            var texts = new List<TranslatedTextData<StoryTextData>>();
            foreach (string sceneName in sceneNames)
            {
                if (_sceneStoryLookup.TryGetValue(sceneName, out IList<TranslatedTextData<StoryTextData>>? sceneTexts))
                    texts.AddRange(sceneTexts);
            }

            var chapters = new Dictionary<int, IList<TranslatedTextData<StoryTextData>>>();
            foreach (TranslatedTextData<StoryTextData> translatedTextData in texts)
            {
                string identifier = translatedTextData.Text.Name[1..3];
                if (!int.TryParse(identifier, out int chapter))
                    continue;

                if (!chapters.TryGetValue(chapter, out IList<TranslatedTextData<StoryTextData>>? group))
                    chapters[chapter] = group = new List<TranslatedTextData<StoryTextData>>();

                group.Add(translatedTextData);
            }

            var chapterRanges = new Dictionary<int, IList<IList<TranslatedTextData<StoryTextData>>>>();
            foreach (int chapter in chapters.Keys)
            {
                if (!chapterRanges.TryGetValue(chapter, out IList<IList<TranslatedTextData<StoryTextData>>>? rangeGroups))
                    chapterRanges[chapter] = rangeGroups = new List<IList<TranslatedTextData<StoryTextData>>>();

                foreach (TranslatedTextData<StoryTextData> translatedTextData in chapters[chapter].OrderBy(g => g.Row))
                {
                    if (rangeGroups.Count <= 0 || rangeGroups[^1][^1].Row + 1 != translatedTextData.Row)
                        rangeGroups.Add(new List<TranslatedTextData<StoryTextData>>());

                    rangeGroups[^1].Add(translatedTextData);
                }
            }

            foreach (int chapter in chapterRanges.Keys)
            {
                IList<IList<TranslatedTextData<StoryTextData>>> rangeGroup = chapterRanges[chapter];

                foreach (IList<TranslatedTextData<StoryTextData>> range in rangeGroup)
                {
                    if (range.Count <= 0)
                        continue;

                    UpdateStoryTextRangeData[] updateRange = range.Select(r => new UpdateStoryTextRangeData
                    {
                        Index = r.Text.Index,
                        Translation = r.Text.Text
                    }).ToArray();

                    CellIdentifier indexCellStart = CellIdentifier.Parse($"C{range[0].Row}");
                    CellIdentifier indexCellEnd = CellIdentifier.Parse($"C{range[^1].Row}");

                    await _sheetManager.UpdateRangeAsync(updateRange, $"{chapter}", indexCellStart, indexCellEnd);

                    CellIdentifier textCellStart = CellIdentifier.Parse($"I{range[0].Row}");
                    CellIdentifier textCellEnd = CellIdentifier.Parse($"I{range[^1].Row}");

                    await _sheetManager.UpdateRangeAsync(updateRange, $"{chapter}", textCellStart, textCellEnd);
                }
            }
        }

        private async Task PopulateChapterData(int chapter)
        {
            CellIdentifier startCell = CellIdentifier.Parse("A2");
            CellIdentifier endCell = CellIdentifier.Parse($"J{ChapterEndRows[chapter - 1]}");

            StoryTextRangeData[]? range = await _sheetManager.GetRangeAsync<StoryTextRangeData>($"{chapter}", startCell, endCell);
            if (range == null)
                return;

            _sceneStoryLookup ??= new Dictionary<string, IList<TranslatedTextData<StoryTextData>>>();
            _chapterScenesLookup ??= new Dictionary<int, HashSet<string>>();

            if (!_chapterScenesLookup.TryGetValue(chapter, out HashSet<string>? chapterScenes))
                _chapterScenesLookup[chapter] = chapterScenes = new HashSet<string>();

            for (var i = 0; i < range.Length; i++)
            {
                if (!_sceneStoryLookup.TryGetValue(range[i].SceneName, out IList<TranslatedTextData<StoryTextData>>? sceneTexts))
                    _sceneStoryLookup[range[i].SceneName] = sceneTexts = new List<TranslatedTextData<StoryTextData>>();

                var translatedText = new TranslatedTextData<StoryTextData>
                {
                    Row = i + 2,
                    Text = new StoryTextData
                    {
                        Name = range[i].EventName,
                        Index = range[i].Index,
                        Speaker = range[i].Speaker,
                        Text = range[i].Translation
                    }
                };

                sceneTexts.Add(translatedText);
                chapterScenes.Add(range[i].SceneName);
            }
        }
    }
}
