---
name: testes
description: Use ao criar, alterar ou rodar testes automatizados do Imperium (backend com xUnit, frontend com Jest e Testing Library), ou quando a tarefa pedir para cobrir uma regra com teste. Explica a estrutura combinada, o que cobrir e o que exige permissão.
---

# Testes automatizados do Imperium

## Situação atual

- Backend: existe o projeto `backend.Tests/` (xUnit) com testes de integração e unitários. Rode com `make test`.
- Frontend: tem testes com Jest e Testing Library (vêm com o Create React App) em `utils/` e em todos os hooks `use...Manager` e de Auth (`useRecuperarSenha`, `useNovaSenha`). Rode com `make test-frontend`.

Instalar pacotes ou mudar `App.test.js` exige permissão do usuário. Antes, explique o que será criado e quais pacotes entram.

## Backend

### Como rodar

- `make test`: roda `dotnet test` em `backend.Tests` com o ambiente limpo (`env -i`), para nada do `.env` chegar aos testes.
- Precisa do Docker rodando: o Testcontainers sobe um `postgres:16-alpine` descartável uma vez por execução e o apaga no fim. Não usa o banco local, o de homologação nem o `.env`.
- Para rodar um teste só: `cd backend.Tests && dotnet test --filter "FullyQualifiedName~NomeDoTeste"`.
- O projeto está no `backend/backend.sln`, então `cd backend && dotnet build` compila os testes também.

### Estrutura

- `backend.Tests/Infra/ImperiumApiFactory.cs`: `WebApplicationFactory<Program>` que sobe a API real apontando para o Postgres do Testcontainers e aplica as migrations. A configuração (connection string, `Jwt__*` com chave só de teste, `CookieSecure`, `SwaggerAuth`) entra por variáveis de ambiente de teste. Todas as classes de teste usam `[Collection(ApiCollection.Nome)]`, que compartilha essa factory.
- `Infra/TokenDeTeste.cs`: gera os mesmos tokens do `TokenService` (vínculo da clínica, admin simulando clínica e login do admin no sistema). O token vai no cookie `auth_token` e leva o `SecurityStamp` do usuário (claim `stamp`), porque o `ValidateService` confere no banco, a cada requisição, usuário ativo, carimbo, vínculo e perfis. Cada cliente de teste envia um `X-Real-IP` próprio (rate limit por IP).
- `Infra/ServicosFalsos.cs`: `TurnstileFalso` (captcha sempre válido, menos `TokenInvalido`) e `EmailFalso` (guarda os e-mails em `factory.Emails`, com `EsperarAsync` porque o envio é em segundo plano).
- `Infra/Cenario.cs`: cria direto no banco clínicas com nome único e usuários com vínculo por perfil, e devolve clientes autenticados: `ClienteAsync(clinica, perfis)`, `UsuarioDaClinicaAsync` (também devolve ids do vínculo e do usuário), `ClienteAdminSimulandoAsync`, `ClienteAdminSistemaAsync` e `ClienteAnonimo`. Também tem o `GeradorCpf` (CPF válido) e as constantes de `Perfis`.
- `Infra/ApiDeTeste.cs`: monta os cenários pelas rotas reais (`CadastrarTutorAsync`, `CadastrarAnimalAsync`, `CadastrarRacaDaClinicaAsync`, `CadastrarRacaDoSistemaAsync`, `CadastrarRacaoDaClinicaAsync`, `CadastrarRacaoDoSistemaAsync`, `CadastrarConsultaAsync`) e lê JSON (`LerAsync<T>`, `LerIdDoDataAsync`, `ResultadoComDados<T>`). `RegistrarAvaliacaoComRacaoAsync` grava uma avaliação mínima no banco para deixar uma ração "em uso".
- Uma pasta por entidade, com `<Entidade>Tests.cs`: `Tutores`, `Animais`, `Racas`, `Racoes`, `Consultas`, `Sistema` (catálogo do admin), `Clinicas` (dashboard, dados da clínica e clínicas do admin) e `Usuarios` (vínculos).
- `Unitarios/`: testes sem banco nem API, para lógica pura (hoje, o cálculo de necessidade energética, com valores do NRC 2006 calculados à parte).

### Regras para escrever testes

- Cada teste cria os próprios dados (clínica nova por teste); nada depende de ordem nem de outro teste.
- O catálogo do sistema (raças e rações com `ClinicaId` nulo) e as seeds são compartilhados por todos os testes. Em listas e opções que trazem itens do sistema, filtre por nome único (`Cenario.Unico()`) ou por `?origem=clinica`.
- Use só os asserts do xUnit. Depois de uma tentativa negada (403, 404 de outra clínica), confira também que o dado não mudou.
- Testes usam só dados fictícios.

O que cobrir em cada funcionalidade:

- Caminho feliz: cadastrar, listar, detalhar, editar, mudar status e excluir.
- Isolamento: um usuário da clínica A não lê, edita, exclui nem vincula dados da clínica B (deve receber 404, e o dado de A continua igual).
- Perfis: perfil sem permissão recebe 403 (por exemplo SECRETARIO tentando cadastrar animal).
- Códigos de erro (ver skill `backend`): 404 para registro da rota inexistente ou de outra clínica, 409 para duplicado ou item em uso, 400 com mensagem em português para validação e regras.
- Regras de negócio: CPF único por clínica, item inativo recusado em registro novo, item oculto recusado, exclusão bloqueada quando em uso, cópia ao editar item do sistema, edição parcial que não zera campos.
- Paginação: total, página, tamanho máximo, busca, ordenação e mais recentes primeiro.

