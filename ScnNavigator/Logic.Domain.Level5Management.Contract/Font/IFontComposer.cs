using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using System.IO;

namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IFontComposer
    {
        void Compose(FontImageData data, Stream output);
    }
}
