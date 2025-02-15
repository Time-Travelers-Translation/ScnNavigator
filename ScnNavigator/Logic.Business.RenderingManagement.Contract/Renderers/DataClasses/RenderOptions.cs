using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Renderers.DataClasses
{
    public class RenderOptions
    {
        public bool DrawBoundingBoxes { get; set; }

        public int VisibleLines { get; set; }
        public int OutlineRadius { get; set; }

        public Color TextColor { get; set; } = Color.Black;
        public Color TipTextColor { get; set; } = Color.Cyan;
        public Color PostTipTextColor { get; set; } = Color.HotPink;
        public Color TextOutlineColor { get; set; } = Color.Transparent;
    }
}
