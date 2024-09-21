using AutoMapper;
using BLL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class TicketController : Controller
    {
        private readonly TicketRepository _ticRepo;
        private readonly IMapper _mapper;

        public TicketController(TicketRepository ticRepo, IMapper mapper)
        {
            _ticRepo = ticRepo;
            _mapper = mapper;
        }

        // GET: TicketController
        public ActionResult Index()
        {
            return View(_mapper.Map<IEnumerable<TicketVM>>(_ticRepo.GetAll()));
        }

        // GET: TicketController/Details/5
        public ActionResult Details(int id)
        {
            return View(_mapper.Map<TicketVM>(_ticRepo.GetbyId(id)));
        }

        // GET: TicketController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_mapper.Map<TicketVM>(_ticRepo.GetbyId(id)));
        }

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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
