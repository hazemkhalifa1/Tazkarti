using AutoMapper;
using DAL.Entities;
using Tazkarti.Models;

namespace Tazkarti.Mapper
{
    public class UserTicketProfile : Profile
    {
        public UserTicketProfile()
        {
            CreateMap<Ticket, UserTicketVM>()
                .ForMember(t => t.EventVM, o => o.MapFrom(t => t.Event))
                .ForMember(t => t.UserName, o => o.MapFrom(t => t.User.UserName));
        }
    }
}
