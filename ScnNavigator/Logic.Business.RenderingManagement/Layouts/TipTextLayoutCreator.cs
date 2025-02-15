using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Layouts.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;

namespace Logic.Business.RenderingManagement.Layouts
{
    internal class TipTextLayoutCreator : TextLayoutCreator, ITipTextLayoutCreator
    {
        private readonly IResourceProvider _resourceProvider;

        public TipTextLayoutCreator(LayoutOptions options, FontImageData fontData, IGlyphProviderFactory glyphProviderFactory,
            ICharacterParser characterParser, IResourceProvider resourceProvider)
            : base(fontData, options, glyphProviderFactory, characterParser, resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        protected override void CreateCharacter(CharacterData character, LayoutContext context)
        {
            Rectangle glyphBoundingBox;

            switch (character)
            {
                case TextureCharacterData textureData:
                    _resourceProvider.TryGet(textureData.Path, "#00 0", Options.ResourcePreference, out Image<Rgba32>? texture);

                    glyphBoundingBox = GetGlyphBoundingBox(textureData, textureData.Location);
                    context.Characters.Add(new TextLayoutTextureCharacterData
                    {
                        Image = texture,
                        Character = textureData,
                        BoundingBox = glyphBoundingBox,
                        GlyphBoundingBox = glyphBoundingBox
                    });

                    return;

                case ModelCharacterData modelData:
                    glyphBoundingBox = GetGlyphBoundingBox(modelData, modelData.Location);
                    context.Characters.Add(new TextLayoutLockedCharacterData
                    {
                        Character = modelData,
                        BoundingBox = glyphBoundingBox,
                        GlyphBoundingBox = glyphBoundingBox
                    });

                    return;

                case MovieCharacterData movieData:
                    glyphBoundingBox = GetGlyphBoundingBox(movieData, movieData.Location);
                    context.Characters.Add(new TextLayoutLockedCharacterData
                    {
                        Character = movieData,
                        BoundingBox = glyphBoundingBox,
                        GlyphBoundingBox = glyphBoundingBox
                    });

                    return;
            }

            base.CreateCharacter(character, context);
        }

        protected override Rectangle GetGlyphBoundingBox(CharacterData character, Point characterLocation)
        {
            switch (character)
            {
                case TextureCharacterData textureData:
                    if (!_resourceProvider.TryGet(textureData.Path, "#00 0", Options.ResourcePreference, out Image<Rgba32>? texture))
                        return new Rectangle(characterLocation, Size.Empty);

                    var scaledWidth = (int)(texture!.Width * GetResourceDimensionScale(texture));
                    var scaledHeight = (int)(texture.Height * GetResourceDimensionScale(texture));
                    var scaledSize = new Size(scaledWidth, scaledHeight);

                    return new Rectangle(characterLocation, scaledSize);

                case ModelCharacterData modelData:
                    Size modelSize = GetModelSize(modelData.Path);
                    return new Rectangle(characterLocation, modelSize);

                case MovieCharacterData:
                    Size movieSize = GetMovieSize();
                    return new Rectangle(characterLocation, movieSize);
            }

            return base.GetGlyphBoundingBox(character, characterLocation);
        }

        protected override Point GetLinePosition(TextLayoutLineData currentLine, Size boundingBox, int linesHeight)
        {
            int x = GetLinePositionX(currentLine, boundingBox.Width);
            int y = GetLinePositionY(currentLine, boundingBox.Height, linesHeight);

            switch (Options.HorizontalAlignment)
            {
                case HorizontalTextAlignment.Left:
                    if (currentLine.Characters.Count <= 0)
                        break;

                    switch (currentLine.Characters[0])
                    {
                        case TextLayoutTextureCharacterData textureLayoutData:
                            if (textureLayoutData.GlyphBoundingBox.Bottom <= y)
                                break;

                            return new Point(x + textureLayoutData.GlyphBoundingBox.Right, y);

                        case { } layoutData:
                            switch (layoutData.Character)
                            {
                                case ModelCharacterData:
                                    if (layoutData.GlyphBoundingBox.Bottom <= y)
                                        break;

                                    return new Point(x + layoutData.GlyphBoundingBox.Right, y);

                                case MovieCharacterData:
                                    if (layoutData.GlyphBoundingBox.Bottom <= y)
                                        break;

                                    return new Point(x + layoutData.GlyphBoundingBox.Right, y);
                            }

                            break;
                    }

                    break;
            }

            return new Point(x, y);
        }

        private float GetResourceDimensionScale(Image<Rgba32> resourceData)
        {
            if (resourceData.Width > resourceData.Height)
                return .5f;

            return .769230769230769230f;
        }

        private Size GetModelSize(string path)
        {
            switch (Path.GetFileNameWithoutExtension(path))
            {
                case "tip0170":
                    return new Size(113, 67);

                case "tip0280":
                    return new Size(114, 170);

                case "tip0580":
                    return new Size(265, 75);

                case "tip1070":
                    return new Size(64, 70);

                case "tip1130":
                    return new Size(114, 122);

                case "tip1140":
                    return new Size(114, 122);

                case "tip1150":
                    return new Size(114, 122);

                case "tip1240":
                    return new Size(183, 67);

                case "tip1510":
                    return new Size(183, 67);

                case "tip1900":
                    return new Size(64, 70);

                case "tip1930":
                    return new Size(114, 70);

                case "tip2400":
                    return new Size(126, 70);

                case "tip2430":
                    return new Size(87, 126);

                case "tip2520":
                    return new Size(143, 72);

                case "tip2900":
                    return new Size(102, 70);
            }

            return Size.Empty;
        }

        private Size GetMovieSize()
        {
            return new Size(176, 97);
        }
    }
}
