using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.Contract
{
    public interface ICodeFlowProvider
    {
        ICodeFlowManager GetCodeFlow();
    }
}
