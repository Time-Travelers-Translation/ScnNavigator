using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;

namespace Logic.Domain.Level5Management.Contract.Image
{
    public interface IImageReader
    {
        RawImageData Read(Stream input);
    }
}
