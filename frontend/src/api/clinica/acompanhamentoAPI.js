import api from "../index"; // sua instância configurada do axios
import endpoints from "../../services/Endpoints"; // seu arquivo de endpoints

export const acompanhamentoAPI = {
  getHistoricoCompleto: async (acompanhamentoId) => {
    try {
      const response = await api.get(
        endpoints.clinica.acompanhamento.getHistoricoCompleto(acompanhamentoId)
      );
      return response.data;
    } catch (error) {
      console.error(`Erro ao buscar o histórico do acompanhamento ${acompanhamentoId}:`, error);
      // Relança o corpo da resposta (ServiceResult), para o Manager mostrar error.message do backend
      throw error.response ? error.response.data : error;
    }
  },

  getPerfisMetabolicos: async () => {
    const response = await api.get(
      endpoints.clinica.acompanhamento.getPerfisMetabolicos
    );
    return response.data;
  },

  getNiveisRestricao: async () => {
    const response = await api.get(
      endpoints.clinica.acompanhamento.getNiveisRestricao
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
  
  iniciarDieta: async (dietaId) => {
    try {
      const response = await api.patch(endpoints.clinica.acompanhamento.iniciarDieta(dietaId));
      return response.data;
    } catch (error) {
      console.error("Erro ao iniciar dieta:", error);
      throw error.response ? error.response.data : error;
    }
  },

  concluirDieta: async (dietaId) => {
    try {
      const response = await api.patch(endpoints.clinica.acompanhamento.concluirDieta(dietaId));
      return response.data;
    } catch (error) {
      console.error("Erro ao concluir dieta:", error);
      throw error.response ? error.response.data : error;
    }
  },

  registrarPesoDieta: async (dietaId, dadosPeso) => {
    try {
      const response = await api.post(endpoints.clinica.acompanhamento.registrarPeso(dietaId), dadosPeso);
      return response.data;
    } catch (error) {
      console.error("Erro ao registrar peso na dieta:", error);
      throw error.response ? error.response.data : error;
    }
  },

  deletarPeso: async (dietaId, registroPesoId) => {
    try {
      const response = await api.delete(
        `${endpoints.clinica.acompanhamento.deletarRegistroPeso(dietaId, registroPesoId)}`
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao deletar registro de peso na dieta:", error);
      throw error.response ? error.response.data : error;
    }
  },
};
