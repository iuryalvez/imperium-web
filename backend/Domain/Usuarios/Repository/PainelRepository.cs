using backend.Domain.Usuarios.DTOs.Painel;
using backend.Domain.Usuarios.Entities;
using backend.Domain.Usuarios.Interfaces;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.Usuarios.Repository
{
    /// <summary>
    /// Contagens do Meu Painel, projetadas direto no banco. Toda consulta filtra pela clínica do token.
    /// </summary>
    public class PainelRepository : IPainelRepository
    {
        private readonly AppDbContext _context;

        public PainelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetUsuarioAsync(int usuarioId)
        {
            return await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);
        }

        public async Task<string?> GetNomeClinicaAsync(int clinicaId)
        {
            return await _context.Clinicas.AsNoTracking()
                .Where(c => c.Id == clinicaId)
                .Select(c => c.NomeClinica)
                .FirstOrDefaultAsync();
        }

        public async Task<ResumoConsultasDto> ResumirConsultasMarcadasAsync(int clinicaId, int usuarioId)
        {
            var consultas = _context.Consultas.AsNoTracking().Where(c => c.ClinicaId == clinicaId && c.UsuarioId == usuarioId);

            // Mesma regra de Consulta.Status (só o cancelamento é gravado)
            return new ResumoConsultasDto
            {
                Total = await consultas.CountAsync(),
                Canceladas = await consultas.CountAsync(c => c.DataCancelamento != null),
                Pendentes = await consultas.CountAsync(c => c.DataCancelamento == null && c.Avaliacao == null),
                EmAndamento = await consultas.CountAsync(c => c.DataCancelamento == null && c.Avaliacao != null && c.Avaliacao.Dieta == null),
                Concluidas = await consultas.CountAsync(c => c.DataCancelamento == null && c.Avaliacao != null && c.Avaliacao.Dieta != null),
            };
        }

        public async Task<ResumoVeterinarioDto> ResumirVeterinarioAsync(int clinicaId, int usuarioId)
        {
            var avaliacoesDoUsuario = _context.Avaliacoes.AsNoTracking()
                .Where(a => a.UsuarioResponsavelId == usuarioId && a.Consulta != null && a.Consulta.ClinicaId == clinicaId);

            return new ResumoVeterinarioDto
            {
                AvaliacoesPreenchidas = await avaliacoesDoUsuario.CountAsync(),
                ConsultasConcluidas = await avaliacoesDoUsuario.CountAsync(a => a.Dieta != null),
                ConsultasPendentesNaClinica = await _context.Consultas.AsNoTracking()
                    .CountAsync(c => c.ClinicaId == clinicaId && c.DataCancelamento == null && c.Avaliacao == null),
            };
        }
    }
}
