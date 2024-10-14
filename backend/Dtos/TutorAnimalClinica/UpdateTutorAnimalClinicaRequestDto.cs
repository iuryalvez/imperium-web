namespace backend.Dtos.TutorAnimalClinica
{
    public class UpdateTutorAnimalClinicaRequestDto
    {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public int AnimalId { get; set; }
        public int ClinicaId { get; set; }
    }
}