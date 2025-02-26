import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const usuario = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.sistema.usuario.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      throw error.response ? error.response.data : error;
    }
  },
};