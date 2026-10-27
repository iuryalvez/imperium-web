using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class AvaliacaoRepository : IAvaliacaoRepository
    {
        private readonly AppDbContext _context;

        public AvaliacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Avaliacao?> GetByIdAndAcompanhamentoIdAsync(int avaliacaoId, int acompanhamentoId)
        {
            return await _context.Avaliacoes
                .FirstOrDefaultAsync(r => r.Id == avaliacaoId && r.AcompanhamentoId == acompanhamentoId);
        }

        public async Task<Avaliacao?> GetByIdAsync(int avaliacaoId)
        {
            // Inclui a Racao para que o nome possa ser usado no DTO
            return await _context.Avaliacoes
                .Include(r => r.Racao)
                .FirstOrDefaultAsync(r => r.Id == avaliacaoId);
        }

        public async Task<Avaliacao?> GetByConsultaIdAsync(int consultaId)
        {
            return await _context.Avaliacoes
                .Include(r => r.Racao)
                .Include(r => r.Dieta)
                .Include(r => r.Consulta)
                    .ThenInclude(c => c.Animal)
                .FirstOrDefaultAsync(r => r.ConsultaId == consultaId);
        }

        public async Task<Avaliacao?> GetByIdWithConsultaAsync(int avaliacaoId)
        {
            return await _context.Avaliacoes
                .Include(r => r.Consulta) // Inclui a consulta associada
                .FirstOrDefaultAsync(r => r.Id == avaliacaoId);
        }

        public async Task<Avaliacao?> GetByIdWithAnimalAsync(int avaliacaoId)
        {
            // Busca o relatório e carrega a entidade Animal relacionada
            return await _context.Avaliacoes
                .Include(r => r.Consulta!.Animal)
                .FirstOrDefaultAsync(r => r.Id == avaliacaoId);
        }

        public async Task AddAsync(Avaliacao avaliacao)
        {
            await _context.Avaliacoes.AddAsync(avaliacao);
        }

        public void Delete(Avaliacao avaliacao)
        {
            _context.Avaliacoes.Remove(avaliacao);
        }
    }
}