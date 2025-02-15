using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.Font;

namespace Logic.Business.TimeTravelersManagement.Resources
{
    internal class FontProvider : IFontProvider
    {
        private readonly IBasePathProvider _basePathProvider;
        private readonly IGamePathProvider _pathProvider;
        private readonly IFontParser _fontParser;

        private readonly Dictionary<string, FontImageData> _fontCache = new();

        public FontProvider(IBasePathProvider basePathProvider, IGamePathProvider pathProvider, IFontParser fontParser)
        {
            _basePathProvider = basePathProvider;
            _pathProvider = pathProvider;
            _fontParser = fontParser;
        }

        public FontImageData? GetMainFont(AssetPreference preference)
        {
            return GetOrParseFont("nrm_main", preference);
        }

        public FontImageData? GetSubtitleFont(AssetPreference preference)
        {
            return GetOrParseFont("nrm_main", preference);
        }

        public FontImageData? GetTitleFont(AssetPreference preference)
        {
            return GetOrParseFont("telop_main", preference);
        }

        public FontImageData? GetRouteFont(AssetPreference preference)
        {
            return GetOrParseFont("telop_player", preference);
        }

        public FontImageData? GetStaffrollFont(AssetPreference preference)
        {
            return GetOrParseFont("staffroll", preference);
        }

        private FontImageData? GetOrParseFont(string fontName, AssetPreference preference)
        {
            string fontPath = GetFontPath(fontName, preference);

            if (_fontCache.TryGetValue(fontPath, out FontImageData? font))
                return font;

            FontImageData? fontData = ParseFontData(fontPath);
            if (fontData == null)
                return null;

            return _fontCache[fontPath] = fontData;
        }

        private FontImageData? ParseFontData(string fontPath)
        {
            if (!File.Exists(fontPath))
                return null;

            using Stream fontStream = File.OpenRead(fontPath);

            return _fontParser.Parse(fontStream);
        }

        private string GetFontPath(string name, AssetPreference preference)
            => _basePathProvider.GetFullPath(_pathProvider.GetFontFilePath(name), preference);
    }
}
