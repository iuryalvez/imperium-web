import api from "../index";
import endpoints from "../../services/Endpoints";

export const consultasDaClinicaAPI = {
  /** Lista paginada: filtro { pagina, tamanhoPagina, busca, ordenarPor, ordem, ... } → { itens, total, pagina, tamanhoPagina } */
  listar: async (filtro) => {
    try {
      const response = await api.get(endpoints.clinica.consulta.listar, { params: filtro });
      return response.data;
    } catch (error) {
      console.error("Erro ao listar consultas:", error);
      // Relança o corpo da resposta (ServiceResult), para o Manager mostrar error.message do backend
      throw error.response ? error.response.data : error;
    }
  },

  detalhes: async (consultaId) => {
    try {
      const url = endpoints.clinica.consulta.detalhes(consultaId);
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      console.error(`Erro ao buscar detalhes da consulta ${consultaId}:`, error);
      // Relança o corpo da resposta (ServiceResult), para o Manager mostrar error.message do backend
      throw error.response ? error.response.data : error;
    }
  },
  
  cadastrar: async (dadosConsulta) => {
    try {
      const response = await api.post(endpoints.clinica.consulta.cadastrar, dadosConsulta);
      return response.data;
    } catch (error) {
      console.error("Erro ao agendar consulta:", error);
      // Relança o corpo da resposta (ServiceResult), para o Manager mostrar error.message do backend
      throw error.response ? error.response.data : error;
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
      // Relança o corpo da resposta (ServiceResult), para o Manager mostrar error.message do backend
      throw error.response ? error.response.data : error;
    }
  },

  deletar: async (consultaId) => {
    try {
      const url = endpoints.clinica.consulta.deletar(consultaId);
      await api.delete(url); // DELETE não retorna conteúdo
    } catch (error) {
      console.error(`Erro ao deletar consulta ${consultaId}:`, error);
      // Relança o corpo da resposta (ServiceResult), para o Manager mostrar error.message do backend
      throw error.response ? error.response.data : error;
    }
  },
  // Cancela a consulta (só pendente); o motivo é opcional
  cancelar: async (consultaId, motivo) => {
    try {
      const response = await api.put(endpoints.clinica.consulta.cancelar(consultaId), { motivo });
      return response.data;
    } catch (error) {
      console.error(`Erro ao cancelar a consulta ${consultaId}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
  // Reabre uma consulta cancelada (volta a ser pendente)
  reabrir: async (consultaId) => {
    try {
      const response = await api.put(endpoints.clinica.consulta.reabrir(consultaId));
      return response.data;
    } catch (error) {
      console.error(`Erro ao reabrir a consulta ${consultaId}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
};