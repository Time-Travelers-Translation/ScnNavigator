using System;
using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.Level5Management.Contract.Image;
using Logic.Domain.Level5Management.InternalContract.Image;

namespace Logic.Domain.Level5Management.Image
{
    internal class ImageReaderFactory : IImageReaderFactory
    {
        private readonly ICoCoKernel _kernel;

        public ImageReaderFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IImageReader Create(int version)
        {
            switch (version)
            {
                case 0:
                    return _kernel.Get<IImg00Reader>();

                default:
                    throw new InvalidOperationException($"Unknown image version {version}.");
            }
        }
    }
}
