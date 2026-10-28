import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

const rotas = endpoints.sistema.racao;

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
 * Catálogo de rações do sistema (admin). "Cópias" são as rações próprias das clínicas.
 */
export const racoesAPI = {
  /** Rações do sistema, paginadas. filtro: { pagina, tamanhoPagina, busca, ordenarPor, ordem, ativo, especie } */
  listar: (filtro) => executar("listar rações do sistema", () => api.get(rotas.listar, { params: filtro })),

  /** Rações próprias das clínicas, paginadas (com nomeClinica). */
  copias: (filtro) => executar("listar cópias de rações", () => api.get(rotas.copias, { params: filtro })),

  detalhes: (id) => executar(`obter a ração ${id}`, () => api.get(rotas.detalhes(id))),

  cadastrar: (dadosRacao) => executar("cadastrar ração", () => api.post(rotas.cadastrar, dadosRacao)),

  editar: (dadosRacao) => executar("editar ração", () => api.put(rotas.editar, dadosRacao)),

  deletar: (id) => executar(`excluir a ração ${id}`, () => api.delete(rotas.deletar(id))),

  setStatus: (id, ativo) =>
    executar(`alterar o status da ração ${id}`, () => api.put(rotas.setStatus(id), null, { params: { ativo } })),
};
