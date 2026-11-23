import api from "../index"; // configuração do axios
import endpoints from "../../services/Endpoints"; // endpoints

export const animaisDaClinicaAPI = {
  /** Lista paginada: filtro { pagina, tamanhoPagina, busca, ordenarPor, ordem, ... } → { itens, total, pagina, tamanhoPagina } */
  listar: async (filtro) => {
    try {
      const response = await api.get(endpoints.clinica.animal.listar, { params: filtro });
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar animais da clínica:", error);
      throw error.response ? error.response.data : error;
    }
  },
  cadastrar: async (dadosAnimal) => {
    try {
      const response = await api.post(
        endpoints.clinica.animal.cadastrar,
        dadosAnimal,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Erro ao cadastrar animal:", error);
      throw error.response ? error.response.data : error;
    }
  },
  editar: async (dadosAnimalEditados) => {
    try {
      const response = await api.put(
        endpoints.clinica.animal.editar,
        dadosAnimalEditados,
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      return response.data.data;
    } catch (error) {
      console.error("Erro ao editar animal:", error);
      throw error.response ? error.response.data : error;
    }
  },
  deletar: async (animalId) => {
    try {
      const response = await api.delete(
        endpoints.clinica.animal.deletar(animalId)
      );
      return response.data;
    } catch (error) {
      console.error(`Erro ao deletar animal de id: ${animalId}`, error);
      throw error.response ? error.response.data : error;
    }
  },
  // Ativa (true) ou desativa (false) o animal na clínica
  setStatus: async (animalId, ativo) => {
    try {
      const response = await api.put(endpoints.clinica.animal.setStatus(animalId), null, {
        params: { ativo },
      });
      return response.data;
    } catch (error) {
      console.error(`Erro ao alterar o status do animal ${animalId}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
  /** Opções para selects: até 20 { id, nome, isAtivo }, ativos primeiro (inativos vêm para aparecer desabilitados). */
  opcoes: async (busca) => {
    try {
      const response = await api.get(endpoints.clinica.animal.opcoes, { params: { busca } });
      return response.data;
    } catch (error) {
      console.error("Erro ao buscar opções de animais:", error);
      throw error.response ? error.response.data : error;
    }
  },
  detalhes: async (id) => {
    try {
      const response = await api.get(endpoints.clinica.animal.detalhes(id));
      return response.data;
    } catch (error) {
      console.error(`Erro ao buscar detalhes de animais ${id}:`, error);
      throw error.response ? error.response.data : error;
    }
  },
};
