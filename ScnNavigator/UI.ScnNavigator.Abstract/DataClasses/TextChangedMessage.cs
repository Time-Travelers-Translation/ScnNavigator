namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class TextChangedMessage
    {
        public object Sender { get; }
        public string Name { get; }

        public TextChangedMessage(object sender, string name)
        {
            Sender = sender;
            Name = name;
        }
    }
}
