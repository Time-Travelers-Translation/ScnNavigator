using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Parsers;

namespace Logic.Business.RenderingManagement.Parsers
{
    internal class NarrationCharacterParser : CharacterParser<NarratorCharacterContext>, INarrationCharacterParser
    {
        protected override CharacterData? GetCharacter(string text, int position, out int length, NarratorCharacterContext context)
        {
            if (IsNarratorStart(text, position, out length))
            {
                context.IsNarratorText = true;

                return null;
            }

            CharacterData? character = base.GetCharacter(text, position, out length, context);
            if (character is LineBreakCharacterData || context.IsNarratorText)
                return character;

            length = 1;
            return null;
        }

        private bool IsNarratorStart(string text, int position, out int length)
        {
            length = 1;

            if (text[position] != '＊')
                return false;

            if (position + 1 >= text.Length)
                return false;

            if (text[position + 1] != '＊')
                return true;

            length++;

            return true;
        }
    }
}
