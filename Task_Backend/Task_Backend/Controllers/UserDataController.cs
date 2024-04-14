using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Backend.DTO;
using Task_Backend.Models;

namespace Task_Backend.Controllers
{
    [Route("userData")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserDataController> _logger;
        private readonly ModelContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserDataController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<UserDataController> logger,
            ModelContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpGet("getData")]
        [Authorize]
        public async Task<ActionResult<UserData>> GetActualUserData()
        {
            var userId = _userManager.GetUserId(User);
            _logger.LogInformation($"Pobieranie danych użytkownika o ID: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Nieautoryzowany dostęp próba uzyskania danych użytkownika.");
                return StatusCode(401, "Unauthorized");
            }

            var userData = await _context.UserDatas.FirstOrDefaultAsync(u => u.UserId == userId);

            if (userData == null)
            {
                _logger.LogWarning($"Nie znaleziono danych dla użytkownika o ID: {userId}");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInformation($"Zwracanie danych dla użytkownika o ID: {userId}");
            return userData;
        }

        [HttpPost("addData")]
        [Authorize]
        public async Task<ActionResult<UserData>> PostUserData([FromBody] UserData userData)
        {
            if (userData == null)
            {
                _logger.LogWarning("Próba dodania pustych danych użytkownika.");
                return BadRequest(new { error = "Provided user data is null." });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Nieautoryzowany dostęp próba dodania danych użytkownika.");
                return Unauthorized();
            }

            userData.UserId = currentUser.Id;
            userData.UserDataId = Guid.NewGuid().ToString();
            try
            {
                _context.UserDatas.Add(userData);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Dodano dane użytkownika o ID: {userData.UserId}");
                return CreatedAtAction(nameof(GetActualUserData), new { id = userData.UserId }, userData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred when saving the data: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred when saving the data." });
            }
        }

        [HttpPost("hasUserData")]
        [Authorize]
        public async Task<ActionResult<bool>> HasUserData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Nieautoryzowany dostęp próba sprawdzenia danych użytkownika.");
                return Unauthorized();
            }

            var userData = await _context.Set<UserData>().FirstOrDefaultAsync(ud => ud.User == currentUser);
            return userData != null;
        }

        [HttpPut("updateData")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData([FromBody] UserDataDto userDataDto)
        {
            if (userDataDto == null)
            {
                _logger.LogWarning("Próba aktualizacji z pustym DTO.");
                return BadRequest(new { error = "Provided user data is null." });
            }

            var userId = _userManager.GetUserId(User);
            var userData = await _context.UserDatas.FirstOrDefaultAsync(ud => ud.UserId == userId);
            if (userData == null)
            {
                _logger.LogWarning($"Nie znaleziono danych dla użytkownika o ID: {userId}");
                return NotFound(new { error = "User data not found." });
            }

            userData.FirstName = userDataDto.FirstName;
            userData.LastName = userDataDto.LastName;
            userData.City = userDataDto.City;

            try
            {
                _context.UserDatas.Update(userData);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Zaktualizowano dane użytkownika o ID: {userId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Wystąpił błąd przy aktualizacji danych: {ex.Message}");
                return StatusCode(500, new { error = "An error occurred while updating the data." });
            }
        }

    }
}

