using backend.Interfaces;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.Perfil;

namespace backend.Repository
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly AppDbContext _context;
        public PerfilRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Perfil> CreateAsync(Perfil perfilModel)
        {
            await _context.Perfis.AddAsync(perfilModel);
            await _context.SaveChangesAsync();
            return perfilModel;
        }

        public async Task<Perfil?> DeleteAsync(int id)
        {
            var perfilModel = await _context.Perfis.FirstOrDefaultAsync(x => x.Id == id);
            if (perfilModel == null)
            {
                return null;
            }
            _context.Perfis.Remove(perfilModel);
            await _context.SaveChangesAsync();
            return perfilModel;
        }

        public async Task<List<Perfil>> GetAllAsync()
        {
            return await _context.Perfis.ToListAsync();
        }

        public async Task<Perfil?> GetByIdAsync(int id)
        {
            return await _context.Perfis.FindAsync(id);
        }

        public async Task<Perfil?> UpdateAsync(int id, UpdatePerfilRequestDto perfilDto)
        {
            var existingPerfil = await _context.Perfis.FirstOrDefaultAsync(x => x.Id == id);
            if (existingPerfil == null) 
            {
                return null;
            }
            existingPerfil.Nome = perfilDto.Nome;
            existingPerfil.Descricao = perfilDto.Descricao;
            existingPerfil.Tipo = perfilDto.Tipo;

            await _context.SaveChangesAsync();
            return existingPerfil;
        }
    }
}