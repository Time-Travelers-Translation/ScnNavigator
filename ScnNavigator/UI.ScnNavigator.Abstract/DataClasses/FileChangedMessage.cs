namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class FileChangedMessage
    {
        public object Sender { get; }

        public FileChangedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
