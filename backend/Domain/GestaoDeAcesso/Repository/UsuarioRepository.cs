// Infra/Data/Repositories/UsuarioRepository.cs
using backend.Domain.GestaoDeAcesso.Entities;
using backend.Domain.GestaoDeAcesso.Interfaces;
using backend.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public UsuarioRepository(AppDbContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Usuario> GetUserByIdAsync(int userId)
    {
        return await _context.Usuarios.FindAsync(userId);
    }

    public async Task<IEnumerable<UsuarioPerfilClinica>> GetAllVinculosSistemaAsync()
    {
        return await _context.UsuariosPerfisClinicas
            .AsNoTracking()
            .Include(upc => upc.Usuario)
            .Include(upc => upc.Clinica)
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetUnlinkedAdminsAsync(List<int> linkedUserIds, int currentUserId)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.IsAdmin &&
                        !linkedUserIds.Contains(u.Id) &&
                        u.Id != currentUserId) // Exclui o usuário atual
            .ToListAsync();
    }

    public async Task<IEnumerable<UsuarioPerfilClinica>> GetVinculosByClinicIdAsync(int clinicaId)
    {
        return await _context.UsuariosPerfisClinicas
            .AsNoTracking()
            .Include(upc => upc.Usuario)
            .Where(upc => upc.ClinicaId == clinicaId)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<bool> EmailExistsInClinicAsync(string email, int clinicaId)
    {
        return await _context.UsuariosPerfisClinicas
            .AnyAsync(upc => upc.Usuario.Email == email && upc.ClinicaId == clinicaId);
    }

    public async Task<(IdentityResult, Usuario)> CreateUserAsync(Usuario user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return (result, user);
    }

    public async Task AddUserClinicLinkAsync(UsuarioPerfilClinica link)
    {
        await _context.UsuariosPerfisClinicas.AddAsync(link);
    }

    public async Task<bool> ClinicaExistsAsync(int clinicaId)
    {
        return await _context.Clinicas.AnyAsync(c => c.Id == clinicaId);
    }

    public async Task<Usuario> GetAdminUserByIdAsync(int userId)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId && u.IsAdmin);
    }

    public async Task<UsuarioPerfilClinica> GetUserClinicLinkByIdAsync(int upcId)
    {
        return await _context.UsuariosPerfisClinicas
            .Include(upc => upc.Usuario)
            .FirstOrDefaultAsync(upc => upc.Id == upcId);
    }

    public async Task<bool> EmailExistsForAnotherUserAsync(string email, int excludeUserId)
    {
        return await _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeUserId);
    }

    public async Task<IdentityResult> UpdateUserPasswordAsync(Usuario user, string newPassword)
    {
        // UserManager lida com o hash de forma segura
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<List<UsuarioPerfilClinica>> GetAllVinculosByUserIdAsync(int userId)
    {
        return await _context.UsuariosPerfisClinicas
            .Where(upc => upc.UsuarioId == userId)
            .ToListAsync();
    }
    
    public async Task<UsuarioPerfilClinica> GetVinculoByIdAsync(int upcId)
    {
        return await _context.UsuariosPerfisClinicas.FindAsync(upcId);
    }

    public Task RemoveVinculoAsync(UsuarioPerfilClinica vinculo)
    {
        _context.UsuariosPerfisClinicas.Remove(vinculo);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}