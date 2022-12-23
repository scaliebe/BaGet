using System;
using System.Threading;
using System.Threading.Tasks;
using BaGet.Core;
using BaGet.Protocol.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;

namespace BaGet.Web
{
    /// <summary>
    /// The Package Metadata resource, used to fetch packages' information.
    /// See: https://docs.microsoft.com/en-us/nuget/api/registration-base-url-resource
    /// </summary>
    public class PackageMetadataController : Controller
    {
        private readonly IPackageMetadataService _metadata;
        private readonly IAuthenticationService _authentication;

        public PackageMetadataController(IPackageMetadataService metadata, IAuthenticationService authentication)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        // GET v3/registration/{id}.json
        [HttpGet]
        public async Task<ActionResult<BaGetRegistrationIndexResponse>> RegistrationIndexAsync([FromRoute] string apikey, string id, CancellationToken cancellationToken)
        {
            if (!await _authentication.AuthenticateAsync(apikey, cancellationToken))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            var index = await _metadata.GetRegistrationIndexOrNullAsync(id, cancellationToken);
            if (index == null)
            {
                return NotFound();
            }

            return index;
        }

        // GET v3/registration/{id}/{version}.json
        [HttpGet]
        public async Task<ActionResult<RegistrationLeafResponse>> RegistrationLeafAsync([FromRoute] string apikey, string id, string version, CancellationToken cancellationToken)
        {
            if (!await _authentication.AuthenticateAsync(apikey, cancellationToken))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            if (!NuGetVersion.TryParse(version, out var nugetVersion))
            {
                return NotFound();
            }

            var leaf = await _metadata.GetRegistrationLeafOrNullAsync(id, nugetVersion, cancellationToken);
            if (leaf == null)
            {
                return NotFound();
            }

            return leaf;
        }
    }
}
