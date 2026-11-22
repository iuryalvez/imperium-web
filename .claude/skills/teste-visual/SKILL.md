---
name: teste-visual
description: Use para conferir no navegador uma tela do Imperium depois de uma mudança no frontend (ou no backend que afete uma tela), ou para entregar ao usuário um roteiro de teste manual. Explica como combinar ambiente e login, o roteiro do que conferir e os casos da literatura (exemplos do NRC, FEDIAF, WSAVA etc.) para conferir cálculo de dieta, ração e avaliação.
---

# Teste visual do Imperium

## 1. Prepare o ambiente

- Peça permissão antes de subir qualquer coisa. Com o sim, use `make dev-up-api` e `make dev-up-frontend`. Se o usuário já estiver com eles rodando, use os dele.
- Combine qual banco será usado (local, com dados fictícios) e com qual usuário e perfil testar.
- No banco local nada é real: crie quantos animais, consultas, avaliações e dietas o teste pedir, e atualize os que já existem se precisar. Uma avaliação que já tem dieta não pode ser editada, então casos novos (ECC, gestação, emagrecimento) pedem animal e consulta novos. O jeito mais rápido é a API, pela sessão aberta no navegador: `fetch` com `credentials: "include"` em `POST /api/clinica/animais/cadastrar`, `POST /api/clinica/consultas/cadastrar` e `PUT /api/clinica/consultas/{id}/avaliacao` (corpos como em `backend.Tests/Infra/ApiDeTeste.cs`). Ponha "(teste ...)" no nome. A dieta, calcule pela tela, que é o que se quer ver. Em `/dev` ou em produção, pergunte antes.
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
  - O token não é mais amarrado ao User-Agent nem ao IP, então emular celular (preset mobile) não derruba a sessão. Se preferir manter a aba em largura de desktop, a alternativa continua valendo: abra a tela num iframe de 375 px na mesma aba (`document.body.innerHTML = ''` e um `<iframe src="/rota" style="width:375px;height:812px">`).
- Tablet (768 a 991 px): sidebar só com ícones e tabela preenchendo a altura, com rolagem só dentro dela.
- Cores: só as do tema (verdes, cinzas, vermelho de exclusão), sem cor solta.

Abra também o console e a aba de rede:

- Console sem erros nem warnings novos.
- Requisições indo para `/api/...` (ou `/dev/api/...` em homologação), com status esperado.
- Nenhum `clinicaId` no corpo ou na query das requisições de telas da clínica.

## 3. Casos da literatura (cálculo de dieta, ração e avaliação)

Quando a mudança mexer em cálculo energético, ração, formulário de dieta, avaliação ou ECC, confira também os casos de `casos-literatura.md`, nesta pasta. Eles vêm de exemplos resolvidos da literatura usada na monografia (NRC/Carciofi, Ogoshi, FEDIAF, ABINPET, Guimarães e Tudury, WSAVA e outras), cada um com fonte, página, entrada e valor esperado.

- Rode os casos do tipo de dieta que mudou, mais um caso de ração. Tolerância de 1 %, porque as fontes arredondam.
- Os casos de cálculo já rodam no `make test` (`CasosLiteraturaTests` e `RacaoLiteraturaTests`). No navegador, confira que a tela mostra o mesmo valor, a fórmula certa e os limites do formulário.
- Se um caso que batia passar a divergir, pare e mostre ao usuário: a fonte é o gabarito. Os casos que já divergem estão em "Divergências: decisões", com a decisão de cada um e as pendências; não mude o sistema por um deles fora do que foi decidido.
- Ao ler uma fonte nova ou mudar uma fórmula, atualize `casos-literatura.md` e o arquivo do grupo em `literatura/`.

## 4. Entrega

Relate item a item o que passou, o que falhou (com o passo para reproduzir) e o que não foi possível testar e por quê. Nos casos da literatura, cite o ID e o valor obtido. Se subiu API ou frontend, pergunte se pode derrubar ao terminar.
