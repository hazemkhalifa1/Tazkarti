using AutoMapper;
using DAL.Entities;
using Tazkarti.Models;

namespace Tazkarti.Mapper
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketVM>();
        }
    }
}
