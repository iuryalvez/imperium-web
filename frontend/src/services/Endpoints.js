const API_BASE_URL = '/api';

const endpoints = {
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
    escolherVinculoAdmin: `${API_BASE_URL}/auth/sistema/admin`,
    checarStatus: `${API_BASE_URL}/auth/check`,
  },
  sistema: {
    clinica: {
      listar: `${API_BASE_URL}/sistema/clinicas`,
      cadastrar: `${API_BASE_URL}/sistema/clinicas/cadastrar`,
      detalhes: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
      desativar: (clinicaId) => `${API_BASE_URL}/sistema/clinicas/${clinicaId}`,
    },
    animal: {
      listar: `${API_BASE_URL}/sistema/animais`,
    },
    usuario: {
      listar: `${API_BASE_URL}/sistema/usuarios`,
      cadastrar: `${API_BASE_URL}/sistema/usuarios/cadastrar`,
      editar: `${API_BASE_URL}/sistema/usuarios/editar`,
    },
  },
  clinica: {
    dados: `${API_BASE_URL}/clinica/dados`,
    buscarPorNome: (nome) => `${API_BASE_URL}/clinica/buscar?nome=${encodeURIComponent(nome)}`,
    animal: {
      cadastrar: (tutorId) => `${API_BASE_URL}/clinica/animais/tutor/${tutorId}/cadastrar`,
      detalhes: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      deletar: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}`,
      listarPorTutor: (tutorId) => `${API_BASE_URL}/clinica/animais/tutor_${tutorId}/animais`,
      editar: (animalId) => `${API_BASE_URL}/clinica/animais/${animalId}/editar`,
    },
    consulta: {
      cadastrar: (clinicaId) => `${API_BASE_URL}/clinica//${clinicaId}/cadastrar`,
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
      cadastrar: `${API_BASE_URL}/clinica/racoes/cadastrar`,
      listar: `${API_BASE_URL}/clinica/racoes`,
      detalhes: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
      editar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}/editar`,
      deletar: (racaoId) => `${API_BASE_URL}/clinica/racoes/${racaoId}`,
    },
    tutor: {
      listar: `${API_BASE_URL}/clinica/tutores`,
      detalhes: (tutorId) => `${API_BASE_URL}/clinica/tutor_${tutorId}`,
      cadastrar: (clinicaId) => `${API_BASE_URL}/clinica/${clinicaId}/tutores/cadastrar`,
      editar: (tutorId) => `${API_BASE_URL}/clinica/tutores/${tutorId}`,
      deletar: (tutorId) => `${API_BASE_URL}/clinica/tutores/tutor_${tutorId}`,
    },
    usuario: {
      cadastrar:`${API_BASE_URL}/sistema/usuarios/cadastrar`,
      listar: `${API_BASE_URL}/clinica/usuarios`,
      detalhes: (usuarioId) => `${API_BASE_URL}/clinica/usuarios/${usuarioId}`,
      editar: `${API_BASE_URL}/clinica/usuarios/editar`,
      atualizarPerfil: (upcId) => `${API_BASE_URL}/clinica/usuarios/${upcId}/perfil`,
      habilitar: (upcId) => `${API_BASE_URL}/clinica/usuarios/${upcId}/habilitar`,
      desabilitar: (upcId) => `${API_BASE_URL}/clinica/usuarios/${upcId}/desabilitar`,
      listarCamposCadastro: (entidade) => `${API_BASE_URL}/sistema/campos-cadastro/${entidade}`,
    },
  }
};

export default endpoints;
