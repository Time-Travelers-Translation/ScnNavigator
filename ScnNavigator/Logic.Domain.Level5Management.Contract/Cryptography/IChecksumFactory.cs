using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Kuriimu2.KryptographyAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Exceptions.Cryptography;

namespace Logic.Domain.Level5Management.Contract.Cryptography
{
    [MapException(typeof(ChecksumFactoryException))]
    public interface IChecksumFactory
    {
        IChecksum<uint> CreateCrc32();
        IChecksum<uint> CreateCrc32Jam();
        IChecksum<ushort> CreateCrc16();
    }
}
