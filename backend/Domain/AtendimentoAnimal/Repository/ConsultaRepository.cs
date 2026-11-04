using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.Utils.Paginacao;
using backend.Domain.AtendimentoAnimal.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.AtendimentoAnimal.Repository
{
    // Infra/Data/Repositories/ConsultaRepository.cs
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly AppDbContext _context;
        public ConsultaRepository(AppDbContext context) { _context = context; }

        // Lista branca de campos de ordenação aceitos em ConsultaFiltro.OrdenarPor
        private static readonly Dictionary<string, Expression<Func<Consulta, object?>>> Ordenacoes = new()
        {
            ["dataConsulta"] = c => c.DataConsulta,
            ["nomeAnimal"] = c => c.Animal!.Nome,
        };

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
                    .ThenInclude(r => r.Dieta)

                // 4. Quem cancelou (quando cancelada)
                .Include(c => c.CanceladaPorUsuario);
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

        public async Task<PaginaResultado<Consulta>> ListarDaClinicaAsync(ConsultaFiltro filtro, int clinicaId, int? usuarioId)
        {
            var query = GetBaseQuery()
                .AsNoTracking()
                .Where(c => c.ClinicaId == clinicaId);

            if (filtro.AnimalId.HasValue) query = query.Where(c => c.AnimalId == filtro.AnimalId.Value);
            if (filtro.TutorId.HasValue) query = query.Where(c => c.TutorId == filtro.TutorId.Value);
            if (filtro.DataInicio.HasValue) query = query.Where(c => c.DataConsulta >= filtro.DataInicio.Value.ToUniversalTime());
            if (filtro.DataFim.HasValue) query = query.Where(c => c.DataConsulta <= filtro.DataFim.Value.ToUniversalTime());

            // "Minhas" consultas: sem usuário identificado no token, o filtro não devolve nada (nunca a lista inteira)
            if (filtro.MarcadasPorMim == true) query = query.Where(c => usuarioId != null && c.UsuarioId == usuarioId);
            if (filtro.AtendidasPorMim == true) query = query.Where(c => usuarioId != null && c.Avaliacao != null && c.Avaliacao.UsuarioResponsavelId == usuarioId);

            // Status calculado, traduzido para o banco (mesma regra de Consulta.Status)
            query = filtro.Status switch
            {
                StatusConsulta.Cancelada => query.Where(c => c.DataCancelamento != null),
                StatusConsulta.Concluida => query.Where(c => c.DataCancelamento == null && c.Avaliacao != null && c.Avaliacao.Dieta != null),
                StatusConsulta.EmAndamento => query.Where(c => c.DataCancelamento == null && c.Avaliacao != null && c.Avaliacao.Dieta == null),
                StatusConsulta.Pendente => query.Where(c => c.DataCancelamento == null && c.Avaliacao == null),
                _ => query
            };

            var busca = filtro.BuscaNormalizada;
            if (busca != null)
            {
                var padrao = QueryablePaginacaoExtensions.PadraoBusca(busca);
                query = query.Where(c => EF.Functions.ILike(c.Animal!.Nome!, padrao)
                                      || (c.Tutor != null && EF.Functions.ILike(c.Tutor.Nome, padrao)));
            }

            return await query.Ordenar(filtro, Ordenacoes, "dataConsulta", c => c.Id).PaginarAsync(filtro);
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