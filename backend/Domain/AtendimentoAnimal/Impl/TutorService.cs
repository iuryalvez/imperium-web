using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly AppDbContext _context;

        public TutorService(ITutorRepository tutorRepository, AppDbContext context)
        {
            _tutorRepository = tutorRepository;
            _context = context;
        }

        public async Task<ServiceResult> UpdateTutorAsync(EditarTutorClinicaDto dto, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tutor = await _tutorRepository.GetByIdWithIncludesAsync(dto.Id, clinicaId);
                if (tutor == null)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = $"Tutor com ID {dto.Id} não encontrado nesta clínica." };
                }

                // 1. Atualiza propriedades do tutor (Nome, CPF, etc.)
                tutor.Nome = dto.Nome.Trim();
                tutor.Cpf = dto.Cpf.Trim();
                // ... Mapeie todas as outras propriedades ...

                // 2. Sincroniza os vínculos com Animais
                var idsAnimaisAtuais = tutor.TutoresAnimais.Select(ta => ta.AnimalId).ToList();
                var idsAnimaisDesejados = dto.AnimalIds ?? new List<int>();

                // ... (Lógica idêntica à de animais para validar, remover e adicionar vínculos de animais) ...

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResult { Succeeded = true, Message = "Tutor atualizado com sucesso.", Data = new { TutorId = tutor.Id } };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar o erro 'ex'
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao atualizar o tutor." };
            }
        }
    }
}
