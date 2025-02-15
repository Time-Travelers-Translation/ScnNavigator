using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.InternalContract.Font;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Logic.Domain.Level5Management.Font
{
    internal class PspGlyphProvider : IPspGlyphProvider
    {
        private readonly FontImageData _fontData;

        private Image<Rgba32>[]? _images;

        public PspGlyphProvider(FontImageData fontData)
        {
            _fontData = fontData;
        }

        public Image<Rgba32>? GetGlyphImage(ushort character, bool isFurigana, Color glyphColor)
        {
            if (_images == null)
            {
                if (_fontData.Images.Length <= 0)
                    return null;

                _images = new Image<Rgba32>[_fontData.Images.Length];
                for (var i = 0; i < _images.Length; i++)
                    _images[i] = _fontData.Images[i].Image.GetImage();
            }

            GlyphData? fontGlyph = GetGlyph(character, isFurigana);
            if (fontGlyph == null)
                return null;

            if (fontGlyph.Description.Width == 0 || fontGlyph.Description.Height == 0)
                return null;

            var srcRect = new Rectangle(fontGlyph.Location.X, fontGlyph.Location.Y, fontGlyph.Description.Width, fontGlyph.Description.Height);

            return _images[fontGlyph.Location.Index].Clone(context => context.Crop(srcRect));
        }

        public GlyphData? GetGlyph(ushort character, bool isFurigana)
        {
            FontGlyphData fontGlyphs = isFurigana ?
                _fontData.Font.SmallFont :
                _fontData.Font.LargeFont;

            if (fontGlyphs.Glyphs.TryGetValue(character, out GlyphData? glyph))
                return glyph;

            if (fontGlyphs.Glyphs.TryGetValue(fontGlyphs.FallbackCharacter, out glyph))
                return glyph;

            return null;
        }
    }
}
