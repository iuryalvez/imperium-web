using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using backend.Domain.AtendimentoAnimal.DTOs.Animal;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Infra.Data;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly AppDbContext _context;

        // Lista branca de campos de ordenação aceitos em AnimalFiltro.OrdenarPor
        private static readonly Dictionary<string, Expression<Func<Animal, object?>>> Ordenacoes = new()
        {
            ["dataCadastro"] = a => a.DataCadastro,
            ["nome"] = a => a.Nome,
            ["especie"] = a => a.Especie,
        };

        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Animal?> GetByIdAsync(int animalId)
        {
            return await _context.Animais
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task<Animal?> GetByIdAndClinicaIdWithIncludesAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                // Inclui a coleção de vínculos com tutores na consulta
                .Include(a => a.TutoresAnimais)
                // Inclui o acompanhamento para o detalhe devolver o acompanhamentoId
                .Include(a => a.Acompanhamento)
                // Garante que o animal pertence à clínica especificada
                .Where(a => a.ClinicaId == clinicaId)
                // Busca o animal específico pelo ID
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task<PaginaResultado<Animal>> ListarDaClinicaAsync(AnimalFiltro filtro, int clinicaId)
        {
            var query = _context.Animais
                .AsNoTracking()
                .AsSplitQuery() // evita multiplicar linhas com a coleção de tutores ao paginar
                .Include(a => a.TutoresAnimais!).ThenInclude(ta => ta.Tutor)
                .Include(a => a.Acompanhamento) // Incluindo o Acompanhamento Individualizado
                .Include(a => a.Raca) // Incluindo a Raça
                .Where(a => a.ClinicaId == clinicaId);

            if (filtro.Ativo.HasValue) query = query.Where(a => a.IsAtivo == filtro.Ativo.Value);
            if (filtro.Especie.HasValue) query = query.Where(a => a.Especie == filtro.Especie.Value);
            if (filtro.TutorId.HasValue) query = query.Where(a => a.TutoresAnimais!.Any(ta => ta.TutorId == filtro.TutorId.Value));

            var busca = filtro.BuscaNormalizada;
            if (busca != null)
            {
                var padrao = QueryablePaginacaoExtensions.PadraoBusca(busca);
                query = query.Where(a => EF.Functions.ILike(a.Nome!, padrao)
                                      || a.TutoresAnimais!.Any(ta => EF.Functions.ILike(ta.Tutor!.Nome, padrao)));
            }

            return await query.Ordenar(filtro, Ordenacoes, "dataCadastro", a => a.Id).PaginarAsync(filtro);
        }

        public async Task<List<OpcaoDto>> ListarOpcoesAsync(int clinicaId, string? busca)
        {
            var query = _context.Animais.AsNoTracking().Where(a => a.ClinicaId == clinicaId);
            if (!string.IsNullOrWhiteSpace(busca))
                query = query.Where(a => EF.Functions.ILike(a.Nome!, QueryablePaginacaoExtensions.PadraoBusca(busca.Trim())));

            return await query
                .OrderByDescending(a => a.IsAtivo).ThenBy(a => a.Nome)
                .Take(OpcaoDto.LimiteOpcoes)
                .Select(a => new OpcaoDto
                {
                    Id = a.Id,
                    Nome = a.Nome + (a.Especie == EspecieAnimal.Cao ? " (Cão)" : " (Gato)"),
                    IsAtivo = a.IsAtivo
                })
                .ToListAsync();
        }

        public async Task<bool> IsAnimalInClinicAsync(int animalId, int clinicaId)
        {
            // Verifica se o animal pertence à clínica e está ATIVO
            return await _context.Animais
                .AnyAsync(a =>
                    a.Id == animalId &&
                    a.ClinicaId == clinicaId &&
                    a.IsAtivo);
        }

        public async Task AddTutorLinkAsync(TutorAnimal link)
        {
            await _context.TutoresAnimais.AddAsync(link);
        }

        public async Task<bool> AllAnimalsExistInClinicAsync(List<int> animalIds, int clinicaId)
        {
            if (animalIds == null || !animalIds.Any())
            {
                return true; // Lista vazia é considerada válida
            }

            // Conta quantos dos IDs fornecidos pertencem à clínica
            var validAnimalsCount = await _context.Animais
                .AsNoTracking()
                .CountAsync(a => a.ClinicaId == clinicaId && animalIds.Contains(a.Id));

            // A validação passa se a contagem for igual ao número de IDs únicos que recebemos.
            return validAnimalsCount == animalIds.Distinct().Count();
        }

        public async Task<bool> AllAnimalsActiveAsync(List<int> animalIds, int clinicaId)
        {
            if (animalIds == null || !animalIds.Any()) return true;

            var ativos = await _context.Animais
                .AsNoTracking()
                .CountAsync(a => a.ClinicaId == clinicaId && a.IsAtivo && animalIds.Contains(a.Id));
            return ativos == animalIds.Distinct().Count();
        }

        public void RemoveTutorLinks(IEnumerable<TutorAnimal> links)
        {
            _context.TutoresAnimais.RemoveRange(links);
        }

        public async Task<Animal?> GetHistoricoCompletoAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                // Carrega dados diretos do animal
                .Include(a => a.Raca)

                // Carrega os tutores através da tabela de junção
                .Include(a => a.TutoresAnimais!)
                    .ThenInclude(ta => ta.Tutor)

                // Carrega o acompanhamento
                .Include(a => a.Acompanhamento)

                // Carrega as consultas do animal (a linha do tempo)
                .Include(a => a.Consultas!.OrderByDescending(c => c.DataConsulta))
                    .ThenInclude(c => c.Avaliacao)
                        .ThenInclude(av => av!.Dieta)
                            .ThenInclude(d => d!.Racao)

                // Carrega os registros de peso semanais da dieta (emagrecimento)
                .Include(a => a.Consultas!)
                    .ThenInclude(c => c.Avaliacao)
                        .ThenInclude(av => av!.Dieta)
                            .ThenInclude(d => d!.RegistrosPeso)

                // A partir de cada consulta, carrega o usuário responsável
                .Include(a => a.Consultas)
                    .ThenInclude(c => c.Usuario)

                // Garante que estamos pegando o animal correto da clínica correta
                .Where(a => a.Id == animalId && a.ClinicaId == clinicaId)
                .FirstOrDefaultAsync();
        }

        public async Task<Animal?> GetByIdAndClinicIdWithDetailsAsync(int animalId, int clinicaId)
        {
            return await _context.Animais
                .Include(a => a.TutoresAnimais).ThenInclude(ta => ta.Tutor)
                .Include(a => a.Raca)
                .Where(a => a.ClinicaId == clinicaId)
                .FirstOrDefaultAsync(a => a.Id == animalId);
        }

        public async Task AddAsync(Animal animal)
        {
            await _context.Animais.AddAsync(animal);
        }

        public Task UpdateAsync(Animal animal)
        {
            _context.Animais.Update(animal);
            return Task.CompletedTask;
        }

        public async Task<bool> HasConsultasAsync(int animalId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(c => c.AnimalId == animalId);
        }

        public void Remove(Animal animal)
        {
            _context.Animais.Remove(animal);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}