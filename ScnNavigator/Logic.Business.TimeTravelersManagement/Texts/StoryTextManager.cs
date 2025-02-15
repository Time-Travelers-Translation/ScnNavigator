using CrossCutting.Abstract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Domain.Kuriimu2.KryptographyAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Cryptography;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Scene;

namespace Logic.Business.TimeTravelersManagement.Texts
{
    internal abstract class StoryTextManager : IStoryTextManager
    {
        private readonly IPckReader _pckReader;
        private readonly IConfigurationReader<RawConfigurationEntry> _configReader;
        private readonly IEventTextParser _eventParser;
        private readonly IStoryboardManager _storyboardManager;
        private readonly IFloReader _flowReader;
        private readonly IGamePathProvider _pathProvider;

        private readonly IChecksum<uint> _crc32;
        private readonly IChecksum<uint> _jamCrc32;

        private readonly IDictionary<string, IDictionary<uint, Stream>> _pckLookup;
        private readonly IDictionary<string, IDictionary<uint, EventText[]>> _configLookup;

        private IDictionary<string, string>? _titleLookup;

        public StoryTextManager(IPckReader pckReader, IConfigurationReader<RawConfigurationEntry> configReader, IEventTextParser eventParser,
            IChecksumFactory checksumFactory, IStoryboardManager storyboardManager, IFloReader flowReader, IGamePathProvider pathProvider)
        {
            _pckReader = pckReader;
            _configReader = configReader;
            _eventParser = eventParser;
            _storyboardManager = storyboardManager;
            _flowReader = flowReader;
            _pathProvider = pathProvider;

            _crc32 = checksumFactory.CreateCrc32();
            _jamCrc32 = checksumFactory.CreateCrc32Jam();

            _pckLookup = new Dictionary<string, IDictionary<uint, Stream>>();
            _configLookup = new Dictionary<string, IDictionary<uint, EventText[]>>();
        }

        public TextData? GetTitleText(string sceneName)
        {
            _titleLookup ??= ReadFlow();

            if (!_titleLookup.TryGetValue(sceneName, out string? title))
                return null;

            return new TextData
            {
                Name = sceneName,
                Text = title
            };
        }

        public StoryTextData[] GetStoryTexts(string sceneName)
        {
            EventText[]? textEntries = GetConfigurationEntries(sceneName);
            if (textEntries == null)
                return Array.Empty<StoryTextData>();

            string[] textIdentifiers = _storyboardManager.GetStoryTextIdentifiers(sceneName);
            IDictionary<uint, EventText[]> eventTextLookup = textEntries.GroupBy(x => x.Hash).ToDictionary(x => x.Key, x => x.OrderBy(y => y.SubId).ToArray());

            var result = new List<StoryTextData>();
            foreach (string textIdentifier in textIdentifiers)
            {
                uint identifierHash = _jamCrc32.ComputeValue(textIdentifier);

                if (!eventTextLookup.TryGetValue(identifierHash, out EventText[]? identifiedTextEntries))
                    continue;

                foreach (EventText textEntry in identifiedTextEntries)
                {
                    if (string.IsNullOrEmpty(textEntry.Text))
                        continue;

                    string? speakerText = null;
                    if (textEntry.Text.StartsWith("＊＊"))
                        speakerText = "＊＊";
                    else if (textEntry.Text.StartsWith('＊'))
                        speakerText = "＊";
                    else
                    {
                        int speakerEndIndex = textEntry.Text.IndexOf('「');
                        if (speakerEndIndex > 0)
                            speakerText = textEntry.Text[..speakerEndIndex];
                    }

                    result.Add(new StoryTextData
                    {
                        Name = textIdentifier,
                        Index = textEntry.SubId,
                        Text = textEntry.Text,
                        Speaker = speakerText
                    });
                }
            }

            return result.ToArray();
        }

        protected abstract string GetFullPath(string resourcePath);

        private EventText[]? GetConfigurationEntries(string sceneName)
        {
            string identifier = sceneName[..3];
            uint hash = _crc32.ComputeValue(sceneName);

            EventText[]? textEntries;
            if (_configLookup.TryGetValue(identifier, out IDictionary<uint, EventText[]>? textEntryLookup))
            {
                if (textEntryLookup.TryGetValue(hash, out textEntries))
                    return textEntries;
            }

            if (!_pckLookup.TryGetValue(identifier, out IDictionary<uint, Stream>? archiveEntryLookup))
            {
                IDictionary<uint, Stream>? pckEntries = ReadPckFile(identifier);
                if (pckEntries == null)
                    return null;

                archiveEntryLookup = _pckLookup[identifier] = pckEntries;
            }

            if (!archiveEntryLookup.TryGetValue(hash, out Stream? archiveStream))
                return null;

            textEntries = ReadConfig(archiveStream);

            _configLookup[identifier] = new Dictionary<uint, EventText[]>();
            _configLookup[identifier][hash] = textEntries;

            return textEntries;
        }

        private IDictionary<uint, Stream>? ReadPckFile(string identifier)
        {
            string pckFile = GetFullPath(_pathProvider.GetEventPckFilePath(identifier));
            if (!File.Exists(pckFile))
                return null;

            using Stream pckStream = File.OpenRead(pckFile);
            IList<HashArchiveEntry> archiveEntries = _pckReader.Read(pckStream);

            var result = new Dictionary<uint, Stream>();
            foreach (HashArchiveEntry entry in archiveEntries)
            {
                result[entry.Hash] = new MemoryStream();
                entry.Content.CopyTo(result[entry.Hash]);
            }

            return result;
        }

        private EventText[] ReadConfig(Stream input)
        {
            input.Position = 0;
            Configuration<RawConfigurationEntry> config = _configReader.Read(input, StringEncoding.Sjis);

            return _eventParser.Parse(config).Texts;
        }

        private IDictionary<string, string> ReadFlow()
        {
            string flowPath = GetFullPath(_pathProvider.GetFlowFilePath());
            if (!File.Exists(flowPath))
                return new Dictionary<string, string>();

            using Stream floStream = File.OpenRead(flowPath);
            FloData floData = _flowReader.Read(floStream);

            var result = new Dictionary<string, string>();
            foreach (FloTitleData floTitle in floData.Titles)
            {
                if (!string.IsNullOrEmpty(floTitle.Text))
                    result[floTitle.SceneName] = floTitle.Text;
            }

            return result;
        }
    }
}
