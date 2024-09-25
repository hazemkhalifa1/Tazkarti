using AutoMapper;
using BLL.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, IMapper mapper, UserManager<AppUser> userManager, TicketRepository ticketRepo)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.User = User.IsInRole("Admin");
            return View(_mapper.Map<IEnumerable<EventVM>>(await _unitOfWork.EventRepository.GetAllAsync()));
        }

        public async Task<IActionResult> Booking(int id)
        {
            ViewBag.User = User.IsInRole("Admin");
            return View(_mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }
        [HttpPost]
        public async Task<IActionResult> Booking(int id, int numOfTicket)
        {
            ViewBag.User = User.IsInRole("Admin");
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _unitOfWork.TicketRepository.BookAsync(id, userId, numOfTicket);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
