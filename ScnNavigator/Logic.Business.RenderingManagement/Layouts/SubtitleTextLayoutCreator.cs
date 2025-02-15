using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Layouts;
using Logic.Business.TimeTravelersManagement.Contract.Resources;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.Font;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Layouts
{
    internal class SubtitleTextLayoutCreator : TextLayoutCreator, ISubtitleTextLayoutCreator
    {
        public SubtitleTextLayoutCreator(LayoutOptions options, FontImageData fontData, IGlyphProviderFactory glyphProviderFactory,
            ICharacterParser characterParser, IResourceProvider resourceProvider) 
            : base(fontData, options, glyphProviderFactory, characterParser, resourceProvider)
        {
        }

        protected override Rectangle GetCharacterBoundingBox(CharacterData character, Point characterLocation, out bool isVisible)
        {
            Rectangle boundingBox = base.GetCharacterBoundingBox(character, characterLocation, out isVisible);

            if (character is SubtitleFontCharacterData subtitleCharacter)
                isVisible = subtitleCharacter.IsVisible;

            return boundingBox;
        }
    }
}
