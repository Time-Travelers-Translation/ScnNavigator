using System;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.Level5Management.Contract.DataClasses.Font;
using Logic.Domain.Level5Management.Contract.Enums;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.InternalContract.Font;

namespace Logic.Domain.Level5Management.Font
{
    internal class GlyphProviderFactory : IGlyphProviderFactory
    {
        private readonly ICoCoKernel _kernel;

        public GlyphProviderFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IGlyphProvider Create(FontImageData fontData)
        {
            switch (fontData.Platform)
            {
                case PlatformType.Ctr:
                    return _kernel.Get<ICtrGlyphProvider>(new ConstructorParameter("fontData", fontData));

                case PlatformType.Psp:
                    return _kernel.Get<IPspGlyphProvider>(new ConstructorParameter("fontData", fontData));

                default:
                    throw new InvalidOperationException($"Unsupported platform {fontData.Platform} for glyph providers.");
            }
        }
    }
}
