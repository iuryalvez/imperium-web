using System.ComponentModel;
using backend.Data;
using backend.Dtos.Tutor;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

public class ClinicaService : IClinicaService
{
    private readonly AppDbContext _context;

    public ClinicaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Clinica?> ObterClinicaAsync(int clinicaId)
    {
        return await _context.Clinicas.FindAsync(clinicaId);
    }

    public async Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId)
    {
        return await _context.TutoresClinicas
            .Include(tc => tc.Tutor)
            .AnyAsync(tc => tc.ClinicaId == clinicaId && tc.Tutor!.Cpf == cpf);
    }
    public async Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId)
    {
        return await _context.UsuariosPerfisClinicas
            .Include(upc => upc.Usuario)
            .AnyAsync(upc => upc.ClinicaId == clinicaId && upc.Usuario!.Cpf == cpf);
    }

    public async Task<List<TutorClinica>> ObterVinculosTutoresDaClinicaAsync(int clinicaId)
    {
        return await _context.TutoresClinicas
            .Include(tc => tc.Tutor)
            .Where(tc => tc.ClinicaId == clinicaId)
            .ToListAsync();
    }

    public async Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaAsync(int clinicaId)
    {
        return await _context.AnimaisClinicas
            .Include(ac => ac.Animal)
            .Where(ac => ac.ClinicaId == clinicaId)
            .ToListAsync();
    }
}