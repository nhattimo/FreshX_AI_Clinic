namespace API.Models
{
    public class Diagnosis
    {
        public int Id { get; set; } // Mã định danh chẩn đoán
        public string Name { get; set; } // Tên chẩn đoán
        public string Description { get; set; } // Mô tả chẩn đoán
        public ICollection<DiagnosisSymptom> DiagnosisSymptoms { get; set; }
        public ICollection<Advice> Advices { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
        public ICollection<Meal> Meals { get; set; }
        public ICollection<Note> Notes { get; set; }
    }


}