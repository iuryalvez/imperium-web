import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const usuariosDaClinicaAPI = {
  cadastrar: async (dadosFormulario) => {
    try {
      const response = await api.post(endpoints.clinica.usuario.cadastrar, dadosFormulario, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      return response.data
    } catch (error) {
      console.error("Erro ao cadastrar usuário:", error);
      throw error.response ? error.response.data : error;
    }
  },

  listar: async () => {
    try {
      const response = await api.get(endpoints.clinica.usuario.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      throw error.response ? error.response.data : error;
    }
  },

  atualizarPerfil: async (upcId, novoPerfil) => {
    try {
      const responde = await api.put(endpoints.clinica.usuario.atualizarPerfil(upcId), {novoPerfil: novoPerfil});
      return responde.data
    } catch (error) {
      console.error(`Erro ao modificar perfil do usuário com vinculoId ${upcId}`, error);
      throw error.response ? error.responde.data : error;
    }
  },

  habilitar: async (upcId) => {
    try {
      const response = await api.put(endpoints.clinica.usuario.habilitar(upcId));
      return response.data;
    } catch (error) {
      console.error(`Erro ao habilitar usuário com vinculoID ${upcId}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
  
  desabilitar: async (upcId) => {
    try {  
      const response = await api.put(endpoints.clinica.usuario.desabilitar(upcId));
      return response.data;
    } catch (error) {
      console.error(`Erro ao desabilitar usuário com vinculoID ${upcId}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
};
