using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;

namespace Logic.Domain.Level5Management.Contract.Resource
{
    public interface IAnmcReader
    {
        AnmcData Read(Stream input);
    }
}
