namespace API.Models
{
    public class Advice
    {
        public int Id { get; set; } // Mã định danh lời khuyên
        public int DiagnosisId { get; set; } // Mã chẩn đoán liên quan
        public Diagnosis Diagnosis { get; set; } // Tham chiếu đến đối tượng chẩn đoán
        public string Content { get; set; } // Nội dung lời khuyên
    }

}