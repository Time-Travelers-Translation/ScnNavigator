using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Parsers;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement.Parsers
{
    internal class TipCharacterParser : CharacterParser<TipCharacterContext>, ITipCharacterParser
    {
        protected override CharacterData? GetCharacter(string text, int position, out int length, TipCharacterContext context)
        {
            if (context.SetTexture)
            {
                length = 0;

                context.SetTexture = false;
                return context.Texture;
            }

            if (context.SetModel)
            {
                length = 0;

                context.SetModel = false;
                return context.Model;
            }

            if (context.SetMovie)
            {
                length = 0;

                context.SetMovie = false;
                return context.Movie;
            }

            if (IsTexture(text, position, out length, out string? path, out Point? pos))
            {
                return context.Texture = new TextureCharacterData
                {
                    Path = path!,
                    Location = pos!.Value
                };
            }

            if (IsModel(text, position, out length, out path, out pos))
            {
                return context.Model = new ModelCharacterData
                {
                    Path = path!,
                    Location = pos!.Value
                };
            }

            if (IsMovie(text, position, out length, out path, out pos))
            {
                return context.Movie = new MovieCharacterData
                {
                    Path = path!,
                    Location = pos!.Value
                };
            }

            CharacterData? character = base.GetCharacter(text, position, out length, context);
            context.SetTexture = context.Texture != null && character is LineBreakCharacterData;
            context.SetModel = context.Model != null && character is LineBreakCharacterData;
            context.SetMovie = context.Movie != null && character is LineBreakCharacterData;

            return character;
        }

        private bool IsTexture(string text, int position, out int length, out string? path, out Point? pos)
        {
            return TryParseObjectInfo("texture", text, position, out length, out path, out pos);
        }

        private bool IsModel(string text, int position, out int length, out string? path, out Point? pos)
        {
            return TryParseObjectInfo("model", text, position, out length, out path, out pos);
        }

        private bool IsMovie(string text, int position, out int length, out string? path, out Point? pos)
        {
            return TryParseObjectInfo("movie", text, position, out length, out path, out pos);
        }

        private bool TryParseObjectInfo(string objectIdentifier, string text, int position, out int length, out string? texPath, out Point? texPos)
        {
            length = 1;
            texPath = null;
            texPos = null;

            if (text[position] != ':')
                return false;

            if (position + length + objectIdentifier.Length >= text.Length)
                return false;

            objectIdentifier += '=';
            if (text[(position + length)..(position + length + objectIdentifier.Length)] != objectIdentifier)
                return false;

            int colonIndex = text.IndexOf(':', position + length + objectIdentifier.Length);
            if (colonIndex < 0)
                return false;

            texPath = text[(position + length + 8)..colonIndex];
            length = colonIndex - position + 1;

            if (position + length + 4 >= text.Length)
                return false;

            if (text[(position + length)..(position + length + 4)] != "pos=")
                return false;

            int commaIndex = text.IndexOf(',', position + length + 4);
            if (commaIndex < 0)
                return false;

            if (!int.TryParse(text[(position + length + 4)..commaIndex], out int posX))
                return false;
            length = commaIndex - position + 1;

            colonIndex = text.IndexOf(':', position + length);
            if (colonIndex < 0)
                return false;

            if (!int.TryParse(text[(position + length)..colonIndex], out int posY))
                return false;

            texPos = new Point(posX, posY);
            length = colonIndex + 1;

            return true;
        }
    }
}
