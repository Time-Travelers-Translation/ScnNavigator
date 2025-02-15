using System.Reflection;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.Contract.Aspects;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class PropertyAspectProvider : IPropertyAspectProvider
    {
        public int? GetPropertyColumn(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ColumnAttribute>();
            return attribute?.Column.Value;
        }

        public CellType GetPropertyCellType(PropertyInfo property)
        {
            var cellTypeAttribute = property.GetCustomAttribute<CellTypeAttribute>();
            if (cellTypeAttribute != null)
                return cellTypeAttribute.CellType;

            switch (Type.GetTypeCode(property.PropertyType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return CellType.Number;

                case TypeCode.Boolean:
                    return CellType.Boolean;

                default:
                    return CellType.String;
            }
        }
    }
}
