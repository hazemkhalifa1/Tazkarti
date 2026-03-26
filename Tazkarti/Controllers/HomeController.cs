using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.User = User.IsInRole("Admin");
            var EventsVM = _mapper.Map<IEnumerable<EventVM>>(await _unitOfWork.EventRepository.GetAllAsync());
            return View(EventsVM);
        }

        [Authorize]

        public async Task<IActionResult> EventDetails(Guid id)
        {
            ViewBag.User = User.IsInRole("Admin");
            return View(_mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }

        [Authorize]
        public async Task<IActionResult> TicketsDetails(Guid id, int numOfTicket)
        {
            ViewBag.User = User.IsInRole("Admin");
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
        [HttpPost]
        public async Task<IActionResult> Booking(Guid id, TicketDetailsViewModel ticketDetailsViewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
                return View("TicketsDetails", ticketDetailsViewModel);
            }

            ViewBag.User = User.IsInRole("Admin");

            var Event = await _unitOfWork.EventRepository.GetbyIdAsync(ticketDetailsViewModel.EventId);
            if (Event == null) return BadRequest();

            foreach (var info in ticketDetailsViewModel.Tickets)
            {
                Ticket ticket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    EventID = ticketDetailsViewModel.EventId,
                    Name = info.Name,
                    Email = info.Email,
                    PhoneNumber = info.PhoneNumber
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
                return View(nameof(TicketsDetails), ticketDetailsViewModel);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
