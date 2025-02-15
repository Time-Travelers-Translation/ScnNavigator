using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;

namespace Logic.Domain.Level5Management.Contract.Scene
{
    public interface IFloReader
    {
        FloData Read(Stream input);
    }
}
