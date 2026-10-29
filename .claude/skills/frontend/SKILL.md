---
name: frontend
description: Use sempre que for criar, alterar, revisar ou refatorar o frontend do Imperium (React + Ant Design + Tailwind) — telas em features/Sistema ou features/Clinica, componentes genéricos em components/, chamadas em api/, cores e tema em theme/, helpers em utils/ ou contextos em services/ — mesmo que o pedido não diga "frontend" (ex.: "cria a tela de vacinas", "adiciona um botão na tabela de animais").
---

# Frontend do Imperium

Antes de começar, leia a skill `contexto` (o que é o sistema e o que nunca pode quebrar).

Stack: React 18 (Create React App), Ant Design 5, Tailwind 3, axios, dayjs, react-router-dom 6, recharts e react-imask.
A raiz é `imperium-web/frontend/src/`.

As pastas de `src/` são:

- `features/`: telas, organizadas por escopo e entidade (seção 2).
- `components/`: componentes genéricos, sem regra de negócio (seção 4).
- `api/`: uma camada de chamadas HTTP por entidade (seção 5).
- `services/`: contextos globais, mapa de rotas da API e integrações externas (seção 6).
- `utils/`: funções puras e helpers de formulário (seção 7).
- `theme/`: cores, tema do antd e mensagens de validação (seção 8).
- `assets/`: imagens e ícones.
- `index.css` (na raiz de `src/`): variáveis CSS das cores e classes globais.

Antes de criar algo, procure se já existe em `components/`, `utils/` ou `services/`. Duplicar um helper que já existe é o erro mais comum e deixa o projeto inconsistente.

## 1. Primeiro decida o escopo: Sistema ou Clínica

Sistema é a gestão dos tenants, usada pelo administrador do Imperium (`isAdmin`):

- Pasta `features/Sistema/<Entidade>DoSistema/` e rota `/sistema/...`.
- Em `App.js`, fica sob `<ProtectedRoute requiresAdmin />` com `<LayoutLogged />`.
- API em `api/sistema/<entidade>API.js`, usando `endpoints.sistema.<entidade>`.
- Não tem menu lateral: a tela tem botão "Voltar" (`window.history.back()`) no topo e atalho em `SistemaInicioAdmin`.
- Pode listar e editar dados de qualquer clínica.

Clínica é o tenant, usada pelos usuários vinculados a uma clínica (GESTOR, VETERINARIO, SECRETARIO, TECNICO):

- Pasta `features/Clinica/<Entidade>DaClinica/` e rota `/clinica/...`.
- Em `App.js`, fica sob `<ProtectedRoute requiresAuth />` com `<LayoutAuthenticated />`.
- API em `api/clinica/<entidade>DaClinicaAPI.js`, usando `endpoints.clinica.<entidade>`.
- Entrada no menu em `components/layout/Sidebar/Sidebar.jsx`.
- Nunca envie `clinicaId` no payload. O backend lê a clínica do token, e aceitar do frontend abriria brecha entre clínicas.

Se não ficar claro qual escopo, pergunte antes de criar arquivos.

## 2. Estrutura de uma feature

Uma feature em `features/<Escopo>/<Entidade>Da<Escopo>/` tem:

- `<Entidade>Da<Escopo>.jsx`: a página, só layout, consumindo o hook.
- `use<Entidade>Da<Escopo>Manager.js`: estado, chamadas à API e handlers.
- `components/`: componentes usados só por esta feature, cada um na sua pasta:
  - `Tabela<Entidade>Da<Escopo>/` com `Tabela<Entidade>Da<Escopo>.jsx` e `colunas.js` (colunas fixas, sem a coluna "Ações").
  - `FormularioCadastro<Entidade>Da<Escopo>/FormCadastro<Entidade>Da<Escopo>.jsx`.
  - `FormularioEdicao<Entidade>Da<Escopo>/FormEdicao<Entidade>Da<Escopo>.jsx`.
  - `FormularioDetalhes<Entidade>Da<Escopo>/FormDetalhes<Entidade>Da<Escopo>.jsx`.

Sub-telas que pertencem à entidade ficam dentro dela como sub-feature, com o mesmo padrão. Exemplos: `AnimaisDaClinica/Acompanhamento/` (com `Acompanhamento.jsx`, `useAcompanhamentoManager.js` e `components/`) e `AnimaisDaClinica/components/GerenciadorDeRacas/` (com `useRacasDaClinicaManager.js`).

Referências vivas para copiar: `features/Sistema/ClinicasDoSistema`, `features/Clinica/UsuariosDaClinica` e `features/Clinica/AnimaisDaClinica`.

