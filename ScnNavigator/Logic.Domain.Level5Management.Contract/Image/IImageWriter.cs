using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using System.IO;

namespace Logic.Domain.Level5Management.Contract.Image
{
    public interface IImageWriter
    {
        void Write(RawImageData data, Stream output);
    }
}
