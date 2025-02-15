using CrossCutting.Core.Contract.DependencyInjection;
using Logic.Business.RenderingManagement.Contract;
using Logic.Business.RenderingManagement.Contract.Parsers;
using Logic.Business.RenderingManagement.InternalContract.Parsers;

namespace Logic.Business.RenderingManagement
{
    internal class CharacterParserProvider : ICharacterParserProvider
    {
        private readonly ICoCoKernel _kernel;

        public CharacterParserProvider(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public ICharacterParser GetSubtitleParser()
        {
            return _kernel.Get<ISubtitleCharacterParser>();
        }

        public ICharacterParser GetNarrationParser()
        {
            return _kernel.Get<INarrationCharacterParser>();
        }

        public ICharacterParser GetTipParser()
        {
            return _kernel.Get<ITipCharacterParser>();
        }

        public ICharacterParser GetDefaultParser()
        {
            return _kernel.Get<ICharacterParser>();
        }
    }
}
