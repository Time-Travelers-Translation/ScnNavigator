using Logic.Domain.Level5Management.Contract.Image;
using System;
using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.Level5Management.InternalContract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class ImageWriterFactory : IImageWriterFactory
    {
        private readonly ICoCoKernel _kernel;

        public ImageWriterFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IImageWriter Create(int version)
        {
            switch (version)
            {
                case 0:
                    return _kernel.Get<IImg00Writer>();

                default:
                    throw new InvalidOperationException($"Unknown font version {version}.");
            }
        }
    }
}
