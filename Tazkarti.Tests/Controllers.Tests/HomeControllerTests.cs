using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Tazkarti.Controllers;
using Tazkarti.Models;

namespace Tazkarti.Tests.Controllers.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<UserManager<AppUser>> _userManager;
        private readonly Mock<ILogger<HomeController>> _logger;
        private readonly Mock<IStringLocalizer<SharedResource>> Localizer;

        public HomeControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<HomeController>>();
            Localizer = new Mock<IStringLocalizer<SharedResource>>();

            var userStore = new Mock<IUserStore<AppUser>>();
            _userManager = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            _controller = new HomeController(_unitOfWork.Object, _logger.Object, _mapper.Object, _userManager.Object, Localizer.Object);

            // Mock the User property
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithEventVMList()
        {
            // Arrange
            var events = new List<Event> { new Event { Id = Guid.NewGuid(), Name = "Event 1" } };
            var eventVMs = new List<EventVM> { new EventVM { Id = events[0].Id, Name = "Event 1" } };

            _unitOfWork.Setup(u => u.EventRepository.GetAllAsync()).ReturnsAsync(events);
            _mapper.Setup(m => m.Map<IEnumerable<EventVM>>(events)).Returns(eventVMs);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<EventVM>>(viewResult.ViewData.Model);
            Assert.Equal(eventVMs, model);
        }

        [Fact]
        public async Task EventDetails_ReturnsViewResult_WithEventVM()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var ev = new Event { Id = eventId, Name = "Event 1" };
            var eventVM = new EventVM { Id = eventId, Name = "Event 1" };

            _unitOfWork.Setup(u => u.EventRepository.GetbyIdAsync(eventId)).ReturnsAsync(ev);
            _mapper.Setup(m => m.Map<EventVM>(ev)).Returns(eventVM);

            // Act
            var result = await _controller.EventDetails(eventId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EventVM>(viewResult.ViewData.Model);
            Assert.Equal(eventVM, model);
        }

        [Fact]
        public async Task TicketsDetails_ValidEvent_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var ev = new Event { Id = eventId, Name = "Event 1" };
            _unitOfWork.Setup(u => u.EventRepository.GetbyIdAsync(eventId)).ReturnsAsync(ev);

            // Act
            var result = await _controller.TicketsDetails(eventId, 2);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TicketDetailsViewModel>(viewResult.ViewData.Model);
            Assert.Equal(eventId, model.EventId);
            Assert.Equal(2, model.NumberOfTickets);
            Assert.Equal(2, model.Tickets.Count);
        }

        [Fact]
        public async Task Booking_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var ev = new Event { Id = eventId, Name = "Event 1", NoOfTickets = 10 };
            var model = new TicketDetailsViewModel
            {
                EventId = eventId,
                Tickets = new List<TicketInfo> { new TicketInfo { Name = "User 1", Email = "user1@ex.com", PhoneNumber = "123" } }
            };

            _unitOfWork.Setup(u => u.EventRepository.GetbyIdAsync(eventId)).ReturnsAsync(ev);
            _unitOfWork.Setup(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Booking(eventId, model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Index), redirectToActionResult.ActionName);
            _unitOfWork.Verify(u => u.TicketRepository.AddAsync(It.IsAny<Ticket>()), Times.Once);
            _unitOfWork.Verify(u => u.EventRepository.Update(ev), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(9, ev.NoOfTickets);
        }

        [Fact]
        public async Task UserTickets_ReturnsViewResult_WithUserTickets()
        {
            // Arrange
            var userId = "test-user-id";
            var tickets = new List<Ticket> { new Ticket { Id = Guid.NewGuid(), UserId = userId } };
            var ticketVMs = new List<TicketVM> { new TicketVM { Id = tickets[0].Id } };

            // Setup ClaimsPrincipal to return userId
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _unitOfWork.Setup(u => u.TicketRepository.GetAllForUserAsync(userId)).ReturnsAsync(tickets);
            _mapper.Setup(m => m.Map<IEnumerable<TicketVM>>(tickets)).Returns(ticketVMs);

            // Act
            var result = await _controller.UserTickets();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TicketVM>>(viewResult.ViewData.Model);
            Assert.Equal(ticketVMs, model);
        }

        [Fact]
        public async Task UserTickets_NoUserId_RedirectsToIndex()
        {
            // Arrange
            // Setup ClaimsPrincipal without NameIdentifier
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "mock"));
            Localizer.Setup(l => l["User not found"]).Returns(new LocalizedString("User not found", "User not found. Please log in again."));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.UserTickets();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Index), redirectToActionResult.ActionName);
            Assert.Equal("User not found. Please log in again.", _controller.ViewBag.ErrorMessage);
        }
    }
}
