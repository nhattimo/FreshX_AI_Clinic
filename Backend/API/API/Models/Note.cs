namespace API.Models
{
    public class Note
    {
        public int Id { get; set; } // Mã định danh của ghi chú
        public string Content { get; set; } // Nội dung của ghi chú
        public DateTime CreatedDate { get; set; } // Ngày tạo ghi chú
        public int MedicalRecordId { get; set; } // Khóa ngoại tới hồ sơ y tế
        public MedicalRecord MedicalRecord { get; set; } // Đối tượng hồ sơ y tế
        public int DiagnosisId { get; set; } // Khóa ngoại tới chẩn đoán
        public Diagnosis Diagnosis { get; set; } // Đối tượng chẩn đoán
    }

}