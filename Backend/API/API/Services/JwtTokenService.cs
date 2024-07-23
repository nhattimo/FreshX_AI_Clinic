using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Server.Models;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    // Constructor để khởi tạo các giá trị secret key, issuer và audience
    public JwtTokenService(string secretKey, string issuer, string audience)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
    }

    // Phương thức để tạo JWT token
    public string GenerateJwtToken(AccountEmp user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey); // Mã hóa secret key thành mảng byte
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username), // Thêm claim cho Username
                new Claim(ClaimTypes.Role, user.Role.RoleName) // Thêm claim cho Role
            }),
            Expires = DateTime.UtcNow.AddHours(1), // Đặt thời gian hết hạn cho token
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature), // Thiết lập cách thức mã hóa và ký token
            Issuer = _issuer,
            Audience = _audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor); // Tạo token
        return tokenHandler.WriteToken(token); // Trả về token dưới dạng chuỗi
    }
}
