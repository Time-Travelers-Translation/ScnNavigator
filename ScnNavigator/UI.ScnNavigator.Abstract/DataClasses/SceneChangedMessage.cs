namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SceneChangedMessage
    {
        public object Target { get; }
        public string Scene { get; }

        public SceneChangedMessage(object target, string scene)
        {
            Target = target;
            Scene = scene;
        }
    }
}
