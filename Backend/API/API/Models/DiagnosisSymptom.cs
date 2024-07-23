namespace API.Models
{
    public class DiagnosisSymptom
    {
        public int DiagnosisId { get; set; } // Khóa ngoại tới chẩn đoán
        public Diagnosis Diagnosis { get; set; } // Đối tượng chẩn đoán
        public int SymptomId { get; set; } // Khóa ngoại tới triệu chứng
        public Symptom Symptom { get; set; } // Đối tượng triệu chứng
    }

}