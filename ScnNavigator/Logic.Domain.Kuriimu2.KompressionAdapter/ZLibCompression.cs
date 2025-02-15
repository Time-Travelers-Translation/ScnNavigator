using Logic.Domain.Kuriimu2.KompressionAdapter.InternalContract;
using Kompression;
using Kompression.Contract;

namespace Logic.Domain.Kuriimu2.KompressionAdapter
{
    internal class ZLibCompression : IZLibCompression
    {
        private readonly ICompression _compression;

        public ZLibCompression()
        {
            _compression = Compressions.ZLib.Build();
        }

        public void Decompress(Stream input, Stream output)
        {
            _compression.Decompress(input, output);
        }

        public void Compress(Stream input, Stream output)
        {
            _compression.Compress(input, output);
        }
    }
}
