import api from "../index";
import endpoints from "../../services/Endpoints";

export const consultasDaClinicaAPI = {
  listar: async () => {
    try {
      const response = await api.get(endpoints.clinica.consulta.listar);
      return response.data;
    } catch (error) {
      console.error("Erro ao listar consultas:", error);
      throw error;
    }
  },

  detalhes: async (consultaId) => {
    try {
      const url = endpoints.clinica.consulta.detalhes(consultaId);
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      console.error(`Erro ao buscar detalhes da consulta ${consultaId}:`, error);
      throw error;
    }
  },
  
  cadastrar: async (dadosConsulta) => {
    try {
      const response = await api.post(endpoints.clinica.consulta.cadastrar, dadosConsulta);
      return response.data;
    } catch (error) {
      console.error("Erro ao agendar consulta:", error);
      throw error;
    }
  },

  editar: async (consultaId, dadosConsulta) => {
    try {
      // PUT /consultas/editar: o id vai no corpo, como nas demais entidades
      const response = await api.put(endpoints.clinica.consulta.editar, {
        ...dadosConsulta,
        id: consultaId,
      });
      return response.data; // PUT retorna NoContent, mas podemos retornar o corpo da resposta se houver
    } catch (error) {
      console.error(`Erro ao editar consulta ${consultaId}:`, error);
      throw error;
    }
  },

  deletar: async (consultaId) => {
    try {
      const url = endpoints.clinica.consulta.deletar(consultaId);
      await api.delete(url); // DELETE não retorna conteúdo
    } catch (error) {
      console.error(`Erro ao deletar consulta ${consultaId}:`, error);
      throw error;
    }
  },
};