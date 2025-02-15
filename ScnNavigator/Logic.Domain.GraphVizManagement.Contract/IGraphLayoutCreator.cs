using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GraphVizManagement.Contract
{
    public interface IGraphLayoutCreator
    {
        Graph Create(string graphSyntax);
    }
}
