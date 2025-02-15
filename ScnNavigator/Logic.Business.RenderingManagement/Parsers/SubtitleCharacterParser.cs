using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Parsers;

namespace Logic.Business.RenderingManagement.Parsers
{
    internal class SubtitleCharacterParser : CharacterParser<SubtitleCharacterContext>, ISubtitleCharacterParser
    {
        protected override CharacterData? GetCharacter(string text, int position, out int length, SubtitleCharacterContext context)
        {
            CharacterData? character = base.GetCharacter(text, position, out length, context);

            if (IsSubtitleEnd(text, position))
            {
                length = 1;
                context.IsSubtitle = false;
            }

            if (character is FontCharacterData fontCharacter)
            {
                character = new SubtitleFontCharacterData
                {
                    IsSpeaker = !context.IsSubtitle,
                    Character = fontCharacter.Character,
                    IsFurigana = fontCharacter.IsFurigana,
                    IsTip = fontCharacter.IsTip,
                    TipNumber = fontCharacter.TipNumber
                };
            }

            if (IsSubtitleStart(text, position))
            {
                length = 1;
                context.IsSubtitle = true;
            }

            return character;
        }

        private bool IsSubtitleStart(string text, int position)
        {
            return text[position] is '“' or '「';
        }

        private bool IsSubtitleEnd(string text, int position)
        {
            return text[position] is '”' or '」';
        }
    }
}
