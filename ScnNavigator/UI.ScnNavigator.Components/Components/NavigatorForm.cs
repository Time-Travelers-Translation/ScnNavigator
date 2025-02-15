using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Controls;
using ImGui.Forms.Controls.Base;
using ImGui.Forms.Localization;
using ImGui.Forms.Modals;
using Logic.Business.TranslationManagement.Contract;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Components.Components
{
    public partial class NavigatorForm : Component, ISaveableComponent
    {
        private readonly IEventBroker _eventBroker;
        private readonly ISpeakerTranslationManager _speakerTranslationManager;
        private readonly IStringResourceProvider _stringResourceProvider;

        private readonly HashSet<string> _changedSpeakers;

        private readonly IDictionary<object, TabPage> _chapterPageLookup;
        private readonly IDictionary<TabPage, object> _pageChapterLookup;

        public NavigatorForm(SceneNavigator sceneNavigator, IComponentFactory formFactory, IEventBroker eventBroker,
            ISpeakerTranslationManager speakerTranslationManager, IStringResourceProvider stringResourceProvider)
        {
            InitializeComponent();

            _eventBroker = eventBroker;
            _speakerTranslationManager = speakerTranslationManager;
            _stringResourceProvider = stringResourceProvider;

            _changedSpeakers = new HashSet<string>();

            _chapterPageLookup = new Dictionary<object, TabPage>();
            _pageChapterLookup = new Dictionary<TabPage, object>();

            _chapterTabControl.PageRemoving += async (s, e) => e.Cancel = !await CanCloseTabPage(e.Page);

            InitializeChapterTabs(sceneNavigator, formFactory);

            eventBroker.Subscribe<ChapterChangedMessage>(message => MarkChangedChapter(message.Sender, true));
            eventBroker.Subscribe<ChapterSavedMessage>(message => MarkChangedChapter(message.Sender, false));
            eventBroker.Subscribe<SpeakerChangedMessage>(message => MarkChangedSpeaker(message.Speaker));
            eventBroker.Subscribe<ApplicationDisabledMessage>(_ => ToggleForm(false));
            eventBroker.Subscribe<ApplicationEnabledMessage>(_ => ToggleForm(true));
        }

        private void ToggleForm(bool enabled)
        {
            _chapterTabControl.Enabled = enabled;
        }

        private void InitializeChapterTabs(SceneNavigator sceneNavigator, IComponentFactory componentFactory)
        {
            int[] chapters = GetChapters(sceneNavigator);
            foreach (int chapter in chapters)
            {
                IList<Node<SceneEntry>> nodes = CreateSceneNodes(sceneNavigator, chapter);

                Component chapterForm = componentFactory.CreateChapterForm(nodes);
                var graphTabPage = new TabPage(chapterForm)
                {
                    Title = $"Chapter {chapter}"
                };

                _chapterTabControl.AddPage(graphTabPage);

                _chapterPageLookup[chapterForm] = graphTabPage;
                _pageChapterLookup[graphTabPage] = chapterForm;
            }

            _chapterTabControl.SelectedPage = _chapterTabControl.Pages[0];
        }

        private int[] GetChapters(SceneNavigator sceneNavigator)
        {
            var chapterIndicators = new HashSet<int>();
            foreach (SceneEntry scene in sceneNavigator.Scenes)
            {
                string indicator = scene.Name[1..3];
                if (int.TryParse(indicator, out int chapterIndicator))
                    chapterIndicators.Add(chapterIndicator);
            }

            return chapterIndicators.ToArray();
        }

        private IList<Node<SceneEntry>> CreateSceneNodes(SceneNavigator sceneNavigator, int chapter)
        {
            IEnumerable<SceneEntry> chapterScenes = sceneNavigator.Scenes.Where(x => x.Name[2] - '0' == chapter);

            var sceneDict = new Dictionary<short, Node<SceneEntry>>();
            foreach (SceneEntry scene in chapterScenes)
            {
                if (!sceneDict.TryGetValue(scene.Id, out Node<SceneEntry>? node))
                    sceneDict[scene.Id] = node = new Node<SceneEntry>(scene) { Text = scene.Name };

                foreach (SceneEntryBranch? branch in scene.Branches)
                {
                    if (branch == null)
                        continue;

                    if (!sceneDict.TryGetValue(branch.Scene.Id, out Node<SceneEntry>? child))
                        sceneDict[branch.Scene.Id] = child = new Node<SceneEntry>(branch.Scene) { Text = branch.Scene.Name };

                    node.AddChild(child);
                }
            }

            return sceneDict.Values.ToArray();
        }

        private void MarkChangedChapter(object sender, bool changed)
        {
            if (_chapterPageLookup.TryGetValue(sender, out TabPage? chapterPage))
                chapterPage.HasChanges = changed;

            if (changed)
            {
                RaiseFileChanged();
                return;
            }

            AttemptRaiseFileSaved();
        }

        private void MarkChangedSpeaker(string speaker)
        {
            _changedSpeakers.Add(speaker);

            RaiseFileChanged();
        }

        public void Save()
        {
            foreach (TabPage page in _pageChapterLookup.Keys)
            {
                if (!page.HasChanges)
                    continue;

                RaiseChapterSaveRequested(_pageChapterLookup[page]);
            }

            if (_changedSpeakers.Count > 0)
            {
                string[] speakers = _changedSpeakers.ToArray();
                _speakerTranslationManager.UpdateSpeakers(speakers).Wait();

                _changedSpeakers.Clear();
            }

            AttemptRaiseFileSaved();
        }

        private void AttemptRaiseFileSaved()
        {
            if (_pageChapterLookup.Keys.All(x => !x.HasChanges) && _changedSpeakers.Count <= 0)
                RaiseFileSaved();
        }

        private async Task<bool> CanCloseTabPage(TabPage page)
        {
            if (!page.HasChanges)
                return true;

            RaiseApplicationDisabled();

            DialogResult result = await GetUnsavedChangesResult();

            if (result == DialogResult.Cancel)
            {
                RaiseApplicationEnabled();

                return false;
            }

            if (result == DialogResult.Yes)
            {
                if (_pageChapterLookup.TryGetValue(page, out object? form))
                    RaiseChapterSaveRequested(form);
            }

            RaiseApplicationEnabled();

            return true;
        }

        private async Task<DialogResult> GetUnsavedChangesResult()
        {
            LocalizedString caption = _stringResourceProvider.FileCloseUnsavedChangesCaption();
            LocalizedString text = _stringResourceProvider.FileCloseUnsavedChangesText();
            return await MessageBox.ShowYesNoCancelAsync(caption, text);
        }

        private void RaiseFileChanged()
        {
            _eventBroker.Raise(new FileChangedMessage(this));
        }

        private void RaiseFileSaved()
        {
            _eventBroker.Raise(new FileSavedMessage(this));
        }

        private void RaiseChapterSaveRequested(object sender)
        {
            _eventBroker.Raise(new ChapterSaveRequestedMessage(sender));
        }

        private void RaiseApplicationDisabled()
        {
            _eventBroker.Raise(new ApplicationDisabledMessage());
        }

        private void RaiseApplicationEnabled()
        {
            _eventBroker.Raise(new ApplicationEnabledMessage());
        }
    }
}
