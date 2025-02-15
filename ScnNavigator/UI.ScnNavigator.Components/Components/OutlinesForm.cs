using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Base;
using Logic.Business.TranslationManagement.Contract;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;

namespace UI.ScnNavigator.Components.Components
{
    internal partial class OutlinesForm : Component, ISaveableComponent
    {
        private readonly OutlineData _data;

        private readonly IEventBroker _eventBroker;
        private readonly IOutlineTranslationManager _outlineTranslationManager;

        public OutlinesForm(OutlineData data, IEventBroker eventBroker, IPreviewComponentFactory previewFactory,
            ITranslationSettingsProvider translationSettingsProvider, IOutlineTranslationManager outlineTranslationManager)
        {
            OutlinePreviewData previewData = CreateOutlinePreviewData(data, translationSettingsProvider, outlineTranslationManager);
            InitializeComponent(previewData, previewFactory, eventBroker);

            _data = data;

            _eventBroker = eventBroker;
            _outlineTranslationManager = outlineTranslationManager;

            eventBroker.Subscribe<OutlineChangedMessage>(MarkChangedOutline);
        }

        public void Save()
        {
            _outlineTranslationManager.UpdateOutlines(_data.Route).Wait();

            RaiseFileSaved();
        }

        private OutlinePreviewData CreateOutlinePreviewData(OutlineData data, ITranslationSettingsProvider translationSettingsProvider, IOutlineTranslationManager outlineTranslationManager)
        {
            TextData[]? translatedTexts = null;
            if (translationSettingsProvider.IsTranslationEnabled())
                translatedTexts = outlineTranslationManager.GetOutlines(data.Route).Result;

            return new OutlinePreviewData
            {
                Route = data.Route,
                Texts = data.Data.Texts.Select(t => new TextData { Name = data.Route, Text = t.Text }).ToArray(),
                TranslatedTexts = translatedTexts
            };
        }

        private void MarkChangedOutline(OutlineChangedMessage message)
        {
            if (message.Sender != _previewForm)
                return;

            RaiseFileChanged();
        }

        private void RaiseFileChanged()
        {
            _eventBroker.Raise(new FileChangedMessage(this));
        }

        private void RaiseFileSaved()
        {
            _eventBroker.Raise(new FileSavedMessage(this));
        }
    }
}
