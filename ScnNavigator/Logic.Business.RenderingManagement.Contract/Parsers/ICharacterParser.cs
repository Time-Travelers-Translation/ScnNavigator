using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Contract.Parsers
{
    [MapException(typeof(RenderingManagementException))]
    public interface ICharacterParser
    {
        IList<CharacterData> Parse(string text);
    }
}
