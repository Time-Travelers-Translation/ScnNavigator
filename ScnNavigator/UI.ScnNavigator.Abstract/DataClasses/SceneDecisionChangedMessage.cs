namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SceneDecisionChangedMessage
    {
        public object Sender { get; }
        public string Scene { get; }

        public SceneDecisionChangedMessage(object sender, string scene)
        {
            Sender = sender;
            Scene = scene;
        }
    }
}
