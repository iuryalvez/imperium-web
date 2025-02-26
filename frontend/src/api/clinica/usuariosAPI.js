import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const usuario = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.clinica.usuario.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      throw error.response ? error.response.data : error;
    }
  },

  habilitar: async (upcId) => {
    try {
      const url = endpoints.clinica.usuario.habilitar(upcId);
      console.log('Generated URL:', url); // Add this line 
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
