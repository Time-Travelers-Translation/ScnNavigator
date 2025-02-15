using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class CellData
    {
        public ExtendedValueData UserEnteredValue { get; set; }
    }
}
