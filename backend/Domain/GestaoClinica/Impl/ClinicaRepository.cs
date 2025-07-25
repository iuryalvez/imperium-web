using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interface;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.GestaoClinica.Impl
{
    public class ClinicaRepository : IClinicaRepository
    {
        private readonly AppDbContext _context;
        public ClinicaRepository(AppDbContext context) { _context = context; }

        public async Task<Clinica?> GetByIdAsync(int clinicaId) => await _context.Clinicas.FindAsync(clinicaId);

        public async Task<IEnumerable<GetClinicaDto>> GetAllAsync()
        {
            // O AsNoTracking() é uma ótima otimização para consultas de apenas leitura
            return await _context.Clinicas
                .AsNoTracking()
                .Select(c => new GetClinicaDto
                {
                    Id = c.Id,
                    NomeClinica = c.NomeClinica ?? string.Empty, // Garante que não será nulo
                    NomeFantasia = c.NomeFantasia,
                    Cnpj = c.Cnpj,
                    Endereco = c.Endereco,
                    EndNumero = c.EndNumero,
                    EndEstado = c.EndEstado,
                    EndCidade = c.EndCidade,
                    Cep = c.Cep,
                    Telefone1 = c.Telefone1,
                    Whatsapp = c.Whatsapp,
                    Email = c.Email,
                    Instagram = c.Instagram,
                    DataCad = c.DataCad,
                    IsAtiva = c.IsAtiva
                })
                .ToListAsync();
        }

        public async Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId)
        {
            return await _context.Clinicas.AnyAsync(c => c.Cnpj == cnpj && c.Id != excludeClinicaId);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}