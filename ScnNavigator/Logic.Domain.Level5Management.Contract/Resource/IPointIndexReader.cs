using System.Collections.Generic;
using System.IO;

namespace Logic.Domain.Level5Management.Contract.Resource
{
    public interface IPointIndexReader
    {
        IList<short>? Read(Stream input);
    }
}
