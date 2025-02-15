using System.Collections.Generic;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Font
{
    public class FontGlyphData
    {
        public ushort FallbackCharacter { get; set; }
        public int MaxHeight { get; set; }
        public IDictionary<ushort, GlyphData> Glyphs { get; set; }
    }
}
