using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Infra.Data;
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
                .Include(c => c.Animal)
                   .ThenInclude(a => a.AcompanhamentoIndividualizado)
                .Include(c => c.Tutor)
                .Include(c => c.Usuario) // Para o nome do veterinário
                .Include(c => c.Relatorio); // Para saber se já foi preenchido
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

        public async Task<IEnumerable<Consulta>> GetAllByClinicIdAsync(int clinicaId)
        {
            return await GetBaseQuery()
                .Where(c => c.ClinicaId == clinicaId)
                .AsNoTracking()
                .ToListAsync();
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