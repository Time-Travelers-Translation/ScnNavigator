using Logic.Business.TimeTravelersManagement.Contract.Paths;
using Logic.Domain.Level5Management.Contract.Enums;

namespace Logic.Business.TimeTravelersManagement.Paths
{
    internal class GamePathProvider : IGamePathProvider
    {
        private readonly PlatformType _platformType;

        public GamePathProvider(TimeTravelersManagementConfiguration config)
        {
            _platformType = config.Platform;
        }

        public string GetPlatformPath()
        {
            switch (_platformType)
            {
                case PlatformType.Ctr:
                    return "ctr";

                case PlatformType.Psp:
                    return "psp";

                default:
                    throw new InvalidOperationException($"Unsupported platform {_platformType}.");
            }
        }

        public string GetFlowFilePath()
        {
            switch (_platformType)
            {
                case PlatformType.Ctr:
                    return Path.Combine("common", "menu", "tt1.flo");

                case PlatformType.Psp:
                    return Path.Combine("psp", "script", "tt1.flo");

                default:
                    throw new InvalidOperationException($"Unsupported platform {_platformType}.");
            }
        }

        public string GetEventPckFilePath(string identifier)
            => Path.Combine(GetPlatformPath(), "txt", "event", "pck", $"{identifier}.pck");

        public string GetFontFilePath(string name)
            => Path.Combine(GetPlatformPath(), "fnt", $"{name}.xf");

        public string GetStoryBoardFolderPath()
            => Path.Combine(GetPlatformPath(), "script", "event");

        public string GetStoryBoardFilePath(string name)
            => Path.Combine(GetPlatformPath(), "script", "event", $"{name}.stb");

        public string GetTipTitleFilePath()
            => Path.Combine(GetPlatformPath(), "txt", "tip", "Tip_List_ja.cfg.bin");

        public string GetTipTextFilePath(int tipIndex)
            => Path.Combine(GetPlatformPath(), "txt", "tip", $"TIP{tipIndex:000}_ja.cfg.bin");
    }
}
