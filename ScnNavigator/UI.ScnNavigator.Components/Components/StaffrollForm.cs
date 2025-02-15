using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls.Base;
using Logic.Business.TranslationManagement.Contract;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.InternalContract.Mergers;

namespace UI.ScnNavigator.Components.Components
{
    internal partial class StaffrollForm : Component, ISaveableComponent
    {
        private readonly IEventBroker _eventBroker;
        private readonly IStaffrollTranslationManager _staffrollTranslationManager;

        public StaffrollForm(StaffrollData data, IEventBroker eventBroker, IPreviewComponentFactory previewFactory,
            ITranslationSettingsProvider translationSettingsProvider, IStaffrollTranslationManager staffrollTranslationManager,
            IStaffrollPreviewDataMerger previewDataMerger)
        {
            StaffRollPreviewData previewData = CreateStaffrollPreviewData(data, previewDataMerger, translationSettingsProvider, staffrollTranslationManager);
            InitializeComponent(previewData, previewFactory, eventBroker);

            _eventBroker = eventBroker;
            _staffrollTranslationManager = staffrollTranslationManager;

            eventBroker.Subscribe<StaffrollChangedMessage>(MarkChangedStaffroll);
        }

        public void Save()
        {
            _staffrollTranslationManager.UpdateStaffRoles().Wait();

            RaiseFileSaved();
        }

        private StaffRollPreviewData CreateStaffrollPreviewData(StaffrollData data, IStaffrollPreviewDataMerger previewDataMerger,
            ITranslationSettingsProvider translationSettingsProvider, IStaffrollTranslationManager staffrollTranslationManager)
        {
            StaffrollTextData[]? translations = null;
            if (translationSettingsProvider.IsTranslationEnabled())
                translations = staffrollTranslationManager.GetStaffRolls().Result;

            return previewDataMerger.Merge(data.Data.Texts, translations);
        }

        private void MarkChangedStaffroll(StaffrollChangedMessage message)
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
