namespace API.Models
{
    public class Customer
    {
        public int Id { get; set; } // Mã định danh của khách hàng
        public string Name { get; set; } // Tên của khách hàng
        public DateTime BirthDate { get; set; } // Ngày sinh của khách hàng
        public string Gender { get; set; } // Giới tính của khách hàng
        public string Address { get; set; } // Địa chỉ của khách hàng
        public string Phone { get; set; } // Số điện thoại của khách hàng
        public string Email { get; set; } // Email của khách hàng
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<AccountCus> AccountCus { get; set; }
        public ICollection<Insurance> Insurances { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
    }

}