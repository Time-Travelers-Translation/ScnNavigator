using CrossCutting.Core.Contract.DependencyInjection;
using System;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.InternalContract.Font;

namespace Logic.Domain.Level5Management.Font
{
    internal class FontWriterFactory : IFontWriterFactory
    {
        private readonly ICoCoKernel _kernel;

        public FontWriterFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IFontWriter Create(int version)
        {
            switch (version)
            {
                case 0:
                    return _kernel.Get<IFnt00Writer>();

                case 1:
                    return _kernel.Get<IFnt01Writer>();

                default:
                    throw new InvalidOperationException($"Unknown font version {version}.");
            }
        }
    }
}
