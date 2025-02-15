namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class SelectedTipChangedMessage
    {
        public object Sender { get; }
        public int Id { get; }

        public SelectedTipChangedMessage(object sender, int id)
        {
            Sender = sender;
            Id = id;
        }
    }
}
