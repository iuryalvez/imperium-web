---
name: frontend
description: Use sempre que for criar, alterar, revisar ou refatorar o frontend do Imperium (React + Ant Design + Tailwind) — telas em features/Sistema ou features/Clinica, componentes genéricos em components/, chamadas em api/, cores e tema em theme/, helpers em utils/ ou contextos em services/ — mesmo que o pedido não diga "frontend" (ex.: "cria a tela de vacinas", "adiciona um botão na tabela de animais").
---

# Frontend do Imperium

Stack: React 18 (Create React App), Ant Design 5, Tailwind 3, axios, dayjs, react-router-dom 6, recharts, react-imask.
Raiz: `imperium-web/frontend/src/`.

```
src/
├── features/   # telas, organizadas por escopo e entidade (seção 2)
├── components/ # componentes genéricos, sem regra de negócio (seção 4)
├── api/        # uma camada de chamadas HTTP por entidade (seção 5)
├── services/   # contextos globais, rotas da API e integrações externas (seção 6)
├── utils/      # funções puras e helpers de formulário (seção 7)
├── theme/      # cores, tema do antd e mensagens de validação (seção 8)
└── assets/     # imagens e ícones
```

Antes de criar algo, procure se já existe em `components/`, `utils/` ou `services/`. Duplicar um helper que já existe é o erro mais comum e deixa o projeto inconsistente.

## 1. Primeiro decida o escopo: Sistema ou Clínica

| | **Sistema** (gestão dos tenants) | **Clínica** (o tenant) |
|---|---|---|
| Pasta | `features/Sistema/<Entidade>DoSistema/` | `features/Clinica/<Entidade>DaClinica/` |
| Quem usa | Administrador do Imperium (`isAdmin`) | Usuários vinculados a uma clínica (GESTOR, VETERINARIO, SECRETARIO, TECNICO) |
| Rota | `/sistema/...` | `/clinica/...` |
| Guarda de rota em `App.js` | `<ProtectedRoute requiresAdmin />` + `<LayoutLogged />` | `<ProtectedRoute requiresAuth />` + `<LayoutAuthenticated />` |
| API | `api/sistema/<entidade>API.js` → `endpoints.sistema.<entidade>` | `api/clinica/<entidade>DaClinicaAPI.js` → `endpoints.clinica.<entidade>` |
| Menu | Botão "Voltar" (`window.history.back()`) no topo; atalho em `SistemaInicioAdmin` | Entrada no `components/layout/Sidebar/Sidebar.jsx` |
| Tenant | Pode listar/editar qualquer clínica | Nunca envie `clinicaId` no payload: o backend lê a clínica do token, e aceitar do frontend abriria brecha entre clínicas |

Se não ficar claro qual escopo, pergunte antes de criar arquivos.

## 2. Estrutura de uma feature

```
features/<Escopo>/<Entidade>Da<Escopo>/
├── <Entidade>Da<Escopo>.jsx            # página: só layout, consome o hook
├── use<Entidade>Da<Escopo>Manager.js   # estado, chamadas à API e handlers
└── components/                         # componentes usados só por esta feature
    ├── Tabela<Entidade>Da<Escopo>/
    │   ├── Tabela<Entidade>Da<Escopo>.jsx
    │   └── colunas.js                  # colunas fixas (sem a coluna "Ações")
    ├── FormularioCadastro<Entidade>Da<Escopo>/FormCadastro<Entidade>Da<Escopo>.jsx
    ├── FormularioEdicao<Entidade>Da<Escopo>/FormEdicao<Entidade>Da<Escopo>.jsx
    └── FormularioDetalhes<Entidade>Da<Escopo>/FormDetalhes<Entidade>Da<Escopo>.jsx
```

Sub-telas que pertencem à entidade ficam dentro dela como sub-feature com o mesmo padrão (ex.: `AnimaisDaClinica/Acompanhamento/` com `Acompanhamento.jsx` + `useAcompanhamentoManager.js` + `components/`; `AnimaisDaClinica/components/GerenciadorDeRacas/` com `useRacasDaClinicaManager.js`).

