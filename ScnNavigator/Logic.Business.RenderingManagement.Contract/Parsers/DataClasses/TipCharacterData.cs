namespace Logic.Business.RenderingManagement.Contract.Parsers.DataClasses
{
    public class TipCharacterData: CharacterData
    {
        public required bool IsTip { get; init; }
        public required int TipNumber { get; init; }
        public override bool IsVisible => false;
    }
}
