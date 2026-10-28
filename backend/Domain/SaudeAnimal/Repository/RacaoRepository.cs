using System.Linq.Expressions;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.SaudeAnimal.DTOs.Racao;
using backend.Domain.SaudeAnimal.Entities;
using backend.Domain.SaudeAnimal.Interfaces;
using backend.Domain.Utils.Paginacao;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.SaudeAnimal.Repository
{
    public class RacaoRepository : IRacaoRepository
    {
        private readonly AppDbContext _context;

        // Lista branca de campos de ordenação aceitos em RacaoFiltro.OrdenarPor
        private static readonly Dictionary<string, Expression<Func<Racao, object?>>> Ordenacoes = new()
        {
            ["dataCadastro"] = r => r.DataCadastro,
            ["nome"] = r => r.Nome,
            ["especie"] = r => r.Especie,
        };

        public RacaoRepository(AppDbContext context) { _context = context; }

        public async Task<PaginaResultado<Racao>> ListarDaClinicaAsync(RacaoFiltro filtro, int clinicaId)
        {
            var query = AplicarFiltro(_context.Racoes.AsNoTracking()
                .Where(r => r.ClinicaId == null || r.ClinicaId == clinicaId), filtro);

            if (filtro.Origem == "sistema") query = query.Where(r => r.ClinicaId == null);
            else if (filtro.Origem == "clinica") query = query.Where(r => r.ClinicaId == clinicaId);

            if (filtro.Oculta.HasValue)
                query = query.Where(r => r.OcultaEmClinicas!.Any(o => o.ClinicaId == clinicaId) == filtro.Oculta.Value);

            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", r => r.Id).PaginarAsync(filtro);
        }

        public async Task<List<OpcaoDto>> ListarOpcoesDaClinicaAsync(int clinicaId, string? busca, EspecieAnimal? especie)
        {
            // Sistema + próprias, sem as ocultas pela clínica; ativas primeiro
            var query = _context.Racoes.AsNoTracking()
                .Where(r => (r.ClinicaId == null && !r.OcultaEmClinicas!.Any(o => o.ClinicaId == clinicaId))
                         || r.ClinicaId == clinicaId);

            if (especie.HasValue) query = query.Where(r => r.Especie == especie.Value);
            if (!string.IsNullOrWhiteSpace(busca))
                query = query.Where(r => EF.Functions.ILike(r.Nome, QueryablePaginacaoExtensions.PadraoBusca(busca.Trim())));

            return await query
                .OrderByDescending(r => r.IsAtivo).ThenBy(r => r.Nome)
                .Take(OpcaoDto.LimiteOpcoes)
                .Select(r => new OpcaoDto { Id = r.Id, Nome = r.Nome, IsAtivo = r.IsAtivo })
                .ToListAsync();
        }

        public async Task<Racao?> GetAvailableByIdAndClinicIdAsync(int racaoId, int clinicaId)
        {
            // Instead of a simple FindAsync(id), this method adds a security layer:
            // the feed must be a system feed (ClinicaId null) or belong to the clinic. Feeds of other clinics are never returned.
            return await _context.Racoes
                .FirstOrDefaultAsync(r => r.Id == racaoId && (r.ClinicaId == null || r.ClinicaId == clinicaId));
        }

        public async Task<HashSet<int>> GetIdsOcultasAsync(int clinicaId, IEnumerable<int> racaoIds)
        {
            var ids = racaoIds.ToList();
            var ocultas = await _context.RacoesOcultasClinicas.AsNoTracking()
                .Where(o => o.ClinicaId == clinicaId && ids.Contains(o.RacaoId))
                .Select(o => o.RacaoId)
                .ToListAsync();
            return ocultas.ToHashSet();
        }

        public async Task<RacaoOcultaClinica?> GetOcultaAsync(int racaoId, int clinicaId) =>
            await _context.RacoesOcultasClinicas.FirstOrDefaultAsync(o => o.RacaoId == racaoId && o.ClinicaId == clinicaId);

        public async Task AddOcultaAsync(RacaoOcultaClinica oculta) => await _context.RacoesOcultasClinicas.AddAsync(oculta);

        public void RemoveOculta(RacaoOcultaClinica oculta) => _context.RacoesOcultasClinicas.Remove(oculta);

        public async Task<PaginaResultado<Racao>> ListarSistemaAsync(RacaoFiltro filtro)
        {
            var query = AplicarFiltro(_context.Racoes.AsNoTracking().Where(r => r.ClinicaId == null), filtro);
            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", r => r.Id).PaginarAsync(filtro);
        }

        public async Task<PaginaResultado<Racao>> ListarCopiasAsync(RacaoFiltro filtro)
        {
            // Rações próprias das clínicas (inclui as cópias feitas a partir das rações do sistema)
            var query = _context.Racoes.AsNoTracking().Include(r => r.Clinica).Where(r => r.ClinicaId != null);
            query = AplicarFiltro(query, filtro, buscarTambemClinica: true);
            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", r => r.Id).PaginarAsync(filtro);
        }

        public async Task<Racao?> GetByIdAsync(int racaoId) =>
            await _context.Racoes.Include(r => r.Clinica).FirstOrDefaultAsync(r => r.Id == racaoId);

        public async Task<bool> EmUsoAsync(int racaoId)
        {
            return await _context.Dietas.AsNoTracking().AnyAsync(d => d.RacaoId == racaoId)
                || await _context.Avaliacoes.AsNoTracking().AnyAsync(a => a.RacaoId == racaoId);
        }

        public async Task AddAsync(Racao racao) => await _context.Racoes.AddAsync(racao);

        public void Delete(Racao racao) => _context.Racoes.Remove(racao);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        private static IQueryable<Racao> AplicarFiltro(IQueryable<Racao> query, RacaoFiltro filtro, bool buscarTambemClinica = false)
        {
            if (filtro.Ativo.HasValue) query = query.Where(r => r.IsAtivo == filtro.Ativo.Value);
            if (filtro.Especie.HasValue) query = query.Where(r => r.Especie == filtro.Especie.Value);

            var busca = filtro.BuscaNormalizada;
            if (busca != null)
            {
                var padrao = QueryablePaginacaoExtensions.PadraoBusca(busca);
                query = buscarTambemClinica
                    ? query.Where(r => EF.Functions.ILike(r.Nome, padrao) || EF.Functions.ILike(r.Clinica!.NomeClinica!, padrao))
                    : query.Where(r => EF.Functions.ILike(r.Nome, padrao));
            }
            return query;
        }
    }
}
