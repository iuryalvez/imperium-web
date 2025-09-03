using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Services
{
    public class AcompanhamentoService : IAcompanhamentoService
    {
        private readonly IAcompanhamentoRepository _repository;
        private readonly AppDbContext _context;

        public AcompanhamentoService(IAcompanhamentoRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<ServiceResult> CreateAsync(int animalId)
        {
            // Evita criar acompanhamento duplicado
            if (await _repository.ExistsByAnimalIdAsync(animalId))
            {
                return new ServiceResult
                {
                    Succeeded = false,
                    Message = "Já existe um acompanhamento para este animal."
                };
            }

            var acompanhamento = new AcompanhamentoIndividualizado
            {
                AnimalId = animalId,
                DataInicio = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _repository.AddAsync(acompanhamento);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                Succeeded = true,
                Message = "Acompanhamento criado com sucesso.",
                Data = new { AcompanhamentoId = acompanhamento.Id }
            };
        }

        public async Task<AcompanhamentoIndividualizado?> GetByAnimalIdAsync(int animalId)
        {
            return await _repository.GetByAnimalIdAsync(animalId);
        }
    }
}
