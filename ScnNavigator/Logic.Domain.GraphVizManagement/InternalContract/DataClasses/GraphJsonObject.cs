using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.InternalContract.DataClasses
{
    internal class GraphJsonObject
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Pos { get; set; }
    }
}
