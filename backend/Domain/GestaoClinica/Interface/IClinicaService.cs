using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.GestaoClinica.Interface
{
    public interface IClinicaService
    {
        public Task<Clinica?> ObterClinicaAsync(int clinicaId);
        public Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId);
        public Task<List<TutorClinica>> ObterVinculosTutoresDaClinicaAsync(int clinicaId);
        public Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaAsync(int clinicaId);
        public Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaComTutoresAsync(int clinicaId);
    }
}
