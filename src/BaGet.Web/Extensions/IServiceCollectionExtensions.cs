using System;
using System.IO;
using BaGet.Core;
using BaGet.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BaGet
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBaGetWebApplication(
            this IServiceCollection services,
            Action<BaGetApplication> configureAction)
        {
            services
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddApplicationPart(typeof(PackageContentController).Assembly)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddRazorPages();

            IConfiguration iconfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true).Build();

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(System.IO.Path.Combine(iconfig.GetSection("Storage")["Path"], "webkeys")))
                .SetApplicationName("BaGetWebKeyStore")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(91));


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = new PathString("/Login");
                    o.ExpireTimeSpan = TimeSpan.FromDays(90);
                });

            services.AddSingleton(iconfig);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpContextAccessor();
            services.AddTransient<IUrlGenerator, BaGetUrlGenerator>();

            services.AddBaGetApplication(configureAction);

            return services;
        }
    }
}
