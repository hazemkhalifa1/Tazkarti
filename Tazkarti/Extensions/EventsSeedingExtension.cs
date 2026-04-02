using DAL.Context;
using DAL.Entities;

namespace Tazkarti.extension
{
    public static class EventsSeedingExtension
    {
        public static async Task EventSeeding(this WebApplication app)
        {
            using var Services = app.Services.CreateAsyncScope();
            var Service = Services.ServiceProvider;
            var _logger = Service.GetRequiredService<ILogger<Program>>();
            try
            {
                var dbContext = Service.GetRequiredService<AppDbContext>();
                var events = new List<Event>
                {
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "Music Concert",
                        NameAr = "حفل موسيقي",
                        place = "Cairo Opera House",
                        placeAr = "دار الأوبرا المصرية",
                        Time = DateTime.Now.AddDays(5),
                        NoOfTickets = 150,
                        Price = 300,
                        Info = "Live music concert with famous bands.",
                        InfoAr = "حفل موسيقي مباشر مع فرق مشهورة"
                    },
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tech Conference",
                        NameAr = "مؤتمر التكنولوجيا",
                        place = "Smart Village",
                        placeAr = "القرية الذكية",
                        Time = DateTime.Now.AddDays(10),
                        NoOfTickets = 200,
                        Price = 500,
                        Info = "Conference about latest tech trends.",
                        InfoAr = "مؤتمر عن أحدث اتجاهات التكنولوجيا"
                    },
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "Football Match",
                        NameAr = "مباراة كرة قدم",
                        place = "Cairo Stadium",
                        placeAr = "استاد القاهرة",
                        Time = DateTime.Now.AddDays(2),
                        NoOfTickets = 500,
                        Price = 150,
                        Info = "Exciting football match between top teams.",
                        InfoAr = "مباراة كرة قدم مثيرة بين أفضل الفرق"
                    },
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "Art Exhibition",
                        NameAr = "معرض فني",
                        place = "Zamalek Gallery",
                        placeAr = "جاليري الزمالك",
                        Time = DateTime.Now.AddDays(7),
                        NoOfTickets = 80,
                        Price = 100,
                        Info = "Modern art exhibition.",
                        InfoAr = "معرض فن حديث"
                    },
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "Stand-up Comedy",
                        NameAr = "ستاند أب كوميدي",
                        place = "Downtown Theater",
                        placeAr = "مسرح وسط البلد",
                        Time = DateTime.Now.AddDays(3),
                        NoOfTickets = 120,
                        Price = 200,
                        Info = "Night full of laughter.",
                        InfoAr = "ليلة مليئة بالضحك"
                    }
                };
                if (!dbContext.Events.Any())
                {
                    await dbContext.Events.AddRangeAsync(events);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding events.");
                throw;
            }
        }
    }
}
