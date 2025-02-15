using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;

namespace Logic.Domain.GraphVizManagement.Contract
{
    public interface IGraphSyntaxCreator
    {
        string Create<TData>(IList<Node<TData>> nodes);
        string Create<TData>(IList<Node<TData>> nodes, GraphOptions options);

        string Create(IList<Node> nodes);
        string Create(IList<Node> nodes, GraphOptions options);
    }
}
