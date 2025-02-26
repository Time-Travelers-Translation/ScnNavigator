﻿using Logic.Business.TimeTravelersManagement.Contract.Enums;
using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Business.TimeTravelersManagement.Contract.Texts;
using Logic.Domain.Level5Management.Contract.ConfigBinary;
using Logic.Domain.Level5Management.Contract.DataClasses.ConfigBinary;

namespace Logic.Business.TimeTravelersManagement.Texts
{
    internal class PatchedTipTextManager : TipTextManager, IPatchedTipTextManager
    {
        private readonly IBasePathProvider _basePathProvider;

        public PatchedTipTextManager(IBasePathProvider basePathProvider, IGamePathProvider pathProvider,
            IEventTextParser eventTextParser, IConfigurationReader<RawConfigurationEntry> configReader)
            : base(pathProvider, eventTextParser, configReader)
        {
            _basePathProvider = basePathProvider;
        }

        protected override string GetFullPath(string relativePath)
            => _basePathProvider.GetFullPath(relativePath, AssetPreference.Patch);
    }
}
