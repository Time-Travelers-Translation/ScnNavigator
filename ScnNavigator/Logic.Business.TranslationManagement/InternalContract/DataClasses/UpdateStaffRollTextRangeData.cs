using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;

namespace Logic.Business.TranslationManagement.InternalContract.DataClasses
{
    internal class UpdateStaffRollTextRangeData
    {
        [Column("D")]
        public string? Translation { get; set; }
    }
}
