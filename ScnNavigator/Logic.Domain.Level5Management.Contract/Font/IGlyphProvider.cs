using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;

namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IGlyphProvider
    {
        Image<Rgba32>? GetGlyphImage(ushort character, bool isFurigana, Color glyphColor);
        GlyphData? GetGlyph(ushort character, bool isFurigana);
    }
}
