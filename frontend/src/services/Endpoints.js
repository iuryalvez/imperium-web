const API_BASE_URL = '/api';

const endpoints = {
  sistema: {
    clinica: {
      listar: `${API_BASE_URL}/sistema/clinicas`,
      nova: `${API_BASE_URL}/sistema/clinicas/nova-clinica`,
      detalhes: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
      desativar: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
    },
    animal: {
      listar: `${API_BASE_URL}/sistema/animais`,
    },
    usuario: {
      listar: `${API_BASE_URL}/sistema/usuarios`,
    },
  },
  clinica: {
    animal: {
      novo: (tutorId) => `${API_BASE_URL}/clinica/animais/tutor_${tutorId}-novo-animal`,
      detalhes: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      deletar: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      listarPorTutor: (tutorId) => `${API_BASE_URL}/clinica/animais/tutor_${tutorId}/animais`,
      editar: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}/editar`,
    },
    autenticacao: {
      signup: `${API_BASE_URL}/auth/signup`,
      login: `${API_BASE_URL}/auth/login`,
      logout: `${API_BASE_URL}/auth/logout`,
      recuperarSenha: `${API_BASE_URL}/auth/forgot-password`,
      resetPassword: `${API_BASE_URL}/auth/reset-password`,
      sendEmail: `${API_BASE_URL}/auth/send-email`,
      listarVinculos: `${API_BASE_URL}/auth/sistema`,
      listarVinculosParaTrocar: `${API_BASE_URL}/auth/sistema/trocar-vinculo`,
      escolherVinculo: (upcId) => `${API_BASE_URL}/auth/sistema/${upcId}`,
      checarStatus: `${API_BASE_URL}/auth/check`,
    },
    clinica: {
      dados: `${API_BASE_URL}/clinica/dados`,
    },
    consulta: {
      nova: `${API_BASE_URL}/clinica/nova-consulta`,
      listar: `${API_BASE_URL}/clinica/consultas`,
      detalhes: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}`,
      deletar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}`,
      editar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/editar`,
      relatorio: {
        detalhes: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
        criar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
        editar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
        deletar: (consultaId) => `${API_BASE_URL}/clinica/consultas/${consultaId}/relatorio`,
      },
    },
    racao: {
      nova: `${API_BASE_URL}/clinica/racoes/nova-racao`,
      listar: `${API_BASE_URL}/clinica/racoes`,
      detalhes: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
      editar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}/editar`,
      deletar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
    },
    tutor: {
      listar: `${API_BASE_URL}/clinica/tutores`,
      detalhes: (tutorId) => `${API_BASE_URL}/clinica/tutor_${tutorId}`,
      cadastrar: `${API_BASE_URL}/clinica/tutores/cadastrar`,
      editar: (tutorId) => `${API_BASE_URL}/clinica/tutores/${tutorId}`,
      deletar: (tutorId) => `${API_BASE_URL}/clinica/tutores/tutor_${tutorId}`,
    },
    usuario: {
      cadastrar: (perfilId) => `${API_BASE_URL}/clinica/usuarios/cadastrar+${perfilId}`,
      listar: `${API_BASE_URL}/clinica/usuarios`,
      detalhes: (usuarioId) => `${API_BASE_URL}/clinica/usuarios/${usuarioId}`,
      editar: (upcId) => `${API_BASE_URL}/clinica/usuarios/${upcId}/editar`,
      habilitar: (upcId) => `${API_BASE_URL}/clinica/usuarios/${upcId}/habilitar`,
      desabilitar: (upcId) => `${API_BASE_URL}/clinica/usuarios/${upcId}/desabilitar`,
    },
  }
};

export default endpoints;
