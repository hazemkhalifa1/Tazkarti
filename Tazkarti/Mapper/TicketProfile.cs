using AutoMapper;
using DAL.Entities;
using Tazkarti.Models;

namespace Tazkarti.Mapper
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketVM>()
                .ForMember(t => t.EventName, o => o.MapFrom(t => t.Event.Name))
                .ForMember(t => t.UserName, o => o.MapFrom(t => t.user.UserName));
        }
    }
}
