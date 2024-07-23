using System.ComponentModel.DataAnnotations;

namespace API.Server.Models
{
    public class Role
    {
        public int Id { get; set; } // Mã định danh của vai trò
        public string RoleName { get; set; } // Tên của vai trò
        public ICollection<AccountEmp> AccountEmp { get; set; } // Danh sách tài khoản nhân viên
    }

}
