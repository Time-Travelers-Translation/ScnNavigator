using System.IO;

namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IFontTypeReader
    {
        int Read(Stream input);
        int Peek(Stream input);
    }
}
