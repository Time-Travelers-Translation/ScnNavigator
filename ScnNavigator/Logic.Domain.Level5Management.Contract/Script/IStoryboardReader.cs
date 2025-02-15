using System.IO;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Level5Management.Contract.DataClasses.Script;
using Logic.Domain.Level5Management.Contract.Exceptions.Script;

namespace Logic.Domain.Level5Management.Contract.Script
{
    [MapException(typeof(StoryboardReaderException))]
    public interface IStoryboardReader
    {
        Storyboard Read(Stream input);
    }
}
