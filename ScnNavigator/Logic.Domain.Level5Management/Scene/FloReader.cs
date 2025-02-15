using System.IO;
using System.Text;
using Logic.Domain.Kuriimu2.KomponentAdapter.Contract;
using Logic.Domain.Level5Management.Contract.DataClasses.Scene;
using Logic.Domain.Level5Management.Contract.Scene;

namespace Logic.Domain.Level5Management.Scene
{
    internal class FloReader : IFloReader
    {
        private readonly IBinaryFactory _binaryFactory;

        public FloReader(IBinaryFactory binaryFactory)
        {
            _binaryFactory = binaryFactory;
        }

        public FloData Read(Stream input)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjis = Encoding.GetEncoding("Shift-JIS");

            using IBinaryReaderX br = _binaryFactory.CreateReader(input, sjis, true);

            br.BaseStream.Position = 0xC;
            int table3Count = br.ReadInt32();
            int table3Offset = br.ReadInt32();

            var titles = new FloTitleData[table3Count];
            for (var i = 0; i < table3Count; i++)
            {
                br.BaseStream.Position = table3Offset + i * 0x200;
                string sceneTitle = br.ReadNullTerminatedString();

                br.BaseStream.Position = table3Offset + i * 0x200 + 0x5C;
                string sceneName = br.ReadNullTerminatedString();

                titles[i] = new FloTitleData
                {
                    SceneName = sceneName,
                    Text = sceneTitle
                };
            }

            return new FloData
            {
                Titles = titles
            };
        }
    }
}
