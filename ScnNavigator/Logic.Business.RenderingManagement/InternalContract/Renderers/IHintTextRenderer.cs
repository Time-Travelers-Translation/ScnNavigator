using CrossCutting.Core.Contract.Aspects;
using Logic.Business.RenderingManagement.Contract.Exceptions;
using Logic.Business.RenderingManagement.Contract.Renderers;

namespace Logic.Business.RenderingManagement.InternalContract.Renderers
{
    [MapException(typeof(RenderingManagementException))]
    public interface IHintTextRenderer : ITextRenderer
    {
    }
}
