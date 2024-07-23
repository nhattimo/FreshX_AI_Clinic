namespace API.Models
{
    public class Insurance
    {
        public int Id { get; set; } // Mã định danh của bảo hiểm
        public string Provider { get; set; } // Nhà cung cấp bảo hiểm
        public string PolicyNumber { get; set; } // Số hợp đồng bảo hiểm
        public DateTime ExpirationDate { get; set; } // Ngày hết hạn
        public int CustomerId { get; set; } // Khóa ngoại tới khách hàng
        public Customer Customer { get; set; } // Đối tượng khách hàng
    }

}