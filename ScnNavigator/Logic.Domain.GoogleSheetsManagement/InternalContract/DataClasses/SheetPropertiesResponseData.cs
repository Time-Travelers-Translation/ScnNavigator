using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal class SheetPropertiesResponseData
    {
        public int SheetId { get; set; }
        public string Title { get; set; }
        public int Index { get; set; }
        public string SheetType { get; set; }
        public GridPropertiesResponseData GridProperties { get; set; }
        public ColorResponseData TabColor { get; set; }
    }
}
