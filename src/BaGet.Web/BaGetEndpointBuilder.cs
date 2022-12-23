using BaGet.Core;
using BaGet.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using System;
using System.Web;

namespace BaGet
{
    public class BaGetEndpointBuilder
    {
        private BaGetOptions _options;

        public void MapEndpoints(IEndpointRouteBuilder endpoints, BaGetOptions options)
        {
            _options = options ?? throw new ArgumentException(nameof(options));

            endpoints.MapRazorPages();

            MapServiceIndexRoutes(endpoints);
            MapPackagePublishRoutes(endpoints);
            MapSymbolRoutes(endpoints);
            MapSearchRoutes(endpoints);
            MapPackageMetadataRoutes(endpoints);
            MapPackageContentRoutes(endpoints);
        }

        public void MapServiceIndexRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: Routes.IndexRouteName,
                pattern: "{apikey}/v3/index.json",
                defaults: new { controller = "ServiceIndex", action = "Get", apikey = HttpUtility.UrlEncode(_options.ApiKey) });
        }

        public void MapPackagePublishRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: Routes.UploadPackageRouteName,
                pattern: "{apikey}/api/v2/package",
                defaults: new { controller = "PackagePublish", action = "Upload", apikey = HttpUtility.UrlEncode(_options.ApiKey) },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("PUT") });

            endpoints.MapControllerRoute(
                name: Routes.DeleteRouteName,
                pattern: "{apikey}/api/v2/package/{id}/{version}",
                defaults: new { controller = "PackagePublish", action = "Delete", apikey = HttpUtility.UrlEncode(_options.ApiKey) },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("DELETE") });

            endpoints.MapControllerRoute(
                name: Routes.RelistRouteName,
                pattern: "{apikey}/api/v2/package/{id}/{version}",
                defaults: new { controller = "PackagePublish", action = "Relist", apikey = HttpUtility.UrlEncode(_options.ApiKey) },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("POST") });
        }

        public void MapSymbolRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: Routes.UploadSymbolRouteName,
                pattern: "{apikey}/api/v2/symbol",
                defaults: new { controller = "Symbol", action = "Upload", apikey = HttpUtility.UrlEncode(_options.ApiKey) },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("PUT") });

            endpoints.MapControllerRoute(
                name: Routes.SymbolDownloadRouteName,
                pattern: "{apikey}/api/download/symbols/{file}/{key}/{file2}",
                defaults: new { controller = "Symbol", action = "Get", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.PrefixedSymbolDownloadRouteName,
                pattern: "{apikey}/api/download/symbols/{prefix}/{file}/{key}/{file2}",
                defaults: new { controller = "Symbol", action = "Get", apikey = HttpUtility.UrlEncode(_options.ApiKey) });
        }

        public void MapSearchRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: Routes.SearchRouteName,
                pattern: "{apikey}/v3/search",
                defaults: new { controller = "Search", action = "Search", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.AutocompleteRouteName,
                pattern: "{apikey}/v3/autocomplete",
                defaults: new { controller = "Search", action = "Autocomplete", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            // This is an unofficial API to find packages that depend on a given package.
            endpoints.MapControllerRoute(
                name: Routes.DependentsRouteName,
                pattern: "{apikey}/v3/dependents",
                defaults: new { controller = "Search", action = "Dependents", apikey = HttpUtility.UrlEncode(_options.ApiKey) });
        }

        public void MapPackageMetadataRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
               name: Routes.RegistrationIndexRouteName,
               pattern: "{apikey}/v3/registration/{id}/index.json",
               defaults: new { controller = "PackageMetadata", action = "RegistrationIndex", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.RegistrationLeafRouteName,
                pattern: "{apikey}/v3/registration/{id}/{version}.json",
                defaults: new { controller = "PackageMetadata", action = "RegistrationLeaf", apikey = HttpUtility.UrlEncode(_options.ApiKey) });
        }

        public void MapPackageContentRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: Routes.PackageVersionsRouteName,
                pattern: "{apikey}/v3/package/{id}/index.json",
                defaults: new { controller = "PackageContent", action = "GetPackageVersions", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.PackageDownloadRouteName,
                pattern: "{apikey}/v3/package/{id}/{version}/{idVersion}.nupkg",
                defaults: new { controller = "PackageContent", action = "DownloadPackage", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.PackageDownloadManifestRouteName,
                pattern: "{apikey}/v3/package/{id}/{version}/{id2}.nuspec",
                defaults: new { controller = "PackageContent", action = "DownloadNuspec", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.PackageDownloadReadmeRouteName,
                pattern: "{apikey}/v3/package/{id}/{version}/readme",
                defaults: new { controller = "PackageContent", action = "DownloadReadme", apikey = HttpUtility.UrlEncode(_options.ApiKey) });

            endpoints.MapControllerRoute(
                name: Routes.PackageDownloadIconRouteName,
                pattern: "{apikey}/v3/package/{id}/{version}/icon",
                defaults: new { controller = "PackageContent", action = "DownloadIcon", apikey = HttpUtility.UrlEncode(_options.ApiKey)});
        }
    }
}
