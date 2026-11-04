---
name: testes
description: Use ao criar, alterar ou rodar testes automatizados do Imperium (backend com xUnit, frontend com Jest e Testing Library), ou quando a tarefa pedir para cobrir uma regra com teste. Explica a estrutura combinada, o que cobrir e o que exige permissão.
---

# Testes automatizados do Imperium

## Situação atual

- Backend: existe o projeto `backend.Tests/` (xUnit) com testes de integração e unitários. Rode com `make test`.
- Frontend: ainda não tem testes. Existe só o `src/App.test.js` padrão do Create React App, que está quebrado (procura um texto "learn react" que não existe).

Instalar pacotes ou mudar `App.test.js` exige permissão do usuário. Antes, explique o que será criado e quais pacotes entram.

## Backend

### Como rodar

- `make test`: roda `dotnet test` em `backend.Tests` com o ambiente limpo (`env -i`), para nada do `.env` chegar aos testes.
- Precisa do Docker rodando: o Testcontainers sobe um `postgres:16-alpine` descartável uma vez por execução e o apaga no fim. Não usa o banco local, o de homologação nem o `.env`.
- Para rodar um teste só: `cd backend.Tests && dotnet test --filter "FullyQualifiedName~NomeDoTeste"`.
- O projeto está no `backend/backend.sln`, então `cd backend && dotnet build` compila os testes também.

### Estrutura

- `backend.Tests/Infra/ImperiumApiFactory.cs`: `WebApplicationFactory<Program>` que sobe a API real apontando para o Postgres do Testcontainers e aplica as migrations. A configuração (connection string, `Jwt__*` com chave só de teste, `CookieSecure`, `SwaggerAuth`) entra por variáveis de ambiente de teste. Todas as classes de teste usam `[Collection(ApiCollection.Nome)]`, que compartilha essa factory.
- `Infra/TokenDeTeste.cs`: gera os mesmos tokens do `TokenService` (vínculo da clínica, admin simulando clínica e login do admin no sistema). O token vai no cookie `auth_token`, e o cliente envia `User-Agent` e `X-Forwarded-For` iguais às claims, porque o `ValidateService` confere IP e navegador.
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

- Jest e Testing Library, que já vêm com o Create React App. Rodar com `npm test` em `frontend/`.
- Testar `utils/` (formatters, validators, sanitizers) com testes unitários simples.
- Testar Managers com a camada `api/` mockada: estados de loading, submit, mensagens de sucesso e erro.
- Testar componentes genéricos de `components/` pelas props e pelo que o usuário vê.
- Consertar ou substituir o `App.test.js` padrão faz parte de começar os testes do frontend (com permissão).

## Regras

- Nunca pule, desative ou enfraqueça um teste para ele passar. Se um teste falha, investigue e conte ao usuário o que encontrou.
- Testes usam só dados fictícios.
- Relate o resultado com a saída real do comando (quantos passaram e falharam).
