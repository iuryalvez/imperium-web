const API_BASE_URL = "/api";

const endpoints = {
  autenticacao: {
    checarStatus: `${API_BASE_URL}/auth/check`,
    login: `${API_BASE_URL}/auth/login`,
    logout: `${API_BASE_URL}/auth/logout`,
    recuperarSenha: `${API_BASE_URL}/auth/forgot-password`,
    resetPassword: `${API_BASE_URL}/auth/reset-password`,
    listarVinculos: `${API_BASE_URL}/auth/sistema`,
    escolherVinculo: (upcId) => `${API_BASE_URL}/auth/sistema/${upcId}`,
    escolherVinculoAdmin: `${API_BASE_URL}/auth/sistema/admin`,
    trocarVinculo: `${API_BASE_URL}/auth/sistema/trocar-vinculo`,
  },
  sistema: {
    clinica: {
      listar: `${API_BASE_URL}/sistema/clinicas`,
      opcoes: `${API_BASE_URL}/sistema/clinicas/opcoes`,
      // listarInativas: `${API_BASE_URL}/sistema/clinicas/inativas`, // TODO
      cadastrar: `${API_BASE_URL}/sistema/clinicas/cadastrar`,
      editar: `${API_BASE_URL}/sistema/clinicas/editar`,
      detalhes: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
      deletar: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
    },
    usuario: {
      buscarPorNome: (nome) =>
        `${API_BASE_URL}/sistema/usuarios/buscar?nome=${encodeURIComponent(
          nome,
        )}`,
      listar: `${API_BASE_URL}/sistema/usuarios`,
      cadastrar: `${API_BASE_URL}/sistema/usuarios/cadastrar`,
      editar: `${API_BASE_URL}/sistema/usuarios/editar`,
      setStatus: (usuarioId) =>
        `${API_BASE_URL}/sistema/usuarios/${usuarioId}/status`,
      // deletar: (usuarioId) => `${API_BASE_URL}/sistema/usuarios/${usuarioId}`, // acredito que funciona mas estamos usando o setStatus como soft delete por enquanto
      perfil: {
        check: `${API_BASE_URL}/sistema/perfil`,
        editar: `${API_BASE_URL}/sistema/perfil`,
      },
    },
    // Catálogo de raças do sistema (admin). As cópias são as raças próprias das clínicas.
    raca: {
      listar: `${API_BASE_URL}/sistema/racas`,
      copias: `${API_BASE_URL}/sistema/racas/copias`,
      cadastrar: `${API_BASE_URL}/sistema/racas/cadastrar`,
      detalhes: (racaId) => `${API_BASE_URL}/sistema/racas/${racaId}`,
      editar: `${API_BASE_URL}/sistema/racas/editar`,
      deletar: (racaId) => `${API_BASE_URL}/sistema/racas/${racaId}`,
      setStatus: (racaId) => `${API_BASE_URL}/sistema/racas/${racaId}/status`,
    },
    // Catálogo de rações do sistema (admin). As cópias são as rações próprias das clínicas.
    racao: {
      listar: `${API_BASE_URL}/sistema/racoes`,
      copias: `${API_BASE_URL}/sistema/racoes/copias`,
      cadastrar: `${API_BASE_URL}/sistema/racoes/cadastrar`,
      detalhes: (racaoId) => `${API_BASE_URL}/sistema/racoes/${racaoId}`,
      editar: `${API_BASE_URL}/sistema/racoes/editar`,
      deletar: (racaoId) => `${API_BASE_URL}/sistema/racoes/${racaoId}`,
      setStatus: (racaoId) => `${API_BASE_URL}/sistema/racoes/${racaoId}/status`,
    },
  },
  clinica: {
    editar: `${API_BASE_URL}/clinica/editar`,
    check: `${API_BASE_URL}/clinica/dashboard`, // Nova rota GET do dashboard
    meuPainel: `${API_BASE_URL}/clinica/meu-painel`, // Meu Painel do usuário logado (todos os perfis)
    acompanhamento: {
      getHistoricoCompleto: (acompanhamentoId) =>
        `${API_BASE_URL}/clinica/acompanhamentos/${acompanhamentoId}/historico-completo`,
      getPerfisMetabolicos: `${API_BASE_URL}/clinica/acompanhamentos/perfis-metabolicos`,
      getNiveisRestricao: `${API_BASE_URL}/clinica/acompanhamentos/niveis-restricao`,
      // Rota para criar uma dieta associada a um acompanhamento
      criarDieta: (acompanhamentoId) =>
        `${API_BASE_URL}/clinica/dietas`,
      iniciarDieta: (id) => `${API_BASE_URL}/clinica/dietas/${id}/iniciar`,
      concluirDieta: (id) => `${API_BASE_URL}/clinica/dietas/${id}/concluir`,
      registrarPeso: (id) => `${API_BASE_URL}/clinica/dietas/${id}/registros-peso`,
      deletarRegistroPeso: (id, registroId) => `${API_BASE_URL}/clinica/dietas/${id}/registros-peso/${registroId}`,
      editarDieta: (dietaId) =>
        `${API_BASE_URL}/clinica/dietas/${dietaId}`,
      deletarDieta: (dietaId) =>
        `${API_BASE_URL}/clinica/dietas/${dietaId}`,
    },
    animal: {
      listar: `${API_BASE_URL}/clinica/animais`,
      opcoes: `${API_BASE_URL}/clinica/animais/opcoes`,
      detalhes: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      cadastrar: `${API_BASE_URL}/clinica/animais/cadastrar`,
      editar: `${API_BASE_URL}/clinica/animais/editar`,
      deletar: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      setStatus: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}/status`,
    },
    // Raças vistas pela clínica: as do sistema + as próprias
    raca: {
      listar: `${API_BASE_URL}/clinica/racas`,
      opcoes: `${API_BASE_URL}/clinica/racas/opcoes`,
      cadastrar: `${API_BASE_URL}/clinica/racas/cadastrar`,
      detalhes: (racaId) => `${API_BASE_URL}/clinica/racas/${racaId}`,
      editar: `${API_BASE_URL}/clinica/racas/editar`,
      deletar: (racaId) => `${API_BASE_URL}/clinica/racas/${racaId}`,
      setStatus: (racaId) => `${API_BASE_URL}/clinica/racas/${racaId}/status`,
      ocultar: (racaId) => `${API_BASE_URL}/clinica/racas/${racaId}/ocultar`,
    },
    consulta: {
      listar: `${API_BASE_URL}/clinica/consultas`,
      detalhes: (consultaId) =>
        `${API_BASE_URL}/clinica/consultas/${consultaId}`,
      cadastrar: `${API_BASE_URL}/clinica/consultas/cadastrar`,
      editar: `${API_BASE_URL}/clinica/consultas/editar`,
      cancelar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/cancelar`,
      reabrir: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/reabrir`,
      deletar: (consultaId) =>
        `${API_BASE_URL}/clinica/consultas/${consultaId}`,
    },
    avaliacao: {
      getByConsultaId: (consultaId) =>
        `${API_BASE_URL}/clinica/consultas/${consultaId}/avaliacao`,
      upsert: (consultaId) =>
        `${API_BASE_URL}/clinica/consultas/${consultaId}/avaliacao`, // Rota PUT para criar/editar
      deletar: (avaliacaoId) =>
        `${API_BASE_URL}/clinica/avaliacoes/${avaliacaoId}`,
    },
    racao: {
      listar: `${API_BASE_URL}/clinica/racoes`,
      cadastrar: `${API_BASE_URL}/clinica/racoes/cadastrar`,
      opcoes: `${API_BASE_URL}/clinica/racoes/opcoes`,
      detalhes: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
      editar: `${API_BASE_URL}/clinica/racoes/editar`,
      deletar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
      setStatus: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}/status`,
      ocultar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}/ocultar`,
    },
    tutor: {
      listar: `${API_BASE_URL}/clinica/tutores`,
      opcoes: `${API_BASE_URL}/clinica/tutores/opcoes`,
      detalhes: (tutorId) => `${API_BASE_URL}/clinica/tutores/${tutorId}`,
      cadastrar: `${API_BASE_URL}/clinica/tutores/cadastrar`,
      editar: `${API_BASE_URL}/clinica/tutores/editar`,
      deletar: (tutorId) => `${API_BASE_URL}/clinica/tutores/${tutorId}`,
      setStatus: (tutorId) => `${API_BASE_URL}/clinica/tutores/${tutorId}/status`,
    },
    usuario: {
      listar: `${API_BASE_URL}/clinica/usuarios`,
      cadastrar: `${API_BASE_URL}/clinica/usuarios/cadastrar`,
      editar: `${API_BASE_URL}/clinica/usuarios/editar`,
      setStatus: (upcId) => `${API_BASE_URL}/clinica/vinculos/${upcId}/status`,
    },
  },
};

export default endpoints;
