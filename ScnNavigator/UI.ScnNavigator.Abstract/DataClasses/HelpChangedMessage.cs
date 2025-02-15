namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class HelpChangedMessage
    {
        public object Sender { get; }
        public int Id { get; }

        public HelpChangedMessage(object sender, int id)
        {
            Sender = sender;
            Id = id;
        }
    }
}
