using backend.Dtos.Perfil;
using backend.Models;

namespace backend.Mappers
{
    public static class PerfilMappers
    {
        public static PerfilDto ToPerfilDto(this Perfil perfilModel)
        {
            return new PerfilDto
            {
                Id = perfilModel.Id,
                Nome = perfilModel.Nome,
                Descricao = perfilModel.Descricao,
                Tipo = perfilModel.Tipo,
                UsuariosPerfisClinicas = perfilModel.UsuariosPerfisClinicas.Select(upc => upc.ToUsuarioPerfilClinicaDto()).ToList()
            };
        }
        public static Perfil ToPerfilFromCreateDTO (this CreatePerfilRequestDto perfilDto) 
        {
            return new Perfil
            {
                Nome = perfilDto.Nome,
                Descricao = perfilDto.Descricao,
                Tipo = perfilDto.Tipo,
                UsuariosPerfisClinicas = perfilDto.UsuariosPerfisClinicas.Select(upc => upc.ToUsuarioPerfilClinicaFromDto()).ToList()
            };
        }
    }
}