Há nomes legados fora do padrão (por exemplo `RacoesDaClinica/useRacoesManager.js`, `api/clinica/acompanhamentoAPI.js`). Não copie; em código novo use o nome completo com `DaClinica` ou `DoSistema`.

## 3. Responsabilidades dentro da feature

### Página

- Desestrutura tudo de `use<Entidade>Manager()`. Não chama API nem guarda estado de negócio, assim a tela fica fácil de ler e a lógica pode ser testada e reaproveitada.
- Cabeçalho com `h1` em `className="text-2xl font-bold text-[var(--verde-medio)]"` e botão `type="primary"` com `<PlusOutlined />` para cadastrar.
- Não troca a tela inteira por spinner enquanto a lista carrega: a `Table` recebe `loading`. `LoadingSpinner` só para carregamento inicial de tela sem tabela.
- A tabela recebe dados e callbacks (`onEdit`, `onViewDetails`, `onToggleStatus`...).
- Modais via `components/modal/ModalPadrao.jsx`. Em `modo="criacao"` ou `modo="edicao"`, passe `formId` igual ao `id` do `<Form>` e `isSubmitting`; o botão de submit do rodapé usa `form={formId}`. Em `modo="detalhes"` é só leitura, com `onEdit` e `onDelete` opcionais. Ícones: `FormOutlined` (cadastro), `EditOutlined` (edição) e `ProfileOutlined` (detalhes).
- Use o `Modal` nativo do antd só quando o rodapé for diferente do padrão (exemplo: "Simular acesso"), reaproveitando as classes `modal-scroll-container` e `modal-footer-actions`.

### Manager

- `export const use<Entidade>Manager = () => { ... return { ... } }`, com export nomeado.
- Estados típicos: `isSubmitting`, `loadingRowKey` (loading por linha), `isModalOpen`, `isModalDetalhesOpen`, `editing<Entidade>` e `viewing<Entidade>`.
- A lista usa `usePaginacaoServidor` (seção 9). Para recarregar depois de salvar, use `lista.recarregar`.
- Handlers com prefixo `handle`: `handleOpenModalCadastro`, `handleEditar<Entidade>`, `handleOpenModalDetalhes`, `handleCloseModal`, `handleSubmit` e `handleToggleStatus`.
- O `handleSubmit(values)` faz: `setIsSubmitting(true)`; se há `editing<Entidade>`, chama `editar({ ...editing, ...payload })`, senão `cadastrar(payload)`; mostra `message.success(...)`; fecha o modal; recarrega a lista; e no `finally` faz `setIsSubmitting(false)`.
- Antes de enviar, use `sanitizeToNumbers` em CPF, CNPJ, CEP e telefone, e `sanitizePayload` nas edições. O AutoMapper do backend ignora `null` mas grava `""`, e é assim que um campo é limpo.
- Erros com `message.error(error.message || "Erro ao ...")`.
- Navegação com `useNavigate` e autenticação com `useAuth()`.

### Tabela

- `colunas.js` exporta por padrão o array das colunas de dados (`title`, `dataIndex`, `key`, `sorter`, `render`) e, quando houver filtros, a função `traduzirFiltros<Entidade>`.
- O componente monta a coluna "Ações" com `useMemo`: botões `size="small"` com `Tooltip`, `EyeOutlined`, `EditOutlined` e o toggle de status com `Popconfirm`, em `fixed: "right"`.
- A tabela é `<Table rowKey="id" pagination={paginacao} onChange={handleTableChange} scroll={{ x: "max-content" }} />`.
- Seletor de colunas visíveis com `Dropdown` e `Checkbox` é opcional (modelo: `TabelaUsuariosDaClinica`).

### Formulários

- `const [form] = Form.useForm();`, `layout="vertical"`, `onFinish` vindo do pai e `initialValues` na edição.
- `id` do Form no formato `form-<cadastro|edicao>-<entidade>-<escopo>` (exemplo: `form-edicao-usuario-clinica`).
- Organização com `Row gutter={24}`, `Col xs={24} lg={12}` e `Divider orientation="left" plain`.
- IDs ocultos com `<Form.Item name="id" hidden><Input /></Form.Item>`.
- Detalhes com `Descriptions bordered` e os formatadores de `utils/formatters.js` e `utils/renderers.js`.

## 4. Componentes genéricos

