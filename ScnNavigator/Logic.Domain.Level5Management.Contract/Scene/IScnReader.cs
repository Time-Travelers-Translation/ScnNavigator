using System.IO;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using Logic.Domain.Level5Management.Contract.Exceptions.Scene;

namespace Logic.Domain.Level5Management.Contract.Scene
{
    [MapException(typeof(ScnReaderException))]
    public interface IScnReader
    {
        SceneNavigator Read(Stream input);
    }
}
