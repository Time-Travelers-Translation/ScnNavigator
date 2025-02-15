using CrossCutting.Abstract.DataClasses;

namespace Logic.Business.TimeTravelersManagement.Contract.Texts
{
    public interface ITipTextManager
    {
        TextData? GetTitle(int tipIndex);
        TextData? GetText(int tipIndex);
    }
}
