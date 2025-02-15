using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Resources
{
    internal class ImageResourceProvider : IImageResourceProvider
    {
        public Image<Rgba32>? GetDecisionResource()
        {
            return GetResourceImage("decision.png");
        }

        public Image<Rgba32>? GetDecisionBackgroundResource()
        {
            return GetResourceImage("decision_bg.png");
        }

        public Image<Rgba32>? GetDecisionArrowResource()
        {
            return GetResourceImage("decision_arrow.png");
        }

        public Image<Rgba32>? GetNarrationResource()
        {
            return GetResourceImage("narration.png");
        }

        public Image<Rgba32>? GetHintButtonResource()
        {
            return GetResourceImage("hint_btn.png");
        }

        public Image<Rgba32>? GetBackButtonResource()
        {
            return GetResourceImage("back_btn.png");
        }

        public Image<Rgba32>? GetHintCaptionResource()
        {
            return GetResourceImage("hint_caption.png");
        }

        public Image<Rgba32>? GetSceneTitleBackgroundResource()
        {
            return GetResourceImage("title_bg.png");
        }

        public Image<Rgba32>? GetOutlineBackgroundResource()
        {
            return GetResourceImage("outline_bg.png");
        }

        public Image<Rgba32>? GetTipBackgroundResource()
        {
            return GetResourceImage("tip_bg.png");
        }

        public Image<Rgba32>? GetTipTitleResource()
        {
            return GetResourceImage("tip_title.png");
        }

        public Image<Rgba32>? GetTipIconResource()
        {
            return GetResourceImage("tip_icon.png");
        }

        public Image<Rgba32>? GetHelpBackgroundResource()
        {
            return GetResourceImage("help_bg.png");
        }

        public Image<Rgba32>? GetTutorialBackgroundResource()
        {
            return GetResourceImage("tutorial_bg.png");
        }
        public Image<Rgba32>? GetRevealTutorialBackgroundResource()
        {
            return GetResourceImage("tutorial_bg2.png");
        }

        public Image<Rgba32>? GetTimeLockTutorialBackgroundResource()
        {
            return GetResourceImage("tutorial_bg3.png");
        }

        public Image<Rgba32>? GetTimeLockResource()
        {
            return GetResourceImage("time_lock.png");
        }

        private Image<Rgba32>? GetResourceImage(string name)
        {
            Stream? imageStream = GetResourceStream(name);
            if (imageStream == null)
                return null;

            return Image.Load<Rgba32>(imageStream);
        }

        private Stream? GetResourceStream(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly?.GetManifestResourceStream(name);
        }
    }
}
