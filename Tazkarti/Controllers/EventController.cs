using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;
using Tazkarti.Utitly;

namespace Tazkarti.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: EventController
        public async Task<ActionResult> Index()
            => View(_mapper.Map<IEnumerable<EventVM>>(await _unitOfWork.EventRepository.GetAllAsync()));

        // GET: EventController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(_mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }

        public ActionResult Create() => View(new EventVM());

        // POST: EventController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventVM eventVM)
        {
            try
            {
                if (eventVM.Image is not null)
                    eventVM.ImageName = DocumentSetting.UploadFile(eventVM.Image, "Images");
                Event e = _mapper.Map<Event>(eventVM);
                e.NoOfAvailableTickets = e.NoOfTickets;
                await _unitOfWork.EventRepository.AddAsync(e);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(nameof(Index), eventVM);
            }
        }

        // GET: EventController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View(_mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }

        // POST: EventController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EventVM eventVM)
        {
            if (id != eventVM.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    if (eventVM.Image is not null)
                    {
                        if (!string.IsNullOrEmpty(eventVM.ImageName))
                            DocumentSetting.DeleteFile(eventVM.ImageName, "Images");
                        eventVM.ImageName = DocumentSetting.UploadFile(eventVM.Image, "Images");
                    }

                    await _unitOfWork.EventRepository.UpdateAsync(_mapper.Map<Event>(eventVM));
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(eventVM);
        }

        // GET: EventController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(_mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }

        // POST: EventController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, EventVM eventVM)
        {
            if (id != eventVM.Id)
                return BadRequest();
            try
            {
                _unitOfWork.EventRepository.Delete(_mapper.Map<Event>(eventVM));
                if (eventVM.ImageName is not null)
                    DocumentSetting.DeleteFile(eventVM.ImageName, "Images");
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(eventVM);
        }
    }
}
