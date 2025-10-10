using AutoMapper;
using MediPorta.API.DTO;
using MediPorta.Domain.Entites;

namespace MediPorta.Application.Mappings
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
