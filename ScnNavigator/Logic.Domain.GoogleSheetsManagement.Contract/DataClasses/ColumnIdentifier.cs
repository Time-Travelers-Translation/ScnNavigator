using System.Text;

namespace Logic.Domain.GoogleSheetsManagement.Contract.DataClasses
{
    public struct ColumnIdentifier
    {
        public int Value { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            int totalColumn = Value;
            while (totalColumn > 0)
            {
                int column = totalColumn % 26;

                sb.Append((char)(column - 1 + 'A'));

                totalColumn -= column;
                totalColumn /= 26;
            }

            return sb.ToString();
        }

        public static ColumnIdentifier Parse(string identifier)
        {
            var totalColumn = 0;
            for (var i = 0; i < identifier.Length; i++)
            {
                int column = identifier[i] - 'A' + 1;
                if (column is > 0 and <= 26)
                {
                    totalColumn = totalColumn * 26 + column;
                    continue;
                }

                throw new InvalidOperationException($"Unsupported symbol found in column identifier (Position={i}).");
            }

            return new ColumnIdentifier
            {
                Value = totalColumn
            };
        }
    }
}
