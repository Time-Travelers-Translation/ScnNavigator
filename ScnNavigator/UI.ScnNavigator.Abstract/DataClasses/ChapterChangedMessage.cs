using System.Reflection;

namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class ChapterChangedMessage
    {
        public object Sender { get; }

        public ChapterChangedMessage(object sender)
        {
            Sender = sender;
        }
    }
}
