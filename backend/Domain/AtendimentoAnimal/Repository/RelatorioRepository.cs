using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interface;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly AppDbContext _context;

        public RelatorioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Relatorio?> GetByIdAndAcompanhamentoIdAsync(int relatorioId, int acompanhamentoId)
        {
            return await _context.Relatorios
                .FirstOrDefaultAsync(r => r.Id == relatorioId && r.AcompanhamentoIndividualizadoId == acompanhamentoId);
        }

        public async Task<Relatorio?> GetByIdAsync(int relatorioId)
        {
            // Inclui a Racao para que o nome possa ser usado no DTO
            return await _context.Relatorios
                .Include(r => r.Racao)
                .FirstOrDefaultAsync(r => r.Id == relatorioId);
        }

        public async Task<Relatorio?> GetByConsultaIdAsync(int consultaId)
        {
            return await _context.Relatorios
                .Include(r => r.Racao)
                .FirstOrDefaultAsync(r => r.ConsultaId == consultaId);
        }

        public async Task<Relatorio?> GetByIdWithConsultaAsync(int relatorioId)
        {
            return await _context.Relatorios
                .Include(r => r.Consulta) // Inclui a consulta associada
                .FirstOrDefaultAsync(r => r.Id == relatorioId);
        }

        public async Task<Relatorio?> GetByIdWithAnimalAsync(int relatorioId)
        {
            // Busca o relatório e carrega a entidade Animal relacionada
            return await _context.Relatorios
                .Include(r => r.Consulta!.Animal)
                .FirstOrDefaultAsync(r => r.Id == relatorioId);
        }

        public async Task AddAsync(Relatorio relatorio)
        {
            await _context.Relatorios.AddAsync(relatorio);
        }

        public void Delete(Relatorio relatorio)
        {
            _context.Relatorios.Remove(relatorio);
        }
    }
}