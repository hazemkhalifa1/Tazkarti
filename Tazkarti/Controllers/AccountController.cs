using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LogUserVM userVM)
        {
            if (!ModelState.IsValid)
                return View(userVM);
            var user = await _userManager.FindByEmailAsync(userVM.Email);
            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, userVM.Pass))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, userVM.Pass, userVM.RememberMe, false);
                    if (result.Succeeded) return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Incorrect E-mail Or Password");
            return View(userVM);
        }

        public ActionResult Registartion()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Registartion(UserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                await _roleManager.CreateAsync(role);
            }
            var user = new AppUser
            {
                Email = model.Email,
                Agree = model.Agree,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var resul = await _userManager.AddToRoleAsync(user, "User");
                return Redirect(nameof(Login));
            }
            else
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            return View(model);
        }
    }
}