Um componente vai para `components/` quando não conhece nenhuma entidade de negócio e é (ou claramente será) usado por mais de uma feature. Se ele só serve a uma feature, fica em `features/.../components/`. Na dúvida, comece dentro da feature e promova quando a segunda feature precisar: promover cedo demais cria componentes cheios de `if` para casos específicos.

As pastas atuais são:

- `components/atoms/`: peças pequenas de UI com props simples, como `LoadingSpinner.jsx`, `LabelComInfo.jsx` e `SelectBuscaRemota.jsx`.
- `components/modal/`: modais base, como `ModalPadrao.jsx`.
- `components/layout/`: estrutura da aplicação, como `LayoutAuthenticated.jsx`, `LayoutLogged.jsx`, `LayoutPublic.jsx`, `Header/AppHeader.jsx` e `Sidebar/Sidebar.jsx`.
- `components/servicesComponents/`: componentes de infraestrutura sem UI própria, como `ProtectedRoute.js`.

Uma pasta nova (por exemplo `components/tabela/`) só quando surgir um grupo que não cabe nas existentes, com nome em português e minúsculo.

Regras de um componente genérico:

- Nome em PascalCase e em português, arquivo `.jsx` com `export default`. Componente com CSS próprio ganha pasta própria (`Header/AppHeader.jsx` com `AppHeader.css`).
- Recebe tudo por props com nomes em português e valores padrão quando fizer sentido (como `modo = "edicao"` no `ModalPadrao`). Aceita `children` quando for contêiner.
- Não chama a API, não usa `useAuth` e não guarda estado de negócio: quem decide é a feature. Estado puramente visual (aberto ou fechado, colunas visíveis) pode ficar no componente.
- Estilo com classes Tailwind do tema (`text-verde-medio`, `border-verde-escuro`) ou tokens do antd, nunca cor hex solta.
- Comentário curto no topo explicando para que serve e as props principais.
- Ao alterar um componente genérico, procure todos os usos (`grep -rn "NomeDoComponente" src/`) e confirme que nenhuma tela quebra.

## 5. Camada de API

Os arquivos de `api/` são:

- `index.js`: a instância do axios (`baseURL` vem de `REACT_APP_API_BASE_URL`, com `withCredentials: true`).
- `autenticacaoAPI.js`: login, logout e vínculos.
- `sistema/<entidade>API.js`, como `clinicasAPI.js` e `usuariosDoSistemaAPI.js`.
- `clinica/<entidade>DaClinicaAPI.js`, como `animaisDaClinicaAPI.js` e `usuariosDaClinicaAPI.js`.

Como adicionar chamadas:

1. Adicione as rotas em `services/Endpoints.js`, dentro de `sistema.<entidade>` ou `clinica.<entidade>`, sempre com o prefixo `${API_BASE_URL}`. Rotas com parâmetro são funções, como `detalhes: (id) => ...`.
2. Crie ou atualize o arquivo da entidade exportando um objeto nomeado:

   ```js
   import api from "../index";
   import endpoints from "../../services/Endpoints";

   export const vacinasDaClinicaAPI = {
     listar: async (filtro) => {
       try {
         const response = await api.get(endpoints.clinica.vacina.listar, { params: filtro });
         return response.data;
       } catch (error) {
         console.error("Erro ao buscar vacinas da clínica:", error);
         throw error.response ? error.response.data : error;
       }
     },
     // opcoes, detalhes, cadastrar, editar, deletar, setStatus...
   };
   ```

3. Use verbos em português e consistentes: `listar(filtro)`, `opcoes(busca, ...)`, `detalhes`, `cadastrar`, `editar`, `deletar` e `setStatus(id, ativo)`. Listas paginadas enviam o filtro como `params`; status vai na query (`api.put(url, null, { params: { ativo } })`).
4. Sempre relance o erro (`throw error.response ? error.response.data : error`) para o Manager mostrar a mensagem.
5. Nunca use `axios` direto nem monte URL na feature: tudo passa por `api/index.js` e `Endpoints.js`, para o prefixo `/api` e os cookies funcionarem igual em produção e em homologação (`/dev`).
6. As URLs precisam bater com as rotas do controller no backend (skill `backend`).

## 6. Services

- `AuthContext.js`: `useAuth()` expõe `isLogged`, `isAuthenticated`, `isAdmin`, `perfis`, `userName`, `nomeClinica`, `checkAuth` e as ações de vínculo. Use para esconder botões por perfil, lembrando que a segurança real é do backend.
- `NotificationContext.js`: `useNotification()` devolve `abrirNotificacao(tipo, titulo, descricao)` para avisos maiores. Para feedback rápido de ação, use `message` do antd.
- `Endpoints.js`: mapa único de rotas da API.
- `viaCepService.js`: exemplo de integração externa. Uma API de terceiros nova ganha um `<nome>Service.js` aqui.