Referências vivas para copiar: `features/Sistema/ClinicasDoSistema`, `features/Clinica/UsuariosDaClinica`, `features/Clinica/AnimaisDaClinica`.

## 3. Responsabilidades dentro da feature

### Página `<Entidade>.jsx`
- Desestrutura tudo de `use<Entidade>Manager()`; não chama API nem guarda estado de negócio. Assim a tela fica fácil de ler e a lógica pode ser testada/reaproveitada.
- `if (loading) return <LoadingSpinner />;`
- Cabeçalho: `h1` com `className="text-2xl font-bold text-[var(--verde-medio)]"` e botão `type="primary"` com `<PlusOutlined />` para cadastrar.
- Tabela recebe dados e callbacks (`onEdit`, `onViewDetails`, `onToggleStatus`...).
- Modais via `components/modal/ModalPadrao.jsx`:
  - `modo="criacao" | "edicao"`: passa `formId` igual ao `id` do `<Form>` e `isSubmitting`; o botão de submit do rodapé usa `form={formId}`.
  - `modo="detalhes"`: só leitura; `onEdit`/`onDelete` opcionais.
  - Ícones: `FormOutlined` (cadastro), `EditOutlined` (edição), `ProfileOutlined` (detalhes).
- Use `Modal` nativo do antd só quando o rodapé for diferente do padrão (ex.: "Simular acesso"), reaproveitando as classes `modal-scroll-container` e `modal-footer-actions`.

### Hook `use<Entidade>Manager.js`
- `export const use<Entidade>Manager = () => { ... return { ... } }` (export nomeado).
- Estados típicos: lista, `loading` (inicia `true`), `isSubmitting`, `loadingRowKey` (loading por linha), `isModalOpen`, `isModalDetalhesOpen`, `editing<Entidade>`, `viewing<Entidade>`.
- Lista: `usePaginacaoServidor` (lista paginada no servidor, ver seção 9). Para recarregar depois de salvar, use `lista.recarregar`. A página não troca a tela inteira por spinner enquanto carrega: a `Table` recebe `loading`.
- Handlers com prefixo `handle`: `handleOpenModalCadastro`, `handleEditar<Entidade>`, `handleOpenModalDetalhes`, `handleCloseModal(s)`, `handleSubmit`, `handleToggleStatus`.
- `handleSubmit(values)`: `setIsSubmitting(true)` → se `editing<Entidade>` chama `editar({ ...editing, ...payload })`, senão `cadastrar(payload)` → `message.success(...)` → fecha modal → recarrega lista → `finally setIsSubmitting(false)`.
- Antes de enviar: `sanitizeToNumbers` em CPF/CNPJ/CEP/telefone e `sanitizePayload` em edições (o AutoMapper do backend ignora `null`, mas grava `""`, então é assim que um campo é limpo).
- Erros: `message.error(error.message || "Erro ao ...")`.
- Filtros/buscas locais com `useMemo`. Navegação com `useNavigate`. Auth com `useAuth()`.

### Tabela
- `colunas.js` exporta array default com as colunas de dados (`title`, `dataIndex`, `key`, `sorter`, `render`).
- O componente monta a coluna "Ações" com `useMemo` (botões `size="small"` com `Tooltip`; `EyeOutlined`, `EditOutlined`, toggle com `Popconfirm`), `fixed: "right"`.
- `<Table rowKey="id" pagination={paginacao} onChange={handleTableChange} scroll={{ x: "max-content" }} />` (paginação no servidor, ver seção 9).
- Opcional: seletor de colunas visíveis com `Dropdown` + `Checkbox` (ver `TabelaUsuariosDaClinica`).

### Formulários
- `const [form] = Form.useForm();` `layout="vertical"`, `onFinish` vindo do pai, `initialValues` na edição.
- `id` do Form: `form-<cadastro|edicao>-<entidade>-<escopo>` (ex.: `form-edicao-usuario-clinica`).
- Organização com `Row gutter={24}` / `Col xs={24} lg={12}` e `Divider orientation="left" plain`.
- IDs ocultos: `<Form.Item name="id" hidden><Input /></Form.Item>`.
- Detalhes: `Descriptions bordered` + formatadores de `utils/formatters.js` e `utils/renderers.js`.

