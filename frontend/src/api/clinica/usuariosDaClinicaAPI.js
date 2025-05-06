import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const usuariosDaClinicaAPI = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.clinica.usuario.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar usuários:", error);
      throw error.response ? error.response.data : error;
    }
  },
  cadastrar: async (dadosUsuario) => {
    try {
      const response = await api.post(endpoints.clinica.usuario.cadastrar, dadosUsuario, {
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
  editar: async (dadosUsuarioEditados) => {
    try {
      console.log(dadosUsuarioEditados);
      const response = await api.put(endpoints.clinica.usuario.editar, dadosUsuarioEditados, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      return response.data.data;
    } catch (error) {
      console.error("Erro ao editar usuário:", error);
      throw error.response ? error.response.data : error;
    }
  },
};
