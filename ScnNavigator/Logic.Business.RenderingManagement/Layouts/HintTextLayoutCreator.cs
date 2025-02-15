using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Layouts.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.InternalContract.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.Font;

namespace Logic.Business.RenderingManagement.Layouts
{
    internal class HintTextLayoutCreator : TextLayoutCreator, IHintTextLayoutCreator
    {
        public HintTextLayoutCreator(LayoutOptions options, FontImageData fontData, IGlyphProviderFactory glyphProviderFactory,
            ICharacterParser characterParser, IResourceProvider resourceProvider)
            : base(fontData, options, glyphProviderFactory, characterParser, resourceProvider)
        {
        }

        protected override int GetLinePositionX(TextLayoutLineData currentLine, int boundingWidth)
        {
            switch (Options.HorizontalAlignment)
            {
                case HorizontalTextAlignment.Center:
                    int lineDiff = (Options.LineWidth - currentLine.BoundingBox.Width) / 2;
                    if (lineDiff <= 0)
                        return Options.InitPoint.X;

                    if (!Font.Font.LargeFont.Glyphs.TryGetValue(' ', out GlyphData? spaceGlyph))
                        return Options.InitPoint.X;

                    int spaceWidth = spaceGlyph.Width + Options.TextSpacing;
                    int placeholderCount = lineDiff / spaceWidth;

                    int positionCorrection = spaceWidth * placeholderCount;
                    return Options.InitPoint.X + positionCorrection;
            }

            return base.GetLinePositionX(currentLine, boundingWidth);
        }
    }
}
