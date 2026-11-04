using System.Linq.Expressions;
using backend.Domain.AtendimentoAnimal.DTOs.Tutor;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Domain.Utils.Paginacao;
using Microsoft.EntityFrameworkCore;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class TutorRepository : ITutorRepository
    {
        private readonly AppDbContext _context;

        // Lista branca de campos de ordenação aceitos em TutorFiltro.OrdenarPor
        private static readonly Dictionary<string, Expression<Func<Tutor, object?>>> Ordenacoes = new()
        {
            ["dataCadastro"] = t => t.DataCadastro,
            ["nome"] = t => t.Nome,
        };

        public TutorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CpfExistsInClinicAsync(string cpf, int clinicaId, int? excludeTutorId = null)
        {
            return await _context.Tutores
                .AsNoTracking()
                .AnyAsync(t => t.ClinicaId == clinicaId && t.Cpf == cpf && (excludeTutorId == null || t.Id != excludeTutorId));
        }

        public async Task<Tutor?> GetByIdAndClinicIdWithIncludesAsync(int tutorId, int clinicaId)
        {
            return await _context.Tutores
                // Inclui os vínculos com animais e o animal de cada vínculo (o DTO de detalhe lê id, nome e espécie)
                .Include(t => t.TutoresAnimais!)
                    .ThenInclude(ta => ta.Animal)
                // Garante que o tutor pertence à clínica especificada
                .Where(t => t.ClinicaId == clinicaId)
                // Busca o tutor específico pelo ID
                .FirstOrDefaultAsync(t => t.Id == tutorId);
        }

        public async Task<PaginaResultado<Tutor>> ListarDaClinicaAsync(TutorFiltro filtro, int clinicaId)
        {
            var query = _context.Tutores
                .AsNoTracking()
                .AsSplitQuery() // evita multiplicar linhas com a coleção de animais ao paginar
                // Inclui a tabela de junção Tutor-Animal e, a partir dela, o Animal
                .Include(t => t.TutoresAnimais!)
                    .ThenInclude(ta => ta.Animal)
                // Filtra apenas os tutores da clínica especificada
                .Where(t => t.ClinicaId == clinicaId);

            if (filtro.Ativo.HasValue) query = query.Where(t => t.IsAtivo == filtro.Ativo.Value);

            var busca = filtro.BuscaNormalizada;
            if (busca != null)
            {
                var padrao = QueryablePaginacaoExtensions.PadraoBusca(busca);
                // CPF/CNPJ só quando a busca não tem letras ("Tutor 1" não deve casar com qualquer documento que tenha "1")
                var digitos = busca.Any(char.IsLetter) ? "" : new string(busca.Where(char.IsDigit).ToArray());
                query = query.Where(t => EF.Functions.ILike(t.Nome, padrao)
                                      || EF.Functions.ILike(t.Email!, padrao)
                                      || (digitos.Length > 0 && t.Cpf.Contains(digitos)));
            }

            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", t => t.Id).PaginarAsync(filtro);
        }

        public async Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca, bool incluirCpf)
        {
            var query = _context.Tutores.AsNoTracking().Where(t => t.ClinicaId == clinicaId);
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var padrao = QueryablePaginacaoExtensions.PadraoBusca(busca.Trim());
                // CPF/CNPJ só quando a busca não tem letras ("Tutor 1" não deve casar com qualquer documento que tenha "1")
                var digitos = busca.Any(char.IsLetter) ? "" : new string(busca.Where(char.IsDigit).ToArray());
                // Sem permissão para ver o CPF, a busca também não usa o CPF (evita descobrir o documento por tentativa)
                query = query.Where(t => EF.Functions.ILike(t.Nome, padrao) || (incluirCpf && digitos.Length > 0 && t.Cpf.Contains(digitos)));
            }

            return await query
                .OrderByDescending(t => t.IsAtivo).ThenBy(t => t.Nome)
                .Take(OpcaoDto.LimiteOpcoes)
                .Select(t => new OpcaoDto { Id = t.Id, Nome = t.Nome + (incluirCpf && t.Cpf != "" ? " (" + t.Cpf + ")" : ""), IsAtivo = t.IsAtivo })
                .ToListAsync();
        }

        public async Task<bool> AllTutorsExistInClinicAsync(List<int> tutoresIds, int clinicaId)
        {
            if (tutoresIds == null || !tutoresIds.Any())
            {
                return true; // Se não há IDs para validar, a validação passa.
            }

            // Conta quantos dos IDs fornecidos pertencem à clínica
            var validTutorsCount = await _context.Tutores
                .AsNoTracking()
                .CountAsync(t => t.ClinicaId == clinicaId && tutoresIds.Contains(t.Id));

            // A validação é bem-sucedida se a contagem de tutores válidos for igual ao número
            // de IDs únicos que recebemos.
            return validTutorsCount == tutoresIds.Distinct().Count();
        }

        public async Task<bool> AllTutorsActiveAsync(List<int> tutoresIds, int clinicaId)
        {
            if (tutoresIds == null || !tutoresIds.Any()) return true;

            var ativos = await _context.Tutores
                .AsNoTracking()
                .CountAsync(t => t.ClinicaId == clinicaId && t.IsAtivo && tutoresIds.Contains(t.Id));
            return ativos == tutoresIds.Distinct().Count();
        }

        public async Task AddAsync(Tutor tutor)
        {
            await _context.Tutores.AddAsync(tutor);
        }

        public void Remove(Tutor tutor)
        {
            _context.Tutores.Remove(tutor);
        }

        public async Task<Tutor?> GetByIdAndClinicIdAsync(int tutorId, int clinicaId)
        {
            return await _context.Tutores
                .FirstOrDefaultAsync(t => t.Id == tutorId && t.ClinicaId == clinicaId);
        }

        public async Task<bool> HasConsultasAsync(int tutorId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(c => c.TutorId == tutorId);
        }

        public async Task AddAnimalLinkAsync(TutorAnimal link)
        {
            await _context.TutoresAnimais.AddAsync(link);
        }

        public void RemoveAnimalLinks(IEnumerable<TutorAnimal> links)
        {
            _context.TutoresAnimais.RemoveRange(links);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}