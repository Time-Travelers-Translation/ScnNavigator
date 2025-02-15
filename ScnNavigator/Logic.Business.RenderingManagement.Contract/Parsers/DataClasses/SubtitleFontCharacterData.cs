namespace Logic.Business.RenderingManagement.Contract.Parsers.DataClasses
{
    public class SubtitleFontCharacterData : FontCharacterData
    {
        public required bool IsSpeaker { get; init; }
        public override bool IsVisible => !IsSpeaker;
    }
}
