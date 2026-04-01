using AutoMapper;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tazkarti.Controllers;
using Tazkarti.Models;

namespace Tazkarti.Tests.Controllers.Tests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<UserManager<AppUser>> _userManager;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;

        public UserControllerTests()
        {
            var userStore = new Mock<IUserStore<AppUser>>();
            _userManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _mapper = new Mock<IMapper>();
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _controller = new UserController(_userManager.Object, _mapper.Object, _roleManager.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithUserVMs()
        {
            // Arrange
            var users = new List<AppUser> { new AppUser { Id = "1", Email = "test@ex.com" } }.AsQueryable();
            var userVM = new UserVM { Id = "1", Email = "test@ex.com" };

            _userManager.Setup(u => u.Users).Returns(users);
            _userManager.Setup(u => u.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "Admin" });
            _mapper.Setup(m => m.Map<UserVM>(It.IsAny<AppUser>())).Returns(userVM);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/User/Index.cshtml", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IEnumerable<UserVM>>(viewResult.ViewData.Model);
            Assert.Contains(userVM, model);
            Assert.Equal("Admin", userVM.Role);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithUserVM()
        {
            // Arrange
            string userId = "1";
            var user = new AppUser { Id = userId };
            var userVM = new UserVM { Id = userId };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _mapper.Setup(m => m.Map<UserVM>(user)).Returns(userVM);

            // Act
            var result = await _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/User/Details.cshtml", viewResult.ViewName);
            Assert.Equal(userVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_GET_ReturnsViewResult_WithUserVM()
        {
            // Arrange
            string userId = "1";
            var user = new AppUser { Id = userId };
            var userVM = new UserVM { Id = userId };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _mapper.Setup(m => m.Map<UserVM>(user)).Returns(userVM);

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/User/Edit.cshtml", viewResult.ViewName);
            Assert.Equal(userVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_POST_ValidUpdate_RedirectsToIndex()
        {
            // Arrange
            string userId = "1";
            var user = new AppUser { Id = userId, Email = "old@ex.com" };
            var userVM = new UserVM { Id = userId, Email = "new@ex.com", Role = "User", UserName = "new", PhoneNumber = "123" };

            _userManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
            _userManager.Setup(u => u.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddToRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Edit(userId, userVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserController.Index), redirectToActionResult.ActionName);
            _userManager.Verify(u => u.UpdateAsync(user), Times.Once);
        }
    }
}
