using System.IO;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.Level5Management.Contract.Enums.Compression;
using Logic.Domain.Level5Management.Contract.Exceptions.Compression;

namespace Logic.Domain.Level5Management.InternalContract.Compression
{
    [MapException(typeof(DecompressorException))]
    public interface IDecompressor
    {
        void Decompress(Stream input, Stream output, long offset);
        Stream Decompress(Stream input, long offset);

        CompressionType PeekCompressionType(Stream input, long offset);
    }
}
