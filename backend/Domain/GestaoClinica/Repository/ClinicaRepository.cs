using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.DTOs.Dashboard;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interfaces;
using backend.Domain.Utils.Paginacao;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace backend.Domain.GestaoClinica.Repository
{
    public class ClinicaRepository : IClinicaRepository
    {
        private readonly AppDbContext _context;
        public ClinicaRepository(AppDbContext context) { _context = context; }

        public async Task<Clinica?> GetByIdAsync(int clinicaId) => await _context.Clinicas.FindAsync(clinicaId);

        public async Task<Clinica?> GetByIdWithDetailsAsync(int clinicaId)
        {
            return await _context.Clinicas
                .Include(c => c.Racoes)              // Carrega a coleção de rações
                .Include(c => c.Consultas)           // Carrega a coleção de consultas
                .Include(c => c.Tutores)             // Carrega a coleção de tutores
                .Include(c => c.UsuariosPerfisClinicas)
                .FirstOrDefaultAsync(c => c.Id == clinicaId);
        }

        public async Task<GetClinicaDashboardDto?> GetClinicaDashboardAsync(int clinicaId)
        {
            // Projects the entity directly into the DTO to optimize database counts without fetching related collections
            return await _context.Clinicas
                .Where(c => c.Id == clinicaId)
                .Select(c => new GetClinicaDashboardDto
                {
                    Id = c.Id,
                    NomeClinica = c.NomeClinica,
                    NomeFantasia = c.NomeFantasia,
                    Cnpj = c.Cnpj,
                    Telefone = c.Telefone,
                    Email = c.Email,
                    Whatsapp = c.Whatsapp,
                    Instagram = c.Instagram,
                    DataCadastro = c.DataCadastro,
                    IsAtiva = c.IsAtiva,
                    LogoUrl = c.LogoUrl,
                    Cep = c.Cep,
                    Logradouro = c.Logradouro,
                    Complemento = c.Complemento,
                    Bairro = c.Bairro,
                    Cidade = c.Cidade,
                    Estado = c.Estado,
                    Uf = c.Uf,
                    TotalUsuarios = c.UsuariosPerfisClinicas != null ? c.UsuariosPerfisClinicas.Count() : 0,
                    TotalAnimais = c.Animais != null ? c.Animais.Count(a => a.IsAtivo) : 0,
                    TotalAnimaisInativos = c.Animais != null ? c.Animais.Count(a => !a.IsAtivo) : 0,
                    TotalTutores = c.Tutores != null ? c.Tutores.Count(t => t.IsAtivo) : 0,
                    TotalTutoresInativos = c.Tutores != null ? c.Tutores.Count(t => !t.IsAtivo) : 0,
                    TotalRacas = c.Racas != null ? c.Racas.Count() : 0,
                    TotalRacoes = c.Racoes != null ? c.Racoes.Count() : 0,
                    TotalConsultas = c.Consultas != null ? c.Consultas.Count() : 0,
                    // Counts reports by checking if the 1:1 consultation relation exists
                    TotalAvaliacoes= c.Consultas != null ? c.Consultas.Count(con => con.Avaliacao != null) : 0,
                    // Counts trackings by checking the 1:1 animal relation
                    TotalAcompanhamentos = c.Animais != null ? c.Animais.Count(a => a.Acompanhamento != null) : 0
                })
                .FirstOrDefaultAsync();
        }

        // Lista branca de campos de ordenação aceitos em ClinicaFiltro.OrdenarPor
        private static readonly Dictionary<string, Expression<Func<Clinica, object?>>> Ordenacoes = new()
        {
            ["dataCadastro"] = c => c.DataCadastro,
            ["nomeClinica"] = c => c.NomeClinica,
        };

        public async Task<PaginaResultado<Clinica>> ListarAsync(ClinicaFiltro filtro)
        {
            var query = _context.Clinicas.AsNoTracking();

            if (filtro.Ativa.HasValue) query = query.Where(c => c.IsAtiva == filtro.Ativa.Value);

            var busca = filtro.BuscaNormalizada;
            if (busca != null)
            {
                var padrao = QueryablePaginacaoExtensions.PadraoBusca(busca);
                // CPF/CNPJ só quando a busca não tem letras ("Tutor 1" não deve casar com qualquer documento que tenha "1")
                var digitos = busca.Any(char.IsLetter) ? "" : new string(busca.Where(char.IsDigit).ToArray());
                query = query.Where(c => EF.Functions.ILike(c.NomeClinica!, padrao)
                                      || EF.Functions.ILike(c.NomeFantasia!, padrao)
                                      || (digitos.Length > 0 && c.Cnpj != null && c.Cnpj.Contains(digitos)));
            }

            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", c => c.Id).PaginarAsync(filtro);
        }

        public async Task<List<OpcaoDto>> ListarOpcoesAsync(string? busca)
        {
            var query = _context.Clinicas.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(busca))
                query = query.Where(c => EF.Functions.ILike(c.NomeClinica!, QueryablePaginacaoExtensions.PadraoBusca(busca.Trim())));

            return await query
                .OrderByDescending(c => c.IsAtiva).ThenBy(c => c.NomeClinica)
                .Take(OpcaoDto.LimiteOpcoes)
                .Select(c => new OpcaoDto { Id = c.Id, Nome = c.NomeClinica ?? "", IsAtivo = c.IsAtiva })
                .ToListAsync();
        }

        public async Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId)
        {
            return await _context.Clinicas.AnyAsync(c => c.Cnpj == cnpj && c.Id != excludeClinicaId);
        }

        public async Task AddAsync(Clinica clinica) => await _context.Clinicas.AddAsync(clinica);

        public void Delete(Clinica clinica) => _context.Clinicas.Remove(clinica);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}