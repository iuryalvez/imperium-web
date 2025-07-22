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

                // 1. Mapeamento completo das propriedades do tutor
                tutor.Nome = dto.Nome.Trim();
                tutor.Cpf = dto.Cpf.Trim();
                tutor.Email = dto.Email.Trim();
                tutor.Endereco = dto.Endereco?.Trim();
                tutor.EndNumero = dto.EndNumero;
                tutor.EndEstado = dto.EndEstado?.Trim();
                tutor.EndCidade = dto.EndCidade?.Trim();
                tutor.Cep = dto.Cep?.Trim();
                tutor.Telefone1 = dto.Telefone1?.Trim();
                tutor.Whatsapp = dto.Whatsapp?.Trim();

                // 2. Sincronização dos vínculos com Animais
                if (dto.AnimalIds != null)
                {
                    var idsAnimaisAtuais = tutor.TutoresAnimais.Select(ta => ta.AnimalId).ToList();
                    var idsAnimaisDesejados = dto.AnimalIds;

                    // 2a. Validação: Garante que os animais desejados existem na clínica
                    foreach (var animalId in idsAnimaisDesejados)
                    {
                        if (!await _context.AnimaisClinicas.AnyAsync(ac => ac.AnimalId == animalId && ac.ClinicaId == clinicaId))
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResult { Succeeded = false, Message = $"Animal com ID {animalId} não é válido ou não pertence à clínica." };
                        }
                    }

                    // 2b. Remove vínculos que não são mais necessários
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

        public async Task<IEnumerable<GetTutoresClinicaDto>> GetAllByClinicIdAsync(int clinicaId)
        {
            var tutores = await _tutorRepository.GetAllByClinicIdWithIncludesAsync(clinicaId);

            // Mapeia a lista de entidades 'Tutor' para uma lista de 'GetTutoresClinicaDto'
            return tutores.Select(tutor => new GetTutoresClinicaDto
            {
                Id = tutor.Id,
                ClinicaId = clinicaId, // A clínica é o contexto da busca
                Nome = tutor.Nome,
                Cpf = tutor.Cpf,
                Endereco = tutor.Endereco,
                EndNumero = tutor.EndNumero,
                EndEstado = tutor.EndEstado,
                EndCidade = tutor.EndCidade,
                Cep = tutor.Cep,
                Telefone1 = tutor.Telefone1,
                Whatsapp = tutor.Whatsapp,
                Email = tutor.Email,
                DataCad = tutor.DataCad,

                // AQUI ESTÁ O MAPEAMENTO DOS ANIMAIS
                Animais = tutor.TutoresAnimais
                    .Where(ta => ta.Animal != null) // Garante que o animal não seja nulo
                    .Select(ta => new AnimalInfoSimplesDto
                    {
                        Id = ta.Animal.Id,
                        Nome = ta.Animal.Nome,
                        Especie = ta.Animal.Especie
                    }).ToList()
            }).ToList();
        }
    }
}
