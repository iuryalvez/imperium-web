namespace backend.Domain.Utils
{
    /// <summary>
    /// Tipo da falha de um ServiceResult, para o controller escolher o status HTTP sem comparar a mensagem.
    /// Sem valor (nulo), a falha é de regra ou validação e responde 400.
    /// </summary>
    public enum TipoFalha
    {
        /// <summary>Registro inexistente ou de outra clínica: 404.</summary>
        NaoEncontrado = 1,

        /// <summary>Conflito com dado existente (ex.: CPF já cadastrado na clínica): 409.</summary>
        Conflito = 2,
    }
}
