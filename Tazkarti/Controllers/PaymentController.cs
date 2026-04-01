using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;
using Tazkarti.Models;

namespace Tazkarti.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IUnitOfWork unitOfWork, ILogger<PaymentController> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ActionResult> CreateCheckoutSession(TicketDetailsViewModel ticketDetailsViewModel)
        {

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
                return View("Home/TicketsDetails", ticketDetailsViewModel);
            }

            var _event = await _unitOfWork.EventRepository.GetbyIdAsync(ticketDetailsViewModel.EventId);
            var numberOfTickets = ticketDetailsViewModel.NumberOfTickets;

            if (_event == null)
            {
                _logger.LogError("Filed to finde the event with id {eventId}", _event.Id);
                return NotFound();
            }

            var key = Guid.NewGuid().ToString();
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(ticketDetailsViewModel));

            var totalAmount = _event.Price * numberOfTickets;

            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd", // أو عملتك
                        UnitAmount = (long)(totalAmount * 100), // Stripe بتتعامل بالسنت
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"تذاكر {_event.Name}",
                            Description = $"{numberOfTickets} تذكرة"
                        }
                    },
                    Quantity = 1,
                }
            },
                Mode = "payment",
                SuccessUrl = Url.Action("Booking", "Home", new { key = key }, Request.Scheme),
                CancelUrl = Url.Action("TicketsDetails", "Home", new { id = _event.Id, numOfTicket = numberOfTickets }, Request.Scheme)
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // نرجع الـ Session ID للـ View أو نوجه مباشرة
            return Redirect(session.Url);

            // نرجع الـ Session ID للـ View أو نوجه مباشرة
        }
    }
}
