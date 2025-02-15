using System.Text;

namespace Logic.Domain.GoogleSheetsManagement.Contract.DataClasses
{
    public struct CellIdentifier
    {
        public ColumnIdentifier Column { get; set; }
        public int Row { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Column);
            sb.Append(Row);

            return sb.ToString();
        }

        public static CellIdentifier Parse(string identifier)
        {
            int rowStartIndex = -1;
            for (var i = 0; i < identifier.Length; i++)
            {
                int rowDigit = identifier[i] - '0';
                if (rowDigit is < 0 or >= 10)
                    continue;

                rowStartIndex = i;
                break;
            }

            if (rowStartIndex < 0)
                throw new InvalidOperationException("No row identifier specified after the column identifier.");
            if (rowStartIndex == 0)
                throw new InvalidOperationException("No column identifier specified.");

            ColumnIdentifier column = ColumnIdentifier.Parse(identifier[..rowStartIndex]);

            if (!int.TryParse(identifier[rowStartIndex..], out int row))
                throw new InvalidOperationException($"Row identifier is not a number (Position={rowStartIndex}).");

            return new CellIdentifier
            {
                Column = column,
                Row = row
            };
        }
    }
}
