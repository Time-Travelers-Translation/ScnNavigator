using System.Net;
using Logic.Domain.GoogleSheetsManagement.InternalContract;

namespace Logic.Domain.GoogleSheetsManagement
{
    internal class OAuth2Listener : IOAuth2Listener
    {
        private readonly HttpListener _listener;

        private bool _started;

        public OAuth2Listener()
        {
            _started = false;
            _listener = new HttpListener();
        }

        public void Start(string consentUrl)
        {
            if (_started)
                return;

            _listener.Prefixes.Add(consentUrl);
            _listener.Start();

            _started = true;
        }

        /// <summary>
        /// Await the user input request.
        /// </summary>
        /// <returns>The user input request.</returns>
        public async Task<HttpListenerRequest> AwaitRequest()
        {
            HttpListenerContext context = await _listener.GetContextAsync();
            return context.Request;
        }
    }
}
