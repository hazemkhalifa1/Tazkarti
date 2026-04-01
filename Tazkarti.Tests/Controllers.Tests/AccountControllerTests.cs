using DAL.Entities;
using DAL.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Tazkarti.Controllers;
using Tazkarti.Models;

namespace Tazkarti.Tests.Controllers.Tests
{
    public class AccountControllerTests
    {
        private readonly AccountController _controller;
        private readonly Mock<UserManager<AppUser>> _userManager;
        private readonly Mock<SignInManager<AppUser>> _signInManager;
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizer;

        public AccountControllerTests()
        {
            var userStore = new Mock<IUserStore<AppUser>>();
            _userManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _localizer = new Mock<IStringLocalizer<SharedResource>>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
            _signInManager = new Mock<SignInManager<AppUser>>(_userManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _controller = new AccountController(_userManager.Object, _signInManager.Object, _roleManager.Object, _localizer.Object);
        }

        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var userVM = new LogUserVM();

            // Act
            var result = await _controller.Login(userVM);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(userVM, viewResult.Model);
        }

        [Fact]
        public async Task Login_POST_ValidCredentials_RedirectsToHomeIndex()
        {
            // Arrange
            var userVM = new LogUserVM { Email = "test@ex.com", Pass = "Password123!" };
            var user = new AppUser { Email = userVM.Email };

            _userManager.Setup(u => u.FindByEmailAsync(userVM.Email)).ReturnsAsync(user);
            _userManager.Setup(u => u.CheckPasswordAsync(user, userVM.Pass)).ReturnsAsync(true);
            _signInManager.Setup(s => s.PasswordSignInAsync(user, userVM.Pass, userVM.RememberMe, false))
                          .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login(userVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void Registartion_ReturnsViewResult()
        {
            // Act
            var result = _controller.Registartion();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Registartion_POST_ValidModel_CreatesUser_AndRedirectsToLogin()
        {
            // Arrange
            var model = new UserVM { Email = "new@ex.com", UserName = "new", PhoneNumber = "123", Password = "Password!" };
            var user = new AppUser();

            _roleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userManager.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<AppUser>(), "User")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Registartion(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(nameof(AccountController.Login), redirectResult.Url);
        }
    }
}
