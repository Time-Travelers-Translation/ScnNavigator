using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Layouts.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.Font;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Logic.Business.RenderingManagement.Layouts
{
    internal class TextLayoutCreator : ITextLayoutCreator
    {
        private readonly ICharacterParser _characterParser;
        private readonly IResourceProvider _resourceProvider;
        private readonly IGlyphProvider _glyphProvider;

        public FontImageData Font { get; }
        protected LayoutOptions Options { get; }

        public TextLayoutCreator(FontImageData fontData, LayoutOptions options, IGlyphProviderFactory glyphProviderFactory, ICharacterParser characterParser, IResourceProvider resourceProvider)
        {
            _characterParser = characterParser;
            _resourceProvider = resourceProvider;
            _glyphProvider = glyphProviderFactory.Create(fontData);

            Options = options;
            Font = fontData;
        }

        public TextLayoutData Create(string text, Size boundingBox)
        {
            IList<CharacterData> characters = _characterParser.Parse(text);
            return Create(characters, boundingBox);
        }

        public TextLayoutData Create(IList<CharacterData> characters, Size boundingBox)
        {
            return CreateAlignedLayout(characters, boundingBox);
        }

        private TextLayoutData CreateAlignedLayout(IList<CharacterData> parsedCharacters, Size boundingBox)
        {
            if (parsedCharacters.Count <= 0)
                return new TextLayoutData(Array.Empty<TextLayoutLineData>(), new Rectangle(Options.InitPoint, Size.Empty));

            IList<TextLayoutLineData> layoutLines = CreateLines(parsedCharacters);
            for (var i = 0; i < layoutLines.Count; i++)
            {
                TextLayoutLineData layoutLine = layoutLines[i];

                Point linePoint = GetLinePosition(layoutLine, boundingBox, layoutLines.Sum(l => l.BoundingBox.Height));
                linePoint = linePoint with
                {
                    Y = linePoint.Y - i * Options.LineHeight
                };

                var characters = new List<TextLayoutCharacterData>();
                foreach (TextLayoutCharacterData lineCharacter in layoutLine.Characters)
                {
                    if (lineCharacter is TextLayoutLockedCharacterData)
                    {
                        characters.Add(lineCharacter);
                        continue;
                    }

                    lineCharacter.BoundingBox = lineCharacter.BoundingBox with
                    {
                        X = lineCharacter.BoundingBox.X + linePoint.X,
                        Y = lineCharacter.BoundingBox.Y + linePoint.Y
                    };
                    lineCharacter.GlyphBoundingBox = lineCharacter.GlyphBoundingBox with
                    {
                        X = lineCharacter.GlyphBoundingBox.X + linePoint.X,
                        Y = lineCharacter.GlyphBoundingBox.Y + linePoint.Y
                    };

                    characters.Add(lineCharacter);
                }

                layoutLine.Characters = characters;
                layoutLine.BoundingBox = layoutLine.BoundingBox with
                {
                    X = layoutLine.BoundingBox.X + linePoint.X,
                    Y = layoutLine.BoundingBox.Y + linePoint.Y
                };
            }

            var textPoint = new Point(layoutLines.Min(x => x.BoundingBox.X), layoutLines[0].BoundingBox.Y);
            var textSize = new Size(layoutLines.Max(x => x.BoundingBox.Width), layoutLines.Sum(l => l.BoundingBox.Height));

            return new TextLayoutData(layoutLines.AsReadOnly(), new Rectangle(textPoint, textSize));
        }

        protected virtual Point GetLinePosition(TextLayoutLineData currentLine, Size boundingBox, int linesHeight)
        {
            int x = GetLinePositionX(currentLine, boundingBox.Width);
            int y = GetLinePositionY(currentLine, boundingBox.Height, linesHeight);

            return new Point(x, y);
        }

        protected virtual int GetLinePositionX(TextLayoutLineData currentLine, int boundingWidth)
        {
            switch (Options.HorizontalAlignment)
            {
                case HorizontalTextAlignment.Left:
                    return Options.InitPoint.X + currentLine.BoundingBox.X;

                case HorizontalTextAlignment.Center:
                    return Options.InitPoint.X + currentLine.BoundingBox.X + (boundingWidth - Options.InitPoint.X - currentLine.BoundingBox.Width) / 2;

                case HorizontalTextAlignment.Right:
                    return boundingWidth - Options.InitPoint.Y - currentLine.BoundingBox.Width;

                default:
                    throw new InvalidOperationException($"Unsupported text alignment {Options.HorizontalAlignment}.");
            }
        }

        protected virtual int GetLinePositionY(TextLayoutLineData currentLine, int boundingHeight, int linesHeight)
        {
            switch (Options.VerticalAlignment)
            {
                case VerticalTextAlignment.Top:
                    return Options.InitPoint.Y + currentLine.BoundingBox.Y;

                case VerticalTextAlignment.Center:
                    return Options.InitPoint.Y + currentLine.BoundingBox.Y + (boundingHeight - Options.InitPoint.Y - linesHeight) / 2;

                case VerticalTextAlignment.Bottom:
                    return boundingHeight - linesHeight - Options.InitPoint.Y + currentLine.BoundingBox.Y;

                default:
                    throw new InvalidOperationException($"Unsupported text alignment {Options.VerticalAlignment}.");
            }
        }

        private IList<TextLayoutLineData> CreateLines(IList<CharacterData> parsedCharacters)
        {
            var context = new LayoutContext();

            foreach (CharacterData character in parsedCharacters)
                CreateCharacter(character, context);

            if (context.Characters.Count > 0)
            {
                context.Lines.Add(new TextLayoutLineData
                {
                    Characters = context.Characters,
                    BoundingBox = new Rectangle(new Point(0, context.Y), new Size(context.VisibleX, GetLineHeight()))
                });
            }

            return context.Lines;
        }

        protected virtual void CreateCharacter(CharacterData character, LayoutContext context)
        {
            var characterLocation = new Point(context.VisibleX, context.Y);

            switch (character)
            {
                case LineBreakCharacterData:
                    // Add line break character
                    context.Characters.Add(new TextLayoutCharacterData
                    {
                        Character = character,
                        BoundingBox = new Rectangle(characterLocation, Size.Empty),
                        GlyphBoundingBox = new Rectangle(characterLocation, Size.Empty)
                    });

                    // Create line from all current characters
                    context.Lines.Add(new TextLayoutLineData
                    {
                        Characters = context.Characters,
                        BoundingBox = new Rectangle(new Point(0, context.Y), new Size(context.VisibleX, GetLineHeight()))
                    });

                    context.X = 0;
                    context.Y += GetLineHeight();
                    context.VisibleX = 0;

                    context.Characters = new List<TextLayoutCharacterData>();
                    break;

                case IconCharacterData iconCharacter:
                    if (!_resourceProvider.TryGet("#/menu/icon.xa", $"#{iconCharacter.IconName} 0", Options.ResourcePreference, out Image<Rgba32>? iconImage))
                        break;

                    Point iconLocation = characterLocation with
                    {
                        Y = characterLocation.Y + (GetLineHeight() - iconImage!.Height) / 2
                    };

                    context.Characters.Add(new TextLayoutIconCharacterData
                    {
                        Image = iconImage,
                        Character = iconCharacter,
                        BoundingBox = GetCharacterBoundingBox(iconCharacter, characterLocation, out _),
                        GlyphBoundingBox = new Rectangle(iconLocation, iconImage.Size)
                    });
                    break;

                default:
                    Rectangle characterBox = GetCharacterBoundingBox(character, characterLocation, out bool isVisible);

                    if (isVisible && Options.LineWidth > 0 && context.X + characterBox.Width > Options.LineWidth)
                    {
                        context.Lines.Add(new TextLayoutLineData
                        {
                            Characters = context.Characters,
                            BoundingBox = new Rectangle(new Point(0, context.Y), new Size(context.VisibleX, GetLineHeight()))
                        });

                        context.X = 0;
                        context.Y += GetLineHeight();
                        context.VisibleX = 0;

                        context.Characters = new List<TextLayoutCharacterData>();

                        characterLocation = new Point(context.VisibleX, context.Y);
                        characterBox = GetCharacterBoundingBox(character, characterLocation, out isVisible);
                    }

                    Rectangle glyphBox = GetGlyphBoundingBox(character, characterLocation);

                    context.X += characterBox.Width;
                    if (isVisible)
                        context.VisibleX += characterBox.Width;

                    else
                    {
                        characterBox = characterBox with
                        {
                            Width = 0,
                            Height = 0
                        };

                        glyphBox = glyphBox with
                        {
                            Width = 0,
                            Height = 0
                        };
                    }

                    context.Characters.Add(new TextLayoutCharacterData
                    {
                        Character = character,
                        BoundingBox = characterBox,
                        GlyphBoundingBox = glyphBox
                    });

                    break;
            }
        }

        protected virtual Rectangle GetCharacterBoundingBox(CharacterData character, Point characterLocation, out bool isVisible)
        {
            isVisible = true;

            switch (character)
            {
                case FontCharacterData fontCharacter:
                    if (fontCharacter.IsFurigana)
                        break;

                    GlyphData? glyph = _glyphProvider.GetGlyph(fontCharacter.Character, fontCharacter.IsFurigana);
                    if (glyph == null)
                        break;

                    var glyphWidth = (int)(glyph.Width * Options.TextScale);
                    var glyphSize = new Size(glyphWidth + Options.TextSpacing, GetLineHeight());

                    return new Rectangle(characterLocation, glyphSize);

                case BlankCharacterData blankCharacter:
                    var size = new Size(blankCharacter.Width + Options.TextSpacing, GetLineHeight());

                    return new Rectangle(characterLocation, size);
            }

            return new Rectangle(characterLocation, Size.Empty);
        }

        protected virtual Rectangle GetGlyphBoundingBox(CharacterData character, Point characterLocation)
        {
            switch (character)
            {
                case FontCharacterData fontCharacter:
                    if (fontCharacter.IsFurigana)
                        break;

                    GlyphData? glyph = _glyphProvider.GetGlyph(fontCharacter.Character, fontCharacter.IsFurigana);
                    if (glyph == null)
                        break;

                    var glyphWidth = (int)(glyph.Description.Width * Options.TextScale);
                    var glyphHeight = (int)(glyph.Description.Height * Options.TextScale);

                    int glyphX = characterLocation.X + glyph.Description.X;
                    int glyphY = characterLocation.Y + glyph.Description.Y;

                    return new Rectangle(glyphX, glyphY, glyphWidth, glyphHeight);
            }

            return new Rectangle(characterLocation, Size.Empty);
        }

        private int GetLineHeight()
        {
            if (Options.LineHeight > 0)
                return Options.LineHeight;

            return Font.Font.LargeFont.MaxHeight;
        }
    }
}
