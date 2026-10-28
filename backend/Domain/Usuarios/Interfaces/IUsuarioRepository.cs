using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.Usuarios.Entities;
using Microsoft.AspNetCore.Identity;

namespace backend.Domain.Usuarios.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> GetUserByIdAsync(int userId);
        Task<Usuario> GetUserWithTotalClinicsById(int userId);
        Task<IEnumerable<Usuario>> GetUnlinkedAdminsAsync(List<int> linkedUserIds, int currentUserId);
        Task<IEnumerable<UsuarioPerfilClinica>> GetVinculosByClinicIdAsync(int clinicaId);
        Task<bool> EmailExistsAsync(string email);
        Task<Usuario?> GetUserByEmailAsync(string email);
        Task<bool> IsUserLinkedToClinicAsync(int userId, int clinicaId);
        Task<bool> EmailExistsInClinicAsync(string email, int clinicaId);
        Task<bool> EmailExistsForAnotherUserAsync(string email, int excludeUserId);
        Task<bool> EmailExistsInClinicForAnotherUserAsync(string email, int clinicaId, int userId);
        Task<(IdentityResult, Usuario)> CreateUserAsync(Usuario user, string password);
        Task<IdentityResult> ChangeUserPasswordAsync(Usuario user, string currentPassword, string newPassword);
        Task AddUserClinicLinkAsync(UsuarioPerfilClinica link);
        Task<bool> ClinicaExistsAsync(int clinicaId);
        Task<Usuario> GetAdminUserByIdAsync(int userId);
        Task<UsuarioPerfilClinica> GetUserClinicLinkByIdAsync(int upcId);
        Task<IdentityResult> UpdateUserPasswordAsync(Usuario user, string newPassword);
        Task<IEnumerable<UsuarioPerfilClinica>> GetAllVinculosSistemaAsync();
        Task<List<UsuarioPerfilClinica>> GetAllVinculosByUserIdAsync(int userId);
        Task<UsuarioPerfilClinica> GetVinculoByIdAsync(int upcId);
        Task RemoveVinculoAsync(UsuarioPerfilClinica vinculo);
        Task<int> SaveChangesAsync();
    }
}