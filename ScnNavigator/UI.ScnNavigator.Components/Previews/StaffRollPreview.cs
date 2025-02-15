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
    public class StaffRollPreview : BasePreview<StaffrollTextData>
    {
        private StaffRollPreviewData? _data;

        private readonly IEventBroker _eventBroker;
        private readonly ITextRendererProvider _rendererProvider;
        private readonly IScreenResourceProvider _screenProvider;
        private readonly ITextCharacterReplacer _characterReplacer;

        public StaffRollPreview(IEventBroker eventBroker, ITextCharacterReplacer characterReplacer,
            ITextRendererProvider rendererProvider, IScreenResourceProvider screenProvider)
            : base(eventBroker)
        {
            _eventBroker = eventBroker;
            _rendererProvider = rendererProvider;
            _screenProvider = screenProvider;
            _characterReplacer = characterReplacer;

            eventBroker.Subscribe<PreviewChangedMessage<StaffRollPreviewData>>(ChangeStaffrollPreview);
        }

        protected override Image<Rgba32> RenderPreview()
        {
            Image<Rgba32> staffScreen = GetBackground();

            for (var i = 0; i < 14; i++)
            {
                string? text = GetPreviewText(Index + i, out AssetPreference fontPreference);
                if (text == null)
                    continue;

                ITextRenderer? staffRenderer = GetPreviewFlag(Index + i) == 0
                    ? _rendererProvider.GetStaffrollNameRenderer(i * 20, fontPreference)
                    : _rendererProvider.GetStaffrollTitleRenderer(i * 20, fontPreference);
                
                staffRenderer?.Render(staffScreen, text);
            }

            return staffScreen;
        }

        protected override void RaiseTextChanged()
        {
            _eventBroker.Raise(new StaffrollChangedMessage(this));
        }

        private static PreviewData<StaffrollTextData> CreatePreviewData(StaffRollPreviewData data)
        {
            return new PreviewData<StaffrollTextData>
            {
                OriginalTexts = data.Texts,
                TranslatedTexts = data.TranslatedTexts
            };
        }

        private void ChangeStaffrollPreview(PreviewChangedMessage<StaffRollPreviewData> message)
        {
            if (message.Target != this)
                return;

            _data = message.PreviewData;

            InitializeData(CreatePreviewData(message.PreviewData), message.Index);
        }

        private Image<Rgba32> GetBackground()
        {
            return _screenProvider.GetStaffrollScreen();
        }

        private int GetPreviewFlag(int index)
        {
            if (_data.Texts.Length <= index)
                return 0;

            if (_data.TranslatedTexts?.Length > index)
                return _data.TranslatedTexts[index]?.Flag ?? 0;

            return _data.Texts[index]?.Flag ?? 0;
        }

        private string? GetPreviewText(int index, out AssetPreference fontPreference)
        {
            fontPreference = AssetPreference.Original;

            if (_data.Texts.Length <= index)
                return null;

            if (_data.TranslatedTexts?.Length > index && !string.IsNullOrEmpty(_data.TranslatedTexts[index]?.Text))
            {
                fontPreference = AssetPreference.Patch;
                return _characterReplacer.ReplaceCharacters(_data.TranslatedTexts[index]!.Text!);
            }

            return _data.Texts[index]?.Text;
        }
    }
}
