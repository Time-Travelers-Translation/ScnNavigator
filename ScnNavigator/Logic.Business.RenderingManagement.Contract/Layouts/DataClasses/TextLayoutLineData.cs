using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Layouts.DataClasses
{
    public class TextLayoutLineData
    {
        public IList<TextLayoutCharacterData> Characters { get; set; }
        public Rectangle BoundingBox { get; set; }
    }
}