Um contexto novo só quando o estado precisar ser compartilhado por várias telas; registre o Provider em `App.js`, junto dos atuais.

## 7. Utils

- `formatters.js`: exibição, como `formatarData`, `formatarCPF`, `formatarCNPJ`, `formatarTelefone`, `formatarCEP`, `formatarEspecie`, `formatarSexo` e `formatarGrauAtividade`.
- `masks.js`: inputs com máscara para `Form.Item`, como `CpfMaskInput`, `CnpjMaskInput`, `TelefoneMaskInput`, `CepMaskInput` e `CepSearchInput`.
- `validators.js`: `validarCPF` e `validarCNPJ`, usados em `rules` com `validator`.
- `sanitizers.js`: `sanitizeToNumbers` e `sanitizePayload`, antes de enviar.
- `renderers.js`: renderizações reaproveitáveis, como `perfisSistema`, `renderPerfis` e `renderAnimais`.
- `useCepHandler.js`: hook que preenche o endereço pelo CEP (`handleCepSearch`, `loadingCep`).
- `usePaginacaoServidor.js`: hook de lista paginada no servidor. Recebe `listar(filtro)` da api, os filtros iniciais e `traduzirFiltros`; devolve `dados`, `loading`, `paginacao`, `setFiltros`, `setBusca` (com debounce), `handleTableChange` e `recarregar`.

Um helper novo vai para o arquivo do tipo certo, com nome em português no padrão verbo mais alvo (`formatarPeso`, `validarCRMV`) e JSDoc curto. Enum que vem do backend como número ganha um `formatarX` aqui, em vez de `if` espalhado nas telas. Hook reutilizável de formulário segue o modelo `useCepHandler`.

## 8. Tema

- `theme/colors.js` é a única fonte da verdade das cores (`branco`, `cinza.*`, `verde.fraco|realce|claro|medio|escuro`, `vermelho.delete`). O `tailwind.config.js` importa esse arquivo, por isso existem classes como `text-verde-medio`.
- `theme/ant-theme.js` (`themeConfig`) mapeia essas cores para os tokens do antd. Para mudar a aparência de um componente do antd no app todo, ajuste aqui, não com CSS na tela.
- `theme/validateMessages.js` tem as mensagens de validação em português usadas por todos os `Form`.
- `src/index.css` repete as cores como variáveis CSS (`--verde-medio`...) e define classes globais (`modal-scroll-container`, `modal-footer-actions`, `modal-title-icon`). Ao criar ou mudar uma cor, atualize `colors.js` e as variáveis de `index.css`.
- Nas telas, prefira nesta ordem: token ou componente do antd, classe Tailwind do tema e por fim `var(--cor)`. Evite `style={{ color: "#..." }}`.

## 9. Listas, status e selects

- Listas paginam no servidor, no contrato `{ itens, total, pagina, tamanhoPagina }`. O Manager usa `usePaginacaoServidor(api.listar, filtrosIniciais, traduzirFiltros)` e passa `paginacao` e `handleTableChange` para a `Table`. Tamanhos de página: 10, 20, 50 e 100.
- Nas colunas de lista paginada, use `sorter: true` (ordenação no servidor, com `key` igual ao nome aceito pelo backend, como `nome` ou `dataCadastro`) e `filters` com `filterMultiple: false`, traduzidos por `traduzirFiltros<Entidade>`. Não use `sorter` ou `onFilter` locais em lista paginada, porque só agiriam na página atual. A busca por texto fica num `Input.Search` acima da tabela, chamando `setBusca`.
- A ordem padrão vem do backend (mais recentes primeiro); não force `defaultSortOrder` na tela.
- Ativo e inativo: coluna "Status" com `Tag` verde ou vermelha e filtro; botão de ativar ou desativar na coluna Ações com `Popconfirm` e ícones `StopOutlined` e `CheckCircleOutlined` (modelos: `TabelaUsuariosDaClinica` e `TabelaAnimaisDaClinica`). O Excluir continua para cadastro errado; o backend recusa se estiver em uso.
- Selects: inativos aparecem desabilitados com o sufixo "(inativo)", exceto o valor que o registro em edição já tem. Listas grandes usam `SelectBuscaRemota` com `buscarOpcoes={(busca) => api.opcoes(busca, ...)}` e `opcoesAtuais=[{ id, nome }]` do registro em edição. Em `mode="multiple"` o componente guarda o rótulo dos itens já escolhidos.
- Catálogo do sistema (raças e rações): na clínica, item do sistema mostra tag "Sistema"; editar cria cópia da clínica; em vez de excluir ou desativar, a clínica pode "Ocultar para esta clínica". No Sistema, a tela tem o botão "Cópias das clínicas", que troca a lista para os itens próprios das clínicas, com coluna Clínica.
- Todas as listas seguem o contrato paginado, exceto Usuários do Sistema (pendente).

