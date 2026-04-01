using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tazkarti.Controllers;
using Tazkarti.Models;

namespace Tazkarti.Tests.Controllers.Tests
{
    public class EventControllerTests
    {
        private readonly EventController _controller;
        private readonly Mock<IGenaricRepository<Event>> _eventRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        public EventControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _eventRepository = new Mock<IGenaricRepository<Event>>();
            _controller = new EventController(_unitOfWork.Object, _mapper.Object);

            //setup
            _unitOfWork.Setup(u => u.EventRepository).Returns(_eventRepository.Object);

        }

        #region Index Tests
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfEvents()
        {
            // Arrange
            var events = new List<Event> { new Event { Id = Guid.NewGuid(), Name = "Event 1" } };
            var eventVMs = new List<EventVM> { new EventVM { Id = events[0].Id, Name = "Event 1" } };

            _eventRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);
            _mapper.Setup(m => m.Map<IEnumerable<EventVM>>(events)).Returns(eventVMs);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Event/Index.cshtml", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IEnumerable<EventVM>>(viewResult.ViewData.Model);
            Assert.Equal(eventVMs, model);
        }
        #endregion

        #region Details Tests
        [Fact]
        public async Task Details_ReturnsViewResult_WithEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var ev = new Event { Id = eventId, Name = "Event 1" };
            var eventVM = new EventVM { Id = eventId, Name = "Event 1" };

            _eventRepository.Setup(r => r.GetbyIdAsync(eventId)).ReturnsAsync(ev);
            _mapper.Setup(m => m.Map<EventVM>(ev)).Returns(eventVM);

            // Act
            var result = await _controller.Details(eventId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Event/Details.cshtml", viewResult.ViewName);
            var model = Assert.IsType<EventVM>(viewResult.ViewData.Model);
            Assert.Equal(eventVM, model);
        }
        #endregion

        #region Create Tests
        [Fact]
        public void Create_GET_ReturnsViewResult_WithNewEventVM()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Event/Create.cshtml", viewResult.ViewName);
            Assert.IsType<EventVM>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Create_POST_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var eventVM = new EventVM { Name = "New Event" };
            var ev = new Event { Name = "New Event" };

            _mapper.Setup(m => m.Map<Event>(eventVM)).Returns(ev);

            // Act
            var result = await _controller.Create(eventVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(EventController.Index), redirectToActionResult.ActionName);
            _eventRepository.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_POST_Exception_ReturnsViewResultWithModel()
        {
            // Arrange
            var eventVM = new EventVM { Name = "New Event" };
            _mapper.Setup(m => m.Map<Event>(It.IsAny<EventVM>())).Throws(new Exception("Error"));

            // Act
            var result = await _controller.Create(eventVM);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Event/Create.cshtml", viewResult.ViewName);
            Assert.Equal(eventVM, viewResult.ViewData.Model);
            Assert.False(_controller.ModelState.IsValid);
        }
        #endregion

        #region Edit Tests
        [Fact]
        public async Task Edit_GET_ReturnsViewResult_WithEventVM()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var ev = new Event { Id = eventId, Name = "Event 1" };
            var eventVM = new EventVM { Id = eventId, Name = "Event 1" };

            _eventRepository.Setup(r => r.GetbyIdAsync(eventId)).ReturnsAsync(ev);
            _mapper.Setup(m => m.Map<EventVM>(ev)).Returns(eventVM);

            // Act
            var result = await _controller.Edit(eventId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Event/Edit.cshtml", viewResult.ViewName);
            Assert.Equal(eventVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Edit_POST_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventVM = new EventVM { Id = eventId, Name = "Updated Event" };
            var ev = new Event { Id = eventId, Name = "Original Event" };

            _eventRepository.Setup(r => r.GetbyIdAsync(eventId)).ReturnsAsync(ev);
            _mapper.Setup(m => m.Map(eventVM, ev)).Callback(() => ev.Name = eventVM.Name);

            // Act
            var result = await _controller.Edit(eventVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(EventController.Index), redirectToActionResult.ActionName);
            _eventRepository.Verify(r => r.Update(ev), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_NotFound_ReturnsBadRequest()
        {
            // Arrange
            var eventVM = new EventVM { Id = Guid.NewGuid() };
            _eventRepository.Setup(r => r.GetbyIdAsync(eventVM.Id)).ReturnsAsync((Event)null);

            // Act
            var result = await _controller.Edit(eventVM);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Delete_GET_ReturnsViewResult_WithEventVM()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var ev = new Event { Id = eventId, Name = "Event 1" };
            var eventVM = new EventVM { Id = eventId, Name = "Event 1" };

            _eventRepository.Setup(r => r.GetbyIdAsync(eventId)).ReturnsAsync(ev);
            _mapper.Setup(m => m.Map<EventVM>(ev)).Returns(eventVM);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Event/Delete.cshtml", viewResult.ViewName);
            Assert.Equal(eventVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_POST_ValidId_RedirectsToIndex()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventVM = new EventVM { Id = eventId };
            var ev = new Event { Id = eventId };

            _mapper.Setup(m => m.Map<Event>(eventVM)).Returns(ev);

            // Act
            var result = await _controller.Delete(eventId, eventVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(EventController.Index), redirectToActionResult.ActionName);
            _eventRepository.Verify(r => r.Delete(ev), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_POST_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var result = await _controller.Delete(Guid.NewGuid(), new EventVM { Id = Guid.NewGuid() });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        #endregion
    }
}
