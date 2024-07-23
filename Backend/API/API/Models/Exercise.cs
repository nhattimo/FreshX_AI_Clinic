namespace API.Models
{
    public class Exercise
    {
        public int Id { get; set; } // Mã định danh của bài tập
        public string Name { get; set; } // Tên của bài tập
        public string Description { get; set; } // Mô tả của bài tập
        public int DiagnosisId { get; set; } // Khóa ngoại tới chẩn đoán
        public Diagnosis Diagnosis { get; set; } // Đối tượng chẩn đoán
    }

}