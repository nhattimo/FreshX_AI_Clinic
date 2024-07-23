using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Server.DTOs.Account;
using API.Server.Interfaces;
using API.Server.Models;

namespace API.Server.Services
{
    public class AccountService : IAccountService
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly List<AccountEmp> _users; // Danh sách giả lập người dùng

        public AccountService(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
            _users = new List<AccountEmp>(); // Khởi tạo danh sách người dùng giả lập
        }

        // Phương thức để đăng ký người dùng mới
        public async Task<(bool Succeeded, string ErrorMessage)> RegisterAsync(RegisterDto registerDto)
        {
            // Kiểm tra xem người dùng đã tồn tại hay chưa
            if (_users.Any(u => u.Username == registerDto.Email))
            {
                return (false, "User already exists.");
            }

            // Tạo người dùng mới
            var user = new AccountEmp
            {
                Username = registerDto.Email,
                Password = registerDto.Password, // Cần mã hóa mật khẩu trong thực tế
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId = 1 // Đặt RoleId mặc định
            };

            // Lưu người dùng vào danh sách giả lập
            _users.Add(user);

            return (true, null);
        }

        // Phương thức để đăng nhập và tạo token JWT
        public async Task<(bool Succeeded, string Token, string ErrorMessage)> LoginAsync(LoginDto loginDto)
        {
            // Tìm người dùng theo tên đăng nhập
            var user = _users.SingleOrDefault(u => u.Username == loginDto.UserName);

            // Kiểm tra thông tin đăng nhập
            if (user == null || user.Password != loginDto.Password) // Cần kiểm tra mật khẩu mã hóa trong thực tế
            {
                return (false, null, "Invalid login attempt.");
            }

            // Tạo token JWT
            var token = _jwtTokenService.GenerateJwtToken(user);
            return (true, token, null);
        }

        // Phương thức để đăng xuất người dùng
        public async Task LogoutAsync()
        {
            // Nếu bạn muốn thực hiện logic đăng xuất, hãy thêm vào đây
        }
    }
}
