using AutoMapper;
using PetShop.Application.Features.Services.Commands.CreateService;
using PetShop.Domain.Entities;

namespace PetShop.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<CreateServiceCommand, Service>();
        }
    }
}
