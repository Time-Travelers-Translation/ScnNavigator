using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Renderers;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Resources.Contract;
using UI.ScnNavigator.Components.Contract.Texts;
using Logic.Business.TimeTravelersManagement.Contract.Enums;

namespace UI.ScnNavigator.Components.Previews
{
    public class TipPreview : BasePreview<TextData>
    {
        private TipPreviewData? _data;

        private readonly IEventBroker _eventBroker;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly ITextRendererProvider _rendererProvider;
        private readonly ITextCharacterReplacer _characterReplacer;

        public TipPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            IScreenResourceProvider screenProvider, ITextRendererProvider rendererProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _screenProvider = screenProvider;
            _rendererProvider = rendererProvider;
            _characterReplacer = characterReplacer;

            eventBroker.Subscribe<PreviewChangedMessage<TipPreviewData>>(ChangeTipPreview);
        }

        protected override Image<Rgba32> RenderPreview()
        {
            Image<Rgba32> backgroundImage = GetBackground();

            RenderTitle(backgroundImage);
            RenderText(backgroundImage);

            return backgroundImage;
        }

        protected override void RaiseTextChanged()
        {
            _eventBroker.Raise(new TipChangedMessage(this, _data.Id, Index == 0));
        }

        private static PreviewData<TextData> CreatePreviewData(TipPreviewData data)
        {
            return new PreviewData<TextData>
            {
                OriginalTexts = new[] { data.TipTitle, data.TipText },
                TranslatedTexts = new[] { data.TranslatedTipTitle, data.TranslatedTipText }
            };
        }

        private void ChangeTipPreview(PreviewChangedMessage<TipPreviewData> message)
        {
            if (message.Target != this)
                return;

            _data = message.PreviewData;

            InitializeData(CreatePreviewData(message.PreviewData), message.Index);
        }

        private void RenderTitle(Image<Rgba32> bitmap)
        {
            string? text = GetPreviewTitle(out AssetPreference fontPreference);
            if (string.IsNullOrEmpty(text))
                return;

            ITextRenderer? titleRenderer = GetTitleRenderer(fontPreference);
            
            titleRenderer?.Render(bitmap, text);
        }

        private void RenderText(Image<Rgba32> bitmap)
        {
            string? text = GetPreviewText(out AssetPreference fontPreference);
            if (string.IsNullOrEmpty(text))
                return;

            ITextRenderer? textRenderer = GetTextRenderer(fontPreference);
            
            textRenderer?.Render(bitmap, text);
        }

        private Image<Rgba32> GetBackground()
        {
            return _screenProvider.GetTipScreen();
        }

        private ITextRenderer? GetTitleRenderer(AssetPreference fontPreference)
        {
            return _rendererProvider.GetTipTitleRenderer(fontPreference);
        }

        private ITextRenderer? GetTextRenderer(AssetPreference fontPreference)
        {
            return _rendererProvider.GetTipTextRenderer(fontPreference, AssetPreference.PatchOrOriginal);
        }

        private string GetPreviewTitle(out AssetPreference fontPreference)
        {
            string? translatedText = _data.TranslatedTipTitle?.Text;
            string? originalText = _data.TipTitle.Text;

            if (!string.IsNullOrEmpty(translatedText))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(translatedText);
            }

            fontPreference = AssetPreference.Original;
            return originalText ?? string.Empty;
        }

        private string GetPreviewText(out AssetPreference fontPreference)
        {
            string? translatedText = _data.TranslatedTipText?.Text;
            string? originalText = _data.TipText.Text;

            if (!string.IsNullOrEmpty(translatedText))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(translatedText);
            }

            fontPreference = AssetPreference.Original;
            return originalText ?? string.Empty;
        }
    }
}