Nome dos testes no formato `Metodo_Situacao_Resultado`, por exemplo `Cadastrar_CpfJaExisteNaClinica_RetornaConflict`.

## Frontend

### Como rodar

- `make test-frontend`: roda `npm test` em `frontend/` uma vez (`CI=true`, `--watchAll=false`, `--maxWorkers=2`), com o ambiente limpo (`env -i`), sem o `.env`. Leva poucos segundos e não precisa do backend nem do Docker.
- Para rodar um arquivo só: `cd frontend && CI=true npm test -- --watchAll=false src/utils/validators.test.js`.
- `npm test` sem argumentos fica em modo watch (para usar enquanto escreve testes).
- Pacotes já instalados: `@testing-library/react` 13.4 (tem `renderHook`), `@testing-library/jest-dom` 5 (configurado em `src/setupTests.js`) e `@testing-library/user-event` 13 (API síncrona: `userEvent.type(input, "texto")`, sem `await` nem `setup()`). Não instale nem atualize sem permissão.

### Estrutura

- O teste fica ao lado do arquivo testado, com o mesmo nome e `.test.js` (padrão do CRA): `utils/validators.test.js`, `features/Clinica/TutoresDaClinica/useTutoresDaClinicaManager.test.js`.
- `utils/`: testes unitários puros (validators, formatters, sanitizers, permissoes) e os inputs com máscara (`masks.js`), testados pelo que o usuário digita e pelo valor sem máscara entregue ao `onChange`. `permissoes.test.js` confere que `PERMISSOES` espelha as policies do `Program.cs`: se mudar uma policy no backend, o teste mostra o espelho desatualizado.
- Managers (`use...Manager`): `renderHook` + `act` + `waitFor`, com a camada `api/` mockada por inteiro e só o `message` do antd trocado. Exemplo vivo: `useTutoresDaClinicaManager.test.js`. Todo hook novo de tela ganha um `use...Manager.test.js` ao lado.
- Outras dependências que os hooks usam e que se mocka do mesmo jeito (variáveis usadas na factory do `jest.mock` começam com `mock`):
  - `useAuth`: `jest.mock("../../../services/AuthContext", () => ({ useAuth: jest.fn() }))` e `useAuth.mockReturnValue({ perfis: [...] })` no teste. O `AuthContext` real importa a api de autenticação (axios), então nunca o carregue de verdade.
  - React Router: `jest.mock("react-router-dom", () => ({ ...jest.requireActual("react-router-dom"), useNavigate: () => mockNavigate, useParams: () => ({ acompanhamentoId: "30" }) }))`.
  - Notificações das telas de Auth (`App.useApp()`): `jest.mock("antd", () => ({ ...jest.requireActual("antd"), App: { useApp: () => ({ notification: mockNotification }) } }))`.

```js
jest.mock("../../../api/clinica/tutoresDaClinicaAPI.js", () => ({
  tutoresDaClinicaAPI: { listar: jest.fn(), cadastrar: jest.fn(), editar: jest.fn(), deletar: jest.fn(), setStatus: jest.fn() },
}));
jest.mock("antd", () => ({ ...jest.requireActual("antd"), message: { success: jest.fn(), error: jest.fn() } }));
```

- Nunca importe a api real nem o `App` num teste: o axios é ESM e o Jest do CRA não o transforma ("Cannot use import statement outside a module"). Mocke a camada `api/` com factory, como acima.

### O que cobrir num Manager

- Carga da lista: chama a api com `{ pagina: 1, tamanhoPagina: 10 }`; busca (debounce), página, ordenação e filtros de coluna viram os parâmetros certos.
- Cadastro, edição, exclusão e status: chamam a função certa da api com o payload certo (documentos sem máscara, campos vazios como `""`), mostram a mensagem de sucesso, fecham o modal e recarregam a lista.
- Erro do backend: mostra a `message` que veio no corpo, mantém o modal aberto, não recarrega e libera o loading; sem mensagem, usa o texto padrão.

### O erro que o Manager recebe

- Os mocks rejeitam com o corpo do backend (`{ message: "..." }`), que é o contrato da camada `api/` (skill `frontend`: `throw error.response ? error.response.data : error`). Se uma api relançar o erro bruto do axios, a tela mostra "Request failed with status code 409" em vez da mensagem do backend: corrija a api, não o teste.

### Próximos passos

- Componentes genéricos de `components/` (pelas props e pelo que o usuário vê). O Login não tem hook próprio (usa o `login` do `AuthContext`); testar o `AuthContext` exige mockar o `autenticacaoAPI` inteiro. Componentes do Ant Design com grid ou responsivos podem precisar de um mock de `window.matchMedia` no `setupTests.js`; adicione só quando precisar.
- Avisos "not wrapped in act(...)" e "ReactDOMTestUtils.act is deprecated" vêm da combinação Testing Library 13 + React 18.3; não são falhas.

## Regras

- Nunca pule, desative ou enfraqueça um teste para ele passar. Se um teste falha, investigue e conte ao usuário o que encontrou.
- Testes usam só dados fictícios.
- Relate o resultado com a saída real do comando (quantos passaram e falharam).
