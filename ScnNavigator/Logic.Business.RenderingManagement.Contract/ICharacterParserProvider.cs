using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Parsers;

namespace Logic.Business.RenderingManagement.Contract
{
    [MapException(typeof(RenderingManagementException))]
    public interface ICharacterParserProvider
    {
        ICharacterParser GetSubtitleParser();
        ICharacterParser GetNarrationParser();
        ICharacterParser GetTipParser();
        ICharacterParser GetDefaultParser();
    }
}
