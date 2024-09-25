using AutoMapper;
using BLL.Repositories;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly EventRepository _eventRepo;
        private readonly TicketRepository _ticketRepo;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(EventRepository eventRepository, ILogger<HomeController> logger, IMapper mapper, UserManager<AppUser> userManager, TicketRepository ticketRepo)
        {
            _eventRepo = eventRepository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _ticketRepo = ticketRepo;
        }


        public IActionResult Index()
        {
            ViewBag.User = User.IsInRole("Admin");
            return View(_mapper.Map<IEnumerable<EventVM>>(_eventRepo.GetAll()));
        }

        public IActionResult Booking(int id)
        {
            ViewBag.User = User.IsInRole("Admin");
            return View(_mapper.Map<EventVM>(_eventRepo.GetbyId(id)));
        }
        [HttpPost]
        public IActionResult Booking(int id, int numOfTicket)
        {
            ViewBag.User = User.IsInRole("Admin");
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _ticketRepo.Book(id, userId, numOfTicket);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
