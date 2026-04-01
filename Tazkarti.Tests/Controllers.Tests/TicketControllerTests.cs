using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tazkarti.Controllers;
using Tazkarti.Models;

namespace Tazkarti.Tests.Controllers.Tests
{
    public class TicketControllerTests
    {
        private readonly TicketController _controller;
        private readonly Mock<ITicketRepository> _ticketRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;

        public TicketControllerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _ticketRepository = new Mock<ITicketRepository>();
            _controller = new TicketController(_unitOfWork.Object, _mapper.Object);

            _unitOfWork.Setup(u => u.TicketRepository).Returns(_ticketRepository.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfTickets()
        {
            // Arrange
            var tickets = new List<Ticket> { new Ticket { Id = Guid.NewGuid(), Name = "Ticket 1" } };
            var ticketVMs = new List<TicketVM> { new TicketVM { Id = tickets[0].Id, Name = "Ticket 1" } };

            _ticketRepository.Setup(r => r.Search(null)).ReturnsAsync(tickets);
            _mapper.Setup(m => m.Map<IEnumerable<TicketVM>>(tickets)).Returns(ticketVMs);

            // Act
            var result = await _controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Ticket/Index.cshtml", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IEnumerable<TicketVM>>(viewResult.ViewData.Model);
            Assert.Equal(ticketVMs, model);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithTicket()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId, Name = "Ticket 1" };
            var ticketVM = new TicketVM { Id = ticketId, Name = "Ticket 1" };

            _ticketRepository.Setup(r => r.GetbyIdAsync(ticketId)).ReturnsAsync(ticket);
            _mapper.Setup(m => m.Map<TicketVM>(ticket)).Returns(ticketVM);

            // Act
            var result = await _controller.Details(ticketId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Ticket/Detalis.cshtml", viewResult.ViewName);
            var model = Assert.IsType<TicketVM>(viewResult.ViewData.Model);
            Assert.Equal(ticketVM, model);
        }

        [Fact]
        public async Task Edit_GET_ReturnsViewResult_WithTicketVM()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId, Name = "Ticket 1" };
            var ticketVM = new TicketVM { Id = ticketId, Name = "Ticket 1" };

            _ticketRepository.Setup(r => r.GetbyIdAsync(ticketId)).ReturnsAsync(ticket);
            _mapper.Setup(m => m.Map<TicketVM>(ticket)).Returns(ticketVM);

            // Act
            var result = await _controller.Edit(ticketId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Ticket/Edit.cshtml", viewResult.ViewName);
            Assert.Equal(ticketVM, viewResult.ViewData.Model);
        }

        [Fact]
        public void Edit_POST_RedirectsToIndex()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ticketVM = new TicketVM { Id = ticketId };

            // Act
            var result = _controller.Edit(ticketId, ticketVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TicketController.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_GET_ReturnsViewResult_WithTicketVM()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId, Name = "Ticket 1" };
            var ticketVM = new TicketVM { Id = ticketId, Name = "Ticket 1" };

            _ticketRepository.Setup(r => r.GetbyIdAsync(ticketId)).ReturnsAsync(ticket);
            _mapper.Setup(m => m.Map<TicketVM>(ticket)).Returns(ticketVM);

            // Act
            var result = await _controller.Delete(ticketId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Dashboard/Ticket/Delete.cshtml", viewResult.ViewName);
            Assert.Equal(ticketVM, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Delete_POST_ValidId_RedirectsToIndex()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var ticket = new Ticket { Id = ticketId };
            var ticketVM = new TicketVM { Id = ticketId };

            _ticketRepository.Setup(r => r.GetbyIdAsync(ticketId)).ReturnsAsync(ticket);

            // Act
            var result = await _controller.Delete(ticketId, ticketVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(TicketController.Index), redirectToActionResult.ActionName);
            _ticketRepository.Verify(r => r.Delete(ticket), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
