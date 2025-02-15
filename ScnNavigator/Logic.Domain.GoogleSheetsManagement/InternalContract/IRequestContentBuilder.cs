using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;
using Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract
{
    internal interface IRequestContentBuilder
    {
        UpdateCellsRequestData CreateUpdateCellsRequest<TUpdate>(IList<TUpdate> data, int sheetId, CellIdentifier start, CellIdentifier end);
    }
}
