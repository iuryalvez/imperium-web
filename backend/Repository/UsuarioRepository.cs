using backend.Interfaces;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.Usuario;

namespace backend.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> CreateAsync(Usuario usuarioModel)
        {
            await _context.Usuarios.AddAsync(usuarioModel);
            await _context.SaveChangesAsync();
            return usuarioModel;
        }

        public async Task<Usuario?> DeleteAsync(int id)
        {
            var usuarioModel = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuarioModel == null)
            {
                return null;
            }
            _context.Usuarios.Remove(usuarioModel);
            await _context.SaveChangesAsync();
            return usuarioModel;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> UpdateAsync(int id, UpdateUsuarioRequestDto usuarioDto)
        {
            var existingUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (existingUsuario == null) 
            {
                return null;
            }
            existingUsuario.Nome = usuarioDto.Nome;
            existingUsuario.Email = usuarioDto.Email;
            existingUsuario.Formacao = usuarioDto.Formacao;   
            existingUsuario.Ativo = usuarioDto.Ativo;
            existingUsuario.DataUltimoAcesso = usuarioDto.DataUltimoAcesso;

            await _context.SaveChangesAsync();
            return existingUsuario;
        }
    }
}