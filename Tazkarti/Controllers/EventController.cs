using AutoMapper;
using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkarti.Models;
using Tazkarti.Utitly;

namespace Tazkarti.Controllers
{
    [Route("Dashboard/[controller]/[action]")]
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
            => View("~/Views/Dashboard/Event/Index.cshtml",
                _mapper.Map<IEnumerable<EventVM>>(await _unitOfWork.EventRepository.GetAllAsync()));

        // GET: EventController/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            return View("~/Views/Dashboard/Event/Details.cshtml",
                _mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }

        public ActionResult Create() => View("~/Views/Dashboard/Event/Create.cshtml", new EventVM());

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
                e.Id = Guid.NewGuid();
                await _unitOfWork.EventRepository.AddAsync(e);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("~/Views/Dashboard/Event/Create.cshtml", eventVM);
            }
        }

        // GET: EventController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var eventVM = _mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id));
            return View("~/Views/Dashboard/Event/Edit.cshtml", eventVM);
        }

        // POST: EventController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] EventVM eventVM)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var evant = await _unitOfWork.EventRepository.GetbyIdAsync(eventVM.Id);

                    if (evant is null) return BadRequest();

                    if (eventVM.Image is not null)
                    {
                        if (!string.IsNullOrEmpty(eventVM.ImageName))
                            DocumentSetting.DeleteFile(eventVM.ImageName, "Images");
                        eventVM.ImageName = DocumentSetting.UploadFile(eventVM.Image, "Images");
                    }
                    else
                        eventVM.ImageName = evant.ImageName;
                    _mapper.Map(eventVM, evant);
                    _unitOfWork.EventRepository.Update(evant);
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View("~/Views/Dashboard/Event/Edit.cshtml", eventVM);
        }

        // GET: EventController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            return View("~/Views/Dashboard/Event/Delete.cshtml", _mapper.Map<EventVM>(await _unitOfWork.EventRepository.GetbyIdAsync(id)));
        }

        // POST: EventController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, EventVM eventVM)
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
            return View("~/Views/Dashboard/Event/Delete.cshtml", eventVM);
        }
    }
}
