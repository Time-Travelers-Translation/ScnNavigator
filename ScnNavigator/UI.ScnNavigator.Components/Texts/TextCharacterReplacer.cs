using Logic.Business.TimeTravelersManagement.Contract;
using UI.ScnNavigator.Components.Contract.Texts;

namespace UI.ScnNavigator.Components.Texts
{
    internal class TextCharacterReplacer: ITextCharacterReplacer
    {
        private readonly ICharacterProvider _characterProvider;

        public TextCharacterReplacer(ICharacterProvider characterProvider)
        {
            _characterProvider = characterProvider;
        }

        public string ReplaceCharacters(string text)
        {
            foreach (char originalChar in _characterProvider.GetAll())
            {
                if (_characterProvider.TryGet(originalChar, out char mappedChar))
                    text = text.Replace(originalChar, mappedChar);
            }

            return text;
        }
    }
}
