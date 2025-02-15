using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract
{
    public interface IDataRangeParser
    {
        IReadOnlyCollection<TType> Parse<TType>(List<List<string>?>? list, CellIdentifier start, CellIdentifier end);
    }
}
