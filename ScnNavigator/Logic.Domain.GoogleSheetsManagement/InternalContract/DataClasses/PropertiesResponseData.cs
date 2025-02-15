using System.Text.Json.Serialization;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class PropertiesResponseData
    {
        public string Title { get; set; }
        public string Locale { get; set; }
        [JsonPropertyName("autorecalc")]
        public string AutoRecalculation { get; set; }
        public string TimeZone { get; set; }
        public DefaultFormatResponseData DefaultFormat { get; set; }
    }
}
