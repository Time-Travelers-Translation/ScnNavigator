using Logic.Domain.Level5Management.Contract.Enums.ConfigBinary;

namespace Logic.Business.TimeTravelersManagement.Contract.DataClasses
{
    public class EventTextConfiguration
    {
        public DateTime? LastUpdateDateTime { get; set; }
        public string? LastUpdateUser { get; set; }
        public string? LastUpdateMachine { get; set; }
        public EventText[] Texts { get; set; }

        public StringEncoding StringEncoding { get; set; }
    }
}
