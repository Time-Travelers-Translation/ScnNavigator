using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.InternalContract
{
    public interface IGraphVizExecutor
    {
        string Execute(string command);
    }
}
