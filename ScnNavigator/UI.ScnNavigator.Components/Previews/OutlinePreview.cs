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
    public class OutlinePreview : BasePreview<TextData>
    {
        private readonly IEventBroker _eventBroker;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly ITextRendererProvider _rendererProvider;
        private readonly ITextCharacterReplacer _characterReplacer;

        public OutlinePreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            IScreenResourceProvider screenProvider, ITextRendererProvider rendererProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _rendererProvider = rendererProvider;
            _screenProvider = screenProvider;
            _characterReplacer = characterReplacer;

            eventBroker.Subscribe<PreviewChangedMessage<OutlinePreviewData>>(ChangeOutlinePreview);
        }

        private static PreviewData<TextData> CreatePreviewData(OutlinePreviewData data)
        {
            return new PreviewData<TextData>
            {
                OriginalTexts = data.Texts,
                TranslatedTexts = data.TranslatedTexts ?? Array.Empty<TextData>()
            };
        }

        protected override Image<Rgba32> RenderPreview()
        {
            Image<Rgba32> image = GetBackground();

            string? text = GetPreviewText(out AssetPreference fontPreference);
            if (text == null)
                return image;

            ITextRenderer? renderer = GetRenderer(fontPreference);

            text = _characterReplacer.ReplaceCharacters(text);
            renderer?.Render(image, text);

            return image;
        }

        private void ChangeOutlinePreview(PreviewChangedMessage<OutlinePreviewData> message)
        {
            if (message.Target != this)
                return;

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
            return _screenProvider.GetOutlineScreen();
        }

        private ITextRenderer? GetRenderer(AssetPreference fontPreference)
        {
            return _rendererProvider.GetOutlineRenderer(fontPreference);
        }

        protected override void RaiseTextChanged()
        {
            _eventBroker.Raise(new OutlineChangedMessage(this));
        }
    }
}
