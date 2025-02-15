using System.IO;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Level5Management.Contract.Enums.Compression;
using Logic.Domain.Level5Management.Contract.Exceptions.Compression;

namespace Logic.Domain.Level5Management.InternalContract.Compression
{
    [MapException(typeof(CompressorException))]
    public interface ICompressor
    {
        Stream Compress(Stream input, CompressionType compressionType);
    }
}
