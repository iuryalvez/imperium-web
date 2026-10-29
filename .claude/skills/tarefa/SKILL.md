---
name: tarefa
description: Use no início e no fim de qualquer pedido de trabalho no Imperium. Explica os tipos de tarefa (investigação, plano, implementação, correção, revisão) e o que cada um entrega, o que fazer antes de codar, o que decidir sozinho ou perguntar, e o formato do resumo final.
---

# Como conduzir uma tarefa no Imperium

## Tipos de tarefa

- Investigação: responder uma pergunta sobre o código ou o comportamento. Entrega a resposta com os arquivos e linhas que a sustentam. Não altera nada.
- Plano: propor como fazer algo. Entrega os passos, os arquivos afetados, as decisões em aberto com uma recomendação e os riscos (migration, rota alterada, impacto em outras telas). Não altera nada.
- Implementação: construir algo novo. Entrega o código no working tree, verificado com build, e o roteiro de teste.
- Correção: consertar um bug. Entrega a causa encontrada, a correção mínima e como confirmar que foi resolvido.
- Revisão: avaliar código existente ou um diff. Entrega os problemas encontrados por gravidade, com arquivo e linha, sem alterar nada a menos que pedido.

Se o pedido não deixar claro o tipo, assuma o menos invasivo (investigação ou plano) e diga isso.

## Antes de codar

1. Rode `git status` e `git branch`. Diga a branch e se há alterações não commitadas. Se houver alterações do usuário nos arquivos que vai mexer, pergunte antes.
2. Leia a skill `contexto` e a skill da parte que vai mudar (`backend`, `frontend`, `testes`).
3. Ache um exemplo vivo no projeto que faça algo parecido e siga o mesmo padrão. Se não houver, siga a regra "Quando não houver exemplo no projeto" da skill da parte.
4. Junte as perguntas em uma vez só, cada uma com uma opção recomendada e as alternativas.
5. Anote as suposições que fizer, para relatar no fim.

## O que decidir sozinho, o que perguntar e o que nunca fazer

Decida sozinho:

- Nomes, pastas e detalhes de implementação que seguem o padrão das skills.
- Mensagens de validação e de erro em português.
- Pequenas melhorias dentro dos arquivos da tarefa, desde que relatadas.

Pergunte antes:

- Criar migration ou alterar o banco.
- Mudar rota existente, contrato de API ou regra de negócio.
- Apagar arquivos, instalar ou atualizar dependências.
- Subir ou derrubar API, frontend ou containers.
- Mexer em arquivos fora da tarefa ou criar padrão novo.
- Escolher entre caminhos com efeitos diferentes para o usuário final.

Nunca faça:

- Ler ou alterar o `.env`, ou copiar segredos.
- Commit, push, merge, rebase ou criar e trocar de branch sem pedido explícito.
- Alterar o Bris ou o `https-server`.
- Enfraquecer o isolamento entre clínicas ou aceitar `clinicaId` do cliente.
- Criar cópias paralelas de arquivos (como `arquivo-v2`) em vez de editar o original.

## Durante

- Rode `cd backend && dotnet build` a cada passo relevante no backend e `npm run build` em `frontend/` ao fim das mudanças no frontend.
- Se algo inesperado aparecer (bug fora do escopo, padrão divergente), anote e siga a tarefa; não conserte por conta própria o que está fora dela.

## Resumo final

Feche toda tarefa com um resumo curto contendo:

- Estado: pronto, parcial ou bloqueado.
- Branch e se há alterações não commitadas.
- O que foi feito, arquivo por arquivo em uma linha cada.
- Como conferir: comandos rodados e resultado, e o roteiro de teste visual quando houver tela.
- O que o usuário precisa fazer: migration, mudança no `.env`, commit, teste manual.
- Suposições feitas.
- Achados fora do escopo, com sugestão.
- Se alguma skill ou o `CLAUDE.md` ficou desatualizado e precisa de ajuste.
