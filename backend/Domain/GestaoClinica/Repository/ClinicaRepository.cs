using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interface;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.GestaoClinica.Services
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
                .Include(c => c.TutoresClinicas)     // Carrega a coleção de tutores
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
                    TotalAnimais = c.AnimaisClinicas != null ? c.AnimaisClinicas.Count() : 0,
                    TotalTutores = c.TutoresClinicas != null ? c.TutoresClinicas.Count() : 0,
                    TotalRacas = c.Racas != null ? c.Racas.Count() : 0,
                    TotalRacoes = c.Racoes != null ? c.Racoes.Count() : 0,
                    TotalConsultas = c.Consultas != null ? c.Consultas.Count() : 0,
                    // Counts reports by checking if the 1:1 consultation relation exists
                    TotalAvaliacoes= c.Consultas != null ? c.Consultas.Count(con => con.Avaliacao != null) : 0,
                    // Counts trackings by checking the 1:1 animal relation
                    TotalAcompanhamentos = c.AnimaisClinicas != null ? c.AnimaisClinicas.Count(ac => ac.Animal != null && ac.Animal.Acompanhamento != null) : 0
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Clinica>> GetAllAsync() =>
            await _context.Clinicas.AsNoTracking().ToListAsync();

        public async Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId)
        {
            return await _context.Clinicas.AnyAsync(c => c.Cnpj == cnpj && c.Id != excludeClinicaId);
        }

        public async Task AddAsync(Clinica clinica) => await _context.Clinicas.AddAsync(clinica);

        public void Delete(Clinica clinica) => _context.Clinicas.Remove(clinica);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}