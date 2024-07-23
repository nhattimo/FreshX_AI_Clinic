using API.Server.DTOs.Account;
using API.Server.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    // Inject JwtTokenService qua constructor
    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto model)
    {
        var user = AuthenticateUser(model); // Xác thực người dùng

        if (user == null)
        {
            return Unauthorized(); // Trả về Unauthorized nếu xác thực thất bại
        }

        var token = _jwtTokenService.GenerateJwtToken(user); // Tạo JWT token
        return Ok(new { Token = token }); // Trả về token cho client
    }

    private AccountEmp AuthenticateUser(LoginDto model)
    {
        // Kiểm tra thông tin đăng nhập từ cơ sở dữ liệu hoặc nguồn dữ liệu khác
        // Trả về đối tượng AccountEmp nếu thông tin hợp lệ, ngược lại trả về null
        return new AccountEmp();
    }
}
