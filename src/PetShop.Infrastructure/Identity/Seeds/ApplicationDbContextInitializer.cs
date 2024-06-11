using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetShop.Application.Enums;
using PetShop.Domain.Entities;
using PetShop.Infrastructure.Identity.Contexts;

namespace PetShop.Infrastructure.Identity.Seeds
{
    public static class InitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

            await initializer.InitializeAsync();

            await initializer.SeedAsync();
        }
    }

    public class ApplicationDbContextInitializer
    {
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public ApplicationDbContextInitializer(
            ILogger<ApplicationDbContextInitializer> logger,
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Seed roles
            if (_roleManager.Roles.All(r => r.Name != Roles.Administrator.ToString()))
            {
                await _roleManager.CreateAsync(new Role() { Name = Roles.Administrator.ToString() });
            }

            if (_roleManager.Roles.All(r => r.Name != Roles.Manager.ToString()))
            {
                await _roleManager.CreateAsync(new Role() { Name = Roles.Manager.ToString() });
            }

            if (_roleManager.Roles.All(r => r.Name != Roles.Employee.ToString()))
            {
                await _roleManager.CreateAsync(new Role() { Name = Roles.Employee.ToString() });
            }

            if (_roleManager.Roles.All(r => r.Name != Roles.CustomerService.ToString()))
            {
                await _roleManager.CreateAsync(new Role() { Name = Roles.CustomerService.ToString() });
            }

            if (_roleManager.Roles.All(r => r.Name != Roles.Client.ToString()))
            {
                await _roleManager.CreateAsync(new Role() { Name = Roles.Client.ToString() });
            }

            if (_roleManager.Roles.All(r => r.Name != Roles.Supplier.ToString()))
            {
                await _roleManager.CreateAsync(new Role() { Name = Roles.Supplier.ToString() });
            }

            // Seed admin user
            // Default users
            var administrator = new User { UserName = "administrator", Email = "administrator@gmail.com" };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "Administrator1!");
                var administratorRole = new IdentityRole(Roles.Administrator.ToString());
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
                }
            }
        }
    }
}
