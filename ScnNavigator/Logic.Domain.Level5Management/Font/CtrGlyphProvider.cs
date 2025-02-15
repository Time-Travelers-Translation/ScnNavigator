using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.InternalContract.Font;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Logic.Domain.Level5Management.Font
{
    internal class CtrGlyphProvider : ICtrGlyphProvider
    {
        private readonly FontImageData _fontData;

        private Image<Rgba32>? _image;

        public CtrGlyphProvider(FontImageData fontData)
        {
            _fontData = fontData;
        }

        public Image<Rgba32>? GetGlyphImage(ushort character, bool isFurigana, Color glyphColor)
        {
            if (_image == null)
            {
                if (_fontData.Images.Length <= 0)
                    return null;

                _image = _fontData.Images[0].Image.GetImage();
            }

            GlyphData? fontGlyph = GetGlyph(character, isFurigana);
            if (fontGlyph == null)
                return null;

            if (fontGlyph.Description.Width == 0 || fontGlyph.Description.Height == 0)
                return null;

            ColorMatrix colorMatrix = CreateColorMatrix(fontGlyph.Location.Index, glyphColor);

            var srcRect = new Rectangle(fontGlyph.Location.X, fontGlyph.Location.Y, fontGlyph.Description.Width, fontGlyph.Description.Height);

            return _image.Clone(context => context.Crop(srcRect).Filter(colorMatrix));
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

        private ColorMatrix CreateColorMatrix(int channelIndex, Color targetColor)
        {
            float sourceR = channelIndex == 0 ? 1f : 0;
            float sourceG = channelIndex == 1 ? 1f : 0;
            float sourceB = channelIndex == 2 ? 1f : 0;

            var pixel = targetColor.ToPixel<Rgba32>();
            float targetR = pixel.R / 255f;
            float targetG = pixel.G / 255f;
            float targetB = pixel.B / 255f;

            return new ColorMatrix(
                0f, 0f, 0f, sourceR,
                0f, 0f, 0f, sourceG,
                0f, 0f, 0f, sourceB,
                0f, 0f, 0f, 0f,
                targetR, targetG, targetB, 0f);
        }
    }
}
