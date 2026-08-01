import api from "../index";
import endpoints from "../../services/Endpoints";

export const avaliacoesDaClinicaAPI = {
  getByConsultaId: async (consultaId) => {
    try {
      const url = endpoints.clinica.avaliacao.getByConsultaId(consultaId);
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      // Um erro 404 aqui é esperado se não houver relatório, então podemos tratá-lo
      if (error.response && error.response.status === 404) {
        return null; // Retorna nulo se o relatório não for encontrado
      }
      console.error(`Erro ao buscar relatório da consulta ${consultaId}:`, error);
      throw error;
    }
  },
  
  upsert: async (consultaId, dadosAvaliacao) => {
    try {
      const url = endpoints.clinica.avaliacao.upsert(consultaId);
      const response = await api.put(url, dadosAvaliacao);
      return response.data;
    } catch (error) {
      console.error(`Erro ao salvar relatório da consulta ${consultaId}:`, error);
      throw error;
    }
  },
  
  deletar: async (avaliacaoId) => {
    try {
      const url = endpoints.clinica.avaliacao.deletar(avaliacaoId);
      await api.delete(url);
    } catch (error) {
      console.error(`Erro ao deletar relatório ${avaliacaoId}:`, error);
      throw error;
    }
  },
};