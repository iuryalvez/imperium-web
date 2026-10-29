---
name: teste-visual
description: Use para conferir no navegador uma tela do Imperium depois de uma mudança no frontend (ou no backend que afete uma tela), ou para entregar ao usuário um roteiro de teste manual. Explica como combinar ambiente e login e o roteiro do que conferir.
---

# Teste visual do Imperium

## 1. Prepare o ambiente

- Peça permissão antes de subir qualquer coisa. Com o sim, use `make dev-up-api` e `make dev-up-frontend`. Se o usuário já estiver com eles rodando, use os dele.
- Combine qual banco será usado (local, com dados fictícios) e com qual usuário e perfil testar.
- O login tem Cloudflare Turnstile. Quem faz o login é o usuário. Nunca peça nem digite senha.
- Use o navegador disponível na sessão. Se não houver navegador ou o usuário preferir, entregue o roteiro abaixo para ele seguir.

## 2. Roteiro

Adapte à tela mudada e confira:

- Navegação: a tela abre pelo menu ou atalho certo, e o item de menu aparece só para os perfis permitidos.
- Listagem: dados carregam, busca funciona, paginação e troca de tamanho funcionam, ordenação e filtros vão para o servidor, e a lista vazia mostra mensagem adequada.
- Cadastro: campos obrigatórios avisam, máscaras de CPF, CNPJ, telefone e CEP funcionam, o CEP preenche o endereço, e o sucesso mostra mensagem e atualiza a lista.
- Edição: o formulário abre preenchido, salvar só muda o que foi alterado, e campo limpo fica realmente vazio.
- Detalhes: datas, documentos, enums e status aparecem formatados.
- Status e exclusão: ativar e desativar com confirmação, tag de status correta, e excluir item em uso mostra a mensagem do backend.
- Erros da API: mensagens em português aparecem para o usuário, sem tela quebrada.
- Perfis sem permissão: botões escondidos e, se forçar a ação, o backend responde 403.
- Largura de celular (cerca de 375 px): sem rolagem horizontal da página e modais utilizáveis.
- Cores: só as do tema (verdes, cinzas, vermelho de exclusão), sem cor solta.

Abra também o console e a aba de rede:

- Console sem erros nem warnings novos.
- Requisições indo para `/api/...` (ou `/dev/api/...` em homologação), com status esperado.
- Nenhum `clinicaId` no corpo ou na query das requisições de telas da clínica.

## 3. Entrega

Relate item a item o que passou, o que falhou (com o passo para reproduzir) e o que não foi possível testar e por quê. Se subiu API ou frontend, pergunte se pode derrubar ao terminar.
