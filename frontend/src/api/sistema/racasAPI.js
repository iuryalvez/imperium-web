import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

const rotas = endpoints.sistema.raca;

// Executa a requisição e relança o erro no formato que o Manager exibe (error.message)
const executar = async (descricao, requisicao) => {
  try {
    const response = await requisicao();
    return response.data;
  } catch (error) {
    console.error(`Erro ao ${descricao}:`, error);
    throw error.response ? error.response.data : error;
  }
};

/**
 * Catálogo de raças do sistema (admin). "Cópias" são as raças próprias das clínicas.
 * As telas da clínica usam api/clinica/racasDaClinicaAPI.js.
 */
export const racasAPI = {
  /** Raças do sistema, paginadas. filtro: { pagina, tamanhoPagina, busca, ordenarPor, ordem, ativo, especie } */
  listar: (filtro) => executar("listar raças do sistema", () => api.get(rotas.listar, { params: filtro })),

  /** Raças próprias das clínicas, paginadas (com nomeClinica). */
  copias: (filtro) => executar("listar cópias de raças", () => api.get(rotas.copias, { params: filtro })),

  detalhes: (id) => executar(`obter a raça ${id}`, () => api.get(rotas.detalhes(id))),

  cadastrar: (dadosRaca) => executar("cadastrar raça", () => api.post(rotas.cadastrar, dadosRaca)),

  editar: (dadosRaca) => executar("editar raça", () => api.put(rotas.editar, dadosRaca)),

  deletar: (id) => executar(`excluir a raça ${id}`, () => api.delete(rotas.deletar(id))),

  setStatus: (id, ativo) =>
    executar(`alterar o status da raça ${id}`, () => api.put(rotas.setStatus(id), null, { params: { ativo } })),
};
