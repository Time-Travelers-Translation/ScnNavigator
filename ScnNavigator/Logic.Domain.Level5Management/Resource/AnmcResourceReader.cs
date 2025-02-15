using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using Logic.Domain.Level5Management.Contract.DataClasses.Image;
using Logic.Domain.Level5Management.Contract.DataClasses.Resource;
using Logic.Domain.Level5Management.Contract.Image;
using Logic.Domain.Level5Management.Contract.Resource;
using SixLabors.ImageSharp;

namespace Logic.Domain.Level5Management.Resource
{
    internal class AnmcResourceReader : IAnmcResourceReader
    {
        private readonly IXpckReader _archiveReader;
        private readonly IImageVersionReader _imageVersionReader;
        private readonly IImageReaderFactory _imageReaderFactory;
        private readonly IImageDecoder _imageDecoder;
        private readonly IPointVectorReader _pointVectorReader;
        private readonly IPointIndexReader _pointIndexReader;
        private readonly IAnmcReader _anmcReader;

        public AnmcResourceReader(IXpckReader archiveReader,
            IImageVersionReader imageVersionReader, IImageReaderFactory imageReaderFactory, IImageDecoder imageDecoder,
            IPointVectorReader pointVectorReader, IPointIndexReader pointIndexReader, IAnmcReader anmcReader)
        {
            _archiveReader = archiveReader;
            _imageVersionReader = imageVersionReader;
            _imageReaderFactory = imageReaderFactory;
            _imageDecoder = imageDecoder;
            _pointVectorReader = pointVectorReader;
            _pointIndexReader = pointIndexReader;
            _anmcReader = anmcReader;
        }

        public IList<AnmcResourceData>? Read(Stream input)
        {
            ArchiveData archiveData = _archiveReader.Read(input);
            IList<NamedArchiveEntry> files = archiveData.Files;

            AnmcData? anmcData = ReadAnmc(files);
            if (anmcData == null)
                return null;

            IList<ImageData> images = DecodeImages(files);
            IList<PointVectorData> pointVectors = ReadPointVectors(files);
            IList<IList<short>> pointIndexes = ReadPointIndexes(files);

            IList<AnmcResourceData> resources = ParseResourceData(anmcData, images, pointVectors, pointIndexes);

            return resources;
        }

        private IList<ImageData> DecodeImages(IList<NamedArchiveEntry> files)
        {
            IList<NamedArchiveEntry> imageFiles = files.Where(x => Path.GetExtension(x.Name) == ".xi").ToArray();

            var result = new ImageData[imageFiles.Count];

            foreach (NamedArchiveEntry imageFile in imageFiles)
            {
                int imageVersion = _imageVersionReader.Peek(imageFile.Content);
                IImageReader imageReader = _imageReaderFactory.Create(imageVersion);

                RawImageData imageData = imageReader.Read(imageFile.Content);
                ImageData image = _imageDecoder.Decode(imageData);

                string name = Path.GetFileNameWithoutExtension(imageFile.Name);
                if (!int.TryParse(name, out int index))
                    continue;

                result[index] = image;
            }

            return result;
        }

        private IList<PointVectorData> ReadPointVectors(IList<NamedArchiveEntry> files)
        {
            var result = new List<PointVectorData>();

            IList<NamedArchiveEntry> vectorFiles = files.Where(x => Path.GetExtension(x.Name) == ".pvb").ToArray();

            foreach (NamedArchiveEntry vectorFile in vectorFiles)
            {
                IReadOnlyList<PointVectorData>? pointVectors = _pointVectorReader.Read(vectorFile.Content);
                if (pointVectors == null)
                    continue;

                result.AddRange(pointVectors);
            }

            return result;
        }

        private IList<IList<short>> ReadPointIndexes(IList<NamedArchiveEntry> files)
        {
            IList<NamedArchiveEntry> indexFiles = files.Where(x => Path.GetExtension(x.Name) == ".pbi").ToArray();

            var result = new IList<short>[indexFiles.Count];

            foreach (NamedArchiveEntry indexFile in indexFiles)
            {
                IList<short>? pointIndexes = _pointIndexReader.Read(indexFile.Content);
                if (pointIndexes == null)
                    continue;

                string name = Path.GetFileNameWithoutExtension(indexFile.Name);
                if (!int.TryParse(name, out int index))
                    continue;

                result[index] = pointIndexes;
            }

            return result;
        }

        private AnmcData? ReadAnmc(IList<NamedArchiveEntry> files)
        {
            NamedArchiveEntry? resFile = files.FirstOrDefault(x => x.Name == "RES.bin");
            if (resFile == null)
                return null;

            return _anmcReader.Read(resFile.Content);
        }

        private IList<AnmcResourceData> ParseResourceData(AnmcData anmcData, IList<ImageData> images,
            IList<PointVectorData> pointVectors, IList<IList<short>> pointIndexes)
        {
            var result = new List<AnmcResourceData>();

            for (var i = 0; i < Math.Min(pointIndexes.Count, anmcData.ResourceInfos.Count); i++)
            {
                string name = anmcData.ResourceInfos.FirstOrDefault(x => x.Value.PointVectorIndex == i).Key;
                AnmcIndexData indexData = anmcData.ResourceInfos[name];

                ImageData image = images[indexData.ImageIndex];

                var imageResources = new List<AnmcResourceImageData>();
                for (var pointIndex = 0; pointIndex < pointIndexes[i].Count; pointIndex += 6)
                {
                    PointVectorData topLeftPointVector = pointVectors[pointIndexes[i][pointIndex]];
                    PointVectorData bottomRightPointVector = pointVectors[pointIndexes[i][pointIndex + 3]];

                    var imageResource = new AnmcResourceImageData
                    {
                        Image = image,

                        Location = new PointF(topLeftPointVector.x, topLeftPointVector.y),
                        Size = new SizeF(bottomRightPointVector.x - topLeftPointVector.x, bottomRightPointVector.y - topLeftPointVector.y),

                        UvLocation = new PointF(topLeftPointVector.u, topLeftPointVector.v),
                        UvSize = new SizeF(bottomRightPointVector.u - topLeftPointVector.u, bottomRightPointVector.v - topLeftPointVector.v)
                    };

                    imageResources.Add(imageResource);
                }

                var resourceLocation = new PointF(imageResources.Min(r => r.Location.X), imageResources.Min(r => r.Location.Y));
                var resourceSize = new SizeF(imageResources.Max(x => x.Location.X + x.Size.Width) - resourceLocation.X, imageResources.Max(x => x.Location.Y + x.Size.Height) - resourceLocation.Y);

                result.Add(new AnmcResourceData
                {
                    Name = name,
                    Parts = imageResources,

                    Location = resourceLocation,
                    Size = resourceSize
                });
            }

            return result;
        }
    }
}
