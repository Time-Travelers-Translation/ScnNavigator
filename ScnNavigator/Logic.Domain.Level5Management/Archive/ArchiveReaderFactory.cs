using Logic.Domain.Level5Management.Contract.Archive;
using System;
using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Domain.Level5Management.Contract.Enums.Archive;

namespace Logic.Domain.Level5Management.Archive
{
    internal class ArchiveReaderFactory: IArchiveReaderFactory
    {
        private readonly ICoCoKernel _kernel;

        public ArchiveReaderFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IArchiveReader Create(ArchiveType type)
        {
            switch (type)
            {
                case ArchiveType.Xpck:
                    return _kernel.Get<IXpckReader>();

                case ArchiveType.Xfsp:
                    return _kernel.Get<IXfspReader>();

                default:
                    throw new InvalidOperationException($"Unknown archive type {type}.");
            }
        }
    }
}
