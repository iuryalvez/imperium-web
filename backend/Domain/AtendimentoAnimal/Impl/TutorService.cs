using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IAnimalRepository _animalRepository; // Para validar animais
        private readonly IMapper _mapper;
        private readonly AppDbContext _context; // Para transações

        public TutorService(ITutorRepository tutorRepository, IAnimalRepository animalRepository, IMapper mapper, AppDbContext context)
        {
            _tutorRepository = tutorRepository;
            _animalRepository = animalRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<GetTutoresClinicaDto>> GetAllByClinicIdAsync(int clinicaId)
        {
            var tutores = await _tutorRepository.GetAllByClinicIdWithIncludesAsync(clinicaId);
            return _mapper.Map<IEnumerable<GetTutoresClinicaDto>>(tutores);
        }

        public async Task<GetTutoresClinicaDto?> GetByIdAsync(int tutorId, int clinicaId)
        {
            var tutor = await _tutorRepository.GetByIdWithIncludesAsync(tutorId, clinicaId);
            return _mapper.Map<GetTutoresClinicaDto>(tutor);
        }

        public async Task<ServiceResult> CreateAsync(CadastrarTutorClinicaDto dto, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lógica para reutilizar tutor existente ou criar um novo
                var tutor = await _tutorRepository.FindByCpfAsync(dto.Cpf);
                if (tutor == null)
                {
                    // Tutor não existe no sistema, cria um novo
                    tutor = _mapper.Map<Tutor>(dto);
                    tutor.DataCad = DateTime.UtcNow;
                    await _tutorRepository.AddAsync(tutor);
                    await _tutorRepository.SaveChangesAsync(); // Salva para obter o ID
                }
                else
                {
                    // Tutor já existe, apenas atualiza seus dados com as novas informações
                    _mapper.Map(dto, tutor);
                }

                // Verifica se este tutor já está vinculado a esta clínica
                if (await _tutorRepository.IsTutorLinkedToClinicAsync(tutor.Id, clinicaId))
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult { Succeeded = false, Message = "Este tutor (CPF) já está cadastrado nesta clínica." };
                }

                // Cria o vínculo com a clínica
                await _tutorRepository.AddClinicLinkAsync(new TutorClinica { TutorId = tutor.Id, ClinicaId = clinicaId });

                // Lógica de vínculo com animais (se fornecido)
                if (dto.AnimalId != null && dto.AnimalId.Any())
                {
                    // Validação e vínculo dos animais aqui...
                }

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult { Succeeded = true, Message = "Tutor cadastrado com sucesso.", Data = new { TutorId = tutor.Id } };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar 'ex'
                return new ServiceResult { Succeeded = false, Message = "Erro ao cadastrar tutor." };
            }
        }

        public async Task<ServiceResult> UpdateAsync(EditarTutorClinicaDto dto, int clinicaId)
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

                // 1. MAPEAMENTO AUTOMÁTICO
                // O AutoMapper aplica as alterações do DTO na entidade existente,
                // ignorando propriedades nulas (se o Profile estiver configurado para isso).
                _mapper.Map(dto, tutor);

                // 2. SINCRONIZAÇÃO DE VÍNCULOS
                if (dto.AnimalIds != null)
                {
                    // 2a. VALIDAÇÃO OTIMIZADA
                    // Valida todos os animais com uma única chamada ao banco.
                    var allAnimalsAreValid = await _animalRepository.AllAnimalsExistInClinicAsync(dto.AnimalIds, clinicaId);
                    if (!allAnimalsAreValid)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult { Succeeded = false, Message = "Um ou mais animais selecionados não são válidos ou não pertencem à clínica." };
                    }

                    var idsAnimaisAtuais = tutor.TutoresAnimais.Select(ta => ta.AnimalId).ToList();
                    var idsAnimaisDesejados = dto.AnimalIds;

                    // 2b. Remove vínculos
                    var vinculosParaRemover = tutor.TutoresAnimais.Where(ta => !idsAnimaisDesejados.Contains(ta.AnimalId)).ToList();
                    if (vinculosParaRemover.Any())
                    {
                        _tutorRepository.RemoveAnimalLinks(vinculosParaRemover);
                    }

                    // 2c. Adiciona novos vínculos
                    var idsParaAdicionar = idsAnimaisDesejados.Except(idsAnimaisAtuais).ToList();
                    foreach (var animalId in idsParaAdicionar)
                    {
                        await _tutorRepository.AddAnimalLinkAsync(new TutorAnimal { TutorId = tutor.Id, AnimalId = animalId, DataAssociacao = DateTime.UtcNow });
                    }
                }

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

        public async Task<ServiceResult> SetTutorStatusAsync(int tutorId, int clinicaId, bool ativo)
        {
            var link = await _tutorRepository.GetClinicLinkAsync(tutorId, clinicaId);
            if (link == null)
            {
                return new ServiceResult
                {
                    Succeeded = false,
                    Message = "Tutor não vinculado a esta clínica."
                };
            }

            link.IsAtivo = ativo;
            if (ativo)
            {
                link.DataAssociacao = DateTime.UtcNow;
                link.DataDesativacao = null;
            }
            else link.DataDesativacao = DateTime.UtcNow;

            await _tutorRepository.UpdateClinicLinkAsync(link);

            return new ServiceResult
            {
                Succeeded = true,
                Message = ativo ? "Tutor reativado com sucesso." : "Tutor desativado com sucesso."
            };
        }

        public async Task<ServiceResult> DeleteAsync(int tutorId, int clinicaId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tutor = await _tutorRepository.GetByIdWithIncludesAsync(tutorId, clinicaId);
                if (tutor == null)
                    return new ServiceResult { Succeeded = false, Message = "Tutor não encontrado nesta clínica." };

                // Remove todos os vínculos Tutor-Animal na clínica atual
                var vinculosAnimaisParaRemover = tutor.TutoresAnimais
                    .Where(ta => ta.Animal.AnimaisClinicas.Any(ac => ac.ClinicaId == clinicaId))
                    .ToList();
                if (vinculosAnimaisParaRemover.Any())
                {
                    _tutorRepository.RemoveAnimalLinks(vinculosAnimaisParaRemover);
                }

                // Remove o vínculo Tutor-Clínica
                var vinculoClinica = tutor.TutoresClinicas.First(tc => tc.ClinicaId == clinicaId);
                _tutorRepository.RemoveClinicLink(vinculoClinica);

                // Se o tutor não tiver mais nenhum vínculo com NENHUMA clínica, remove o tutor.
                if (tutor.TutoresClinicas.Count <= 1)
                {
                    _tutorRepository.Remove(tutor);
                }

                await _tutorRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult { Succeeded = true, Message = "Tutor removido da clínica com sucesso." };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logar 'ex'
                return new ServiceResult { Succeeded = false, Message = "Erro ao remover tutor." };
            }
        }
    }
}
