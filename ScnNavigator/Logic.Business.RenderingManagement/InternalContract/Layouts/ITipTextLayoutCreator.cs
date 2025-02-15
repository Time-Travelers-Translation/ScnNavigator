using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Layouts;

namespace Logic.Business.RenderingManagement.InternalContract.Layouts
{
    [MapException(typeof(RenderingManagementException))]
    public interface ITipTextLayoutCreator : ITextLayoutCreator
    {
    }
}
