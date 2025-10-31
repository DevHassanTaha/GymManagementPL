using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService, 
            SignInManager<ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel input)
        {
            if (!ModelState.IsValid)
                return View(input);
            var user = _accountService.ValidateUser(input);

            if (user is null)
            {

                ModelState.AddModelError("InvalidLogin", "Your Account Is Not Allowed");
                return View(input);
            }
            var result = _signInManager.PasswordSignInAsync
                            (user, input.Password, input.RememberMe, false).Result;
            if (result.IsNotAllowed)
                ModelState.AddModelError("InvalidLogin", "Your Account Is Not Allowed");
            if (result.IsLockedOut)
                ModelState.AddModelError("InvalidLogin", "Your Account Is Locked");
            
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
           
            return View(input);

        }
        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().GetAwaiter().GetResult();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
