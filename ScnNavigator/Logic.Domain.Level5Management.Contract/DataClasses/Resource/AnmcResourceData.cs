using System.Collections.Generic;
using System.Diagnostics;
using SixLabors.ImageSharp;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Resource
{
    [DebuggerDisplay("{Name}")]
    public class AnmcResourceData
    {
        public string Name { get; set; }
        public IList<AnmcResourceImageData> Parts { get; set; }

        public PointF Location { get; set; }
        public SizeF Size { get; set; }
    }
}
