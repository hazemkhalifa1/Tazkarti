using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
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
        public ActionResult Index(int? SearchValue = null)
        {
            var result = _mapper.Map<IEnumerable<TicketVM>>(_unitOfWork.TicketRepository.Search(SearchValue));
            return View(result);
        }

        // GET: TicketController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(_mapper.Map<TicketVM>(await _unitOfWork.TicketRepository.GetbyIdAsync(id)));
        }

        // GET: TicketController/Edit/5
        public async Task<ActionResult> Edit(int id) => View(_mapper.Map<TicketVM>(await _unitOfWork.TicketRepository.GetbyIdAsync(id)));

        // POST: TicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TicketVM ticketVM)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TicketController/Delete/5
        public async Task<ActionResult> Delete(int id) => View(_mapper.Map<TicketVM>(await _unitOfWork.TicketRepository.GetbyIdAsync(id)));

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TicketVM ticketVM)
        {
            try
            {
                _unitOfWork.TicketRepository.Delete(id);
                _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(nameof(Index), ticketVM);
            }
        }
    }
}
