import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

export const racasAPI = {
    /**
   * Lista todas as raças, opcionalmente filtradas por espécie.
   * @param {number | null} [especie] - O ID da espécie (1 para Cão, 2 para Gato). Opcional.
   * @returns {Promise<Array>} Uma lista de raças.
   */
  listar: async (especie) => {
    // Starts with the base URL for listing breeds
    let url = endpoints.sistema.raca.listar;

    // If a species is provided, add it as a query parameter
    if (especie) {
      url += `?especie=${especie}`;
    }

    try {
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      console.error("Erro ao listar raças:", error);
      throw error; // Re-lança o erro para ser tratado pelo componente que chamou
    }
  },

  /**
   * Obtém os detalhes de uma única raça pelo seu ID.
   * @param {number} id - O ID da raça.
   * @returns {Promise<Object>} O objeto da raça.
   */
  detalhes: async (id) => {
    try {
      const response = await api.get(endpoints.sistema.raca.detalhes(id));
      return response.data;
    } catch (error) {
      console.error(`Erro ao obter detalhes da raça ${id}:`, error);
      throw error;
    }
  },

  /**
   * Cria uma nova raça.
   * @param {object} dadosRaca - Os dados para a nova raça (CriarRacaDto).
   * @returns {Promise<Object>} O objeto da raça recém-criada.
   */
  cadastrar: async (dadosRaca) => {
    try {
      const response = await api.post(endpoints.sistema.raca.cadastrar, dadosRaca);
      return response.data;
    } catch (error) { // The missing braces were here
      console.error("Erro ao cadastrar raça:", error);
      throw error;
    }
  },

  /**
   * Atualiza uma raça existente.
   * @param {number} id - O ID da raça a ser atualizada.
   * @param {object} dadosRaca - Os dados atualizados para a raça (AtualizarRacaDto).
   * @returns {Promise<Object>} A resposta do servidor.
   */
  editar: async (id, dadosRaca) => {
    try {
      const response = await api.put(endpoints.sistema.raca.editar(id), dadosRaca);
      return response; // PUT retorna NoContent (204), então retornamos a resposta completa
    } catch (error) {
      console.error(`Erro ao editar raça ${id}:`, error);
      throw error;
    }
  },

  /**
   * Deleta uma raça pelo seu ID.
   * @param {number} id - O ID da raça a ser deletada.
   * @returns {Promise<Object>} A resposta do servidor.
   */
  deletar: async (id) => {
    try {
      const response = await api.delete(endpoints.sistema.raca.deletar(id));
      return response; // DELETE retorna NoContent (204), então retornamos a resposta completa
    } catch (error) {
      console.error(`Erro ao deletar raça ${id}:`, error);
      throw error;
    }
  },
};