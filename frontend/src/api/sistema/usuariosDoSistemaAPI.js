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
  listarCamposCadastroSistema: async (entidade) => {
    try {
      const url = endpoints.sistema.usuario.listarCamposCadastro(entidade);
      console.log("Buscando campos em:", url);
      const responseTeste = await fetch(url);
      console.log("Status da resposta:", responseTeste.status);
      if (!responseTeste.ok) {
        throw new Error(`Erro ao buscar campos. Status: ${responseTeste.status}`);
      }
      const response = await api.get(endpoints.sistema.usuario.listarCamposCadastro(entidade));
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar campos do usuário:", error);
      throw error.response ? error.response.data : error;
    }
  },
};