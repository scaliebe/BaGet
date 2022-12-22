using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BaGet.Web.Pages
{
    public class UploadModel : PageModel
    {
        public readonly IHttpContextAccessor _currentContextAccessor;
        private readonly IConfiguration _configuration;

        public UploadModel(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _currentContextAccessor = httpContextAccessor;
        }

        public IActionResult OnGet()
        {
            if (!_currentContextAccessor.HttpContext.User.Claims
                .Any(x => x.Type == "BaGet") ||
                _currentContextAccessor.HttpContext.User.Claims.
                Single(x => x.Type == "BaGet").Value != _configuration["ApiKey"])
            {
                return RedirectToPage("Login");
            }

            return Page();
        }
    }
}
