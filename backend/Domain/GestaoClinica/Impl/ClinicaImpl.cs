using backend.Infra.Data;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interface;
using Microsoft.EntityFrameworkCore;
using backend.Domain.AtendimentoAnimal.Entities;

public class ClinicaImpl : ClinicaInterface
{
    private readonly AppDbContext _context;

    public ClinicaImpl(AppDbContext context)
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

    public async Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaComTutoresAsync(int clinicaId)
    {
        return await _context.AnimaisClinicas
            .AsNoTracking() // Use AsNoTracking para consultas de leitura, melhora o desempenho
            .Include(ac => ac.Animal!)
                .ThenInclude(a => a.TutoresAnimais!) // Inclui a coleção de vínculos TutorAnimal
                    .ThenInclude(ta => ta.Tutor)   // Para cada TutorAnimal, inclui o Tutor
            .Where(ac => ac.ClinicaId == clinicaId && ac.Animal != null) // Garante que o animal existe
            .ToListAsync();
    }
}