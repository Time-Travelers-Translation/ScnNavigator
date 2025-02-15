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
    internal class TutorialPreview : BasePreview<TextData>
    {
        private TutorialPreviewData? _data;

        private readonly IEventBroker _eventBroker;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly ITextRendererProvider _rendererProvider;
        private readonly ITextCharacterReplacer _characterReplacer;

        public TutorialPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            IScreenResourceProvider screenProvider, ITextRendererProvider rendererProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _screenProvider = screenProvider;
            _rendererProvider = rendererProvider;
            _characterReplacer = characterReplacer;

            eventBroker.Subscribe<PreviewChangedMessage<TutorialPreviewData>>(ChangeTutorialPreview);
        }

        protected override Image<Rgba32> RenderPreview()
        {
            Image<Rgba32> backgroundImage = GetBackground();

            RenderText(backgroundImage);

            return backgroundImage;
        }

        protected override void RaiseTextChanged()
        {
            _eventBroker.Raise(new TutorialChangedMessage(this, _data.Id));
        }

        private void ChangeTutorialPreview(PreviewChangedMessage<TutorialPreviewData> message)
        {
            if (message.Target != this)
                return;

            _data = message.PreviewData;

            InitializeData(CreatePreviewData(message.PreviewData), message.Index);
        }

        private static PreviewData<TextData> CreatePreviewData(TutorialPreviewData data)
        {
            var originalTexts = new TextData[data.TutorialTexts.Length + 1];

            originalTexts[0] = data.TutorialTitle;
            for (var i = 0; i < data.TutorialTexts.Length; i++)
                originalTexts[i + 1] = data.TutorialTexts[i];

            TextData?[]? translatedTexts = null;
            if (data is { TranslatedTutorialTexts: { }, TranslatedTutorialTitle: { } })
            {
                translatedTexts = new TextData[data.TranslatedTutorialTexts.Length + 1];

                translatedTexts[0] = data.TranslatedTutorialTitle;
                for (var i = 0; i < data.TranslatedTutorialTexts.Length; i++)
                    translatedTexts[i + 1] = data.TranslatedTutorialTexts[i];
            }

            return new PreviewData<TextData>
            {
                OriginalTexts = originalTexts,
                TranslatedTexts = translatedTexts
            };
        }

        private void RenderText(Image<Rgba32> bitmap)
        {
            string text = GetPreviewText(out AssetPreference fontPreference);
            if (string.IsNullOrEmpty(text))
                return;

            ITextRenderer? textRenderer = GetTextRenderer(fontPreference);
            
            textRenderer?.Render(bitmap, text);
        }

        private Image<Rgba32> GetBackground()
        {
            if (_data.Id == 28)
                return _screenProvider.GetRevealTutorialScreen();

            if (_data.Id == 29)
                return _screenProvider.GetTimeLockTutorialScreen();

            return _screenProvider.GetTutorialScreen();
        }

        private ITextRenderer? GetTextRenderer(AssetPreference fontPreference)
        {
            if (_data.Id == 28)
                return _rendererProvider.GetRevealTutorialTextRenderer(fontPreference, AssetPreference.PatchOrOriginal);

            if (_data.Id == 29)
                return _rendererProvider.GetTimeLockTutorialTextRenderer(fontPreference, AssetPreference.PatchOrOriginal);

            return _rendererProvider.GetTutorialTextRenderer(fontPreference, AssetPreference.PatchOrOriginal);
        }

        private string GetPreviewText(out AssetPreference fontPreference)
        {
            string? translatedText;
            string? originalText;

            if (Index == 0)
            {
                translatedText = _data.TranslatedTutorialTexts?[0]?.Text;
                originalText = _data.TutorialTexts[0].Text;
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
    }
}
