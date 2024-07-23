namespace API.Models
{
    public class Symptom
    {
        public int Id { get; set; } // Mã định danh của triệu chứng
        public string Name { get; set; } // Tên của triệu chứng
        public string Description { get; set; } // Mô tả của triệu chứng
        public ICollection<DiagnosisSymptom> DiagnosisSymptoms { get; set; }
    }


}