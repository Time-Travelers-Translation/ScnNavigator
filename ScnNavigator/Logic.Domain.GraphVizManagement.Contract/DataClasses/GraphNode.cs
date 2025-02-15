using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.Contract.DataClasses
{
    public class GraphNode
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public GraphPoint Position { get; set; }
    }
}
