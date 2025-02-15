using Logic.Domain.Level5Management.Contract.DataClasses.Font;

namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IGlyphProviderFactory
    {
        IGlyphProvider Create(FontImageData fontData);
    }
}
