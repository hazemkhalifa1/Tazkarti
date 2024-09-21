using AutoMapper;
using DAL.Entities;
using Tazkarti.Models;

namespace Tazkarti.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserVM>();
        }
    }
}
