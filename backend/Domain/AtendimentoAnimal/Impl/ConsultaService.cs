using AutoMapper;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Domain.GestaoDeAcesso.Interfaces;

namespace backend.Domain.AtendimentoAnimal.Impl
{
    public class ConsultaService : IConsultaService
    {
        private readonly IConsultaRepository _consultaRepository;
        private readonly IAnimalRepository _animalRepository; // Para validar o animal
        private readonly ITutorRepository _tutorRepository;   // Para validar o tutor
        private readonly IMapper _mapper;

        public ConsultaService(IConsultaRepository consultaRepository,
                               IAnimalRepository animalRepository,
                               ITutorRepository tutorRepository,
                               IMapper mapper)
        {
            _consultaRepository = consultaRepository;
            _animalRepository = animalRepository;
            _tutorRepository = tutorRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Busca todas as consultas de uma clínica.
        /// </summary>
        public async Task<IEnumerable<GetConsultaDto>> GetAllByClinicIdAsync(int clinicaId)
        {
            var consultas = await _consultaRepository.GetAllByClinicIdAsync(clinicaId);
            return _mapper.Map<IEnumerable<GetConsultaDto>>(consultas);
        }

        /// <summary>
        /// Busca os detalhes de uma consulta específica.
        /// </summary>
        public async Task<DetalhesConsultaDto?> GetDetailsByIdAsync(int consultaId, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAsync(consultaId, clinicaId);
            return _mapper.Map<DetalhesConsultaDto>(consulta);
        }

        public async Task<ServiceResult> CreateAsync(UpsertConsultaDto dto, int clinicaId, int usuarioId)
        {
            // Validações de Negócio
            if (await _animalRepository.GetByIdWithIncludesAsync(dto.AnimalId, clinicaId) == null)
                return new ServiceResult { Succeeded = false, Message = "Animal não encontrado ou não pertence a esta clínica." };
            if (await _tutorRepository.GetByIdWithIncludesAsync(dto.TutorId, clinicaId) == null)
                return new ServiceResult { Succeeded = false, Message = "Tutor não encontrado ou não pertence a esta clínica." };

            var consulta = _mapper.Map<Consulta>(dto);
            consulta.ClinicaId = clinicaId;
            consulta.UsuarioId = usuarioId;
            consulta.TutorIsCadastrado = true; // Já que validamos o ID

            await _consultaRepository.AddAsync(consulta);
            await _consultaRepository.SaveChangesAsync();

            var resultDto = _mapper.Map<DetalhesConsultaDto>(consulta);
            return new ServiceResult { Succeeded = true, Message = "Consulta agendada com sucesso.", Data = resultDto };
        }

        public async Task<ServiceResult> UpdateAsync(int consultaId, UpsertConsultaDto dto, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAsync(consultaId, clinicaId);
            if (consulta == null)
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };

            // Mapeia apenas os campos do DTO para a entidade existente
            _mapper.Map(dto, consulta);

            await _consultaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Consulta atualizada com sucesso." };
        }

        public async Task<ServiceResult> DeleteAsync(int consultaId, int clinicaId)
        {
            var consulta = await _consultaRepository.GetByIdAsync(consultaId, clinicaId);
            if (consulta == null)
            {
                return new ServiceResult { Succeeded = false, Message = "Consulta não encontrada." };
            }

            // Regra de Negócio Opcional: Impedir exclusão se já houver um relatório.
            if (consulta.Relatorio != null)
            {
                return new ServiceResult { Succeeded = false, Message = "Não é possível excluir uma consulta que já possui um relatório preenchido." };
            }

            _consultaRepository.Delete(consulta);
            await _consultaRepository.SaveChangesAsync();

            return new ServiceResult { Succeeded = true, Message = "Consulta excluída com sucesso." };
        }

        // Implementação dos outros métodos (GetAll, GetById, Delete)
        // ...
    }
}