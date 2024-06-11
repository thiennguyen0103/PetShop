using Microsoft.EntityFrameworkCore;
using PetShop.Application.Interfaces.Repositories;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Identity.Contexts;

namespace PetShop.Infrastructure.Data.Repositories
{
    public class ServiceRepositoryAsync : GenericRepositoryAsync<Service>, IServiceRepositoryAsync
    {
        private readonly DbSet<Service> _services;
        public ServiceRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _services = dbContext.Set<Service>();
        }
    }
}
