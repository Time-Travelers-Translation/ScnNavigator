using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class ExtendedValueData
    {
        public string StringValue { get; set; }
        public long? NumberValue { get; set; }
        public bool? BoolValue { get; set; }
        public string FormulaValue { get; set; }
        public ErrorValueData ErrorValue { get; set; }
    }
}
