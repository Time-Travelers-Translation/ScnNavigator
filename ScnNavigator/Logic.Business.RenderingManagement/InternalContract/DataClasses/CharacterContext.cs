namespace Logic.Business.RenderingManagement.InternalContract.DataClasses
{
    internal class CharacterContext
    {
        public bool IsTip { get; set; }
        public int TipNumber { get; set; } = -1;
        public bool IsFuriganaBottom { get; set; }
        public bool IsFuriganaTop { get; set; }
    }
}
