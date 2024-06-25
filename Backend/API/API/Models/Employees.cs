using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace API.Server.Models
{
    public class Employees
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        [MaxLength(20)]
        public string Status { get; set; }

        public string AccountEmpId { get; set; } // Chỉnh sửa kiểu dữ liệu thành string

        [ForeignKey("AccountEmpId")]
        public virtual AccountEmp AccountEmp { get; set; }  // One-to-one relationship

        public int RoleId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public Roles Role { get; set; }
    }
}
