using System.Linq.Expressions;
using backend.Domain.AtendimentoAnimal.DTOs.Raca;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Utils.Paginacao;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class RacaRepository : IRacaRepository
    {
        private readonly AppDbContext _context;

        // Lista branca de campos de ordenação aceitos em RacaFiltro.OrdenarPor
        private static readonly Dictionary<string, Expression<Func<Raca, object?>>> Ordenacoes = new()
        {
            ["dataCadastro"] = r => r.DataCadastro,
            ["nome"] = r => r.Nome,
            ["especie"] = r => r.Especie,
        };

        public RacaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginaResultado<Raca>> ListarDaClinicaAsync(RacaFiltro filtro, int clinicaId)
        {
            var query = AplicarFiltro(_context.Racas.AsNoTracking()
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
            var query = _context.Racas.AsNoTracking()
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

        public async Task<Raca?> GetDisponivelParaClinicaAsync(int id, int clinicaId)
        {
            return await _context.Racas
                .FirstOrDefaultAsync(r => r.Id == id && (r.ClinicaId == null || r.ClinicaId == clinicaId));
        }

        public async Task<HashSet<int>> GetIdsOcultasAsync(int clinicaId, IEnumerable<int> racaIds)
        {
            var ids = racaIds.ToList();
            var ocultas = await _context.RacasOcultasClinicas.AsNoTracking()
                .Where(o => o.ClinicaId == clinicaId && ids.Contains(o.RacaId))
                .Select(o => o.RacaId)
                .ToListAsync();
            return ocultas.ToHashSet();
        }

        public async Task<RacaOcultaClinica?> GetOcultaAsync(int racaId, int clinicaId)
        {
            return await _context.RacasOcultasClinicas
                .FirstOrDefaultAsync(o => o.RacaId == racaId && o.ClinicaId == clinicaId);
        }

        public async Task AddOcultaAsync(RacaOcultaClinica oculta)
        {
            await _context.RacasOcultasClinicas.AddAsync(oculta);
        }

        public void RemoveOculta(RacaOcultaClinica oculta)
        {
            _context.RacasOcultasClinicas.Remove(oculta);
        }

        public async Task<PaginaResultado<Raca>> ListarSistemaAsync(RacaFiltro filtro)
        {
            var query = AplicarFiltro(_context.Racas.AsNoTracking().Where(r => r.ClinicaId == null), filtro);
            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", r => r.Id).PaginarAsync(filtro);
        }

        public async Task<PaginaResultado<Raca>> ListarCopiasAsync(RacaFiltro filtro)
        {
            // Raças próprias das clínicas (inclui as cópias feitas a partir das raças do sistema)
            var query = _context.Racas.AsNoTracking().Include(r => r.Clinica).Where(r => r.ClinicaId != null);
            query = AplicarFiltro(query, filtro, buscarTambemClinica: true);
            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", r => r.Id).PaginarAsync(filtro);
        }

        public async Task<Raca?> GetByIdAsync(int id)
        {
            return await _context.Racas.Include(r => r.Clinica).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> EmUsoAsync(int racaId)
        {
            return await _context.Animais.AsNoTracking().AnyAsync(a => a.RacaId == racaId);
        }

        public async Task AddAsync(Raca raca)
        {
            await _context.Racas.AddAsync(raca);
        }

        public void Delete(Raca raca)
        {
            _context.Racas.Remove(raca);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private static IQueryable<Raca> AplicarFiltro(IQueryable<Raca> query, RacaFiltro filtro, bool buscarTambemClinica = false)
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
