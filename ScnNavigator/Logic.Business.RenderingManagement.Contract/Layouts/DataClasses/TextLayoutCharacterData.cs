using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Layouts.DataClasses
{
    public class TextLayoutCharacterData
    {
        public CharacterData Character { get; set; }
        public Rectangle BoundingBox { get; set; }
        public Rectangle GlyphBoundingBox { get; set; }
    }
}
