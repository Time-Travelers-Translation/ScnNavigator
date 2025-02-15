namespace Logic.Business.RenderingManagement.Contract.Parsers.DataClasses
{
    public class IconCharacterData : TipCharacterData
    {
        public required string IconName { get; init; }
        public override bool IsVisible => true;
    }
}
