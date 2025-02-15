using System;
using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.InternalContract.Font;

namespace Logic.Domain.Level5Management.Font
{
    internal class FontReaderFactory : IFontReaderFactory
    {
        private readonly ICoCoKernel _kernel;

        public FontReaderFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IFontReader Create(int version)
        {
            switch (version)
            {
                case 0:
                    return _kernel.Get<IFnt00Reader>();

                case 1:
                    return _kernel.Get<IFnt01Reader>();
                
                default:
                    throw new InvalidOperationException($"Unknown font version {version}.");
            }
        }
    }
}
