namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class ChapterSaveRequestedMessage
    {
        public object Sender { get; }

        public ChapterSaveRequestedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
