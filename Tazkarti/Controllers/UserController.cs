using AutoMapper;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<AppUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        //_mapper.Map<List<UserVM>>().ForEach(u => u.Role = _userManager.GetRolesAsync(_mapper.Map<AppUser>(u)).Result.First().ToString()
        // GET: UserController
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var users = _userManager.Users.ToList();
            List<UserVM> userVMs = new List<UserVM>();
            foreach (AppUser user in users)
            {
                UserVM userVM = _mapper.Map<UserVM>(user);
                var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
                userVM.Role = role;
                userVMs.Add(userVM);
            }
            return View(userVMs);
        }

        // GET: UserController/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(string id)
        {
            UserVM mappUsaer = _mapper.Map<UserVM>(await _userManager.FindByIdAsync(id));
            return View(mappUsaer);
        }

        // GET: UserController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            UserVM mappUser = _mapper.Map<UserVM>(_userManager.FindByIdAsync(id).Result);
            return View(mappUser);
        }

        // POST: UserController/Edit/5
        [Authorize(Roles = "Admin")]
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
                var UserRoles = _userManager.GetRolesAsync(user).Result[0];
                if (UserRoles != userVM.Role)
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles);
                    await _userManager.AddToRoleAsync(user, userVM.Role);
                }
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [Authorize(Roles = "Admin")]
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
