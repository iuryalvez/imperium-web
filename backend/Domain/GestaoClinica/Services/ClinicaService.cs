using backend.Domain.Utils.Paginacao;
using backend.Infra.Data;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interfaces;
using Microsoft.EntityFrameworkCore;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.DTOs.Dashboard;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;
using backend.Domain.GestaoClinica.Mappers;

namespace backend.Domain.GestaoClinica.Services
{
    public class ClinicaService : IClinicaService
    {
        private readonly AppDbContext _context;
        private readonly IClinicaRepository _clinicaRepository;

        public ClinicaService(AppDbContext context,
                              IClinicaRepository clinicaRepository)
        {
            _context = context;
            _clinicaRepository = clinicaRepository;
        }

        /// <summary>Lista paginada das clínicas (admin), mais recentes primeiro por padrão.</summary>
        public async Task<PaginaResultado<GetClinicaDto>> ListarAsync(ClinicaFiltro filtro)
        {
            var pagina = await _clinicaRepository.ListarAsync(filtro);
            return pagina.Converter(itens => itens.Select(ClinicaMapper.ParaDto));
        }

        /// <summary>Opções para selects: até 20 clínicas { id, nome, isAtivo }, ativas primeiro.</summary>
        public async Task<List<OpcaoDto>> ListarOpcoesAsync(string? busca)
        {
            return await _clinicaRepository.ListarOpcoesAsync(busca);
        }

        public async Task<GetClinicaDto?> GetByIdAsync(int clinicaId)
        {
            var clinica = await _clinicaRepository.GetByIdAsync(clinicaId);
            return clinica == null ? null : ClinicaMapper.ParaDto(clinica);
        }

        public async Task<ServiceResult> GetClinicaDashboardAsync(int clinicaId)
        {
            var dashboardData = await _clinicaRepository.GetClinicaDashboardAsync(clinicaId);

            if (dashboardData == null)
            {
                return ServiceResult.NaoEncontrado("Clínica não encontrada.");
            }

            return new ServiceResult { Succeeded = true, Data = dashboardData };
        }

        public async Task<ServiceResult> CreateAsync(CadastrarClinicaDto dto)
        {
            var clinica = ClinicaMapper.ParaEntidade(dto);
            clinica.DataCadastro = DateTime.UtcNow;

            await _clinicaRepository.AddAsync(clinica);
            await _clinicaRepository.SaveChangesAsync();

            var resultDto = ClinicaMapper.ParaDto(clinica);
            return new ServiceResult { Succeeded = true, Message = "Clínica cadastrada com sucesso.", Data = resultDto };
        }

        public async Task<ServiceResult> UpdateClinicaAsync(EditarClinicaAdminDto dto)
        {
            var clinica = await _clinicaRepository.GetByIdAsync(dto.Id);
            if (clinica == null)
                return ServiceResult.NaoEncontrado("Clínica não encontrada.");

            if (!string.IsNullOrWhiteSpace(dto.Cnpj) && await _clinicaRepository.CnpjExistsForAnotherClinicAsync(dto.Cnpj, dto.Id))
                return ServiceResult.Conflito("Este CNPJ já está em uso.");

            ClinicaMapper.Aplicar(dto, clinica);
            await _clinicaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Clínica atualizada com sucesso." };
        }

        public async Task<ServiceResult> UpdateOwnClinicAsync(int clinicaId, EditarClinicaDto dto)
        {
            // Busca a clínica usando o ID do token, garantindo a segurança
            var clinica = await _clinicaRepository.GetByIdAsync(clinicaId);
            if (clinica == null)
                return ServiceResult.NaoEncontrado("Clínica não encontrada ou acesso não permitido.");

            // Validação de negócio (ex: CNPJ único)
            if (!string.IsNullOrWhiteSpace(dto.Cnpj) && await _clinicaRepository.CnpjExistsForAnotherClinicAsync(dto.Cnpj, clinicaId))
            {
                return ServiceResult.Conflito("Este CNPJ já está em uso por outra clínica.");
            }

            // Aplica na entidade só o que veio preenchido no DTO (ver ClinicaMapper)
            ClinicaMapper.Aplicar(dto, clinica);

            // Salva as alterações no banco de dados
            await _clinicaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Clínica atualizada com sucesso." };
        }

        public async Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId)
        {
            return await _context.Tutores
                .AnyAsync(t => t.ClinicaId == clinicaId && t.Cpf == cpf);
        }
        public async Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId)
        {
            return await _context.UsuariosPerfisClinicas
                .Include(upc => upc.Usuario)
                .AnyAsync(upc => upc.ClinicaId == clinicaId && upc.Usuario!.Cpf == cpf);
        }

        public async Task<List<Tutor>> ObterTutoresDaClinicaAsync(int clinicaId)
        {
            return await _context.Tutores
                .Where(t => t.ClinicaId == clinicaId)
                .ToListAsync();
        }

        public async Task<List<Animal>> ObterAnimaisDaClinicaAsync(int clinicaId)
        {
            return await _context.Animais
                .Where(a => a.ClinicaId == clinicaId)
                .ToListAsync();
        }

        public async Task<List<Animal>> ObterAnimaisDaClinicaComTutoresAsync(int clinicaId)
        {
            return await _context.Animais
                .AsNoTracking() // Use AsNoTracking para consultas de leitura, melhora o desempenho
                .Include(a => a.TutoresAnimais!) // Inclui a coleção de vínculos TutorAnimal
                    .ThenInclude(ta => ta.Tutor)   // Para cada TutorAnimal, inclui o Tutor
                .Where(a => a.ClinicaId == clinicaId)
                .ToListAsync();
        }

        public async Task<ServiceResult> DeleteAsync(int clinicaId)
        {
            var clinica = await _clinicaRepository.GetByIdWithDetailsAsync(clinicaId); // Necessário incluir dependentes para verificar
            if (clinica == null)
                return ServiceResult.NaoEncontrado("Clínica não encontrada.");

            if (clinica.UsuariosPerfisClinicas.Any()) // Lógica de verificação de dependentes
                return ServiceResult.Conflito("Clínica possui usuários vinculados.");

            _clinicaRepository.Delete(clinica);
            await _clinicaRepository.SaveChangesAsync();
            return new ServiceResult { Succeeded = true, Message = "Clínica excluída com sucesso." };
        }
    }
}