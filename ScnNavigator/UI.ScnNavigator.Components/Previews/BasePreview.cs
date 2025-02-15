using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;

namespace UI.ScnNavigator.Components.Previews
{
    public abstract partial class BasePreview<TTextData> : ImGui.Forms.Controls.Base.Component
        where TTextData : TextData
    {
        private PreviewData<TTextData>? _data;

        protected int Index { get; private set; } = -1;

        public BasePreview(IEventBroker eventBroker)
        {
            InitializeComponent();

            _previousTextButton!.Clicked += (_, _) => UpdateTextIndex(Index - 1);
            _nextTextButton!.Clicked += (_, _) => UpdateTextIndex(Index + 1);

            _translationTextBox!.TextChanged += UpdateText;

            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));
        }

        protected abstract Image<Rgba32> RenderPreview();

        protected TTextData? GetTextData()
        {
            return _data?.OriginalTexts[Index];
        }

        protected TTextData? GetTranslatedTextData()
        {
            return _data?.TranslatedTexts?.Length > Index ? _data.TranslatedTexts[Index] : null;
        }

        protected void InitializeData(PreviewData<TTextData> data, int index)
        {
            _data = data;

            if (data.TranslatedTexts?.Length > 0)
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

            Index = index;

            UpdateTextIndex(index);
        }

        private void ToggleForm(bool enabled)
        {
            _textPreview.Enabled = enabled;

            _originalTextBox.Enabled = enabled;
            _translationTextBox.Enabled = enabled;

            UpdateArrowButtons(enabled ? Index : -1);
        }

        private void UpdateArrowButtons(int index)
        {
            _previousTextButton.Enabled = _data != null && index > 0 && _data.OriginalTexts.Length > 1;
            _nextTextButton.Enabled = _data != null && index >= 0 && index < _data.OriginalTexts.Length - 1 && _data.OriginalTexts.Length > 1;
        }

        private void UpdateTextIndex(int index)
        {
            index = Math.Clamp(index, 0, _data.OriginalTexts.Length - 1);
            UpdateArrowButtons(index);

            bool wasChanged = Index != index;
            Index = index;

            TTextData? originalText = GetTextData();
            TTextData? translatedText = GetTranslatedTextData();
            SetTextData(originalText, translatedText);

            UpdatePreview();

            if (wasChanged)
                RaiseIndexChanged();
        }

        private void UpdateText(object? sender, string changedText)
        {
            TTextData? translatedText = GetTranslatedTextData();
            if (translatedText == null)
                return;

            translatedText.Text = changedText;

            UpdatePreview();

            RaiseTextChanged();
        }

        protected void UpdatePreview()
        {
            Image<Rgba32> previewImage = RenderPreview();

            _textPreview.Image = ImageResource.FromImage(previewImage);
        }

        private void SetTextData(TTextData? textData, TTextData? translatedTextData)
        {
            _translationTextBox.TextChanged -= UpdateText;

            _originalTextBox.SetText(textData?.Text ?? string.Empty);
            _translationTextBox.SetText(translatedTextData?.Text ?? string.Empty);

            _translationTextBox.TextChanged += UpdateText;
        }

        protected virtual void RaiseTextChanged() { }

        protected virtual void RaiseIndexChanged() { }
    }
}
