using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.Contract.Renderers;
using Logic.Business.RenderingManagement.Contract.Renderers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Domain.Level5Management.Contract.Font;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Logic.Business.RenderingManagement.Renderers
{
    internal class TextRenderer : ITextRenderer
    {
        private readonly RenderOptions _options;
        private readonly ITextLayoutCreator _layoutCreator;
        private readonly IGlyphProvider _glyphProvider;

        public TextRenderer(RenderOptions options, ITextLayoutCreator layoutCreator,
            IGlyphProviderFactory glyphProviderFactory)
        {
            _options = options;
            _layoutCreator = layoutCreator;
            _glyphProvider = glyphProviderFactory.Create(layoutCreator.Font);
        }

        public void Render(Image<Rgba32> image, string text)
        {
            TextLayoutData textLayout = _layoutCreator.Create(text, image.Size);

            var bufferImage = new Image<Rgba32>(image.Width, image.Height);

            RenderLines(bufferImage, textLayout.Lines);

            if (_options.DrawBoundingBoxes)
                bufferImage.Mutate(x => x.Draw(Color.Red, 1f, textLayout.BoundingBox));

            image.Mutate(x => x.DrawImage(bufferImage, 1f));
        }

        private void RenderLines(Image<Rgba32> image, IReadOnlyList<TextLayoutLineData> lines)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                bool isLineVisible = _options.VisibleLines <= 0 || lines.Count - i <= _options.VisibleLines;

                RenderLine(image, lines[i], isLineVisible);
            }
        }

        private void RenderLine(Image<Rgba32> image, TextLayoutLineData line, bool isLineVisible)
        {
            if (_options.TextOutlineColor != Color.Transparent)
                foreach (TextLayoutCharacterData character in line.Characters)
                    RenderCharacterOutline(image, character, isLineVisible);

            foreach (TextLayoutCharacterData character in line.Characters)
            {
                RenderCharacter(image, character, isLineVisible);

                if (_options.DrawBoundingBoxes)
                    image.Mutate(x => x.Draw(Color.RebeccaPurple, 1f, character.BoundingBox));
            }

            if (_options.DrawBoundingBoxes)
                image.Mutate(x => x.Draw(Color.PaleVioletRed, 1f, line.BoundingBox));
        }

        private void RenderCharacterOutline(Image<Rgba32> image, TextLayoutCharacterData character, bool isLineVisible)
        {
            if (_options.OutlineRadius <= 0 || character.GlyphBoundingBox.Width == 0 || character.GlyphBoundingBox.Height == 0)
                return;

            DrawCharacterOutline(image, character, isLineVisible);
        }

        private void RenderCharacter(Image<Rgba32> image, TextLayoutCharacterData character, bool isLineVisible)
        {
            if (character.GlyphBoundingBox.Width == 0 || character.GlyphBoundingBox.Height == 0)
                return;

            DrawCharacter(image, character, isLineVisible);
        }

        protected virtual void DrawCharacter(Image<Rgba32> image, TextLayoutCharacterData character, bool isLineVisible)
        {
            Color textColor = _options.TextColor;
            if (character.Character is TipCharacterData { IsTip: true } tipCharacter)
                textColor = tipCharacter.TipNumber <= 314 ? _options.TipTextColor : _options.PostTipTextColor;

            switch (character)
            {
                case TextLayoutIconCharacterData iconCharacter:
                    image.Mutate(x => x.DrawImage(iconCharacter.Image!, character.GlyphBoundingBox.Location, isLineVisible ? 1f : .25f));
                    break;
            }

            switch (character.Character)
            {
                case FontCharacterData fontCharacter:
                    if (fontCharacter.IsFurigana)
                        break;

                    Image<Rgba32>? glyph = _glyphProvider.GetGlyphImage(fontCharacter.Character, fontCharacter.IsFurigana, textColor);
                    if (glyph == null)
                        break;

                    glyph.Mutate(context => context.Resize(character.GlyphBoundingBox.Size));
                    image.Mutate(x => x.DrawImage(glyph, character.GlyphBoundingBox.Location, isLineVisible ? 1f : .25f));

                    break;
            }
        }

        private void DrawCharacterOutline(Image<Rgba32> image, TextLayoutCharacterData character, bool isLineVisible)
        {
            switch (character.Character)
            {
                case FontCharacterData fontCharacter:
                    Image<Rgba32>? glyph = _glyphProvider.GetGlyphImage(fontCharacter.Character, fontCharacter.IsFurigana, Color.White);
                    if (glyph == null)
                        break;

                    Color centerColor = _options.TextOutlineColor;
                    if (!isLineVisible)
                        centerColor = centerColor.WithAlpha(.25f);

                    for (var y = 0; y < glyph.Height; y++)
                    {
                        for (var x = 0; x < glyph.Width; x++)
                        {
                            PointF centerLocation = new(character.GlyphBoundingBox.X + x, character.GlyphBoundingBox.Y + y);
                            PointF[] ellipse = new EllipsePolygon(centerLocation, _options.OutlineRadius).Points.ToArray();

                            if (glyph[x, y].A > 0)
                                image.Mutate(z => z.FillPolygon(centerColor, ellipse));
                        }
                    }

                    break;
            }
        }
    }
}
