namespace API.Models
{
    /// <summary>
    /// Quản lý thông tin các cuộc hẹn giữa khách hàng và nhân viên của phòng khám
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; } // Mã định danh của cuộc hẹn
        public DateTime AppointmentDate { get; set; } // Ngày và giờ của cuộc hẹn
        public int CustomerId { get; set; } // Mã định danh của khách hàng (bệnh nhân)
        public Customer Customer { get; set; } // Tham chiếu đến đối tượng khách hàng
        public int EmployeeId { get; set; } // Mã định danh của nhân viên (bác sĩ hoặc nhân viên y tế)
        public Employee Employee { get; set; } // Tham chiếu đến đối tượng nhân viên
        public string Description { get; set; } // Mô tả chi tiết về cuộc hẹn
        public string Status { get; set; } // Trạng thái của cuộc hẹn (Scheduled, Completed, Cancelled)
    }
}