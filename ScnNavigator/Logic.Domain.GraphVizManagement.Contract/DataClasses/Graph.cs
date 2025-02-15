using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.Contract.DataClasses
{
    public class Graph
    {
        public string Name { get; set; }
        public GraphSize Size { get; set; }
        public GraphNode[] Nodes { get; set; }
        public GraphEdge[] Edges { get; set; }
    }
}