## 4. Componentes genéricos (`src/components/`)

Um componente vai para `components/` quando **não conhece nenhuma entidade de negócio** e é (ou claramente será) usado por mais de uma feature. Se ele só serve a uma feature, fica em `features/.../components/`. Na dúvida, comece dentro da feature e promova para `components/` quando a segunda feature precisar; promover cedo demais cria componentes cheios de `if` para casos específicos.

Pastas atuais e o que cada uma recebe:

| Pasta | O que guarda | Exemplos |
|---|---|---|
| `components/atoms/` | Peças pequenas de UI, um elemento visual com props simples | `LoadingSpinner.jsx`, `LabelComInfo.jsx`, `SelectBuscaRemota.jsx` |
| `components/modal/` | Modais base reutilizáveis | `ModalPadrao.jsx` |
| `components/layout/` | Estrutura da aplicação (layouts por tipo de rota, cabeçalho, menu) | `LayoutAuthenticated.jsx`, `LayoutLogged.jsx`, `LayoutPublic.jsx`, `Header/AppHeader.jsx`, `Sidebar/Sidebar.jsx` |
| `components/servicesComponents/` | Componentes de infraestrutura sem UI própria | `ProtectedRoute.js` |

Uma pasta nova (ex.: `components/tabela/`, `components/formulario/`) só quando surgir um grupo de componentes que não cabe nas existentes; use nome em português, minúsculo, como as atuais.

Regras para um componente genérico:
- Nome em PascalCase e em português, arquivo `.jsx` com `export default`. Componente com CSS próprio ganha pasta própria (`Header/AppHeader.jsx` + `AppHeader.css`).
- Recebe tudo por props com nomes em português e valores padrão quando fizer sentido (como `modo = "edicao"` e `width = "min(800px, 95vw)"` no `ModalPadrao`); aceita `children` quando for um contêiner.
- Não chama a API, não usa `useAuth` nem guarda estado de negócio: quem decide o quê é a feature. Estado puramente visual (aberto/fechado, colunas visíveis) pode ficar no componente.
- Estilo com classes Tailwind ligadas ao tema (`text-verde-medio`, `border-verde-escuro`) ou tokens do antd; nunca hex solto no componente.
- Comentário curto no topo explicando para que serve e as props principais, porque é a documentação que o próximo dev vai ler.
- Ao alterar um componente genérico, procure todos os usos (`grep -rn "NomeDoComponente" src/`) e confirme que nenhuma tela quebra.

## 5. Camada de API (`src/api/`)

```
api/
├── index.js                 # instância axios (baseURL = REACT_APP_API_BASE_URL, withCredentials: true)
├── autenticacaoAPI.js       # login, logout, vínculos
├── sistema/<entidade>API.js             # ex.: clinicasAPI.js, usuariosDoSistemaAPI.js
└── clinica/<entidade>DaClinicaAPI.js     # ex.: animaisDaClinicaAPI.js, usuariosDaClinicaAPI.js
```

1. Adicione as rotas em `services/Endpoints.js`, em `sistema.<entidade>` ou `clinica.<entidade>`, sempre com o prefixo `${API_BASE_URL}` (`/api`). Rotas com parâmetro são funções, como `detalhes: (clinicaId) => ...` em `sistema.clinica`. Parâmetros de busca com `encodeURIComponent`.
2. Crie/atualize o arquivo da entidade exportando um objeto nomeado:
   ```js
   import api from "../index";
   import endpoints from "../../services/Endpoints";

   export const vacinasDaClinicaAPI = {
     listar: async () => {
       try {
         const response = await api.get(endpoints.clinica.vacina.listar);
         return response.data;
       } catch (error) {
         console.error("Erro ao buscar vacinas da clínica:", error);
         throw error.response ? error.response.data : error;
       }
     },
     // cadastrar, editar, deletar, setStatus...
   };
   ```
