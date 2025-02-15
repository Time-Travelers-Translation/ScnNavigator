namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SceneSavedMessage
    {
        public string SceneName { get; set; }

        public SceneSavedMessage(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
