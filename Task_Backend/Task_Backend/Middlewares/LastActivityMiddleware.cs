using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Task_Backend.Models;

namespace Task_Backend.Middlewares
{
    public class LastActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LastActivityMiddleware> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LastActivityMiddleware(
            RequestDelegate next,
            ILogger<LastActivityMiddleware> logger,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _next = next;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InvokeAsync(HttpContext context, ModelContext dbContext)
        {
            if (context.User.Identity == null)
            {
                throw new ArgumentNullException(nameof(context), "HttpContext cannot be null");
            }


            if (context.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("UserId claim is null for an authenticated user.");
                }
                else
                {
                    var user = await dbContext.Users.FindAsync(userIdClaim.Value);
                    if (user == null)
                    {
                        _logger.LogWarning($"User not found with ID: {userIdClaim.Value}");
                    }
                    else
                    {
                        if (!user.LastActive.HasValue)
                        {
                            if (!await _roleManager.RoleExistsAsync("User"))
                            {
                                await _roleManager.CreateAsync(new IdentityRole("User"));
                            }
                            await _userManager.AddToRoleAsync(user, "User");
                        }

                        user.LastActive = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }

            await _next(context);
        }
    }
}
