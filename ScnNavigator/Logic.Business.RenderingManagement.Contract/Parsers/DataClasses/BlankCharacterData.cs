namespace Logic.Business.RenderingManagement.Contract.Parsers.DataClasses
{
    public class BlankCharacterData : CharacterData
    {
        public required int Width { get; init; }
        public override bool IsVisible => true;
    }
}
