using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Options;

namespace BaGet.Core
{
    public class ApiKeyAuthenticationService : IAuthenticationService
    {
        private readonly string _apiKey;

        public ApiKeyAuthenticationService(IOptionsSnapshot<BaGetOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _apiKey = string.IsNullOrEmpty(options.Value.ApiKey) ? null : options.Value.ApiKey;

            if(_apiKey != null)
                _apiKey = HttpUtility.UrlDecode(_apiKey);
        }

        public Task<bool> AuthenticateAsync(string apiKey, CancellationToken cancellationToken)
            => Task.FromResult(Authenticate(apiKey));

        private bool Authenticate(string apiKey)
        {
            // No authentication is necessary if there is no required API key.
            if (_apiKey == null) return true;

            return _apiKey == apiKey;
        }
    }
}
