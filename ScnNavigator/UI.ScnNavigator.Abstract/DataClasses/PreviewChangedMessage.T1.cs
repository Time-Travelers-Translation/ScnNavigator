namespace UI.ScnNavigator.Abstract.DataClasses
{
    public class PreviewChangedMessage<TPreviewData>
    {
        public object Target { get; }
        public TPreviewData PreviewData { get; }
        public int Index { get; }

        public PreviewChangedMessage(object target, TPreviewData previewData, int index)
        {
            Target = target;
            PreviewData = previewData;
            Index = index;
        }
    }
}
