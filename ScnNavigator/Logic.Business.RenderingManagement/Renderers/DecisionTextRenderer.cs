using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Renderers;
using Logic.Business.RenderingManagement.InternalContract.Renderers;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Logic.Business.RenderingManagement.Renderers
{
    internal class DecisionTextRenderer : IDecisionTextRenderer
    {
        private readonly ITextRenderer? _unselectedDecisionRenderer;
        private readonly ITextRenderer? _selectedDecisionRenderer;

        private readonly Image<Rgba32> _textBuffer;

        public DecisionTextRenderer(ITextRendererProvider textRendererProvider, AssetPreference fontPreference)
        {
            _unselectedDecisionRenderer = textRendererProvider.GetUnselectedDecisionRenderer(fontPreference);
            _selectedDecisionRenderer = textRendererProvider.GetSelectedDecisionRenderer(fontPreference);

            _textBuffer = new Image<Rgba32>(295, 67);
        }

        public void Render(Image<Rgba32> image, string text)
        {
            _textBuffer.Mutate(x => x.Clear(Color.Transparent));
            _unselectedDecisionRenderer?.Render(_textBuffer, text);

            image.Mutate(x => x.DrawImage(_textBuffer, new Point(13, 47), 1f));

            _textBuffer.Mutate(x => x.Clear(Color.Transparent));
            _selectedDecisionRenderer?.Render(_textBuffer, text);

            image.Mutate(x => x.DrawImage(_textBuffer, new Point(13, 127), 1f));
        }
    }
}
