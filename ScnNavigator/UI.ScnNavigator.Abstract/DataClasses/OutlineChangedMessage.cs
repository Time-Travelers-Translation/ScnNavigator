namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class OutlineChangedMessage
    {
        public object Sender { get; }

        public OutlineChangedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
