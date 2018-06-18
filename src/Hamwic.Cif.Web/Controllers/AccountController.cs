using Hamwic.Cif.Core.Entities;
using Hamwic.Cif.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hamwic.Cif.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // GET
        /// <summary>
        /// Captures the return url (if there is one) to populate on the LoginModel
        /// </summary>
        /// <param name="returnUrl">string: the returnUrl to redirect the user to when logged in</param>
        /// <returns>View</returns>
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            return Content("Logged in");
        }

        // GET
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //GET
        [Authorize]
        public IActionResult Logout()
        {
            //clear the authentication token and re-direct to the login page

            return RedirectToAction("Login");
        }

        //GET
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return Content("Forgot Password");
        }
    }
}