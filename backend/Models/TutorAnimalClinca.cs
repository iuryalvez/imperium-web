namespace backend.Models {
    public class TutorAnimalClinica {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
    }
}
