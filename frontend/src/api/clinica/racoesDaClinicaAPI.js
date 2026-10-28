import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

const rotas = endpoints.clinica.racao;

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
 * Rações vistas pela clínica: as do sistema (compartilhadas) + as próprias.
 * Ração do sistema: editar cria uma cópia da clínica; não pode ser excluída nem desativada, só ocultada.
 */
export const racoesDaClinicaAPI = {
  /** Lista paginada. filtro: { pagina, tamanhoPagina, busca, ordenarPor, ordem, ativo, especie, origem, oculta } */
  listar: (filtro) => executar("listar as rações", () => api.get(rotas.listar, { params: filtro })),

  /** Opções para selects: até 20 { id, nome, isAtivo }, sem as rações ocultas pela clínica. */
  opcoes: (busca, especie) =>
    executar("buscar opções de rações", () => api.get(rotas.opcoes, { params: { busca, especie } })),

  detalhes: (id) => executar(`obter detalhes da ração ${id}`, () => api.get(rotas.detalhes(id))),

  cadastrar: (dadosRacao) => executar("cadastrar a ração", () => api.post(rotas.cadastrar, dadosRacao)),

  editar: (dadosRacao) => executar("editar a ração", () => api.put(rotas.editar, dadosRacao)),

  deletar: (id) => executar(`deletar a ração ${id}`, () => api.delete(rotas.deletar(id))),

  setStatus: (id, ativo) =>
    executar(`alterar o status da ração ${id}`, () => api.put(rotas.setStatus(id), null, { params: { ativo } })),

  ocultar: (id, oculta) =>
    executar(`ocultar a ração ${id}`, () => api.put(rotas.ocultar(id), null, { params: { oculta } })),
};
