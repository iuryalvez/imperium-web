import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const usuariosDoSistemaAPI = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.sistema.usuario.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      throw error.response ? error.response.data : error;
    }
  },
  cadastrar: async (dadosUsuario) => {
    try {
      const response = await api.post(endpoints.sistema.usuario.cadastrar, dadosUsuario, {
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
  editar: async (upcId, dadosEditados) => {
    try {
      const response = await api.put(endpoints.sistema.usuario.editar(upcId), dadosEditados, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      return response.data;
    } catch (error) {
      console.error("Erro ao editar usuário:", error);
      throw error.response ? error.response.data : error;
    }
  },
};