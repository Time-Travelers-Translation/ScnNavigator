using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Layouts.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.InternalContract.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement
{
    internal class TextLayoutCreatorProvider : ITextLayoutCreatorProvider
    {
        private readonly ICoCoKernel _kernel;
        private readonly ICharacterParserProvider _characterParserProvider;
        private readonly IFontProvider _fontProvider;

        public TextLayoutCreatorProvider(ICoCoKernel kernel, ICharacterParserProvider characterParserProvider, IFontProvider fontProvider)
        {
            _kernel = kernel;
            _characterParserProvider = characterParserProvider;
            _fontProvider = fontProvider;
        }

        public ITextLayoutCreator? GetIntroSubtitleLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                VerticalAlignment = VerticalTextAlignment.Bottom,
                InitPoint = new Point(0, 15),
                LineHeight = 21,
                LineWidth = 0
            };

            ICharacterParser characterParser = _characterParserProvider.GetSubtitleParser();
            return GetTextLayoutCreator<ISubtitleTextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetSubtitleLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                VerticalAlignment = VerticalTextAlignment.Bottom,
                InitPoint = new Point(0, 15),
                LineHeight = 21,
                LineWidth = 286
            };

            ICharacterParser characterParser = _characterParserProvider.GetSubtitleParser();
            return GetTextLayoutCreator<ISubtitleTextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetNarrationLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Left,
                VerticalAlignment = VerticalTextAlignment.Top,
                InitPoint = new Point(16, 1),
                LineHeight = 25,
                LineWidth = 286
            };

            ICharacterParser characterParser = _characterParserProvider.GetNarrationParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetDecisionLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                VerticalAlignment = VerticalTextAlignment.Center,
                LineHeight = 25,
                LineWidth = 258
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetBadEndTitleLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                InitPoint = new Point(0, 89),
                LineHeight = 25,
                LineWidth = 240
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetBadEndHintLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                InitPoint = new Point(61, 96),
                LineHeight = 25,
                LineWidth = 200,
                HorizontalAlignment = HorizontalTextAlignment.Center
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<IHintTextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetSceneTitleLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetTitleFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                InitPoint = new Point(1, 107),
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetRouteNameLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetRouteFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                InitPoint = new Point(1, 80)
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetOutlineLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                LineHeight = 25,
                LineWidth = 280,
                InitPoint = new Point(16, 0)
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetTipTitleLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                InitPoint = new Point(78, 11)
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetTipTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                LineWidth = 369,
                LineHeight = 25,
                InitPoint = new Point(16, 40),
                ResourcePreference = resourcePreference
            };

            ICharacterParser characterParser = _characterParserProvider.GetTipParser();
            return GetTextLayoutCreator<ITipTextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetHelpTitleLayoutCreator(AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                InitPoint = new Point(151, 29)
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetHelpTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                LineWidth = 344,
                LineHeight = 25,
                InitPoint = new Point(44, 56),
                ResourcePreference = resourcePreference
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetTutorialTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                LineWidth = 283,
                LineHeight = 20,
                InitPoint = new Point(99, 90),
                ResourcePreference = resourcePreference
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetRevealTutorialTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                VerticalAlignment = VerticalTextAlignment.Center,
                HorizontalAlignment = HorizontalTextAlignment.Center,
                LineWidth = 400,
                LineHeight = 23,
                TextSpacing = 0,
                ResourcePreference = resourcePreference
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetTimeLockTutorialTextLayoutCreator(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            FontImageData? fontImage = _fontProvider.GetMainFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                VerticalAlignment = VerticalTextAlignment.Center,
                HorizontalAlignment = HorizontalTextAlignment.Center,
                LineWidth = 400,
                LineHeight = 24,
                ResourcePreference = resourcePreference
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetStaffrollTitleLayoutCreator(int y, AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetStaffrollFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                LineWidth = 400,
                TextScale = .82f,
                InitPoint = new Point(0, y + 3)
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        public ITextLayoutCreator? GetStaffrollNameLayoutCreator(int y, AssetPreference fontPreference)
        {
            FontImageData? fontImage = _fontProvider.GetStaffrollFont(fontPreference);
            if (fontImage == null)
                return null;

            var options = new LayoutOptions
            {
                HorizontalAlignment = HorizontalTextAlignment.Center,
                LineWidth = 400,
                InitPoint = new Point(0, y)
            };

            ICharacterParser characterParser = _characterParserProvider.GetDefaultParser();
            return GetTextLayoutCreator<ITextLayoutCreator>(fontImage, options, characterParser);
        }

        private TLayoutCreator GetTextLayoutCreator<TLayoutCreator>(FontImageData fontImage, LayoutOptions options, ICharacterParser characterParser)
            where TLayoutCreator : class
        {
            return _kernel.Get<TLayoutCreator>(
                new ConstructorParameter("fontData", fontImage),
                new ConstructorParameter("options", options),
                new ConstructorParameter("characterParser", characterParser));
        }
    }
}
