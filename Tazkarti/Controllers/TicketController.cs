using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    [Route("Dashboard/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class TicketController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: TicketController
        public async Task<ActionResult> Index(string? SearchValue = null)
        {
            var tickets = await _unitOfWork.TicketRepository.Search(SearchValue);
            var result = _mapper.Map<IEnumerable<TicketVM>>(tickets);
            return View("~/Views/Dashboard/Ticket/Index.cshtml", result);
        }

        // GET: TicketController/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            return View("~/Views/Dashboard/Ticket/Detalis.cshtml",
                _mapper.Map<TicketVM>(await _unitOfWork.TicketRepository.GetbyIdAsync(id)));
        }

        // GET: TicketController/Edit/5
        public async Task<ActionResult> Edit(Guid id) => View("~/Views/Dashboard/Ticket/Edit.cshtml", _mapper.Map<TicketVM>(await _unitOfWork.TicketRepository.GetbyIdAsync(id)));

        // POST: TicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, TicketVM ticketVM)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("~/Views/Dashboard/Ticket/Edit.cshtml");
            }
        }

        // GET: TicketController/Delete/5
        public async Task<ActionResult> Delete(Guid id) => View("~/Views/Dashboard/Ticket/Delete.cshtml", _mapper.Map<TicketVM>(await _unitOfWork.TicketRepository.GetbyIdAsync(id)));

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, TicketVM ticketVM)
        {
            try
            {
                var tic = await _unitOfWork.TicketRepository.GetbyIdAsync(id);
                _unitOfWork.TicketRepository.Delete(tic);
                _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("~/Views/Dashboard/Ticket/Delete.cshtml", ticketVM);
            }
        }
    }
}
