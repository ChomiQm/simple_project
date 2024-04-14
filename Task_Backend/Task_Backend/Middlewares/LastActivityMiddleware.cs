using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Task_Backend.Models;

namespace Task_Backend.Middlewares
{
    public class LastActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LastActivityMiddleware> _logger;

        public LastActivityMiddleware(RequestDelegate next, ILogger<LastActivityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ModelContext dbContext, IServiceScopeFactory scopeFactory)
        {
            if (context.User.Identity == null)
            {
                throw new ArgumentNullException(nameof(context), "HttpContext cannot be null");
            }

            if (context.User.Identity.IsAuthenticated)
            {
                try
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim == null)
                        {
                            _logger.LogWarning("UserId claim is null for an authenticated user.");
                        }
                        else
                        {
                            var user = await userManager.FindByIdAsync(userIdClaim.Value);
                            if (user == null)
                            {
                                _logger.LogWarning($"User not found with ID: {userIdClaim.Value}");
                            }
                            else
                            {
                                if (!user.LastActive.HasValue)
                                {
                                    if (!await roleManager.RoleExistsAsync("User"))
                                    {
                                        await roleManager.CreateAsync(new IdentityRole("User"));
                                    }
                                    await userManager.AddToRoleAsync(user, "User");
                                }

                                user.LastActive = DateTime.UtcNow;
                                await userManager.UpdateAsync(user);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the user's last activity.");
                    throw;
                }

            }

            await _next(context);
        }
    }
}

