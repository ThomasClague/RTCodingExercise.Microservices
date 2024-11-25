using Contracts.Constants;
using Identity.API.Contracts;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Data
{
    public class RoleAndUserSeeder : IRoleAndUserSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleAndUserSeeder> _logger;

        public RoleAndUserSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RoleAndUserSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedRolesAndUsersAsync()
        {
            try
            {
                var roles = new[] { Roles.Admin, Roles.User };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                        _logger.LogInformation($"Role '{role}' created");
                    }
                }

                
                await CreateUserIfNotExists("admin", "user", "admin@regtransfers.com", "Password123*", Roles.Admin);
                await CreateUserIfNotExists("standard", "user", "user@regtransfers.com", "Password123*", Roles.User);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while seeding roles and users: {Message}", ex.Message);
                throw;
            }
        }

        private async Task CreateUserIfNotExists(string firstname, string lastName, string email, string password, string role)
        {
            var user = new ApplicationUser
            {
                FirstName = firstname,
                LastName = lastName,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var testUser = await _userManager.FindByEmailAsync(email);
            if (testUser != null) 
            {
                await _userManager.DeleteAsync(testUser);
            }

            if (await _userManager.FindByEmailAsync(email) == null)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        _logger.LogInformation($"User '{email}' created and assigned to role '{role}'");
                    }
                    else
                    {
                        _logger.LogError($"Role '{role}' does not exist, so user '{email}' could not be assigned to it");
                    }
                }
                else
                {
                    _logger.LogError("Error creating user '{Email}': {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation($"User '{email}' already exists");
                var existingUser = await _userManager.FindByEmailAsync(email);
                var userRoles = await _userManager.GetRolesAsync(existingUser);

                if (!userRoles.Contains(role))
                {
                    await _userManager.AddToRoleAsync(existingUser, role);
                    _logger.LogInformation($"User '{email}' assigned to role '{role}'");
                }
            }
        }
    }
}