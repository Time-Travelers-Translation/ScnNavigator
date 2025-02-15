using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Renderers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Layouts;
using Logic.Business.RenderingManagement.InternalContract.Renderers;
using Logic.Domain.Level5Management.Contract.Font;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Logic.Business.RenderingManagement.Renderers
{
    internal class TipTextRenderer : TextRenderer, ITipTextRenderer
    {
        public TipTextRenderer(RenderOptions options, ITipTextLayoutCreator layoutCreator, IGlyphProviderFactory glyphProviderFactory)
            : base(options, layoutCreator, glyphProviderFactory)
        {
        }

        protected override void DrawCharacter(Image<Rgba32> image, TextLayoutCharacterData character, bool isLineVisible)
        {
            switch (character)
            {
                case TextLayoutTextureCharacterData textureCharacter:
                    if (textureCharacter.Image == null)
                        return;

                    Image<Rgba32> texture = textureCharacter.Image.Clone(x => x.Resize(textureCharacter.BoundingBox.Size));
                    image.Mutate(x => x.DrawImage(texture, textureCharacter.BoundingBox.Location, 1f));

                    return;
            }

            base.DrawCharacter(image, character, isLineVisible);
        }
    }
}
