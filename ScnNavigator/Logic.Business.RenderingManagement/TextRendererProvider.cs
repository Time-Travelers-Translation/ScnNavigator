using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Layouts;
using Logic.Business.RenderingManagement.Contract.Renderers;
using Logic.Business.RenderingManagement.Contract.Renderers.DataClasses;
using Logic.Business.RenderingManagement.InternalContract.Renderers;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using SixLabors.ImageSharp;

namespace Logic.Business.RenderingManagement
{
    internal class TextRendererProvider : ITextRendererProvider
    {
        private readonly ICoCoKernel _kernel;
        private readonly ITextLayoutCreatorProvider _layoutCreatorProvider;

        public TextRendererProvider(ICoCoKernel kernel, ITextLayoutCreatorProvider layoutCreatorProvider)
        {
            _kernel = kernel;
            _layoutCreatorProvider = layoutCreatorProvider;
        }

        public ITextRenderer? GetIntroSubtitleRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetIntroSubtitleLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                VisibleLines = 2,
                OutlineRadius = 3,
                TextColor = Color.FromRgb(0xCE, 0xCE, 0xCE),
                TipTextColor = Color.FromRgb(0x6D, 0xC5, 0xDC),
                TextOutlineColor = Color.Black
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetSubtitleRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetSubtitleLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                VisibleLines = 2,
                OutlineRadius = 3,
                TextColor = Color.FromRgb(0xCE, 0xCE, 0xCE),
                TipTextColor = Color.FromRgb(0x6D, 0xC5, 0xDC),
                TextOutlineColor = Color.Black
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetNarrationRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetNarrationLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xFD, 0xFD, 0xFD),
                TipTextColor = Color.FromRgb(0x6D, 0xC5, 0xDC)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetUnselectedDecisionRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetDecisionLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xFD, 0xFD, 0xFD)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetSelectedDecisionRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetDecisionLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xF3, 0xB6, 0x00)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer GetDecisionRenderer(AssetPreference fontPreference)
        {
            return _kernel.Get<IDecisionTextRenderer>(
                new ConstructorParameter("fontPreference", fontPreference));
        }

        public ITextRenderer? GetBadEndTitleRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetBadEndTitleLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xFD, 0xFD, 0xFD)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetBadEndHintRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetBadEndHintLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xE4, 0x9C, 0x6B),
                DrawBoundingBoxes = true
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetSceneTitleRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetSceneTitleLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xFD, 0xFD, 0xFD)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetRouteNameRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetRouteNameLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xFD, 0xFD, 0xFD)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetOutlineRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetOutlineLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xDD, 0xD7, 0x67)
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetTipTitleRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetTipTitleLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.Black
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetTipTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetTipTextLayoutCreator(fontPreference, resourcePreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0x6D, 0xC5, 0xDC)
            };

            return GetTextRenderer<ITipTextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetHelpTitleRenderer(AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetHelpTitleLayoutCreator(fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.Black
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetHelpTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetHelpTextLayoutCreator(fontPreference, resourcePreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.Black
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetTutorialTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetTutorialTextLayoutCreator(fontPreference, resourcePreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.White
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetRevealTutorialTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetRevealTutorialTextLayoutCreator(fontPreference, resourcePreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.Black,
                TextOutlineColor = Color.White,
                OutlineRadius = 2
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetTimeLockTutorialTextRenderer(AssetPreference fontPreference, AssetPreference resourcePreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetTimeLockTutorialTextLayoutCreator(fontPreference, resourcePreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.Black
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetStaffrollTitleRenderer(int y, AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetStaffrollTitleLayoutCreator(y, fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xCE, 0xCE, 0xCE),
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        public ITextRenderer? GetStaffrollNameRenderer(int y, AssetPreference fontPreference)
        {
            ITextLayoutCreator? layoutCreator = _layoutCreatorProvider.GetStaffrollNameLayoutCreator(y, fontPreference);
            if (layoutCreator == null)
                return null;

            var options = new RenderOptions
            {
                TextColor = Color.FromRgb(0xCE, 0xCE, 0xCE),
            };

            return GetTextRenderer<ITextRenderer>(options, layoutCreator);
        }

        private TTextRenderer GetTextRenderer<TTextRenderer>(RenderOptions options, ITextLayoutCreator layoutCreator)
            where TTextRenderer : class
        {
            return _kernel.Get<TTextRenderer>(
                new ConstructorParameter("options", options),
                new ConstructorParameter("layoutCreator", layoutCreator));
        }
    }
}
