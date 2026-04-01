using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Resource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Text.Json;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResource> Localizer;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, IMapper mapper, UserManager<AppUser> userManager, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            Localizer = localizer;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.ErrorMessage = Localizer["User not found"];
                _logger.LogError("User ID not found in claims.");
                return RedirectToAction(nameof(Index));
            }
            var TicketsVM = _mapper.Map<IEnumerable<UserTicketVM>>(await _unitOfWork.TicketRepository.GetAllForUserAsync(userId));
            return View(TicketsVM);
        }

        public async Task<IActionResult> Index()
            => View(_mapper.Map<IEnumerable<EventVM>>(await _unitOfWork.EventRepository.GetAllAsync()));


        [Authorize]

        public async Task<IActionResult> EventDetails(Guid id)
        => View(_mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));


        [Authorize]
        public async Task<IActionResult> TicketsDetails(Guid id, int numOfTicket)
        {
            var eventItem = await _unitOfWork.EventRepository.GetbyIdAsync(id);
            if (eventItem == null) return NotFound();

            var model = new TicketDetailsViewModel
            {
                EventId = eventItem.Id,
                NumberOfTickets = numOfTicket,
                Tickets = Enumerable.Range(0, numOfTicket)
                                    .Select(i => new TicketInfo())
                                    .ToList()
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Booking(string key)
        {
            var serializedData = HttpContext.Session.GetString(key);
            if (string.IsNullOrEmpty(serializedData))
            {
                _logger.LogError("No ticket details found in session for key {key}", key);
                return BadRequest();
            }
            var ticketDetailsViewModel = JsonSerializer.Deserialize<TicketDetailsViewModel>(serializedData);
            if (ticketDetailsViewModel == null)
            {
                _logger.LogError("Failed to deserialize ticket details for key {key}", key);
                return BadRequest();
            }

            var Event = await _unitOfWork.EventRepository.GetbyIdAsync(ticketDetailsViewModel.EventId);
            if (Event == null) return BadRequest();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.ErrorMessage = Localizer["User not found"];
                _logger.LogError("User ID not found in claims.");
                return View(ticketDetailsViewModel);
            }
            foreach (var info in ticketDetailsViewModel.Tickets)
            {
                Ticket ticket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    EventID = ticketDetailsViewModel.EventId,
                    Name = info.Name,
                    Email = info.Email,
                    PhoneNumber = info.PhoneNumber,
                    UserId = userId
                };
                await _unitOfWork.TicketRepository.AddAsync(ticket);
                Event.NoOfTickets -= 1;
            }
            try
            {
                _unitOfWork.EventRepository.Update(Event);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError(ex, "Error occurred while booking tickets for event {eventId}", Event.Id);
                return View(nameof(TicketsDetails), ticketDetailsViewModel.NumberOfTickets);
            }
            return RedirectToAction(nameof(UserTickets));
        }
    }
}
