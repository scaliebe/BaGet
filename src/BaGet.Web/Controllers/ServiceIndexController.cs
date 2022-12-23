using System;
using System.Threading;
using System.Threading.Tasks;
using BaGet.Core;
using BaGet.Protocol.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaGet.Web
{
    /// <summary>
    /// The NuGet Service Index. This aids NuGet client to discover this server's services.
    /// </summary>
    public class ServiceIndexController : Controller
    {
        private readonly IServiceIndexService _serviceIndex;
        private readonly IAuthenticationService _authentication;

        public ServiceIndexController(IServiceIndexService serviceIndex, IAuthenticationService authentication)
        {
            _serviceIndex = serviceIndex ?? throw new ArgumentNullException(nameof(serviceIndex));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        // GET v3/index
        [HttpGet]
        public async Task<ServiceIndexResponse> GetAsync([FromRoute]string apikey, CancellationToken cancellationToken)
        {
            if (!await _authentication.AuthenticateAsync(apikey, cancellationToken))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            return await _serviceIndex.GetAsync(cancellationToken);
        }
    }
}
