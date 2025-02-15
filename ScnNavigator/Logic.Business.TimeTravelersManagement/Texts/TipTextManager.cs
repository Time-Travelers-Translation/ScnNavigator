using CrossCutting.Abstract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;

namespace Logic.Business.TimeTravelersManagement.Texts
{
    internal abstract class TipTextManager : ITipTextManager
    {
        private readonly IGamePathProvider _pathProvider;
        private readonly IEventTextParser _eventTextParser;
        private readonly IConfigurationReader<RawConfigurationEntry> _configReader;

        private readonly Dictionary<int, TextData> _titleCache = new();
        private readonly Dictionary<int, TextData> _textCache = new();

        public TipTextManager(IGamePathProvider pathProvider, IEventTextParser eventTextParser,
            IConfigurationReader<RawConfigurationEntry> configReader)
        {
            _pathProvider = pathProvider;
            _eventTextParser = eventTextParser;
            _configReader = configReader;
        }

        public TextData? GetTitle(int tipIndex)
        {
            if (_titleCache.TryGetValue(tipIndex, out TextData? cachedTitle))
                return cachedTitle;

            string path = GetFullPath(_pathProvider.GetTipTitleFilePath());
            EventTextConfiguration? eventConfig = ReadEventTexts(path);
            if (eventConfig == null)
                return null;

            for (var i = 0; i < eventConfig.Texts.Length; i++)
                _titleCache[i + 1] = new TextData { Text = eventConfig.Texts[i].Text };

            return _titleCache[tipIndex];
        }

        public TextData? GetText(int tipIndex)
        {
            if (_textCache.TryGetValue(tipIndex, out TextData? cachedText))
                return cachedText;
            
            string path = GetFullPath(_pathProvider.GetTipTextFilePath(tipIndex));
            EventTextConfiguration? eventConfig = ReadEventTexts(path);
            if (eventConfig == null)
                return null;

            return _textCache[tipIndex] = new TextData { Text = eventConfig.Texts[0].Text };
        }

        protected abstract string GetFullPath(string relativePath);

        private EventTextConfiguration? ReadEventTexts(string filePath)
        {
            using Stream titleFileStream = File.OpenRead(filePath);

            try
            {
                Configuration<RawConfigurationEntry> config = _configReader.Read(titleFileStream, StringEncoding.Sjis);
                return _eventTextParser.Parse(config);
            }
            catch
            {
                return null;
            }
        }
    }
}
