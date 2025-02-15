namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class TutorialChangedMessage
    {
        public object Sender { get; }
        public int Id { get; }

        public TutorialChangedMessage(object sender, int id)
        {
            Sender = sender;
            Id = id;
        }
    }
}
