using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using API.Models;

namespace API.Server.Models
{
    public class AccountEmp
    {
        public int Id { get; set; } // Mã định danh của tài khoản nhân viên
        public string Username { get; set; } // Tên tài khoản
        public string Password { get; set; } // Mật khẩu
        public string Email { get; set; } // Email của tài khoản
        public int EmployeeId { get; set; } // Khóa ngoại tới nhân viên
        public Employee Employee { get; set; } // Đối tượng nhân viên
        public int RoleId { get; set; } // Khóa ngoại tới vai trò
        public Role Role { get; set; } // Đối tượng vai trò
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

