using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly ILogger<AnimalService> _logger;
    private readonly AppDbContext _context;

    public AnimalService(IAnimalRepository animalRepository,
                         ITutorRepository tutorRepository,
                         IMapper mapper,
                         ILogger<AnimalService> logger,
                         AppDbContext context)
    {
        _animalRepository = animalRepository;
        _tutorRepository = tutorRepository;
        _mapper = mapper;
        _logger = logger;
        _context = context;
    }

    public async Task<ServiceResult> CreateAsync(CadastrarAnimalClinicaDto dto, int clinicaId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Usa AutoMapper para criar o Animal
            var animal = _mapper.Map<Animal>(dto);
            animal.Nascimento = dto.Nascimento.ToUniversalTime();
            animal.DataCad = DateTime.UtcNow;
            animal.DataModificado = DateTime.UtcNow;

            await _animalRepository.AddAsync(animal);
            await _context.SaveChangesAsync(); // salva para gerar o ID

            // Vínculo com clínica
            await _animalRepository.AddVinculoClinicaAsync(new AnimalClinica
            {
                AnimalId = animal.Id,
                ClinicaId = clinicaId,
                DataAssociacao = DateTime.UtcNow,
                IsAtivo = true
            });

            // Vínculos com tutores (se houver)
            if (dto.TutorIds?.Any() == true)
            {
                var allTutorsAreValid = await _tutorRepository.AllTutorsExistInClinicAsync(dto.TutorIds, clinicaId);
                if (!allTutorsAreValid)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult
                    {
                        Succeeded = false,
                        Message = "Um ou mais tutores selecionados não são válidos ou não pertencem a esta clínica."
                    };
                }

                foreach (var tutorId in dto.TutorIds.Distinct())
                {
                    _context.TutoresAnimais.Add(new TutorAnimal
                    {
                        AnimalId = animal.Id,
                        TutorId = tutorId,
                        DataAssociacao = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResult
            {
                Succeeded = true,
                Message = "Animal cadastrado com sucesso.",
                Data = new { AnimalId = animal.Id }
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new ServiceResult
            {
                Succeeded = false,
                Message = "Ocorreu um erro ao cadastrar o animal."
            };
        }
    }

    public async Task<IEnumerable<GetAnimaisClinicaDto>> GetAllByClinicIdAsync(int clinicaId)
    {
        var animais = await _animalRepository.GetAnimaisByClinicIdWithIncludesAsync(clinicaId);
        return _mapper.Map<IEnumerable<GetAnimaisClinicaDto>>(animais);
    }

    public async Task<GetAnimaisClinicaDto?> GetDetailsByIdAsync(int animalId, int clinicaId)
    {
        var animal = await _animalRepository.GetAnimalDetailsAsync(animalId, clinicaId);
        return _mapper.Map<GetAnimaisClinicaDto?>(animal);
    }

    public async Task<ServiceResult> UpdateAsync(EditarAnimalClinicaDto dto, int clinicaId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var animal = await _animalRepository.GetByIdWithIncludesAsync(dto.Id, clinicaId);
            if (animal == null)
            {
                return new ServiceResult
                {
                    Succeeded = false,
                    Message = $"Animal com ID {dto.Id} não encontrado nesta clínica."
                };
            }

            // Atualiza apenas campos informados (AutoMapper com NullIgnore)
            _mapper.Map(dto, animal);
            animal.DataModificado = DateTime.UtcNow;

            // Sincroniza vínculos com tutores
            if (dto.TutorIds != null)
            {
                var allTutorsAreValid = await _tutorRepository.AllTutorsExistInClinicAsync(dto.TutorIds, clinicaId);
                if (!allTutorsAreValid)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult
                    {
                        Succeeded = false,
                        Message = "Um ou mais tutores selecionados não são válidos para esta clínica."
                    };
                }

                var idsAtuais = animal.TutoresAnimais.Select(ta => ta.TutorId).ToList();
                var idsDesejados = dto.TutorIds;

                // Remove vínculos antigos
                var paraRemover = animal.TutoresAnimais
                    .Where(ta => !idsDesejados.Contains(ta.TutorId))
                    .ToList();

                if (paraRemover.Any())
                    _animalRepository.RemoveTutorLinks(paraRemover);

                // Adiciona novos vínculos
                var paraAdicionar = idsDesejados.Except(idsAtuais).ToList();
                foreach (var tutorId in paraAdicionar)
                {
                    await _animalRepository.AddTutorLinkAsync(new TutorAnimal
                    {
                        AnimalId = animal.Id,
                        TutorId = tutorId,
                        DataAssociacao = DateTime.UtcNow
                    });
                }
            }

            await _animalRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResult
            {
                Succeeded = true,
                Message = "Animal atualizado com sucesso.",
                Data = new { AnimalId = animal.Id }
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new ServiceResult
            {
                Succeeded = false,
                Message = "Ocorreu um erro interno ao atualizar o animal."
            };
        }
    }

    public async Task<ServiceResult> DesativarVinculoAnimalAsync(int animalId, int clinicaId)
    {
        // Usaremos uma transação para garantir a atomicidade
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var vinculo = await _context.AnimaisClinicas
                .FirstOrDefaultAsync(ac => ac.AnimalId == animalId && ac.ClinicaId == clinicaId);

            if (vinculo == null)
            {
                _logger.LogWarning("Vínculo Animal-Clínica não encontrado para animal {AnimalId} e clínica {ClinicaId}", animalId, clinicaId);
                return new ServiceResult { Succeeded = false, Message = "Vínculo do animal com a clínica não encontrado." };
            }

            if (!vinculo.IsAtivo)
            {
                _logger.LogInformation("Vínculo para animal {AnimalId} já está inativo.", animalId);
                return new ServiceResult { Succeeded = true, Message = "O vínculo já estava inativo." }; // Não é um erro
            }

            vinculo.IsAtivo = false;
            vinculo.DataDesativacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Vínculo do animal {AnimalId} com a clínica {ClinicaId} foi desativado.", animalId, clinicaId);
            return new ServiceResult { Succeeded = true, Message = "Vínculo do animal com a clínica foi desativado com sucesso." };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao desativar vínculo do animal {AnimalId}", animalId);
            return new ServiceResult { Succeeded = false, Message = "Ocorreu um erro interno ao processar a solicitação." };
        }
    }
}