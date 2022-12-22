using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Newtonsoft.Json;

namespace BaGet.Web.Pages
{
    public class LoginModel : PageModel
    {
        public readonly IHttpContextAccessor _currentContextAccessor;
        private readonly IConfiguration _configuration;

        public LoginModel(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _currentContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGet()
        {
            if (_currentContextAccessor.HttpContext.User.Claims.Any(x => x.Type == "BaGet"))
            {
                if(_currentContextAccessor.HttpContext.User.Claims.Single(x => x.Type == "BaGet").Value == _configuration["ApiKey"])
                    return RedirectToPage("Index");
            }

            await removeClaims();

            return Page();
        }

        public async Task<IActionResult> OnGetLogoff()
        {
            await removeClaims();
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostAsync(string userSecret)
        {
            if (userSecret == _configuration["ApiKey"])
            {
                await createIdentity(userSecret);
                return RedirectToPage("Index");
            }

            return Page();
        }

        private async Task removeClaims()
        {
            if (_currentContextAccessor.HttpContext.User
                 .Claims.Any(x => x.Type == "BaGet"))
            {

                var claim = _currentContextAccessor.HttpContext.User
                    .Claims.Where(x => x.Type == "BaGet");

                var climeCount = claim.Count();
                for (int i = 0; i < climeCount; i++)
                    (_currentContextAccessor.HttpContext.User.Identity as ClaimsIdentity)
                        .RemoveClaim(claim.ElementAt(i));

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        private async Task createIdentity(string secret)
        {
            if (_currentContextAccessor.HttpContext.User
                 .Claims.Any(x => x.Type == "BaGet"))
            {
                var claim = _currentContextAccessor.HttpContext.User
                    .Claims.Where(x => x.Type == "BaGet");

                var climeCount = claim.Count();
                for (int i = 0; i < climeCount; i++)
                    (_currentContextAccessor.HttpContext.User.Identity as ClaimsIdentity)
                        .RemoveClaim(claim.ElementAt(i));

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("BaGet", secret));

            await _currentContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true
                    });
        }
    }
}
