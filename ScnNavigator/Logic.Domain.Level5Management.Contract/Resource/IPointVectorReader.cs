using System.Collections.Generic;
using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;

namespace Logic.Domain.Level5Management.Contract.Resource
{
    public interface IPointVectorReader
    {
        IReadOnlyList<PointVectorData>? Read(Stream input);
    }
}
