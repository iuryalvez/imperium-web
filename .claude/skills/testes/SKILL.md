---
name: testes
description: Use ao criar, alterar ou rodar testes automatizados do Imperium (backend com xUnit, frontend com Jest e Testing Library), ou quando a tarefa pedir para cobrir uma regra com teste. Explica a estrutura combinada, o que cobrir e o que exige permissão.
---

# Testes automatizados do Imperium

## Situação atual

O projeto ainda não tem testes automatizados. No frontend existe só o `src/App.test.js` padrão do Create React App, que está quebrado (procura um texto "learn react" que não existe).

Criar o projeto de testes, instalar pacotes ou mudar `App.test.js` exige permissão do usuário. Antes disso, explique o que será criado e quais pacotes entram.

## Backend

Estrutura combinada:

- Projeto `backend.Tests` ao lado de `backend/`, com xUnit.
- Testes de integração da API com `WebApplicationFactory<Program>`, chamando as rotas reais.
- Banco real com Testcontainers para PostgreSQL. Precisa do Docker rodando e não usa o banco local nem o `.env`.
- Tokens de teste gerados com uma chave JWT só de teste, definida na configuração do próprio projeto de testes. Nunca use a chave real.
- Rodar com `dotnet test`.

O que cobrir em cada funcionalidade:

- Caminho feliz: cadastrar, listar, detalhar, editar, mudar status e excluir.
- Isolamento: um usuário da clínica A não lê, edita, exclui nem vincula dados da clínica B (deve receber "não encontrado" ou 403).
- Perfis: perfil sem permissão recebe 403 (por exemplo TECNICO tentando cadastrar animal).
- Validação: entrada inválida recebe 400 com mensagem em português.
- Regras de negócio: CPF único por clínica, item inativo recusado em registro novo, item oculto recusado, exclusão bloqueada quando em uso, cópia ao editar item do sistema, edição parcial que não zera campos.
- Paginação: total, página, tamanho máximo, busca, ordenação e mais recentes primeiro.

Nome dos testes no formato `Metodo_Situacao_Resultado`, por exemplo `Cadastrar_CpfJaExisteNaClinica_RetornaBadRequest`.

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