## 10. Registrar a tela

- Em `App.js`, importe a página e adicione a `<Route>` no bloco de guarda certo (seção 1).
- Na Clínica, adicione o item no `Sidebar.jsx` (`key` igual ao path e `label` com `<NavLink>`), respeitando os perfis que podem ver a tela.

## 11. Passo a passo: tela de CRUD nova

Modelo vivo: `features/Clinica/AnimaisDaClinica` e `features/Sistema/RacasDoSistema` (com `GerenciadorDeRacas` na clínica).

1. Decida o escopo (Sistema ou Clínica) e confirme que o backend já tem as rotas. Se não tiver, faça primeiro o backend (skill `backend`).
2. Adicione as rotas em `services/Endpoints.js` (listar, opcoes, detalhes, cadastrar, editar, deletar, setStatus).
3. Crie o arquivo em `api/sistema/` ou `api/clinica/` com os verbos padrão.
4. Crie a pasta da feature com a página, o Manager e `components/`.
5. Escreva o Manager com `usePaginacaoServidor`, os estados de modal e os handlers de submit, status e exclusão.
6. Escreva a página só com layout: cabeçalho, busca, tabela e modais.
7. Crie a tabela com `colunas.js` (incluindo Status e `traduzirFiltros`), a coluna Ações e os formulários de cadastro, edição e detalhes.
8. Nos campos que escolhem outra entidade, use `SelectBuscaRemota` com a rota `/opcoes`.
9. Registre a rota em `App.js` e, na Clínica, o item no `Sidebar.jsx`.
10. Rode `npm run build` em `frontend/` e confirme que compila sem erros e sem warnings novos.
11. Faça ou entregue o teste visual pela skill `teste-visual`.

## 12. Passo a passo: componente genérico novo

1. Confirme que nada parecido já existe em `components/`, `utils/` ou no antd.
2. Confirme que ele vai servir a mais de uma feature; se não, deixe dentro da feature.
3. Escolha a pasta (`atoms`, `modal`, `layout`...) e um nome em português.
4. Defina as props em português, com valores padrão, sem chamada de API e sem `useAuth`.
5. Estilize só com o tema (Tailwind do tema ou tokens do antd).
6. Escreva o comentário de topo com a finalidade e as props principais.
7. Troque o código duplicado nas features pelo componente, se a tarefa pedir.
8. Procure todos os usos, rode `npm run build` e faça o teste visual nas telas afetadas.

## 13. Quando não houver exemplo no projeto

Se a tarefa pede algo que o projeto ainda não faz (gráfico novo, upload, impressão, drag and drop etc.):

- Siga a prática de mercado da stack (React 18, Ant Design 5, Tailwind) e a documentação oficial, preferindo componentes do antd a bibliotecas novas.
- Mantenha a estrutura: página só com layout, lógica no Manager, chamadas em `api/` com rotas em `Endpoints.js`, cores do tema e nenhum `clinicaId` enviado.
- Explique ao usuário a abordagem e por que ela foi escolhida e espere aprovação antes de tratá-la como padrão. Biblioteca nova só com permissão. Se a abordagem for aprovada, sugira atualizar esta skill.

## 14. Checklist final

- Escopo certo (Sistema ou Clínica) e nomes com `DoSistema` ou `DaClinica`.
- Página sem lógica; lógica toda no Manager.
- Reaproveitou `components/`, `utils/` e `services/` antes de criar algo; o que é genérico foi para `components/` sem regra de negócio.
- Endpoints, arquivo de API, rota e menu registrados.
- Nenhum `clinicaId` enviado pelo frontend em telas de Clínica.
- Lista nova paginada no servidor, com status Ativo e Inativo, toggle com `Popconfirm` e selects mostrando inativos desabilitados.
- Cores só via tema, textos em português e feedback com `message.success` e `message.error`.
- `npm run build` em `frontend/` sem erros e sem warnings novos.
- Teste visual feito ou roteiro entregue ao usuário (skill `teste-visual`).
- Nada alterado em `bris-web` nem em `https-server`.
