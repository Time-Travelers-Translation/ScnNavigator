using ImGui.Forms.Models;
using ImGui.Forms.Resources;
using ImGui.Forms.Factories;
using UI.ScnNavigator.Resources.Contract;
using UI.ScnNavigator.Resources.Contract.Enums;

namespace UI.ScnNavigator.Resources
{
    internal class FontProvider : IFontProvider
    {
        private const string RobotoResource_ = "roboto.ttf";
        private const string NotoJpResource_ = "notojp.ttf";

        private const string RobotoName_ = "Roboto";
        private const string NotoJpName_ = "NotoJp";

        private const int FontSize_ = 15;
        
        public void RegisterFont(Font font)
        {
            FontGlyphRange range;
            switch (font)
            {
                case Font.Roboto:
                    range = FontGlyphRange.Latin;
                    FontFactory.RegisterFromResource(RobotoName_, RobotoResource_, range, "“”☆ωΛμν≦×Θ■▽ψΩ¥Ш♯♂♀∀♪£○●φξ★≧◎△◇Ж◆δД");
                    break;

                case Font.NotoJp:
                    range = FontGlyphRange.ChineseJapanese | FontGlyphRange.Symbols;
                    FontFactory.RegisterFromResource(NotoJpName_, NotoJpResource_, range);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported font {font}.");
            }
        }

        public FontResource GetFont(Font font)
        {
            switch (font)
            {
                case Font.Roboto:
                    return FontFactory.Get(RobotoName_, FontSize_, GetFont(Font.NotoJp));

                case Font.NotoJp:
                    return FontFactory.Get(NotoJpName_, FontSize_);

                default:
                    throw new InvalidOperationException($"Unsupported font {font}.");
            }
        }
    }
}
