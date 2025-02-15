using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UI.ScnNavigator.Resources.Contract;

namespace UI.ScnNavigator.Resources
{
    internal class ScreenResourceProvider : IScreenResourceProvider
    {
        private readonly IImageResourceProvider _imageProvider;

        public ScreenResourceProvider(IImageResourceProvider imageProvider)
        {
            _imageProvider = imageProvider;
        }

        public Image<Rgba32> GetSubtitleScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            image.Mutate(x => x.Clear(Color.Wheat));

            return image;
        }

        public Image<Rgba32> GetNarrationScreen()
        {
            Image<Rgba32> image = CreateBottomScreen();

            image.Mutate(x => x.Clear(Color.Black));

            Image<Rgba32>? narrationImage = _imageProvider.GetNarrationResource();
            if (narrationImage == null)
                return image;

            var narrationPoint = new Point(0, image.Height - narrationImage.Height);
            image.Mutate(x => x.DrawImage(narrationImage, narrationPoint, 1f));

            return image;
        }

        public Image<Rgba32> GetDecisionScreen()
        {
            Image<Rgba32> image = CreateBottomScreen();

            Image<Rgba32>? decisionBgImage = _imageProvider.GetDecisionBackgroundResource();
            if (decisionBgImage == null)
                return image;

            Image<Rgba32>? decisionImage = _imageProvider.GetDecisionResource();
            if (decisionImage == null)
                return image;

            Image<Rgba32>? decisionArrowImage = _imageProvider.GetDecisionArrowResource();
            if (decisionArrowImage == null)
                return image;

            Image<Rgba32> decisionImage2 = decisionImage.Clone();

            var colorMatrix = new ColorMatrix(
                0xF3 / 255f, 0f, 0f, 0f,
                0f, 0xB6 / 255f, 0f, 0f,
                0f, 0f, 0f, 0f,
                0f, 0f, 0f, 1f,
                0f, 0f, 0f, 0f);
            decisionImage2.Mutate(x => x.Filter(colorMatrix));

            image.Mutate(x => x
                .DrawImage(decisionBgImage, Point.Empty, 1f)
                .DrawImage(decisionImage, new Point(12, 46), 1f)
                .DrawImage(decisionImage2, new Point(12, 126), 1f)
                .DrawImage(decisionArrowImage, new Point(29, 148), 1f));

            return image;
        }

        public Image<Rgba32> GetBadEndTitleScreen()
        {
            Image<Rgba32> image = GetNarrationScreen();

            Image<Rgba32>? hintButtonImage = _imageProvider.GetHintButtonResource();
            if (hintButtonImage == null)
                return image;

            var hintButtonPoint = new Point((image.Width - hintButtonImage.Width) / 2, 163);
            image.Mutate(x => x.DrawImage(hintButtonImage, hintButtonPoint, 1f));

            return image;
        }

        public Image<Rgba32> GetBadEndHintScreen()
        {
            Image<Rgba32> image = CreateBottomScreen();

            image.Mutate(x => x.Clear(Color.Black));

            Image? backButtonImage = _imageProvider.GetBackButtonResource();
            if (backButtonImage != null)
            {
                var backButtonPoint = new Point((image.Width - backButtonImage.Width) / 2, image.Height - backButtonImage.Height);
                image.Mutate(x => x.DrawImage(backButtonImage, backButtonPoint, 1f));
            }

            Image? hintCaptionImage = _imageProvider.GetHintCaptionResource();
            if (hintCaptionImage == null)
                return image;

            var hintCaptionPoint = new Point((image.Width - hintCaptionImage.Width) / 2, 54);
            image.Mutate(x => x.DrawImage(hintCaptionImage, hintCaptionPoint, 1f));

            return image;
        }

        public Image<Rgba32> GetSceneTitleScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            Image? titleImage = _imageProvider.GetSceneTitleBackgroundResource();
            if (titleImage != null)
                image.Mutate(x => x.DrawImage(titleImage, Point.Empty, 1f));

            return image;
        }

        public Image<Rgba32> GetOutlineScreen()
        {
            Image<Rgba32> image = CreateBottomScreen();

            Image? outlineImage = _imageProvider.GetOutlineBackgroundResource();
            if (outlineImage != null)
                image.Mutate(x => x.DrawImage(outlineImage, Point.Empty, 1f));

            return image;
        }

        public Image<Rgba32> GetTipScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            Image? tipBgResource = _imageProvider.GetTipBackgroundResource();
            if (tipBgResource != null)
                image.Mutate(x => x.DrawImage(tipBgResource, Point.Empty, 1f));

            Image? tipTitleResource = _imageProvider.GetTipTitleResource();
            if (tipTitleResource != null)
                image.Mutate(x => x.DrawImage(tipTitleResource, new Point(0, 10), 1f));

            Image? tipIconResource = _imageProvider.GetTipIconResource();
            if (tipIconResource != null)
                image.Mutate(x => x.DrawImage(tipIconResource, new Point(5, 10), 1f));

            return image;
        }

        public Image<Rgba32> GetHelpScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            Image? helpBgResource = _imageProvider.GetHelpBackgroundResource();
            if (helpBgResource != null)
                image.Mutate(x => x.DrawImage(helpBgResource, Point.Empty, 1f));

            return image;
        }

        public Image<Rgba32> GetTutorialScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            Image? tutorialBgResource = _imageProvider.GetTutorialBackgroundResource();
            if (tutorialBgResource != null)
            {
                var topLeftPoint = new Point((image.Width - tutorialBgResource.Width) / 2, (image.Height - tutorialBgResource.Height) / 2);
                image.Mutate(x => x.DrawImage(tutorialBgResource, topLeftPoint, 1f));
            }

            return image;
        }

        public Image<Rgba32> GetRevealTutorialScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            Image? tutorialBgResource = _imageProvider.GetRevealTutorialBackgroundResource();
            if (tutorialBgResource != null)
            {
                var topLeftPoint = new Point(0, (image.Height - tutorialBgResource.Height) / 2);
                image.Mutate(x => x.DrawImage(tutorialBgResource, topLeftPoint, 1f));
            }

            return image;
        }

        public Image<Rgba32> GetTimeLockTutorialScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            Image? tutorialBgResource = _imageProvider.GetTimeLockTutorialBackgroundResource();
            if (tutorialBgResource != null)
                image.Mutate(x => x.DrawImage(tutorialBgResource, Point.Empty, 1f));

            return image;
        }

        public Image<Rgba32> GetStaffrollScreen()
        {
            Image<Rgba32> image = CreateTopScreen();

            image.Mutate(x => x.Clear(Color.Black));

            return image;
        }

        private Image<Rgba32> CreateTopScreen()
        {
            return new Image<Rgba32>(400, 240);
        }

        private Image<Rgba32> CreateBottomScreen()
        {
            return new Image<Rgba32>(320, 240);
        }
    }
}
