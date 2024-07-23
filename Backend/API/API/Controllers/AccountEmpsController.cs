using Microsoft.AspNetCore.Mvc;
using API.Server.Interfaces;
using API.Server.DTOs.Account;
using System.Threading.Tasks;

namespace API.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // API endpoint cho đăng ký người dùng mới
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Gọi phương thức RegisterAsync từ IAccountService
            var (succeeded, errorMessage) = await _accountService.RegisterAsync(registerDto);

            // Kiểm tra kết quả đăng ký
            if (!succeeded)
                return BadRequest(errorMessage);

            // Trả về kết quả thành công
            return Ok("User registered successfully.");
        }

        // API endpoint cho đăng nhập người dùng
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Gọi phương thức LoginAsync từ IAccountService
            var (succeeded, token, errorMessage) = await _accountService.LoginAsync(loginDto);

            // Kiểm tra kết quả đăng nhập
            if (succeeded)
                return Ok(new { Token = token });

            // Trả về lỗi nếu đăng nhập thất bại
            return BadRequest(errorMessage);
        }

        // API endpoint cho đăng xuất người dùng
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Gọi phương thức LogoutAsync từ IAccountService
            await _accountService.LogoutAsync();

            // Trả về kết quả thành công
            return Ok("Logout successful.");
        }
    }
}
