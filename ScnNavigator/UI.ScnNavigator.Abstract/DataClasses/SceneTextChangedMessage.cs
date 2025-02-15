namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SceneTextChangedMessage
    {
        public object Sender { get; }
        public string SceneName { get; }
        public string? EventName { get; set; }

        public SceneTextChangedMessage(object sender, string sceneName, string? eventName)
        {
            Sender = sender;
            SceneName = sceneName;
            EventName = eventName;
        }
    }
}
