using backend.Domain.Usuarios.DTOs.Painel;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Utils;

namespace backend.Domain.Usuarios.Services
{
    /// <summary>
    /// Monta o Meu Painel do usuário logado na clínica, com uma seção por perfil.
    /// TECNICO (e GESTOR sem outro perfil) recebem só os dados pessoais.
    /// </summary>
    public class PainelService : IPainelService
    {
        private readonly IPainelRepository _painelRepository;
        private readonly ILogger<PainelService> _logger;

        public PainelService(IPainelRepository painelRepository, ILogger<PainelService> logger)
        {
            _painelRepository = painelRepository;
            _logger = logger;
        }

        public async Task<ServiceResult> ObterMeuPainelAsync(int clinicaId, int usuarioId, List<string> perfis)
        {
            var usuario = await _painelRepository.GetUsuarioAsync(usuarioId);
            if (usuario == null)
            {
                _logger.LogWarning("Meu Painel: usuário {UsuarioId} não encontrado.", usuarioId);
                return new ServiceResult { Succeeded = false, Message = "Usuário não encontrado." };
            }

            var painel = new GetMeuPainelDto
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Formacao = usuario.Formacao,
                Telefone = usuario.Telefone,
                Whatsapp = usuario.Whatsapp,
                NomeClinica = await _painelRepository.GetNomeClinicaAsync(clinicaId),
                Perfis = perfis
            };

            if (perfis.Contains("SECRETARIO"))
                painel.ConsultasMarcadas = await _painelRepository.ResumirConsultasMarcadasAsync(clinicaId, usuarioId);

            if (perfis.Contains("VETERINARIO"))
                painel.Veterinario = await _painelRepository.ResumirVeterinarioAsync(clinicaId, usuarioId);

            return new ServiceResult { Succeeded = true, Data = painel };
        }
    }
}
