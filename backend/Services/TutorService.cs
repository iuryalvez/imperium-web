using System.Security.Claims;
using backend.Data;
using backend.Dtos.Tutor;
using backend.Models;
using Microsoft.EntityFrameworkCore;

public class TutorService
{
    private readonly AppDbContext _context;

    public TutorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId)
    {
        return await _context.TutoresClinicas
            .Include(tc => tc.Tutor)
            .AnyAsync(tc => tc.ClinicaId == clinicaId && tc.Tutor!.Cpf == cpf);
    }
}
