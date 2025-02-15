using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Metrics.DataClasses
{
    public class MetricDetailData
    {
        public MetricDetailType Type { get; set; }
        public MetricDetailLevel Level { get; set; }
        public Rectangle BoundingBox { get; set; }
    }
}
