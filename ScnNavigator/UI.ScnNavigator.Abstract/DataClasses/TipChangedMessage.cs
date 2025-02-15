namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class TipChangedMessage
    {
        public object Sender { get; }
        public int Id { get; }
        public bool IsTitle { get; }

        public TipChangedMessage(object sender, int id, bool isTitle)
        {
            Sender = sender;
            Id = id;
            IsTitle = isTitle;
        }
    }
}
