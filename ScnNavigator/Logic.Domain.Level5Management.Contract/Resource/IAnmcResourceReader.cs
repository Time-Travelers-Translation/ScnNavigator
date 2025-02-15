using System.Collections.Generic;
using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;

namespace Logic.Domain.Level5Management.Contract.Resource
{
    public interface IAnmcResourceReader
    {
        IList<AnmcResourceData>? Read(Stream input);
    }
}
