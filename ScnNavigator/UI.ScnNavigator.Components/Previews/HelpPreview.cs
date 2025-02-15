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
    internal class HelpPreview : BasePreview<TextData>
    {
        private HelpPreviewData? _data;

        private readonly IEventBroker _eventBroker;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly ITextRendererProvider _rendererProvider;
        private readonly ITextCharacterReplacer _characterReplacer;

        public HelpPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            IScreenResourceProvider screenProvider, ITextRendererProvider rendererProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _screenProvider = screenProvider;
            _rendererProvider = rendererProvider;
            _characterReplacer = characterReplacer;

            eventBroker.Subscribe<PreviewChangedMessage<HelpPreviewData>>(ChangeHelpPreview);
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
            _eventBroker.Raise(new HelpChangedMessage(this, _data.Id));
        }

        private void ChangeHelpPreview(PreviewChangedMessage<HelpPreviewData> message)
        {
            if (message.Target != this)
                return;

            _data = message.PreviewData;

            InitializeData(CreatePreviewData(message.PreviewData), message.Index);
        }

        private static PreviewData<TextData> CreatePreviewData(HelpPreviewData data)
        {
            var originalTexts = new TextData[data.HelpTexts.Length + 1];

            originalTexts[0] = data.HelpTitle;
            for (var i = 0; i < data.HelpTexts.Length; i++)
                originalTexts[i + 1] = data.HelpTexts[i];

            TextData?[]? translatedTexts = null;
            if (data is { TranslatedHelpTexts: { }, TranslatedHelpTitle: { } })
            {
                translatedTexts = new TextData[data.TranslatedHelpTexts.Length + 1];

                translatedTexts[0] = data.TranslatedHelpTitle;
                for (var i = 0; i < data.TranslatedHelpTexts.Length; i++)
                    translatedTexts[i + 1] = data.TranslatedHelpTexts[i];
            }

            return new PreviewData<TextData>
            {
                OriginalTexts = originalTexts,
                TranslatedTexts = translatedTexts
            };
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

            text = _characterReplacer.ReplaceCharacters(text);
            ITextRenderer? textRenderer = GetTextRenderer(fontPreference);

            textRenderer?.Render(bitmap, text);
        }

        private Image<Rgba32> GetBackground()
        {
            return _screenProvider.GetHelpScreen();
        }

        private string? GetPreviewTitle(out AssetPreference fontPreference)
        {
            string? translatedText = _data.TranslatedHelpTitle?.Text;
            string? originalText = _data.HelpTitle.Text;

            if (!string.IsNullOrEmpty(translatedText))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(translatedText);
            }

            fontPreference = AssetPreference.Original;
            return originalText ?? string.Empty;
        }

        private string? GetPreviewText(out AssetPreference fontPreference)
        {
            string? translatedText;
            string? originalText;

            if (Index == 0)
            {
                translatedText = _data.TranslatedHelpTexts?[0]?.Text;
                originalText = _data.HelpTexts[0].Text;
            }
            else
            {
                translatedText = GetTranslatedTextData()?.Text;
                originalText = GetTextData()?.Text;
            }

            if (!string.IsNullOrEmpty(translatedText))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(translatedText);
            }

            fontPreference = AssetPreference.Original;
            return originalText ?? string.Empty;
        }

        private ITextRenderer? GetTitleRenderer(AssetPreference fontPreference)
        {
            return _rendererProvider.GetHelpTitleRenderer(fontPreference);
        }

        private ITextRenderer? GetTextRenderer(AssetPreference fontPreference)
        {
            return _rendererProvider.GetHelpTextRenderer(fontPreference, AssetPreference.PatchOrOriginal);
        }
    }
}
