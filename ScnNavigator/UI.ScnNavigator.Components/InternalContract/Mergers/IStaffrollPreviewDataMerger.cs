using CrossCutting.Abstract.DataClasses;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.TimeTravelersManagement.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using UI.ScnNavigator.Components.Contract.Exceptions;

namespace UI.ScnNavigator.Components.InternalContract.Mergers
{
    [MapException(typeof(ScnNavigatorComponentsException))]
    public interface IStaffrollPreviewDataMerger
    {
        StaffRollPreviewData Merge(EventText[] originalTexts, StaffrollTextData[]? translatedTexts);
    }
}
