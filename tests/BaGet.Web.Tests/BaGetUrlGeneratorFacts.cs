using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BaGet.Web.Tests
{
    public class BaGetUrlGeneratorFacts
    {
        [Theory]
        [InlineData(null, "", "https://packages.example.test:8443")]
        [InlineData("", "", "https://packages.example.test:8443")]
        [InlineData("test-key", "", "https://packages.example.test:8443/test-key")]
        [InlineData(null, "/", "https://packages.example.test:8443")]
        [InlineData("test-key", "/", "https://packages.example.test:8443/test-key")]
        [InlineData(null, "/feed", "https://packages.example.test:8443/feed")]
        [InlineData("", "/feed", "https://packages.example.test:8443/feed")]
        [InlineData("test-key", "/feed", "https://packages.example.test:8443/feed/test-key")]
        [InlineData(null, "/feed/", "https://packages.example.test:8443/feed")]
        [InlineData("", "/feed/", "https://packages.example.test:8443/feed")]
        [InlineData("test-key", "/feed/", "https://packages.example.test:8443/feed/test-key")]
        [InlineData("test-key", "/feeds/private", "https://packages.example.test:8443/feeds/private/test-key")]
        [InlineData("test key", "/private feed", "https://packages.example.test:8443/private%20feed/test%20key")]
        public void ResourceUrlsHonorPathBaseAndApiKey(string apiKey, string pathBase, string expectedBaseUrl)
        {
            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("packages.example.test", 8443);
            context.Request.PathBase = new PathString(pathBase);

            var configurationValues = new Dictionary<string, string>();
            if (apiKey != null)
            {
                configurationValues["ApiKey"] = apiKey;
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            using (var services = new ServiceCollection().AddLogging().AddRouting().BuildServiceProvider())
            {
                var target = new BaGetUrlGenerator(
                    new HttpContextAccessor { HttpContext = context },
                    services.GetRequiredService<LinkGenerator>(),
                    configuration);

                Assert.Equal(expectedBaseUrl + "/v3/package", target.GetPackageContentResourceUrl());
                Assert.Equal(expectedBaseUrl + "/v3/registration", target.GetPackageMetadataResourceUrl());
            }
        }
    }
}
