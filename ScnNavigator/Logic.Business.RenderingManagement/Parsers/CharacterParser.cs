using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;

namespace Logic.Business.RenderingManagement.Parsers
{
    internal class CharacterParser<TContext> : ICharacterParser
        where TContext : CharacterContext, new()
    {
        public virtual IList<CharacterData> Parse(string text)
        {
            var result = new List<CharacterData>();
            var context = new TContext();

            var position = 0;
            while (position < text.Length)
            {
                CharacterData? character = GetCharacter(text, position, out int length, context);
                if (character != null)
                    result.Add(character);

                position += length;
            }

            return result;
        }

        protected virtual CharacterData? GetCharacter(string text, int position, out int length, TContext context)
        {
            if (IsLineBreak(text, position, out length))
                return new LineBreakCharacterData();

            if (IsFuriganaStart(text, position))
            {
                length = 1;
                context.IsFuriganaBottom = true;
                context.IsFuriganaTop = false;
            }
            else if (IsFuriganaSplit(text, position, context.IsFuriganaBottom))
            {
                length = 1;
                context.IsFuriganaBottom = false;
                context.IsFuriganaTop = true;
            }
            else if (IsFuriganaEnd(text, position))
            {
                length = 1;
                context.IsFuriganaBottom = true;
                context.IsFuriganaTop = false;
            }
            else if (IsTipStart(text, position, out length, out int tipNumber))
            {
                context.IsTip = true;
                context.TipNumber = tipNumber;
            }
            else if (IsTipEnd(text, position, out length))
            {
                context.IsTip = false;
                context.TipNumber = -1;
            }
            else if (IsIcon(text, position, out length))
            {
                return new IconCharacterData
                {
                    IconName = GetIconName(text, position),
                    IsTip = context.IsTip,
                    TipNumber = context.TipNumber
                };
            }
            else if (IsBlank(text, position, out length))
            {
                return new BlankCharacterData
                {
                    Width = GetBlankWidth(text, position)
                };
            }
            else if (!IsControlCode(text, position, out length))
            {
                length = 1;
                return new FontCharacterData
                {
                    Character = text[position],
                    IsFurigana = context.IsFuriganaTop,
                    IsTip = context.IsTip,
                    TipNumber = context.TipNumber
                };
            }

            return null;
        }

        private bool IsLineBreak(string text, int position, out int length)
        {
            length = 1;

            if (text[position] == '\n')
                return true;

            if (position + 1 >= text.Length)
                return false;

            length = 2;
            return (text[position] == '\r' && text[position + 1] == '\n')
                || (text[position] == '\\' && text[position + 1] == 'n');
        }

        private bool IsControlCode(string text, int position, out int length)
        {
            length = 2;

            if (text[position] != '<')
                return false;

            int endIndex = text.IndexOf('>', position);
            if (endIndex < 0)
                return false;

            if (text.IndexOf('<', position + 1, endIndex - position - 1) >= 0)
                return false;

            length = endIndex - position + 1;
            return true;
        }

        private bool IsFuriganaStart(string text, int position)
        {
            return text[position] == '[';
        }

        private bool IsFuriganaSplit(string text, int position, bool isFuriganaBottom)
        {
            return isFuriganaBottom && text[position] == '/';
        }

        private bool IsFuriganaEnd(string text, int position)
        {
            return text[position] == ']';
        }

        private bool IsTipStart(string text, int position, out int length, out int tipNumber)
        {
            length = 8;
            tipNumber = 0;

            if (position + length >= text.Length)
                return false;

            if (text[position..(position + 4)] != "<TIP")
                return false;

            if (text[position + length - 1] != '>')
                return false;

            return int.TryParse(text[(position + 4)..(position + 7)], out tipNumber);
        }

        private bool IsTipEnd(string text, int position, out int length)
        {
            length = 6;

            if (position + length >= text.Length)
                return false;

            return text[position..(position + length)] == "</TIP>";
        }

        private bool IsIcon(string text, int position, out int length)
        {
            length = 6;

            if (position + length >= text.Length)
                return false;

            if (text[position..(position + length)] != "<ICON\"")
                return false;

            int closeTagPosition = text.IndexOf("\">", position + length, StringComparison.InvariantCulture);
            if (closeTagPosition < 0)
                return false;

            length = closeTagPosition - position + 2;
            return true;
        }

        private string GetIconName(string text, int position)
        {
            int closeTagPosition = text.IndexOf("\">", position + 6, StringComparison.InvariantCulture);
            if (closeTagPosition < 0)
                return string.Empty;

            return text[(position + 6)..closeTagPosition];
        }

        private bool IsBlank(string text, int position, out int length)
        {
            length = 6;

            if (position + length >= text.Length)
                return false;

            if (text[position..(position + length)] != "<BLANK")
                return false;

            int closeTagPosition = text.IndexOf('>', position + length);
            if (closeTagPosition < 0)
                return false;

            length = closeTagPosition - position + 1;
            return true;
        }

        private int GetBlankWidth(string text, int position)
        {
            int closeTagPosition = text.IndexOf('>', position + 6);
            if (closeTagPosition < 0)
                return 0;

            if (!int.TryParse(text[(position + 6)..closeTagPosition], out int width))
                return 0;

            return width;
        }
    }
}
