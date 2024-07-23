namespace API.Models
{
    /// <summary>
    /// Quản lý hồ sơ y tế của khách hàng, bao gồm chi tiết về bệnh án, chẩn đoán, và điều trị
    /// </summary>
    public class MedicalRecord
    {
        public int Id { get; set; } // Mã định danh của hồ sơ y tế
        public DateTime RecordDate { get; set; } // Ngày ghi nhận hồ sơ
        public string Details { get; set; } // Chi tiết hồ sơ
        public int CustomerId { get; set; } // Khóa ngoại tới khách hàng
        public Customer Customer { get; set; } // Đối tượng khách hàng
        public ICollection<Note> Notes { get; set; } // Ghi chú liên quan đến hồ sơ y tế
    }

}