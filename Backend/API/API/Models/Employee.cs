using API.Server.Models;

namespace API.Models
{
    public class Employee
    {
        public int Id { get; set; } // Mã định danh của nhân viên
        public string Name { get; set; } // Tên của nhân viên
        public DateTime BirthDate { get; set; } // Ngày sinh của nhân viên
        public string Gender { get; set; } // Giới tính của nhân viên
        public string Address { get; set; } // Địa chỉ của nhân viên
        public string Phone { get; set; } // Số điện thoại của nhân viên
        public string Email { get; set; } // Email của nhân viên
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<AccountEmp> AccountEmp { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

}