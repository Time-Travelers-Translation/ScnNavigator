using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract
{
    internal interface IOAuth2Listener
    {
        void Start(string consentUrl);
        Task<HttpListenerRequest> AwaitRequest();
    }
}
