using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Parsers.DataClasses
{
    public class TextureCharacterData : CharacterData
    {
        public required string Path { get; init; }
        public required Point Location { get; init; }
        public override bool IsVisible => false;
    }
}
