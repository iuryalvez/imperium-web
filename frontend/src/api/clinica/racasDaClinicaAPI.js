import api from "../index"; // Configuração do axios
import endpoints from "../../services/Endpoints"; // Endpoints

const rotas = endpoints.clinica.raca;

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
 * Raças vistas pela clínica: as do sistema (compartilhadas) + as próprias.
 * Raça do sistema: editar cria uma cópia da clínica; não pode ser excluída nem desativada, só ocultada.
 */
export const racasDaClinicaAPI = {
  /** Lista paginada. filtro: { pagina, tamanhoPagina, busca, ordenarPor, ordem, ativo, especie, origem, oculta } */
  listar: (filtro) => executar("listar raças", () => api.get(rotas.listar, { params: filtro })),

  /** Opções para selects: até 20 { id, nome, isAtivo }, sem as raças ocultas pela clínica. */
  opcoes: (busca, especie) =>
    executar("buscar opções de raças", () => api.get(rotas.opcoes, { params: { busca, especie } })),

  detalhes: (id) => executar(`obter a raça ${id}`, () => api.get(rotas.detalhes(id))),

  cadastrar: (dadosRaca) => executar("cadastrar raça", () => api.post(rotas.cadastrar, dadosRaca)),

  editar: (dadosRaca) => executar("editar raça", () => api.put(rotas.editar, dadosRaca)),

  deletar: (id) => executar(`excluir a raça ${id}`, () => api.delete(rotas.deletar(id))),

  setStatus: (id, ativo) =>
    executar(`alterar o status da raça ${id}`, () => api.put(rotas.setStatus(id), null, { params: { ativo } })),

  ocultar: (id, oculta) =>
    executar(`ocultar a raça ${id}`, () => api.put(rotas.ocultar(id), null, { params: { oculta } })),
};