3. Verbos em português e consistentes: `listar(filtro)`, `opcoes(busca, ...)`, `detalhes`, `cadastrar`, `editar`, `deletar`, `setStatus(id, ativo)`, `buscarPorNome`. Listas paginadas recebem o filtro e enviam como `params` (`api.get(url, { params: filtro })`); status vai na query (`api.put(url, null, { params: { ativo } })`).
4. Sempre relance o erro (`throw error.response ? error.response.data : error`) para o Manager mostrar a mensagem; não engula erros com só um `console.error`.
5. Nunca use `axios` direto nem monte URL na feature: tudo passa por `api/index.js` e `Endpoints.js`, assim o prefixo `/api` e os cookies funcionam igual em produção e em homologação (`/dev`).
6. As URLs precisam bater com as rotas do controller no backend (ver skill `backend`).

## 6. Services (`src/services/`)

Estado global e integrações que não são da nossa API:
- `AuthContext.js`: `useAuth()` expõe `isLogged`, `isAuthenticated`, `isAdmin`, `perfis`, `userName`, `nomeClinica`, `checkAuth` e ações de vínculo (`escolherVinculoAdmin`...). Use para checar perfil na UI (esconder botões), lembrando que a segurança real é do backend.
- `NotificationContext.js`: `useNotification()` retorna `abrirNotificacao(tipo, titulo, descricao)` para avisos maiores; para feedback rápido de ação use `message` do antd.
- `Endpoints.js`: mapa único de rotas da API (ver seção 5).
- `viaCepService.js`: exemplo de integração externa. Uma API de terceiros nova ganha um `<nome>Service.js` aqui, com objeto exportado e tratamento de erro próprio.

Um contexto novo só quando o estado precisar ser compartilhado por várias telas; registre o Provider em `App.js`, no mesmo lugar dos atuais.

## 7. Utils (`src/utils/`)

Funções puras e helpers sem estado de negócio, agrupados por tipo:

| Arquivo | Conteúdo |
|---|---|
| `formatters.js` | Exibição: `formatarData`, `formatarCPF`, `formatarCNPJ`, `formatarTelefone`, `formatarCEP`, `formatarEspecie`, `formatarSexo`, `formatarGrauAtividade` |
| `masks.js` | Inputs com máscara para `Form.Item`: `CpfMaskInput`, `CnpjMaskInput`, `TelefoneMaskInput`, `CepMaskInput`, `CepSearchInput` |
| `validators.js` | `validarCPF`, `validarCNPJ` (usar em `rules` com `validator`) |
| `sanitizers.js` | Antes de enviar: `sanitizeToNumbers`, `sanitizePayload` |
| `renderers.js` | Renderizações reaproveitáveis em tabela/detalhes: `perfisSistema`, `renderPerfis`, `renderAnimais` |
| `useCepHandler.js` | Hook que preenche endereço pelo CEP (`handleCepSearch`, `loadingCep`) |
| `usePaginacaoServidor.js` | Hook de lista paginada no servidor: recebe `listar(filtro)` da api, filtros iniciais e `traduzirFiltros` (filtros de coluna → filtros do servidor); devolve `dados`, `loading`, `paginacao` (props da `Table`), `setFiltros`, `setBusca` (com debounce), `handleTableChange`, `recarregar` |

- Novo helper vai para o arquivo do tipo certo, com nome em português no padrão verbo + alvo (`formatarPeso`, `validarCRMV`) e JSDoc curto.
- Enum que vem do backend como número (ex.: espécie 1 = Cão) ganha um `formatarX` aqui em vez de `if` espalhado nas telas.
- Hook reutilizável de formulário segue o modelo `useCepHandler`: `use<Algo>.js`, recebe `form` e devolve handlers e loading.

## 8. Tema (`src/theme/`)

- `colors.js` é a única fonte da verdade das cores (`branco`, `cinza.*`, `verde.fraco|realce|claro|medio|escuro`, `vermelho.delete`). O `tailwind.config.js` importa esse arquivo, por isso existem classes como `text-verde-medio` e `bg-verde-fraco`.
- `ant-theme.js` (`themeConfig`) mapeia essas cores para os tokens e componentes do antd (`colorPrimary`, `Button`, `Menu`, `Sider`...). Para mudar a aparência de um componente do antd no app todo, ajuste aqui, não com CSS na tela.
- `validateMessages.js` tem as mensagens de validação em pt-BR usadas por todos os `Form`.
- `index.css` repete as cores como variáveis CSS (`--verde-medio`...) e define classes globais (`modal-scroll-container`, `modal-footer-actions`, `modal-title-icon`). Ao criar ou mudar uma cor, atualize `colors.js` **e** as variáveis de `index.css`, senão as duas formas de usar a cor ficam diferentes.
- Nas telas, prefira nesta ordem: token/componente do antd → classe Tailwind do tema → `var(--cor)`. Evite `style={{ color: "#..." }}`.

