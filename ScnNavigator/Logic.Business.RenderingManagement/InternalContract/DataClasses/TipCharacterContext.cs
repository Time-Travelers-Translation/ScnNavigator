using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.InternalContract.DataClasses
{
    internal class TipCharacterContext : CharacterContext
    {
        public TextureCharacterData? Texture { get; set; }
        public ModelCharacterData? Model { get; set; }
        public MovieCharacterData? Movie { get; set; }

        public bool SetTexture { get; set; }
        public bool SetModel { get; set; }
        public bool SetMovie { get; set; }
    }
}
