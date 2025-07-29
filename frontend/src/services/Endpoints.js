const API_BASE_URL = "/api";

const endpoints = {
  autenticacao: {
    checarStatus: `${API_BASE_URL}/auth/check`,
    login: `${API_BASE_URL}/auth/login`,
    logout: `${API_BASE_URL}/auth/logout`,
    // recuperarSenha: `${API_BASE_URL}/auth/forgot-password`, // TODO
    // resetPassword: `${API_BASE_URL}/auth/reset-password`, // TODO
    // sendEmail: `${API_BASE_URL}/auth/send-email`, // TODO
    listarVinculos: `${API_BASE_URL}/auth/sistema`,
    escolherVinculo: (upcId) => `${API_BASE_URL}/auth/sistema/${upcId}`,
    escolherVinculoAdmin: `${API_BASE_URL}/auth/sistema/admin`,
    trocarVinculo: `${API_BASE_URL}/auth/sistema/trocar-vinculo`,
  },
  sistema: {
    clinica: {
      listar: `${API_BASE_URL}/sistema/clinicas`,
      // listarInativas: `${API_BASE_URL}/sistema/clinicas/inativas`, // TODO
      cadastrar: `${API_BASE_URL}/sistema/clinicas/cadastrar`,
      editar: `${API_BASE_URL}/sistema/clinicas/editar`,
      detalhes: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
      deletar: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
    },
    animal: {
      listar: `${API_BASE_URL}/sistema/animais`,
    },
    usuario: {
      buscarPorNome: (nome) =>
        `${API_BASE_URL}/sistema/usuarios/buscar?nome=${encodeURIComponent(
          nome
        )}`,
      listar: `${API_BASE_URL}/sistema/usuarios`,
      cadastrar: `${API_BASE_URL}/sistema/usuarios/cadastrar`,
      editar: `${API_BASE_URL}/sistema/usuarios/editar`,
      setStatus: (usuarioId) =>
        `${API_BASE_URL}/sistema/usuarios/${usuarioId}/status`,
      // deletar: (usuarioId) => `${API_BASE_URL}/sistema/usuarios/${usuarioId}`,
    },
    raca: {
      listar: `${API_BASE_URL}/sistema/racas`,
      cadastrar: `${API_BASE_URL}/sistema/racas`,
      detalhes: (racaId) => `${API_BASE_URL}/sistema/racas/${racaId}`,
      editar: `${API_BASE_URL}/sistema/racas/editar`,
      deletar: (racaId) => `${API_BASE_URL}/sistema/racas/${racaId}`,
    },
  },
  clinica: {
    editar: `${API_BASE_URL}/clinica/editar`,
    animal: {
      listar: `${API_BASE_URL}/clinica/animais/`,
      cadastrar: `${API_BASE_URL}/clinica/animais/cadastrar`,
      // detalhes: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      editar: `${API_BASE_URL}/clinica/animais/editar`,
      deletar: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
    },
    // consulta: {
    //   cadastrar: (clinicaId) => `${API_BASE_URL}/clinica//${clinicaId}/cadastrar`,
    //   listar: `${API_BASE_URL}/clinica/consultas`,
    //   detalhes: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}`,
    //   deletar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}`,
    //   editar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/editar`,
    //   relatorio: {
    //     detalhes: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
    //     criar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
    //     editar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
    //     deletar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
    //   },
    // },
    racao: {
      listar: `${API_BASE_URL}/clinica/racoes`,
      cadastrar: `${API_BASE_URL}/clinica/racoes/cadastrar`,
      detalhes: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
      editar: `${API_BASE_URL}/clinica/racoes/editar`,
      deletar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
    },
    tutor: {
      listar: `${API_BASE_URL}/clinica/tutores`,
      detalhes: (tutorId) => `${API_BASE_URL}/clinica/tutor_${tutorId}`,
      cadastrar: `${API_BASE_URL}/clinica/tutores/cadastrar`,
      editar: `${API_BASE_URL}/clinica/tutores/editar`,
      deletar: (tutorId) => `${API_BASE_URL}/clinica/tutores/${tutorId}`,
    },
    usuario: {
      listar: `${API_BASE_URL}/clinica/usuarios`,
      cadastrar: `${API_BASE_URL}/clinica/usuarios/cadastrar`,
      editar: `${API_BASE_URL}/clinica/usuarios/editar`,
      setStatus: (upcId) => `${API_BASE_URL}/clinica/vinculos/${upcId}/status`,
    }
  },
};

export default endpoints;
