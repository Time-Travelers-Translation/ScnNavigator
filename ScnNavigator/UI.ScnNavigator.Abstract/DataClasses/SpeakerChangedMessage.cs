namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SpeakerChangedMessage
    {
        public string SceneName { get; }
        public string EventName { get; }
        public string Speaker { get; }

        public SpeakerChangedMessage(string sceneName, string eventName, string speaker)
        {
            SceneName = sceneName;
            EventName = eventName;
            Speaker = speaker;
        }
    }
}
