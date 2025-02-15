namespace Logic.Business.TimeTravelersManagement.Contract
{
    public interface ICharacterProvider
    {
        IList<char> GetAll();
        bool TryGet(char originalCharacter, out char mappedCharacter);
    }
}
