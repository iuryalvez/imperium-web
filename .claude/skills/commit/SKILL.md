---
name: commit
description: Use quando o usuário pedir para preparar, revisar ou fazer um commit no Imperium, ou pedir uma mensagem de commit. Explica como separar o que é da tarefa, escrever a mensagem no estilo do projeto e o que nunca fazer (push, commitar sem pedido, incluir .env).
---

# Commit no Imperium

Commit só acontece quando o usuário pede explicitamente. Sem esse pedido, deixe tudo no working tree e, se ajudar, sugira a mensagem.

## 1. Veja o estado

- Rode `git status` e `git branch` e diga em qual branch está e o que mudou.
- Rode `git diff` (e `git diff --staged`, se já houver algo no stage) para ler as mudanças.

## 2. Separe o que é da tarefa

- Liste os arquivos que pertencem à tarefa atual e os que não pertencem.
- Alerte o usuário se aparecer `.env`, `bin/`, `obj/`, `node_modules/`, `build/`, arquivos de IDE ou mudanças de outra tarefa. Esses nunca entram no commit.
- Se o diff mistura assuntos (por exemplo backend e frontend independentes, ou refatoração e funcionalidade), sugira dividir em commits menores e diga quais arquivos iriam em cada um.

## 3. Escreva a mensagem

- Olhe `git log --oneline -15` e siga o estilo usado no projeto.
- Se não houver estilo claro, use `tipo(parte): resumo`, em português, no imperativo, com até 72 caracteres. Tipos: `feat`, `fix`, `refactor`, `docs`, `chore`, `style`, `test`. Exemplo: `feat(clinica): adiciona cadastro de vacinas`.
- No corpo, explique em poucas linhas o porquê da mudança, não o que cada arquivo faz.
- Avise no corpo se o commit traz migration nova ou se o usuário precisa mudar algo no `.env` (sem copiar valores).

## 4. Faça o commit, só se pedido

- Use `git add` só nos arquivos da tarefa, um a um ou por pasta específica. Nunca `git add .` ou `git add -A`.
- Mostre ao usuário a lista de arquivos e a mensagem antes de commitar, se ainda não tiver mostrado.
- Depois do commit, rode `git status` e `git log --oneline -3` e relate o resultado.
- Nunca faça push, merge, rebase nem `--amend` sem pedido explícito. O usuário envia pelo GitHub Desktop.
