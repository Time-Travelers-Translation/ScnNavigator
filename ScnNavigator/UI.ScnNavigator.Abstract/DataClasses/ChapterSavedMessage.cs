namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class ChapterSavedMessage
    {
        public object Sender { get; }

        public ChapterSavedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
