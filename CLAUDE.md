# Imperium Web

Sistema web multi-tenant para clínicas veterinárias, parte do ecossistema "software rural" (EVZ/INF-UFG).
O Bris (`bris-web`) e o proxy `https-server` dividem o mesmo servidor: **só alteramos o Imperium**.

## Ecossistema e deploy
- `https-server`: Nginx único na rede Docker `software-rural-network`, atende `bris.evz.ufg.br` e `imperium.evz.ufg.br`.
- Imperium produção em `/` e `/api/`; homologação em `/dev/` e `/dev/api/`.
- Por isso: rotas da API sempre sob `/api/`, e os nomes de containers (`backend-imperium`, `frontend-imperium`, `*-homolog`) não mudam.

## Stack
- Backend: .NET 10, EF Core 10 + PostgreSQL, AutoMapper 16, JWT em cookie (`backend/`).
- Frontend: React 18 (CRA), Ant Design 5, Tailwind 3, axios (`frontend/`).
- Detalhes de arquitetura e padrões estão nas skills `backend` e `frontend` (`.claude/skills/`). Use-as ao mexer em cada parte.

## Conceitos do domínio
- **Sistema** = administração dos tenants (admin do Imperium). **Clínica** = o tenant.
- Perfis na clínica: GESTOR, VETERINARIO, SECRETARIO, TECNICO; admin do sistema via claim `isAdmin`.
- O `clinicaId` vem sempre do token JWT no backend. O frontend nunca envia `clinicaId`.

## Comandos (sempre pelo Makefile)
O Makefile carrega o `.env` e exporta as variáveis (banco, JWT, Swagger). Comandos `dotnet run` e `dotnet ef` fora dele falham ou usam config errada.
- Rodar API: `make dev-up-api`
- Rodar frontend: `make dev-up-frontend`
- Criar migration: `make dev-add-migration` (pede o nome de forma interativa)
- Aplicar migrations no banco local: `make dev-database-update`
- Verificação rápida sem banco: `cd backend && dotnet build`

## Fluxo de git
- `main` = produção; `dev` = homologação. Branches de tarefa são criadas pelo usuário.
- Merge feito pelo usuário no GitHub Desktop (sem Merge Request).
- Remotes: `origin` = GitLab do projeto (oficial); `github` = portfólio pessoal (só `main` e `dev`, atualizado pelo usuário).
- Claude não faz commit, push, merge, nem cria ou troca de branch. Deixa as alterações no working tree para revisão.

## Regras críticas
- `.env`: não ler nem alterar. Se precisar de alguma informação dele, pergunte ao usuário; se precisar mudar algo, diga o que e onde mudar.
- EF 10: `HasData`/seed só com valores estáticos (nada de `DateTime.UtcNow`, `Guid.NewGuid()`, hash com salt).
- Antes de concluir uma mudança no backend, confira se o modelo mudou e se precisa de migration. Migrations e alterações no banco só com permissão.
- Não alterar rotas existentes sem combinar: o frontend (`services/Endpoints.js`) depende delas.

## Definição de pronto
- `dotnet build` com 0 erros e sem warnings novos.
- Frontend compila (`npm run build` em `frontend/`) quando houver mudança nele.
- Rotas do backend batem com `frontend/src/services/Endpoints.js`.
- Testes automatizados: ainda não existem (pendência do projeto).