## 9. Padrões de listas, status e selects

- **Listas paginam no servidor** (contrato `FiltroBase`/`PaginaResultado` do backend: `{ itens, total, pagina, tamanhoPagina }`). O Manager usa `usePaginacaoServidor(api.listar, filtrosIniciais, traduzirFiltros)` e passa `paginacao` + `handleTableChange` para a `Table` (`pagination={paginacao}` e `onChange`). Tamanhos de página 10/20/50/100.
- Nas colunas de lista paginada, `sorter: true` (ordenação no servidor, `key` igual ao nome aceito pelo backend: `nome`, `dataCadastro`...) e `filters` + `filterMultiple: false` traduzidos por uma função `traduzirFiltros<Entidade>` exportada do `colunas.js`. Não use `sorter`/`onFilter` locais em lista paginada: só agiriam na página atual. Busca por texto num `Input.Search` acima da tabela chamando `setBusca`.
- A ordem padrão vem do backend (mais recentes primeiro); não force `defaultSortOrder` na tela.
- **Ativo/Inativo**: coluna "Status" com `Tag` verde/vermelha e filtro; botão de ativar/desativar na coluna Ações com `Popconfirm` e ícones `StopOutlined`/`CheckCircleOutlined` (modelo: `TabelaUsuariosDaClinica`, `TabelaAnimaisDaClinica`). O Excluir continua para cadastro errado; o backend recusa se estiver em uso.
- **Selects**: inativos aparecem **desabilitados** com o sufixo "(inativo)" (exceto o valor que o registro em edição já tem). Listas grandes usam `SelectBuscaRemota` com `buscarOpcoes={(busca) => api.opcoes(busca, ...)}` e `opcoesAtuais=[{ id, nome }]` do registro em edição.
- **Catálogo do sistema (raças, rações)**: na clínica, item do sistema mostra tag "Sistema"; editar cria cópia da clínica; em vez de excluir/desativar, a clínica pode "Ocultar/Mostrar para esta clínica". No Sistema (admin), a tela tem o botão "Cópias das clínicas", que troca a lista para os itens próprios das clínicas (com coluna Clínica).
- Todas as listas seguem o contrato paginado, exceto Usuários do Sistema (pendente). Selects com `SelectBuscaRemota`: tutores e raças (animal), animais (tutor), animal e tutor (consulta), rações (avaliação e dieta), clínicas (usuário do sistema). Em `mode="multiple"` o componente guarda o rótulo dos itens já escolhidos.

## 10. Registrar a tela

- `App.js`: importar a página e adicionar `<Route>` no bloco de guarda certo (tabela da seção 1).
- Clínica: adicionar item no `Sidebar.jsx` (`key` = path, `label` = `<NavLink>`).

## 11. Checklist final
- [ ] Escopo certo (Sistema x Clínica) e nomes com `DoSistema` / `DaClinica`.
- [ ] Página sem lógica; lógica toda no Manager.
- [ ] Reaproveitou `components/`, `utils/` e `services/` antes de criar algo novo; o que é genérico foi para `components/` sem regra de negócio.
- [ ] Endpoints + arquivo de API + rota + menu registrados.
- [ ] Nenhum `clinicaId` enviado pelo frontend em telas de Clínica.
- [ ] Lista nova paginada no servidor (`usePaginacaoServidor`), com status Ativo/Inativo, toggle com `Popconfirm` e selects mostrando inativos desabilitados.
- [ ] Cores só via tema; textos em português; feedback com `message.success/error`.
- [ ] Rodar `npm start` em `frontend/` e testar cadastro, edição, detalhes e listagem.

Não altere nada em `bris-web` nem em `https-server`.
