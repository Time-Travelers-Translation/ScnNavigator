using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Parsers;

namespace Logic.Business.RenderingManagement.InternalContract.Parsers
{
    [MapException(typeof(RenderingManagementException))]
    public interface ITipCharacterParser : ICharacterParser
    {
    }
}
