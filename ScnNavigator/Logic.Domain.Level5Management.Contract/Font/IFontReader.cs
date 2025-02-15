using System.IO;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;

namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IFontReader
    {
        FontData Read(Stream input);
    }
}
