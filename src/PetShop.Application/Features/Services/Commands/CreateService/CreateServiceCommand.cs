using AutoMapper;
using MediatR;
using PetShop.Application.Interfaces.Repositories;
using PetShop.Application.Wrappers;
using PetShop.Domain.Entities;

namespace PetShop.Application.Features.Services.Commands.CreateService
{
    public class CreateServiceCommand : IRequest<Response<Guid>>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
    }

    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Response<Guid>>
    {
        private readonly IServiceRepositoryAsync _serviceRepository;
        private readonly IMapper _mapper;

        public CreateServiceCommandHandler(IServiceRepositoryAsync serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = _mapper.Map<Service>(request);
            await _serviceRepository.AddAsync(service);

            return new Response<Guid>(service.Id, "Create service successs");
        }
    }
}
