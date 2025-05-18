using backend.Models;

namespace backend.Interfaces
{
    public interface IClinicaService
    {
        public Task<Clinica?> ObterClinicaAsync(int clinicaId);
        public Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        // public Task<bool> AnimalExisteNaClinicaAsync(string cpf, int clinicaId); // passaporte ou algum outro id unico
        public Task<List<TutorClinica>> ObterVinculosTutoresDaClinicaAsync(int clinicaId);
        public Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaAsync(int clinicaId);
    }
}
