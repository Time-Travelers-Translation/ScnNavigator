using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement.Contract.Aspects
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnIdentifier Column { get; }

        public ColumnAttribute(string column)
        {
            Column = ColumnIdentifier.Parse(column);
        }
    }
}
