using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract
{
    public interface IOAuth2TokenProvider
    {
        Task<string?> GetAccessToken();
    }
}
