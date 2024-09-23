using backend.Dtos.Usuario;
using backend.Models;
using Npgsql.Internal;

namespace backend.Mappers
{
    public static class UsuarioMappers
    {
        public static UsuarioDto ToUsuarioDto(this Usuario usuarioModel)
        {
            return new UsuarioDto
            {
                Id = usuarioModel.Id,
                Nome = usuarioModel.Nome,
                Email = usuarioModel.Email,
                DataUltimoAcesso = usuarioModel.DataUltimoAcesso,
                Formacao = usuarioModel.Formacao,
                Ativo = usuarioModel.Ativo
            };
        }
        public static Usuario ToUsuarioFromCreateDTO (this CreateUsuarioRequestDto usuarioDto) 
        {
            return new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email
            };
        }
    }
}