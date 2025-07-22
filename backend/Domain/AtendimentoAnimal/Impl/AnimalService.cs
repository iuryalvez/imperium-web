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
    private readonly ITutorRepository _tutorRepository;
    private readonly AppDbContext _context;

    public AnimalService(IAnimalRepository animalRepository,
                         ITutorRepository tutorRepository,
                         AppDbContext context)
    {
        _animalRepository = animalRepository;
        _tutorRepository = tutorRepository;
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
                DataModificado = DateTime.UtcNow
            };

            await _animalRepository.AddAsync(animal);
            await _context.SaveChangesAsync(); // Salva para obter o ID do animal

            await _animalRepository.AddVinculoClinicaAsync(new AnimalClinica { AnimalId = animal.Id, ClinicaId = clinicaId });

            if (dto.TutorIds != null && dto.TutorIds.Any())
            {
                // 1. Validação Eficiente
                var allTutorsAreValid = await _tutorRepository.AllTutorsExistInClinicAsync(dto.TutorIds, clinicaId);
                if (!allTutorsAreValid)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = "Um ou mais tutores selecionados não são válidos ou não pertencem a esta clínica." };
                }

                // 2. Criação dos Vínculos (apenas se a validação passar)
                foreach (var tutorId in dto.TutorIds.Distinct()) // Usa Distinct para evitar vínculos duplicados
                {
                    var vinculoTutorAnimal = new TutorAnimal
                    {
                        AnimalId = animal.Id,
                        TutorId = tutorId,
                        DataAssociacao = DateTime.UtcNow
                    };
                    _context.TutoresAnimais.Add(vinculoTutorAnimal);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResult { Succeeded = true, Message = "Animal cadastrado com sucesso.", Data = new { AnimalId = animal.Id } };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Logar o erro 'ex'
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro ao cadastrar o animal." };
        }
    }

    public async Task<IEnumerable<GetAnimaisClinicaDto>> GetAllByClinicIdAsync(int clinicaId)
    {
        var animais = await _animalRepository.GetAnimaisByClinicIdWithIncludesAsync(clinicaId);
        return animais.Select(MapToDto).ToList();
    }

    public async Task<GetAnimaisClinicaDto?> GetDetailsByIdAsync(int animalId, int clinicaId)
    {
        var animal = await _animalRepository.GetAnimalDetailsAsync(animalId, clinicaId);
        if (animal == null) return null;
        return MapToDto(animal);
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
            // Apenas atualiza os campos que foram fornecidos no DTO
            if (!string.IsNullOrWhiteSpace(dto.Nome)) animal.Nome = dto.Nome;
            if (dto.RacaId.HasValue) animal.RacaId = dto.RacaId.Value;
            if (dto.Especie.HasValue) animal.Especie = dto.Especie.Value;
            if (dto.Nascimento.HasValue) animal.Nascimento = dto.Nascimento.Value.ToUniversalTime();
            if (dto.Sexo.HasValue) animal.Sexo = dto.Sexo.Value;
            if (dto.Castrado.HasValue) animal.Castrado = dto.Castrado.Value;
            animal.DataModificado = DateTime.UtcNow;

            // 2. Sincroniza os vínculos com Tutores
            if (dto.TutorIds != null)
            {
                // 2a. Validação Otimizada
                var allTutorsAreValid = await _tutorRepository.AllTutorsExistInClinicAsync(dto.TutorIds, clinicaId);
                if (!allTutorsAreValid)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = "Um ou mais tutores selecionados não são válidos para esta clínica." };
                }

                var idsTutoresAtuais = animal.TutoresAnimais.Select(ta => ta.TutorId).ToList();
                var idsTutoresDesejados = dto.TutorIds;

                // 2b. Remove vínculos
                var vinculosParaRemover = animal.TutoresAnimais.Where(ta => !idsTutoresDesejados.Contains(ta.TutorId)).ToList();
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

    // Adicione este método privado dentro da sua classe AnimalService.cs
    private GetAnimaisClinicaDto MapToDto(Animal animal)
    {
        return new GetAnimaisClinicaDto
        {
            Id = animal.Id,
            RacaId = animal.RacaId,
            RacaNome = animal.Raca.Nome,
            Nome = animal.Nome,
            Especie = animal.Especie,
            Nascimento = animal.Nascimento,
            Sexo = animal.Sexo,
            Castrado = animal.Castrado,
            Peso = animal.Peso,
            CondicaoCorporal = animal.CondicaoCorporal,
            GrauAtividade = animal.GrauAtividade,
            Gestacao = animal.IsGestante, // Mapeando da propriedade correta da entidade
            Lactacao = animal.IsLactanteNumeroFilhotes, // Mapeando da propriedade correta da entidade
            SemanaLactacao = animal.SemanaLactacao,
            SemanaGestacao = animal.SemanaGestacao,
            DataCad = animal.DataCad,

            // Mapeia a lista de IDs de tutores
            TutorIds = animal.TutoresAnimais.Select(ta => ta.TutorId).ToList(),

            // Mapeia a lista com informações simplificadas dos tutores
            Tutores = animal.TutoresAnimais
                .Where(ta => ta.Tutor != null)
                .Select(ta => new TutorInfoSimplesDto { Id = ta.TutorId, Nome = ta.Tutor.Nome })
                .ToList()
        };
    }
}