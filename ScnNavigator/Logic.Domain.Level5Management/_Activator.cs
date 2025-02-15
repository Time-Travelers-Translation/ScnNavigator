using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.Level5Management.Archive;
using Logic.Domain.Level5Management.Compression;
using Logic.Domain.Level5Management.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Cryptography;
using Logic.Domain.Level5Management.Contract.Font;
using Logic.Domain.Level5Management.Contract.Image;
using Logic.Domain.Level5Management.Contract.Resource;
using Logic.Domain.Level5Management.Contract.Scene;
using Logic.Domain.Level5Management.Contract.Script;
using Logic.Domain.Level5Management.Cryptography;
using Logic.Domain.Level5Management.Font;
using Logic.Domain.Level5Management.Image;
using Logic.Domain.Level5Management.Resource;
using Logic.Domain.Level5Management.Scene;
using Logic.Domain.Level5Management.Script;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.InternalContract.Compression;
using Logic.Domain.Level5Management.InternalContract.Font;
using Logic.Domain.Level5Management.InternalContract.Image;

namespace Logic.Domain.Level5Management
{
    public class Level5ManagementActivator : IComponentActivator
    {
        public void Activating()
        {
        }

        public void Activated()
        {
        }

        public void Deactivating()
        {
        }

        public void Deactivated()
        {
        }

        public void Register(ICoCoKernel kernel)
        {
            kernel.Register<IScnReader, ScnReader>(ActivationScope.Unique);
            kernel.Register<IFloReader, FloReader>(ActivationScope.Unique);
            kernel.Register<IStoryboardReader, StoryboardReader>(ActivationScope.Unique);

            // Compression
            kernel.Register<ICompressor, Compressor>(ActivationScope.Unique);
            kernel.Register<IDecompressor, Decompressor>(ActivationScope.Unique);

            // Checksums
            kernel.Register<IChecksumFactory, ChecksumFactory>(ActivationScope.Unique);

            // Configurations
            kernel.Register<IConfigurationReader<ConfigurationEntry>, ConfigurationReader>(ActivationScope.Unique);
            kernel.Register<IConfigurationWriter<ConfigurationEntry>, ConfigurationWriter>(ActivationScope.Unique);

            kernel.Register<IConfigurationReader<RawConfigurationEntry>, RawConfigurationReader>(ActivationScope.Unique);
            kernel.Register<IConfigurationWriter<RawConfigurationEntry>, RawConfigurationWriter>(ActivationScope.Unique);

            // Archives
            kernel.Register<IArchiveTypeReader, ArchiveTypeReader>(ActivationScope.Unique);
            kernel.Register<IArchiveReaderFactory, ArchiveReaderFactory>(ActivationScope.Unique);

            kernel.Register<IPckReader, PckReader>(ActivationScope.Unique);

            kernel.Register<IXpckReader, XpckReader>(ActivationScope.Unique);
            kernel.Register<IXpckWriter, XpckWriter>(ActivationScope.Unique);

            kernel.Register<IXfspReader, XfspReader>(ActivationScope.Unique);

            // Images
            kernel.Register<IImageVersionReader, ImageVersionReader>(ActivationScope.Unique);
            kernel.Register<IImageReaderFactory, ImageReaderFactory>(ActivationScope.Unique);
            kernel.Register<IImageWriterFactory, ImageWriterFactory>(ActivationScope.Unique);

            kernel.Register<IImg00Reader, Img00Reader>(ActivationScope.Unique);

            kernel.Register<IImg00Writer, Img00Writer>(ActivationScope.Unique);

            kernel.Register<IImageDecoder, ImageDecoder>(ActivationScope.Unique);
            kernel.Register<IImageEncoder, ImageEncoder>(ActivationScope.Unique);

            kernel.Register<IImageParser, ImageParser>(ActivationScope.Unique);
            kernel.Register<IImageComposer, ImageComposer>(ActivationScope.Unique);

            // Fonts
            kernel.Register<IFontTypeReader, FontVersionReader>(ActivationScope.Unique);
            kernel.Register<IFontReaderFactory, FontReaderFactory>(ActivationScope.Unique);
            kernel.Register<IFontWriterFactory, FontWriterFactory>(ActivationScope.Unique);

            kernel.Register<IFnt00Reader, Fnt00Reader>(ActivationScope.Unique);
            kernel.Register<IFnt01Reader, Fnt01Reader>(ActivationScope.Unique);

            kernel.Register<IFnt00Writer, Fnt00Writer>(ActivationScope.Unique);
            kernel.Register<IFnt01Writer, Fnt01Writer>(ActivationScope.Unique);

            kernel.Register<IFontParser, FontParser>(ActivationScope.Unique);
            kernel.Register<IFontComposer, FontComposer>(ActivationScope.Unique);

            kernel.Register<IGlyphProviderFactory, GlyphProviderFactory>(ActivationScope.Unique);
            kernel.Register<ICtrGlyphProvider, CtrGlyphProvider>();
            kernel.Register<IPspGlyphProvider, PspGlyphProvider>();

            // Layouts
            kernel.Register<IAnmcResourceReader, AnmcResourceReader>(ActivationScope.Unique);
            kernel.Register<IPointVectorReader, PointVectorReader>(ActivationScope.Unique);
            kernel.Register<IPointIndexReader, PointIndexReader>(ActivationScope.Unique);
            kernel.Register<IAnmcReader, AnmcReader>(ActivationScope.Unique);
            kernel.Register<IAnmcResourceParser, AnmcResourceParser>(ActivationScope.Unique);

            kernel.RegisterConfiguration<Level5ManagementConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
