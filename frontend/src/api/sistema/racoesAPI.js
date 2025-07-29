import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const racoesAPI = {
  listar: async (especie) => {
    // Starts with the base URL for listing breeds
    let url = endpoints.clinica.racoes.listar;

    // If a species is provided, add it as a query parameter
    if (especie) {
      url += `?especie=${especie}`;
    }

    try {
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      console.error("Erro as listar as rações:", error);
      throw error; // Re-lança o erro para ser tratado pelo componente que chamou
    }
  },

  detalhes: async (id) => {
    try {
      const response = await api.get(endpoints.clinica.racoes.detalhes(id));
      return response.data;
    } catch (error) {
      console.error(`Erro ao obter detalhes da ração ${id}:`, error);
      throw error;
    }
  },

  cadastrar: async (dadosRacao) => {
    try {
      const response = await api.post(
        endpoints.clinica.racoes.cadastrar,
        dadosRacao
      );
      return response.data;
    } catch (error) {
      // The missing braces were here
      console.error("Erro ao cadastrar a ração:", error);
      throw error;
    }
  },

  editar: async (dadosRacao) => {
    try {
      const response = await api.put(endpoints.clinica.racoes.editar, dadosRacao, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      return response; // PUT retorna NoContent (204), então retornamos a resposta completa
    } catch (error) {
      console.error(`Erro ao editar a ração.`, error);
      throw error;
    }
  },

  deletar: async (id) => {
    try {
      const response = await api.delete(endpoints.clinica.racoes.deletar(id));
      return response; // DELETE retorna NoContent (204), então retornamos a resposta completa
    } catch (error) {
      console.error(`Erro ao deletar a ração ${id}:`, error);
      throw error;
    }
  },
};
