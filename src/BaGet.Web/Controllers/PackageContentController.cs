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
    /// The Package Content resource, used to download content from packages.
    /// See: https://docs.microsoft.com/en-us/nuget/api/package-base-address-resource
    /// </summary>
    public class PackageContentController : Controller
    {
        private readonly IPackageContentService _content;
        private readonly IAuthenticationService _authentication;

        public PackageContentController(IPackageContentService content, IAuthenticationService authentication)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        public async Task<ActionResult<PackageVersionsResponse>> GetPackageVersionsAsync([FromRoute] string apikey, string id, CancellationToken cancellationToken)
        {
            if (!await _authentication.AuthenticateAsync(apikey, cancellationToken))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            var versions = await _content.GetPackageVersionsOrNullAsync(id, cancellationToken);
            if (versions == null)
            {
                return NotFound();
            }

            return versions;
        }

        public async Task<IActionResult> DownloadPackageAsync([FromRoute] string apikey, string id, string version, CancellationToken cancellationToken)
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

            var packageStream = await _content.GetPackageContentStreamOrNullAsync(id, nugetVersion, cancellationToken);
            if (packageStream == null)
            {
                return NotFound();
            }

            return File(packageStream, "application/octet-stream");
        }

        public async Task<IActionResult> DownloadNuspecAsync([FromRoute] string apikey, string id, string version, CancellationToken cancellationToken)
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

            var nuspecStream = await _content.GetPackageManifestStreamOrNullAsync(id, nugetVersion, cancellationToken);
            if (nuspecStream == null)
            {
                return NotFound();
            }

            return File(nuspecStream, "text/xml");
        }

        public async Task<IActionResult> DownloadReadmeAsync([FromRoute] string apikey, string id, string version, CancellationToken cancellationToken)
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

            var readmeStream = await _content.GetPackageReadmeStreamOrNullAsync(id, nugetVersion, cancellationToken);
            if (readmeStream == null)
            {
                return NotFound();
            }

            return File(readmeStream, "text/markdown");
        }

        public async Task<IActionResult> DownloadIconAsync([FromRoute] string apikey, string id, string version, CancellationToken cancellationToken)
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

            var iconStream = await _content.GetPackageIconStreamOrNullAsync(id, nugetVersion, cancellationToken);
            if (iconStream == null)
            {
                return NotFound();
            }

            return File(iconStream, "image/xyz");
        }
    }
}
