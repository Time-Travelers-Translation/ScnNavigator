using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Contract.Layouts
{
    [MapException(typeof(RenderingManagementException))]
    public interface ITextLayoutCreator
    {
        FontImageData Font { get; }

        TextLayoutData Create(string text, Size boundingBox);
        TextLayoutData Create(IList<CharacterData> characters, Size boundingBox);
    }
}
