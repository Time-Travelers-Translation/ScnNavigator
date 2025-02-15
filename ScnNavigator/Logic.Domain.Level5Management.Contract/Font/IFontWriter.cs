using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using System.IO;

namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IFontWriter
    {
        void Write(FontData font, Stream output);
    }
}
