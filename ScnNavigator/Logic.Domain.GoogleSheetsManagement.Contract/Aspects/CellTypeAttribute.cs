using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CellTypeAttribute:Attribute
    {
        public CellType CellType { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CellTypeAttribute"/>.
        /// </summary>
        /// <param name="cellType">The type of the cell.</param>
        public CellTypeAttribute(CellType cellType)
        {
            CellType = cellType;
        }
    }
}
