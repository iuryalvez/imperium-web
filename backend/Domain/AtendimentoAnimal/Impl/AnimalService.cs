using backend.Domain.AtendimentoAnimal;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly AppDbContext _context; // Para validações e transações

    public AnimalService(IAnimalRepository animalRepository, AppDbContext context)
    {
        _animalRepository = animalRepository;
        _context = context;
    }
    
    public async Task<ServiceResult> CadastrarAnimalAsync(CadastrarAnimalClinicaDto dto, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var animal = new Animal
                {
                    Nome = dto.Nome,
                    Especie = dto.Especie,
                    RacaId = dto.RacaId,
                    Nascimento = dto.Nascimento.ToUniversalTime(),
                    Sexo = dto.Sexo,
                    Castrado = dto.Castrado,
                    DataCad = DateTime.UtcNow,
                    DataModificado = DateTime.UtcNow,
                    // Os outros campos (Peso, etc.) terão seus valores padrão do C# (0 ou null)
                };

                await _animalRepository.AddAsync(animal);
                await _context.SaveChangesAsync(); // Salva para obter o ID do animal

                await _animalRepository.AddVinculoClinicaAsync(new AnimalClinica { AnimalId = animal.Id, ClinicaId = clinicaId });

                if (dto.TutorIds != null && dto.TutorIds.Any())
                {
                    // Reutilize a lógica de validação e vínculo do seu antigo controller aqui
                    // Exemplo:
                    foreach (var tutorId in dto.TutorIds)
                    {
                        // Validação...
                        _context.TutoresAnimais.Add(new TutorAnimal { AnimalId = animal.Id, TutorId = tutorId });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return new ServiceResult { Succeeded = true, Message = "Animal cadastrado com sucesso.", Data = new { AnimalId = animal.Id }};
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar o erro 'ex'
                return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro ao cadastrar o animal." };
            }
        }

        public async Task<IEnumerable<GetAnimaisClinicaDto>> GetAllByClinicIdAsync(int clinicaId)
        {
            var animais = await _animalRepository.GetAnimaisByClinicIdWithIncludesAsync(clinicaId);
            return animais.Select(animal => new GetAnimaisClinicaDto
            {
                Id = animal.Id,
                Nome = animal.Nome,
                // ... Mapear todos os campos para o DTO ...
                Tutores = animal.TutoresAnimais
                    .Select(ta => new TutorInfoSimplesDto { Id = ta.TutorId, Nome = ta.Tutor.Nome })
                    .ToList()
            }).ToList();
        }

        public async Task<GetAnimaisClinicaDto?> GetDetailsByIdAsync(int animalId, int clinicaId)
        {
            var animal = await _animalRepository.GetAnimalDetailsAsync(animalId, clinicaId);
            if (animal == null) return null;
            
            // Mapeie a entidade 'Animal' para 'GetAnimaisClinicaDto' aqui
            return new GetAnimaisClinicaDto { /* ... mapeamento ... */ };
        }

    public async Task<ServiceResult> UpdateAnimalAsync(EditarAnimalClinicaDto dto, int clinicaId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var animal = await _animalRepository.GetByIdWithIncludesAsync(dto.Id, clinicaId);
            if (animal == null)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Succeeded = false, Message = $"Animal com ID {dto.Id} não encontrado nesta clínica." };
            }

            // 1. Atualiza as propriedades do animal
            animal.Nome = dto.Nome;
            animal.Especie = dto.Especie;
            // ... Mapeie todas as outras propriedades do DTO para a entidade animal ...
            animal.DataModificado = DateTime.UtcNow;

            // 2. Sincroniza os vínculos com Tutores
            var idsTutoresAtuais = animal.TutoresAnimais.Select(ta => ta.TutorId).ToList();
            var idsTutoresDesejados = dto.TutorIds ?? new List<int>();

            // 2a. Valida se todos os tutores desejados existem na clínica
            foreach (var tutorId in idsTutoresDesejados)
            {
                if (!await _context.TutoresClinicas.AnyAsync(tc => tc.TutorId == tutorId && tc.ClinicaId == clinicaId))
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = $"Tutor com ID {tutorId} não é válido ou não pertence à clínica." };
                }
            }

            // 2b. Remove vínculos que não são mais necessários
            var vinculosParaRemover = animal.TutoresAnimais
                .Where(ta => !idsTutoresDesejados.Contains(ta.TutorId))
                .ToList();
            if (vinculosParaRemover.Any())
            {
                _animalRepository.RemoveTutorLinks(vinculosParaRemover);
            }

            // 2c. Adiciona novos vínculos
            var idsParaAdicionar = idsTutoresDesejados.Except(idsTutoresAtuais).ToList();
            foreach (var tutorId in idsParaAdicionar)
            {
                await _animalRepository.AddTutorLinkAsync(new TutorAnimal { AnimalId = animal.Id, TutorId = tutorId });
            }

            await _animalRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResult { Succeeded = true, Message = "Animal atualizado com sucesso.", Data = new { AnimalId = animal.Id } };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Logar o erro 'ex'
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao atualizar o animal." };
        }
    }
}