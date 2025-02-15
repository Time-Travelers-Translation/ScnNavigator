using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Layouts.DataClasses
{
    public record TextLayoutData(IReadOnlyList<TextLayoutLineData> Lines, Rectangle BoundingBox);
}
