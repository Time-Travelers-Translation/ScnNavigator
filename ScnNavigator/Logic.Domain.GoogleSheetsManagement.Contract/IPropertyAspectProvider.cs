using System.Reflection;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement.Contract
{
    public interface IPropertyAspectProvider
    {
        int? GetPropertyColumn(PropertyInfo property);
        CellType GetPropertyCellType(PropertyInfo property);
    }
}
