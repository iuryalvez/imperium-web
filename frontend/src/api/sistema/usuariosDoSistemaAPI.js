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
  cadastrar: async (dadosFormulario) => {
    try {
      const response = await api.post(endpoints.sistema.usuario.cadastrar, dadosFormulario, {
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
};