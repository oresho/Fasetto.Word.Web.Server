using System.Linq;
using System.Threading.Tasks;
using Fasetto.Word.Web.Server.Data;
using Fasetto.Word.Web.Server.IoC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fasetto.Word.Web.Server.Controllers
{
    /// <summary>
    /// manages the standard web server pages
    /// </summary>
    public class HomeController : Controller
    {
        #region Protected Members
        //manager for handling users (C,D,SEARCHING,ROLES OF USERS)
        protected UserManager<ApplicationUser> userManager;

        //manager for handling signing in and out
        protected SignInManager<ApplicationUser> signInManager;

        //scoped application context
        protected ApplicationDbContext context;
        #endregion


        //Injecting the context 
        public HomeController(ApplicationDbContext _context
            ,UserManager<ApplicationUser> _userManager
            ,SignInManager<ApplicationUser> _signInManager)
        {
            context = _context;
            userManager = _userManager;
            signInManager = _signInManager;
        }

        public IActionResult Index()
        {
        
            context.Database.EnsureCreated();
            #region Add setting to db
            if (!context.Settings.Any())
            {
                context.Settings.Add(new SettingsDataModel
                {
                    Name = "BackgroundColor",
                    Value = "Red"
                });
            }
            //checking to show that new setting is only in local and not on the db
            var settingsLocally = context.Settings.Local.Count();
            var settingsDatabase = context.Settings.Count();

            var firstLocal = context.Settings.Local.FirstOrDefault();
            var firstDatabase = context.Settings.FirstOrDefault();
            //commit setting to db
            context.SaveChanges();
            //recheck to show commit was successful
            settingsLocally = context.Settings.Local.Count();
            settingsDatabase = context.Settings.Count();

            firstLocal = context.Settings.Local.FirstOrDefault();
            firstDatabase = context.Settings.FirstOrDefault();
            #endregion

            return View();
        }

        [Route("create")]
        public async Task<IActionResult> CreateUserAsync()
        {
            var result = await userManager.CreateAsync(new ApplicationUser
            {
                UserName = "angelsix",
                Email = "contact@angelsix.com"
            },password: "password");

            if (result.Succeeded)
            {
                return Content("User was Created", "text/html");
            }

            return Content("User creation Failed","text/html");
        }

        //private area for logged in users
        //changed from demo cookie auth scheme(Identity.Application) to Jwt auth scheme(Bearer) so cookies wont access private without token
        [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]//JwtBearerDefaults.AuthenticationScheme)]
        [Route("private")]
        public IActionResult Private()
        {
            return Content($"This area is private. Welcome {HttpContext.User.Identity.Name}", "text/html");
        }

        [Route("logout")]
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);//when the arguments for this signout are empty the signout doesnt delete cookies

            return Content("done", "text/html");
        }

        //Login page
        [Route("login")]
        public async Task<IActionResult> LoginAync(string returnUrl)
        {
            //signout any previous sessions
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            //return Content("No login for you");

            var result = await signInManager.PasswordSignInAsync("angelsix", "password", true,false);

            if (result.Succeeded)
            {
                return string.IsNullOrEmpty(returnUrl) ? RedirectToAction(nameof(Index)) : RedirectToAction(returnUrl);
            }

            return Content("Failed to Login", "text/html");
        }


    }
}
