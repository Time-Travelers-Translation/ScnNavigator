namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class FileSavedMessage
    {
        public object Sender { get; }

        public FileSavedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
