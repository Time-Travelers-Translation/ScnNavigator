using System.Collections.Generic;
using System.IO;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;
using Logic.Domain.Level5Management.Contract.Resource;
using Logic.Domain.Level5Management.InternalContract.Compression;

namespace Logic.Domain.Level5Management.Resource
{
    internal class PointVectorReader : IPointVectorReader
    {
        private readonly IStreamFactory _streamFactory;
        private readonly IBinaryFactory _binaryFactory;
        private readonly IDecompressor _decompressor;

        public PointVectorReader(IStreamFactory streamFactory, IBinaryFactory binaryFactory, IDecompressor decompressor)
        {
            _streamFactory = streamFactory;
            _binaryFactory = binaryFactory;
            _decompressor = decompressor;
        }

        public IReadOnlyList<PointVectorData>? Read(Stream input)
        {
            using IBinaryReaderX br = _binaryFactory.CreateReader(input, true);

            if (br.ReadString(4) != "XPVB")
                return null;

            br.BaseStream.Position = 8;
            short verticesOffset = br.ReadInt16();
            short verticesSize = br.ReadInt16();
            int verticesCount = br.ReadInt32();

            Stream verticesStream = _streamFactory.CreateSubStream(br.BaseStream, verticesOffset);
            verticesStream = _decompressor.Decompress(verticesStream, 0);

            using IBinaryReaderX vertexReader = _binaryFactory.CreateReader(verticesStream);

            var result = new List<PointVectorData>();

            for (var i = 0; i < verticesCount; i++)
            {
                result.Add(new PointVectorData
                {
                    x = vertexReader.ReadSingle(),
                    y = vertexReader.ReadSingle(),
                    z = vertexReader.ReadSingle(),
                    u = vertexReader.ReadSingle(),
                    v = vertexReader.ReadSingle()
                });
            }

            return result;
        }
    }
}
