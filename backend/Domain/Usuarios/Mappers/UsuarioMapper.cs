using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Usuarios.DTOs.PerfilPessoal;
using backend.Domain.Usuarios.DTOs.Usuario;
using backend.Domain.Usuarios.Entities;
using Riok.Mapperly.Abstractions;

namespace backend.Domain.Usuarios.Mappers
{
    /// <summary>
    /// Conversões do usuário e dos vínculos com clínicas. O código é gerado na compilação pelo Mapperly.
    /// <c>AllowNullPropertyAssignment = false</c> faz a edição parcial: campo nulo no DTO mantém o valor atual.
    /// Senha e perfis nunca passam por aqui: senha é do Identity e perfis são do vínculo, ambos tratados no service.
    /// </summary>
    [Mapper(AllowNullPropertyAssignment = false)]
    public static partial class UsuarioMapper
    {
        #region Escrita

        /// <summary>Cadastro pelo admin do sistema. UserName, datas, senha, clínicas e perfis são definidos pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(CadastrarUsuarioSistemaDto.Senha))]
        [MapperIgnoreSource(nameof(CadastrarUsuarioSistemaDto.ClinicaIds))]
        [MapperIgnoreSource(nameof(CadastrarUsuarioSistemaDto.Perfis))]
        public static partial Usuario ParaEntidade(CadastrarUsuarioSistemaDto dto);

        /// <summary>Cadastro pelo gestor da clínica. UserName, datas, senha e perfis são definidos pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(CadastrarUsuarioClinicaDto.Senha))]
        [MapperIgnoreSource(nameof(CadastrarUsuarioClinicaDto.Perfis))]
        public static partial Usuario ParaEntidade(CadastrarUsuarioClinicaDto dto);

        /// <summary>
        /// Edição pelo admin: aplica os dados pessoais que vieram preenchidos.
        /// IsAdmin e IsAtivo também são copiados, mas o service restaura os valores anteriores (status muda pela rota de status).
        /// </summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(EditarUsuarioSistemaDto.Id))]
        [MapperIgnoreSource(nameof(EditarUsuarioSistemaDto.UpcId))]
        [MapperIgnoreSource(nameof(EditarUsuarioSistemaDto.Perfis))]
        [MapperIgnoreSource(nameof(EditarUsuarioSistemaDto.Senha))]
        public static partial void Aplicar(EditarUsuarioSistemaDto dto, [MappingTarget] Usuario usuario);

        /// <summary>Meu perfil: aplica os dados pessoais que vieram preenchidos. A troca de senha é feita pelo service.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Source)]
        [MapperIgnoreSource(nameof(UpdateMeuPerfilDto.SenhaAtual))]
        [MapperIgnoreSource(nameof(UpdateMeuPerfilDto.NovaSenha))]
        public static partial void Aplicar(UpdateMeuPerfilDto dto, [MappingTarget] Usuario usuario);

        #endregion

        #region Leitura

        /// <summary>Meu perfil (precisa de UsuariosPerfisClinicas carregado para o total de clínicas).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(GetMeuPerfilDto.TotalClinicasVinculadas), Use = nameof(TotalDeClinicas))]
        public static partial GetMeuPerfilDto ParaMeuPerfil(Usuario usuario);

        /// <summary>Linha da lista de usuários do sistema para um admin sem vínculo com clínica.</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(GetUsuariosSistemaDto.CompositeKey), Use = nameof(ChaveDeAdmin))]
        [MapPropertyFromSource(nameof(GetUsuariosSistemaDto.Perfis), Use = nameof(PerfisDeAdmin))]
        [MapValue(nameof(GetUsuariosSistemaDto.UpcId), -1)]
        [MapValue(nameof(GetUsuariosSistemaDto.ClinicaId), -1)]
        [MapValue(nameof(GetUsuariosSistemaDto.NomeClinica), "N/A")]
        public static partial GetUsuariosSistemaDto ParaLinhaDoSistema(Usuario usuario);

