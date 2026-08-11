import api from "../index"; // sua instância configurada do axios
import endpoints from "../../services/Endpoints"; // seu arquivo de endpoints

export const acompanhamentoAPI = {
  getHistoricoCompleto: async (acompanhamentoId) => {
    const response = await api.get(
      endpoints.clinica.acompanhamento.getHistoricoCompleto(acompanhamentoId)
    );
    return response.data;
  },

  getPerfisMetabolicos: async () => {
    const response = await api.get(
      endpoints.clinica.acompanhamento.getPerfisMetabolicos
    );
    return response.data;
  },

  /**
   * Envia os dados para criar uma nova dieta para um relatório dentro de um acompanhamento.
   * @param {number} acompanhamentoId O ID do acompanhamento que contém o relatório.
   * @param {object} dadosDieta O objeto DTO com os dados da dieta (CreateDietaDto).
   * @returns {Promise<object>} A resposta da API.
   */
  criarDieta: async (dadosDieta) => {
    try {
      const response = await api.post(
        // Constrói a URL dinâmica usando a função do arquivo de endpoints
        endpoints.clinica.acompanhamento.criarDieta(),
        // O segundo argumento é o corpo (body) da requisição
        dadosDieta,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao criar dieta:", error);
      // Lança o erro para que a camada de UI (componente/hook) possa tratá-lo
      throw error.response ? error.response.data : error;
    }
  },

  editarDieta: async (dietaId, dadosDieta) => {
    try {
      const response = await api.put(
        // Constrói a URL dinâmica usando a função do arquivo de endpoints
        endpoints.clinica.acompanhamento.editarDieta(dietaId),
        // O segundo argumento é o corpo (body) da requisição
        dadosDieta,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao editar dieta:", error);
      // Lança o erro para que a camada de UI (componente/hook) possa tratá-lo
      throw error.response ? error.response.data : error;
    }
  },
  deletarDieta: async (dietaId) => {
    try {
      const response = await api.delete(
        endpoints.clinica.acompanhamento.deletarDieta(dietaId)
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao deletar dieta:", error);
      throw error.response ? error.response.data : error;
    }
  },
};
