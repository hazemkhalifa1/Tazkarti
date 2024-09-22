using AutoMapper;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // GET: UserController
        public ActionResult Index()
        => View(_mapper.Map<List<UserVM>>(_userManager.Users.ToList()));


        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LogUserVM userVM)
        {
            if (!ModelState.IsValid)
                return View(userVM);
            var user = _userManager.FindByEmailAsync(userVM.Email).Result;
            if (user != null)
            {
                if (_userManager.CheckPasswordAsync(user, userVM.Pass).Result)
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
        public ActionResult Registartion(UserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = new AppUser
            {
                Email = model.Email,
                Agree = model.Agree,
                UserName = model.UserName,
                Role = "User"
            };
            var result = _userManager.CreateAsync(user, model.Password).Result;
            if (result.Succeeded)
                return Redirect(nameof(Login));
            else
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            return View(model);
        }

        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            UserVM mappUsaer = _mapper.Map<UserVM>(await _userManager.FindByIdAsync(id));
            return View(mappUsaer);
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(string id)
        {
            UserVM mappUser = _mapper.Map<UserVM>(_userManager.FindByIdAsync(id).Result);
            return View(mappUser);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UserVM userVM)
        {
            try
            {
                AppUser user = await _userManager.FindByIdAsync(id);
                user.Email = userVM.Email;
                user.PhoneNumber = userVM.PhoneNumber;
                user.UserName = userVM.UserName;
                user.Role = userVM.Role;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
