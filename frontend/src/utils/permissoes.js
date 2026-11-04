/**
 * Permissões da clínica por perfil. Espelham as policies do backend (backend/Program.cs):
 * a segurança real é do backend; aqui só decidimos o que mostrar (menu, rotas e botões).
 * O admin simulando acesso tem só os perfis que escolheu na simulação (não há atalho por isAdmin).
 */
const GESTOR = "GESTOR";
const VETERINARIO = "VETERINARIO";
const SECRETARIO = "SECRETARIO";
const TECNICO = "TECNICO";

export const PERMISSOES = {
  // Animais, raças, rações, acompanhamento (leitura) e Meu Painel
  visualizaTudo: [GESTOR, VETERINARIO, SECRETARIO, TECNICO],
  // Tutores, consultas e avaliações (o TECNICO não acessa)
  visualizaAtendimento: [GESTOR, VETERINARIO, SECRETARIO],
  // Dashboard, dados e usuários da clínica
  gerenciaClinica: [GESTOR],
  gerenciaAnimais: [GESTOR, VETERINARIO, TECNICO],
  gerenciaRacoes: [GESTOR, VETERINARIO, TECNICO],
  // Avaliações e dietas
  gerenciaAcompanhamento: [GESTOR, VETERINARIO],
  gerenciaTutores: [GESTOR, VETERINARIO, SECRETARIO],
  // Consultas: cadastrar, editar, excluir, cancelar e reabrir
  gerenciaConsultas: [GESTOR, VETERINARIO, SECRETARIO],
};

/**
 * Confere se algum dos perfis do usuário dá a permissão pedida.
 * @param {string[]} perfis - perfis do vínculo atual (useAuth().perfis).
 * @param {keyof PERMISSOES} permissao - nome da permissão (ex.: "gerenciaAnimais").
 */
export const temPermissao = (perfis, permissao) =>
  (perfis || []).some((perfil) => (PERMISSOES[permissao] || []).includes(perfil));
