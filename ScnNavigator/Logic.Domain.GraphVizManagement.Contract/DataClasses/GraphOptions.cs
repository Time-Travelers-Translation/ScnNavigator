using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.Contract.DataClasses
{
    public class GraphOptions
    {
        public GraphDirection Direction { get; set; } = GraphDirection.LeftToRight;
        public NodeShape NodeShape { get; set; } = NodeShape.Box;
        public SplineShape SplineShape { get; set; } = SplineShape.Curved;
    }
}
