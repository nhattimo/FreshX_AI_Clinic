namespace API.Models
{
    public class Meal
    {
        public int Id { get; set; } // Mã định danh của bữa ăn
        public string Name { get; set; } // Tên của bữa ăn
        public string Description { get; set; } // Mô tả của bữa ăn
        public int DiagnosisId { get; set; } // Khóa ngoại tới chẩn đoán
        public Diagnosis Diagnosis { get; set; } // Đối tượng chẩn đoán
    }
}