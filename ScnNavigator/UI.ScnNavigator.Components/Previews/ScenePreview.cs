using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Resources;
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
    public partial class ScenePreview : ImGui.Forms.Controls.Base.Component
    {
        private readonly ITextRendererProvider _rendererProvider;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly IEventBroker _eventBroker;
        private readonly ISceneTextNormalizer _textNormalizer;
        private readonly ITextCharacterReplacer _characterReplacer;

        private ScenePreviewData? _sceneData;

        protected int Index { get; private set; } = -1;

        public ScenePreview(ITextRendererProvider rendererProvider, IScreenResourceProvider screenProvider,
            ITextCharacterReplacer characterReplacer, IEventBroker eventBroker, ISceneTextNormalizer textNormalizer)
        {
            InitializeComponent();

            _rendererProvider = rendererProvider;
            _screenProvider = screenProvider;
            _eventBroker = eventBroker;
            _textNormalizer = textNormalizer;
            _characterReplacer = characterReplacer;

            _previousTextButton.Clicked += (_, _) => UpdateTextIndex(Index - 1);
            _nextTextButton.Clicked += (_, _) => UpdateTextIndex(Index + 1);

            _sceneNameTextBox.TextChanged += UpdateSceneName;
            _speakerNameTextBox.TextChanged += UpdateSpeakerName;
            _indexTextBox.TextChanged += UpdateIndex;
            _translationTextBox.TextChanged += UpdateText;

            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));
            eventBroker.Subscribe<PreviewChangedMessage<ScenePreviewData>>(ChangeScenePreview);
            eventBroker.Subscribe<SceneTextChangedMessage>(UpdateScene);
        }

        private void ChangeScenePreview(PreviewChangedMessage<ScenePreviewData> message)
        {
            if (message.Target != this)
                return;

            InitializeScene(message.PreviewData, message.Index);
        }

        private void InitializeScene(ScenePreviewData previewData, int index)
        {
            if (previewData.TranslatedTexts != null || previewData.TranslatedTitle != null)
            {
                if (_textLayout.Items.Count < 3)
                    _textLayout.Items.Add(_translationTextBox);

                _translationTextBox.IsReadOnly = false;
            }
            else
            {
                if (_textLayout.Items.Count >= 3)
                    _textLayout.Items.RemoveAt(2);

                _translationTextBox.IsReadOnly = true;
            }

            _sceneData = previewData;

            Index = index;

            UpdateArrowButtons(index);
            UpdateTextIndex(Index);
        }

        private void UpdateScene(SceneTextChangedMessage message)
        {
            if (message.Sender == this)
                return;

            if (message.SceneName != _sceneData?.SceneName)
                return;

            int textIndex = GetTextIndex();
            if (textIndex < 0)
                return;

            if (_sceneData.Texts[textIndex]?.Name != message.EventName)
                return;

            UpdateTextIndex(Index);
        }

        private void ToggleForm(bool enabled)
        {
            _textPreview.Enabled = enabled;

            _originalStoryTextBox.Enabled = enabled;
            _translationTextBox.Enabled = enabled;

            _sceneNameTextBox.Enabled = enabled;
            _speakerNameTextBox.Enabled = enabled;
            _indexTextBox.Enabled = enabled;

            if (!enabled)
            {
                _previousTextButton.Enabled = false;
                _nextTextButton.Enabled = false;
            }
            else
                UpdateArrowButtons(Index);
        }

        private void UpdateText(object? sender, string changedText)
        {
            if (IsSceneTitle())
            {
                _sceneData!.TranslatedTitle!.Text = changedText;
            }
            else
            {
                int index = GetTextIndex();
                if (index < 0)
                    return;

                StoryTextData? translatedText = _sceneData!.TranslatedTexts?[index];
                if (translatedText == null)
                    return;

                translatedText.Text = changedText;
            }

            RaiseSceneTextChanged();

            UpdatePreview();
        }

        private void UpdateIndex(object? sender, EventArgs e)
        {
            int index = GetTextIndex();
            if (index < 0)
                return;

            StoryTextData? translatedText = _sceneData!.TranslatedTexts?[index];
            if (translatedText == null)
                return;

            if (int.TryParse(_indexTextBox.Text, out int newTextIndex))
                translatedText.Index = newTextIndex;

            UpdatePreview();

            RaiseSceneTextChanged();
        }

        private void UpdateSceneName(object? sender, EventArgs e)
        {
            int index = GetTextIndex();
            if (index < 0)
                return;

            StoryTextData? translatedText = _sceneData!.TranslatedTexts?[index];
            if (translatedText == null)
                return;

            translatedText.Name = _sceneNameTextBox.Text;

            RaiseSceneTextChanged();
        }

        private void UpdateSpeakerName(object? sender, EventArgs e)
        {
            int index = GetTextIndex();
            if (index < 0)
                return;

            TextData? translatedSpeaker = _sceneData!.TranslatedSpeakers?[index];
            if (translatedSpeaker == null)
                return;

            translatedSpeaker.Text = _speakerNameTextBox.Text;

            UpdatePreview();

            string? originalSpeaker = _sceneData?.Texts[index]?.Speaker;
            if (originalSpeaker == null)
                return;

            RaiseSpeakerChanged(originalSpeaker);
        }

        private void UpdateTextIndex(int index)
        {
            if (index < 0)
                return;

            int maxIndex = GetMaxIndex();
            Index = Math.Clamp(index, 0, maxIndex - 1);

            UpdateArrowButtons(Index);

            bool isSceneTitle = IsSceneTitle();

            _indexTextBox.IsReadOnly = isSceneTitle;
            _sceneNameTextBox.IsReadOnly = isSceneTitle;
            _speakerNameTextBox.IsReadOnly = isSceneTitle;

            if (isSceneTitle)
            {
                SetSpeakerName(string.Empty);
                SetSceneName(string.Empty);
                SetTextData(_sceneData!.Title!, _sceneData.TranslatedTitle);
            }
            else
            {
                index = GetTextIndex();

                SetSpeakerName(_sceneData!.TranslatedSpeakers?[index]?.Text ?? _sceneData!.Texts[index].Speaker ?? string.Empty);
                SetSceneName(_sceneData.TranslatedTexts?[index].Name ?? _sceneData!.Texts[index].Name);
                SetTextData(_sceneData!.Texts[index], _sceneData.TranslatedTexts?[index]);
            }

            UpdatePreview();
        }

        private void UpdateArrowButtons(int index)
        {
            _previousTextButton.Enabled = index > 0;
            _nextTextButton.Enabled = index < GetMaxIndex() - 1;
        }

        private int GetMaxIndex()
        {
            if (_sceneData == null)
                return 0;

            int maxIndex = _sceneData.Texts.Length;
            if (_sceneData.Title != null)
                maxIndex++;

            return maxIndex;
        }

        private void SetSpeakerName(string speakerName)
        {
            _speakerNameTextBox.TextChanged -= UpdateSpeakerName;

            _speakerNameTextBox.Text = speakerName;

            _speakerNameTextBox.TextChanged += UpdateSpeakerName;
        }

        private void SetSceneName(string sceneName)
        {
            _sceneNameTextBox.TextChanged -= UpdateSceneName;

            _sceneNameTextBox.Text = sceneName;

            _sceneNameTextBox.TextChanged += UpdateSceneName;
        }

        private void SetTextData(StoryTextData textData, StoryTextData? translatedTextData)
        {
            _translationTextBox.TextChanged -= UpdateText;
            _indexTextBox.TextChanged -= UpdateIndex;

            _indexTextBox.Text = translatedTextData != null ? $"{translatedTextData.Index}" : $"{textData.Index}";
            _originalStoryTextBox.SetText(textData.Text);
            _translationTextBox.SetText(translatedTextData?.Text ?? string.Empty);

            _indexTextBox.TextChanged += UpdateIndex;
            _translationTextBox.TextChanged += UpdateText;
        }

        private void SetTextData(TextData textData, TextData? translatedTextData)
        {
            _translationTextBox.TextChanged -= UpdateText;
            _indexTextBox.TextChanged -= UpdateIndex;

            _indexTextBox.Text = string.Empty;
            _originalStoryTextBox.SetText(textData.Text);
            _translationTextBox.SetText(translatedTextData?.Text ?? string.Empty);

            _indexTextBox.TextChanged += UpdateIndex;
            _translationTextBox.TextChanged += UpdateText;
        }

        protected void UpdatePreview()
        {
            Image<Rgba32>? image = CreatePreview();
            if (image == null)
                return;

            _textPreview.Image = ImageResource.FromImage(image);
        }

        protected virtual Image<Rgba32>? CreatePreview()
        {
            string? text = GetPreviewText(out AssetPreference fontPreference);
            if (text == null)
                return null;

            Image<Rgba32> image = GetBackground();
            ITextRenderer? renderer = GetRenderer(text, fontPreference);
            ITextRenderer? routeRenderer = _rendererProvider.GetRouteNameRenderer(fontPreference);
            
            renderer?.Render(image, text);

            if (IsSceneTitle() && _sceneData!.TranslatedRoute?.Text != null)
                routeRenderer?.Render(image, _sceneData.TranslatedRoute.Text);

            return image;
        }

        private ITextRenderer? GetRenderer(string previewText, AssetPreference fontPreference)
        {
            if (IsSceneTitle())
                return _rendererProvider.GetSceneTitleRenderer(fontPreference);

            if (!IsSpeech(previewText))
                return _rendererProvider.GetNarrationRenderer(fontPreference);

            bool isIntroScene = IsIntroScene();
            return isIntroScene ?
                _rendererProvider.GetIntroSubtitleRenderer(fontPreference) :
                _rendererProvider.GetSubtitleRenderer(fontPreference);
        }

        private Image<Rgba32> GetBackground()
        {
            if (IsSceneTitle())
                return _screenProvider.GetSceneTitleScreen();

            int textIndex = GetTextIndex();
            string originalText = _sceneData!.Texts[textIndex].Text;
            if (IsNarration(originalText) || IsMentalDialogue(originalText))
                return _screenProvider.GetNarrationScreen();

            return _screenProvider.GetSubtitleScreen();
        }

        private int GetTextIndex()
        {
            if (_sceneData?.Title != null)
                return Index - 1;

            return Index;
        }

        private bool IsSceneTitle()
        {
            return _sceneData?.Title != null && Index == 0;
        }

        private bool IsIntroScene()
        {
            return _sceneData?.SceneName.StartsWith("A01") ?? false;
        }

        private bool IsMentalDialogue(string text)
        {
            return text.StartsWith("＊");
        }

        private bool IsNarration(string text)
        {
            return text.StartsWith("＊＊");
        }

        private bool IsSpeech(string text)
        {
            return text.Contains('「') || text.Contains('“');
        }

        private string? GetPreviewText(out AssetPreference fontPreference)
        {
            if (IsSceneTitle())
                return GetSceneTitlePreviewText(out fontPreference);

            return GetScenePreviewText(out fontPreference);
        }

        private string GetSceneTitlePreviewText(out AssetPreference fontPreference)
        {
            string? translatedText = _sceneData!.TranslatedTitle?.Text;
            string? originalText = _sceneData.Title!.Text;

            if (!string.IsNullOrEmpty(translatedText))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(translatedText);
            }

            fontPreference = AssetPreference.Original;
            return originalText ?? string.Empty;
        }

        private string? GetScenePreviewText(out AssetPreference fontPreference)
        {
            fontPreference = AssetPreference.Original;

            var result = new List<string?>();

            int index = GetTextIndex();

            int localIndex;
            do
            {
                localIndex = _sceneData!.TranslatedTexts?[index].Index ?? _sceneData.Texts[index].Index;

                string? normalizedText = GetNormalizedScenePreviewText(index, out AssetPreference localFontPreference);
                result.Add(normalizedText);

                if (localFontPreference == AssetPreference.Patch)
                    fontPreference = localFontPreference;

                index--;
            } while (localIndex > 0 && index >= 0);

            result.Reverse();

            return string.Join('\n', result);
        }

        private string? GetNormalizedScenePreviewText(int index, out AssetPreference fontPreference)
        {
            StoryTextData? translatedText = _sceneData!.TranslatedTexts?[index];
            StoryTextData? originalText = _sceneData.Texts[index];
            TextData? translatedSpeaker = _sceneData.TranslatedSpeakers?[index];

            fontPreference = string.IsNullOrEmpty(translatedText?.Text) ? AssetPreference.Original : AssetPreference.Patch;

            string? normalizedText = _textNormalizer.Normalize(originalText, translatedText, translatedSpeaker);
            if (normalizedText == null)
                return null;

            return _characterReplacer.ReplaceCharacters(normalizedText);
        }

        private string? GetEventName()
        {
            if (IsSceneTitle())
                return _sceneData!.SceneName;

            int textIndex = GetTextIndex();
            return _sceneData!.Texts[textIndex]?.Name
                   ?? _sceneData.TranslatedTexts?[textIndex]?.Name;
        }

        private void RaiseSceneTextChanged()
        {
            _eventBroker.Raise(new SceneTextChangedMessage(this, _sceneData!.SceneName, GetEventName() ?? string.Empty));
        }

        private void RaiseSpeakerChanged(string speaker)
        {
            _eventBroker.Raise(new SpeakerChangedMessage(_sceneData!.SceneName, GetEventName() ?? string.Empty, speaker));
        }
    }
}
