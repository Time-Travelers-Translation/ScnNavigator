using Logic.Business.TimeTravelersManagement.Contract;
using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Domain.Level5Management.Contract.Archive;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Cryptography;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;
using Logic.Domain.Level5Management.Contract.Scene;

namespace Logic.Business.TimeTravelersManagement.Texts
{
    internal class PatchedStoryTextManager : StoryTextManager, IPatchedStoryTextManager
    {
        private readonly IBasePathProvider _basePathProvider;

        public PatchedStoryTextManager(IPckReader pckReader, IConfigurationReader<RawConfigurationEntry> configReader,
            IEventTextParser eventParser, IChecksumFactory checksumFactory, IStoryboardManager storyboardManager,
            IFloReader flowReader, IBasePathProvider basePathProvider, IGamePathProvider pathProvider) 
            : base(pckReader, configReader, eventParser, checksumFactory, storyboardManager, flowReader, pathProvider)
        {
            _basePathProvider = basePathProvider;
        }

        protected override string GetFullPath(string resourcePath)
            => _basePathProvider.GetFullPath(resourcePath, AssetPreference.Patch);
    }
}
