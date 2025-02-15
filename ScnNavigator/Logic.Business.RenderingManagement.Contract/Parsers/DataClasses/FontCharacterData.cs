namespace Logic.Business.RenderingManagement.Contract.Parsers.DataClasses
{
    public class FontCharacterData : TipCharacterData
    {
        public required ushort Character { get; init; }
        public required bool IsFurigana { get; init; }
        public override bool IsVisible => true;
    }
}
