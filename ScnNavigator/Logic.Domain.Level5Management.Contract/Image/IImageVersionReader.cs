using System.IO;

namespace Logic.Domain.Level5Management.Contract.Image
{
    public interface IImageVersionReader
    {
        int Read(Stream input);
        int Peek(Stream input);
    }
}
