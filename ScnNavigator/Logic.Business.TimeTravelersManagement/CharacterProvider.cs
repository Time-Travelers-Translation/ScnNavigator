using System.Text.Json;
using Logic.Business.TimeTravelersManagement.Contract;

namespace Logic.Business.TimeTravelersManagement
{
    internal class CharacterProvider : ICharacterProvider
    {
        private readonly TimeTravelersManagementConfiguration _config;

        private Dictionary<char, char>? _characterMap;

        public CharacterProvider(TimeTravelersManagementConfiguration config)
        {
            _config = config;
        }

        public IList<char> GetAll()
        {
            if (_characterMap == null)
                Initialize();

            return _characterMap!.Keys.ToArray();
        }

        public bool TryGet(char originalCharacter, out char mappedCharacter)
        {
            if (_characterMap == null)
                Initialize();

            return _characterMap!.TryGetValue(originalCharacter, out mappedCharacter);
        }

        private void Initialize()
        {
            if (!File.Exists(_config.PatchMapPath))
            {
                _characterMap = new();
                return;
            }

            string json = File.ReadAllText(_config.PatchMapPath);
            _characterMap = JsonSerializer.Deserialize<Dictionary<char, char>>(json);
        }
    }
}
