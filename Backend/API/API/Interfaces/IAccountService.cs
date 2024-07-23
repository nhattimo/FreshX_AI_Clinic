using API.Server.DTOs.Account;


namespace API.Server.Interfaces
{
    // Định nghĩa interface cho dịch vụ tài khoản
    public interface IAccountService
    {
        // Phương thức để đăng ký tài khoản mới
        Task<(bool Succeeded, string ErrorMessage)> RegisterAsync(RegisterDto registerDto);

        // Phương thức để đăng nhập và trả về token JWT nếu thành công
        Task<(bool Succeeded, string Token, string ErrorMessage)> LoginAsync(LoginDto loginDto);

        // Phương thức để đăng xuất
        Task LogoutAsync();
    }
}
