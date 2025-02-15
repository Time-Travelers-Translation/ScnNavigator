using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Renderers;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Texts;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Components.Previews
{
    public class BadEndPreview : BasePreview<TextData>
    {
        private BadEndPreviewData? _data;

        private readonly IEventBroker _eventBroker;
        private readonly ITextCharacterReplacer _characterReplacer;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly ITextRendererProvider _rendererProvider;

        public BadEndPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            IScreenResourceProvider screenProvider, ITextRendererProvider rendererProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _characterReplacer = characterReplacer;
            _screenProvider = screenProvider;
            _rendererProvider = rendererProvider;

            eventBroker.Subscribe<PreviewChangedMessage<BadEndPreviewData>>(ChangeBadEndPreview);
        }

        private static PreviewData<TextData> CreatePreviewData(BadEndPreviewData data)
        {
            return new PreviewData<TextData>
            {
                OriginalTexts = new[] { data.TitleText, data.HintText },
                TranslatedTexts = new[] { data.TranslatedTitleText, data.TranslatedHintText }
            };
        }

        protected override Image<Rgba32> RenderPreview()
        {
            Image<Rgba32> image = GetBackground();

            string? text = GetPreviewText(out AssetPreference fontPreference);
            if (text == null)
                return image;

            ITextRenderer? renderer = GetRenderer(fontPreference);

            renderer?.Render(image, text);
            
            return image;
        }

        private void ChangeBadEndPreview(PreviewChangedMessage<BadEndPreviewData> message)
        {
            if (message.Target != this)
                return;

            _data = message.PreviewData;

            InitializeData(CreatePreviewData(message.PreviewData), message.Index);
        }

        private string? GetPreviewText(out AssetPreference fontPreference)
        {
            string? translatedText = GetTranslatedTextData()?.Text;
            string? originalText = GetTextData()?.Text;

            if (!string.IsNullOrEmpty(translatedText))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(translatedText);
            }

            fontPreference = AssetPreference.Original;
            return originalText ?? string.Empty;
        }

        private Image<Rgba32> GetBackground()
        {
            if (Index == 0)
                return _screenProvider.GetBadEndTitleScreen();

            return _screenProvider.GetBadEndHintScreen();
        }

        private ITextRenderer? GetRenderer(AssetPreference fontPreference)
        {
            if (Index == 0)
                return _rendererProvider.GetBadEndTitleRenderer(fontPreference);

            return _rendererProvider.GetBadEndHintRenderer(fontPreference);
        }

        protected override void RaiseTextChanged()
        {
            _eventBroker.Raise(new TextChangedMessage(this, _data.Name));
        }
    }
}
