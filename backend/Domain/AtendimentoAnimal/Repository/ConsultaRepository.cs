using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Infra.Data;
using backend.Domain.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    // Infra/Data/Repositories/ConsultaRepository.cs
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly AppDbContext _context;
        public ConsultaRepository(AppDbContext context) { _context = context; }

        private IQueryable<Consulta> GetBaseQuery()
        {
            // Query base com todos os includes necessários para evitar N+1 queries
            return _context.Consultas
                // 1. Dados Básicos da Consulta
                .Include(c => c.Tutor)
                .Include(c => c.Usuario) // Veterinário

                // 2. Dados do Animal e suas sub-entidades (Raça e Acompanhamento)
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Raca)
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Acompanhamento)

                // 3. Dados do Avaliação e sua sub-entidade (Dieta)
                .Include(c => c.Avaliacao)
                    .ThenInclude(r => r.Dieta);
        }

        public async Task<Consulta?> GetByIdAsync(int consultaId)
        {
            return await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == consultaId);
        }

        public async Task<Consulta?> GetByIdWithDetailsAsync(int consultaId)
        {
            return await GetBaseQuery()
                .FirstOrDefaultAsync(c => c.Id == consultaId);
        }

        public async Task<PagedResponse<Consulta>> GetAllByClinicIdAsync(int clinicaId, int page, int pageSize, string? searchNome, DateTime? searchDataInicio, DateTime? searchDataFim)
        {
            var query = GetBaseQuery().Where(c => c.ClinicaId == clinicaId);

            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                query = query.Where(c => 
                    (c.Animal != null && c.Animal.Nome != null && c.Animal.Nome.ToLower().Contains(searchNome.ToLower())) ||
                    (c.Tutor != null && c.Tutor.Nome != null && c.Tutor.Nome.ToLower().Contains(searchNome.ToLower()))
                );
            }

            if (searchDataInicio.HasValue)
            {
                query = query.Where(c => c.DataConsulta.Date >= searchDataInicio.Value.Date);
            }

            if (searchDataFim.HasValue)
            {
                query = query.Where(c => c.DataConsulta.Date <= searchDataFim.Value.Date);
            }

            query = query.OrderByDescending(c => c.DataConsulta);

            return await query.AsNoTracking().ToPagedResponseAsync(page, pageSize);
        }

        public async Task<Consulta?> GetByIdAndClinicIdAsync(int consultaId, int clinicaId)
        {
            return await GetBaseQuery()
                .FirstOrDefaultAsync(c => c.Id == consultaId && c.ClinicaId == clinicaId);
        }

        public async Task AddAsync(Consulta consulta) => await _context.Consultas.AddAsync(consulta);
        public void Delete(Consulta consulta) => _context.Consultas.Remove(consulta);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}