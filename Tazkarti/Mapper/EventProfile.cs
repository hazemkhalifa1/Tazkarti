using AutoMapper;
using DAL.Entities;
using Tazkarti.Models;

namespace Tazkarti.Mapper
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventVM>().ReverseMap();
        }
    }
}
