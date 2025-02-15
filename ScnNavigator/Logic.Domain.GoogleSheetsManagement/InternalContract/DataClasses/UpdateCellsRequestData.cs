using CrossCutting.Core.Contract.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class UpdateCellsRequestData
    {
        public RowData[] Rows { get; set; }
        public string Fields { get; set; } = "userEnteredValue";
        public GridRangeData Range { get; set; }
    }
}