        /// <summary>
        /// Linha da lista de usuários do sistema para um vínculo (precisa de Usuario e Clinica carregados).
        /// Cada linha é um vínculo: o status mostrado é o do vínculo.
        /// </summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(GetUsuariosSistemaDto.CompositeKey), Use = nameof(ChaveDeVinculo))]
        [MapPropertyFromSource(nameof(GetUsuariosSistemaDto.IsAtivo), Use = nameof(VinculoAtivo))]
        [MapPropertyFromSource(nameof(GetUsuariosSistemaDto.Perfis), Use = nameof(PerfisDoVinculo))]
        [MapProperty(nameof(UsuarioPerfilClinica.Id), nameof(GetUsuariosSistemaDto.UpcId))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Clinica.NomeClinica), nameof(GetUsuariosSistemaDto.NomeClinica))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Id), nameof(GetUsuariosSistemaDto.Id))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.UserName), nameof(GetUsuariosSistemaDto.UserName))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Email), nameof(GetUsuariosSistemaDto.Email))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Nome), nameof(GetUsuariosSistemaDto.Nome))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.DataCadastro), nameof(GetUsuariosSistemaDto.DataCadastro))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.DataUltimoAcesso), nameof(GetUsuariosSistemaDto.DataUltimoAcesso))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.IsAdmin), nameof(GetUsuariosSistemaDto.IsAdmin))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Cpf), nameof(GetUsuariosSistemaDto.Cpf))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Formacao), nameof(GetUsuariosSistemaDto.Formacao))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Telefone), nameof(GetUsuariosSistemaDto.Telefone))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Whatsapp), nameof(GetUsuariosSistemaDto.Whatsapp))]
        public static partial GetUsuariosSistemaDto ParaLinhaDoSistema(UsuarioPerfilClinica vinculo);

        /// <summary>Linha da lista de usuários da clínica (precisa de Usuario carregado).</summary>
        [MapperRequiredMapping(RequiredMappingStrategy.Target)]
        [MapPropertyFromSource(nameof(GetUsuariosClinicaDto.IsAtivoNaClinica), Use = nameof(VinculoAtivo))]
        [MapProperty(nameof(UsuarioPerfilClinica.Id), nameof(GetUsuariosClinicaDto.UpcId))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Id), nameof(GetUsuariosClinicaDto.Id))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.UserName), nameof(GetUsuariosClinicaDto.UserName))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Email), nameof(GetUsuariosClinicaDto.Email))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Nome), nameof(GetUsuariosClinicaDto.Nome))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.DataCadastro), nameof(GetUsuariosClinicaDto.DataCadastro))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.DataUltimoAcesso), nameof(GetUsuariosClinicaDto.DataUltimoAcesso))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Cpf), nameof(GetUsuariosClinicaDto.Cpf))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Formacao), nameof(GetUsuariosClinicaDto.Formacao))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Telefone), nameof(GetUsuariosClinicaDto.Telefone))]
        [MapProperty(nameof(@UsuarioPerfilClinica.Usuario.Whatsapp), nameof(GetUsuariosClinicaDto.Whatsapp))]
        public static partial GetUsuariosClinicaDto ParaLinhaDaClinica(UsuarioPerfilClinica vinculo);

        #endregion

        private static int TotalDeClinicas(Usuario usuario) => usuario.UsuariosPerfisClinicas?.Count ?? 0;

        private static string ChaveDeAdmin(Usuario usuario) => $"admin-{usuario.Id}";

        private static List<string> PerfisDeAdmin(Usuario usuario) =>
            usuario.IsAtivo ? new List<string> { "ADMIN" } : new List<string> { "ADMIN", "INATIVO" };

        private static string ChaveDeVinculo(UsuarioPerfilClinica vinculo) => $"link-{vinculo.Id}";

        private static List<string> PerfisDoVinculo(UsuarioPerfilClinica vinculo) =>
            vinculo.Perfis?.ToList() ?? new List<string>();

        private static bool VinculoAtivo(UsuarioPerfilClinica vinculo) =>
            !(vinculo.Perfis != null && vinculo.Perfis.Contains("INATIVO"));
    }
}
