using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.Contract.DataClasses
{
    public class GraphEdge
    {
        public string Id { get; set; }
        public GraphPoint[] Points { get; set; }

        public string EdgePos { get; set; }
    }
}
