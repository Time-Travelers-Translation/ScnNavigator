using System.Collections.Generic;
using System.IO;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.Resource;
using Logic.Domain.Level5Management.InternalContract.Compression;

namespace Logic.Domain.Level5Management.Resource
{
    internal class PointIndexReader : IPointIndexReader
    {
        private readonly IBinaryFactory _binaryFactory;
        private readonly IDecompressor _decompressor;
        private readonly IStreamFactory _streamFactory;

        public PointIndexReader(IBinaryFactory binaryFactory, IDecompressor decompressor, IStreamFactory streamFactory)
        {
            _binaryFactory = binaryFactory;
            _decompressor = decompressor;
            _streamFactory = streamFactory;
        }

        public IList<short>? Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, true);

            if (br.ReadString(4) != "XPVI")
                return null;

            br.BaseStream.Position = 8;
            int pointCount = br.ReadInt32();

            Stream pointStream = _streamFactory.CreateSubStream(br.BaseStream, 12);
            pointStream = _decompressor.Decompress(pointStream, 0);

            using IBinaryReaderX pointReader = _binaryFactory.CreateReader(pointStream);

            var result = new List<short>();

            for (var i = 0; i < pointCount; i++)
                result.Add(pointReader.ReadInt16());

            return result;

        }
    }
}
