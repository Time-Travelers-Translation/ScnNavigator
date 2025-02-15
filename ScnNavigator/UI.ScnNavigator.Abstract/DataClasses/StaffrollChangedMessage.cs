namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class StaffrollChangedMessage
    {
        public object Sender { get; }

        public StaffrollChangedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
