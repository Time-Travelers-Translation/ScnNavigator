using System.Reflection;
using Logic.Domain.GoogleSheetsManagement.Contract;
using Logic.Domain.GoogleSheetsManagement.InternalContract;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class RequestContentBuilder : IRequestContentBuilder
    {
        private readonly IPropertyAspectProvider _aspectProvider;

        public RequestContentBuilder(IPropertyAspectProvider aspectProvider)
        {
            _aspectProvider = aspectProvider;
        }

        public UpdateCellsRequestData CreateUpdateCellsRequest<TUpdate>(IList<TUpdate> data, int sheetId, CellIdentifier start, CellIdentifier end)
        {
            var updateCellsRequest = new UpdateCellsRequestData
            {
                Range = new GridRangeData
                {
                    SheetId = sheetId,
                    StartColumnIndex = start.Column.Value - 1,  // zero-based, inclusive index
                    EndColumnIndex = end.Column.Value,  // zero-based, exclusive index
                    StartRowIndex = start.Row - 1,  // zero-based, inclusive index
                    EndRowIndex = end.Row   // zero-based, exclusive index
                },
                Rows = new RowData[data.Count]
            };

            var columns = new Dictionary<int, PropertyInfo>();
            foreach (PropertyInfo property in typeof(TUpdate).GetProperties())
            {
                int? column = _aspectProvider.GetPropertyColumn(property);
                if (column.HasValue)
                    columns[column.Value] = property;
            }

            for (var row = 0; row < data.Count; row++)
            {
                int columnCount = end.Column.Value - start.Column.Value + 1;
                updateCellsRequest.Rows[row] = new RowData
                {
                    Values = new CellData[columnCount]
                };

                for (var column = 0; column < columnCount; column++)
                {
                    var extendedValue = new ExtendedValueData();
                    updateCellsRequest.Rows[row].Values[column] = new CellData
                    {
                        UserEnteredValue = extendedValue
                    };

                    if (!columns.TryGetValue(column + start.Column.Value, out PropertyInfo? property))
                        continue;

                    CellType cellType = _aspectProvider.GetPropertyCellType(property);
                    object? cellValue = property.GetValue(data[row]);

                    SetCellValue(extendedValue, cellValue, cellType);
                }
            }

            return updateCellsRequest;
        }

        private static void SetCellValue(ExtendedValueData valueData, object? value, CellType type)
        {
            switch (type)
            {
                case CellType.String:
                    valueData.StringValue = (string)Convert.ChangeType(value!, typeof(string));
                    break;

                case CellType.Formula:
                    valueData.FormulaValue = (string)Convert.ChangeType(value!, typeof(string));
                    break;

                case CellType.Number:
                    if (value == null)
                        return;

                    valueData.NumberValue = (long)Convert.ChangeType(value, typeof(long));
                    break;

                case CellType.Boolean:
                    if (value == null)
                        return;

                    valueData.BoolValue = (bool)Convert.ChangeType(value, typeof(bool));
                    break;
            }
        }
    }
}
