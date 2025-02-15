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
    public class DecisionPreview : BasePreview<TextData>
    {
        private DecisionPreviewData? _data;

        private readonly ITextRendererProvider _rendererProvider;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly IEventBroker _eventBroker;
        private readonly ITextCharacterReplacer _characterReplacer;

        public DecisionPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            ITextRendererProvider rendererProvider, IScreenResourceProvider screenProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _rendererProvider = rendererProvider;
            _screenProvider = screenProvider;
            _characterReplacer = characterReplacer;

            eventBroker.Subscribe<PreviewChangedMessage<DecisionPreviewData>>(ChangeDecisionPreview);
        }

        private static PreviewData<TextData> CreatePreviewData(DecisionPreviewData data)
        {
            return new PreviewData<TextData>
            {
                OriginalTexts = data.Texts,
                TranslatedTexts = data.TranslatedTexts
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

        private void ChangeDecisionPreview(PreviewChangedMessage<DecisionPreviewData> message)
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
            return _screenProvider.GetDecisionScreen();
        }

        private ITextRenderer? GetRenderer(AssetPreference fontPreference)
        {
            return _rendererProvider.GetDecisionRenderer(fontPreference);
        }

        private string? GetTextName()
        {
            return GetTextData()?.Name;
        }

        protected override void RaiseTextChanged()
        {
            _eventBroker.Raise(new TextChangedMessage(this, _data.Name));
        }

        protected override void RaiseIndexChanged()
        {
            _eventBroker.Raise(new SceneDecisionChangedMessage(this, GetTextName() ?? string.Empty));
        }
    }
}
