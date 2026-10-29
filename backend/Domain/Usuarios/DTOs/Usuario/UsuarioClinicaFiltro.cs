using backend.Domain.Utils.Paginacao;

namespace backend.Domain.Usuarios.DTOs.Usuario
{
    /// <summary>
    /// Filtro da listagem de usuários (vínculos) da clínica. Busca por nome, e-mail ou CPF.
    /// Ordenação aceita: "dataCadastro" (padrão), "nome".
    /// </summary>
    public class UsuarioClinicaFiltro : FiltroBase
    {
        /// <summary>true = vínculo ativo (sem o perfil INATIVO); false = vínculo inativo.</summary>
        public bool? Ativo { get; set; }

        /// <summary>Perfil do vínculo (GESTOR, VETERINARIO, SECRETARIO, TECNICO).</summary>
        public string? Perfil { get; set; }
    }
}
