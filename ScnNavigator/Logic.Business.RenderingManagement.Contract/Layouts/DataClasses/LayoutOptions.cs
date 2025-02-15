using Logic.Business.RenderingManagement.Contract.Layouts.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Layouts.DataClasses
{
    public class LayoutOptions
    {
        public HorizontalTextAlignment HorizontalAlignment { get; init; } = HorizontalTextAlignment.Left;
        public VerticalTextAlignment VerticalAlignment { get; init; } = VerticalTextAlignment.Top;
        public Point InitPoint { get; init; }
        public int LineHeight { get; init; }
        public int LineWidth { get; init; }
        public float TextScale { get; init; } = 1f;
        public int TextSpacing { get; init; } = 1;
        public AssetPreference ResourcePreference { get; init; }
    }
}
