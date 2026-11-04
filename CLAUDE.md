# Imperium Web

Antes de qualquer tarefa, leia a skill `contexto` (`.claude/skills/contexto/SKILL.md`). Ela explica o que é o sistema, os perfis, as entidades e o que nunca pode quebrar.

O Imperium é um sistema web multi-tenant para clínicas veterinárias, parte do ecossistema "software rural" (EVZ/INF-UFG). O Bris (`bris-web`) e o proxy `https-server` dividem o mesmo servidor, mas só alteramos o Imperium.

## Qual guia usar

- `contexto`: sempre, antes de começar. Domínio, perfis, entidades e regras que não podem quebrar.
- `tarefa`: para entender o tipo de pedido (investigação, plano, implementação, correção, revisão), o que decidir sozinho e o que perguntar, e como fechar com o resumo final.
- `backend`: ao mexer em `backend/` (controllers, services, repositories, DTOs, entidades, migrations, policies).
- `frontend`: ao mexer em `frontend/` (telas, componentes, api, tema, utils).
- `testes`: ao criar ou rodar testes automatizados.
- `teste-visual`: para conferir uma tela no navegador ou entregar um roteiro de teste manual.
- `commit`: quando o usuário pedir para preparar ou fazer um commit.

## Quando não houver exemplo no projeto

Primeiro procure um exemplo vivo no código e siga o mesmo padrão. Se não houver, siga a prática de mercado da stack, mantendo a estrutura do projeto (Controller, Service e Repository no backend; página e Manager no frontend) e as regras de tenant. Explique a abordagem ao usuário e espere aprovação antes de ela virar padrão.

## Ecossistema e deploy

- O `https-server` é um Nginx único na rede Docker `software-rural-network` e atende `bris.evz.ufg.br` e `imperium.evz.ufg.br`.
- O Imperium de produção fica em `/` e `/api/`; o de homologação em `/dev/` e `/dev/api/`.
- Por isso as rotas da API ficam sempre sob `/api/`, e os nomes dos containers (`backend-imperium`, `frontend-imperium` e os `*-homolog`) não mudam.

## Stack

- Backend em `backend/`: .NET 10, EF Core 10 com PostgreSQL, AutoMapper 16 e JWT em cookie.
- Frontend em `frontend/`: React 18 (Create React App), Ant Design 5, Tailwind 3 e axios.

## Conceitos do domínio

- Sistema é a administração dos tenants (admin do Imperium). Clínica é o tenant.
- Perfis na clínica: GESTOR, VETERINARIO, SECRETARIO e TECNICO. O admin do sistema vem da claim `isAdmin`.
- O `clinicaId` vem sempre do token JWT no backend. O frontend nunca envia `clinicaId`.

## Comandos

Use sempre o Makefile, que carrega o `.env` e exporta as variáveis (banco, JWT, Swagger). `dotnet run` e `dotnet ef` fora dele falham ou usam configuração errada.

- Rodar a API: `make dev-up-api`.
- Rodar o frontend: `make dev-up-frontend`.
- Criar migration: `make dev-add-migration` (pede o nome de forma interativa), só com permissão.
- Aplicar migrations no banco local: `make dev-database-update`, só com permissão.
- Verificação rápida do backend sem banco: `cd backend && dotnet build`.
- Verificação do frontend: `cd frontend && npm run build`.
- Testes do backend: `make test` (precisa do Docker; não usa o `.env` nem o banco local).
- Testes do frontend: `make test-frontend` (Jest; não usa o `.env` nem o backend).

Subir API, frontend ou containers exige permissão do usuário.

## Git

- `main` é produção e `dev` é homologação. As branches de tarefa são criadas pelo usuário.
- O merge é feito pelo usuário no GitHub Desktop, sem Merge Request.
- Remotes: `origin` é o GitLab do projeto (oficial); `github` é o portfólio pessoal (só `main` e `dev`, atualizado pelo usuário).
- Claude não faz commit, push, merge, nem cria ou troca de branch, a menos que o usuário peça explicitamente. As alterações ficam no working tree para revisão. Para commits, veja a skill `commit`.

## Regras críticas

- `.env`: não ler nem alterar. Se precisar de alguma informação dele, pergunte ao usuário; se precisar mudar algo, diga o que e onde mudar.
- EF 10: `HasData` e seeds só com valores estáticos (nada de `DateTime.UtcNow`, `Guid.NewGuid()` ou hash com salt).
- Antes de concluir uma mudança no backend, confira se o modelo mudou e se precisa de migration. Migrations e alterações no banco só com permissão.
- Não alterar rotas existentes sem combinar, porque o frontend (`services/Endpoints.js`) depende delas.
- Apagar arquivos, instalar ou atualizar dependências e mexer fora da tarefa pedem permissão.

## Definição de pronto

- `dotnet build` com 0 erros e sem warnings novos.
- O frontend compila (`npm run build` em `frontend/`) e `make test-frontend` passa quando houver mudança nele.
- As rotas do backend batem com `frontend/src/services/Endpoints.js`.
- Mudança de tela conferida pelo teste visual ou com roteiro entregue ao usuário.
- `make test` passando (testes do backend em `backend.Tests`, precisa do Docker; ver skill `testes`). Mudança no backend vem com teste da regra.
