namespace Logic.Domain.Level5Management.Contract.DataClasses.Font
{
    public class GlyphData
    {
        public ushort CodePoint { get; set; }
        public int Width { get; set; }

        public GlyphLocationData Location { get; set; }
        public GlyphDescriptionData Description { get; set; }
    }
}
