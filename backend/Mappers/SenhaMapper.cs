using backend.Dtos.Senha;
using backend.Models;

namespace backend.Mappers
{
    public static class SenhaMapper
    {
        public static SenhaDto ToSenhaDto(this Senha senhaModel)
        {
            return new SenhaDto
            {
                Id = senhaModel.Id,
                SenhaHash = senhaModel.SenhaHash,
                Salt = senhaModel.Salt,
                UsuarioId = senhaModel.UsuarioId
            };
        }

        public static Senha ToSenhaFromDto(this SenhaDto senhaDto)
        {
            return new Senha
            {
                Id = senhaDto.Id,
                SenhaHash = senhaDto.SenhaHash,
                Salt = senhaDto.Salt,
                UsuarioId = senhaDto.UsuarioId
            };
        }
    }
}
