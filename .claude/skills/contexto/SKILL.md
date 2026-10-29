---
name: contexto
description: Leia antes de qualquer tarefa no Imperium. Explica o que é o sistema (multi-tenant de clínicas veterinárias), Sistema x Clínica, perfis, entidades e regras de negócio, domínios do backend, ambientes, pendências e o que nunca pode quebrar.
---

# Contexto do Imperium

## O que é

O Imperium é um sistema web multi-tenant para clínicas veterinárias. Cada clínica é um tenant e só enxerga os próprios dados. Ele faz parte do ecossistema "software rural", desenvolvido na UFG pela Escola de Veterinária e Zootecnia (EVZ) com o Instituto de Informática (INF).

O Bris é outro sistema do mesmo ecossistema e roda no mesmo servidor. Para nós ele é só contexto: nunca alteramos o Bris nem o proxy `https-server`.

O Imperium ainda não está em produção com clientes reais. Mesmo assim, trate os dados como se fossem reais: use só dados fictícios em testes e exemplos.

## Sistema e Clínica

- Sistema é a administração do Imperium: cadastrar clínicas, gerenciar usuários e manter os catálogos compartilhados (raças e rações). Só o admin (claim `isAdmin`) acessa. As rotas ficam em `api/sistema/...` e as telas em `/sistema/...`.
- Clínica é o tenant: o dia a dia da clínica (animais, tutores, consultas, dietas, usuários da clínica). As rotas ficam em `api/clinica/...` e as telas em `/clinica/...`.

## Usuários, perfis e vínculos

- O usuário é global (um login só). Ele se liga a uma ou mais clínicas pelo vínculo `UsuarioPerfilClinica`, que guarda os perfis dele naquela clínica.
- Perfis na clínica: GESTOR (administra a clínica), VETERINARIO, SECRETARIO e TECNICO. O admin do sistema tem `isAdmin` e pode simular acesso a uma clínica.
- Depois do login, o usuário escolhe o vínculo (a clínica). A partir daí o token JWT (em cookie) carrega o `clinicaId` e os perfis daquela clínica.
- O login usa Cloudflare Turnstile como proteção contra robôs.
- O que cada perfil pode fazer é definido pelas policies em `backend/Program.cs` (`VisualizaTudo`, `GerenciaClinica`, `GerenciaAnimais`, `GerenciaTutores`, `GerenciaConsultas`, `GerenciaSistema`...). O frontend só esconde botões; a segurança real é do backend.

## Entidades e regras de negócio

- Clínica: o tenant, mantido pelo admin do sistema.
- Usuário: global, ligado às clínicas pelos vínculos. Fica inativo quando todos os vínculos estão inativos.
- Tutor: pertence a uma clínica (`ClinicaId`). O CPF é único por clínica: o mesmo CPF pode existir em clínicas diferentes, mas não duas vezes na mesma.
- Animal: pertence a uma clínica. Liga-se aos tutores pela tabela N:N `TutorAnimal`, e animal e tutor precisam ser da mesma clínica.
- Raça e Ração: há um catálogo do sistema (`ClinicaId` nulo), mantido pelo admin, e itens próprios de cada clínica. Quando a clínica edita um item do sistema, nasce uma cópia da clínica. A clínica não exclui nem desativa item do sistema, mas pode ocultá-lo só para ela (`RacaOcultaClinica`, `RacaoOcultaClinica`).
- Consulta: pertence a uma clínica, com animal, tutor e usuário responsável. Pode ter uma Avaliação (dados clínicos e nutricionais do animal).
- Dieta: nasce de uma avaliação, usa uma ração e guarda o cálculo da necessidade energética (`CalculoNecessidadeEnergetica` em `SaudeAnimal/Services`), conforme o tipo de dieta (manutenção, crescimento, gestação, lactação, emagrecimento). Tem registros semanais de peso (`RegistroPesoDieta`).
- Acompanhamento: agrupa as avaliações de um animal ao longo do tempo.
- Dieta e Acompanhamento não têm `ClinicaId` próprio: chegam à clínica pela avaliação, consulta ou animal. Qualquer consulta a elas precisa passar por esse caminho para respeitar o tenant.

Regras gerais:

- Cadastros têm status ativo ou inativo. Desativar é o normal; inativo continua no histórico, mas não pode ser escolhido em registro novo.
- Excluir só é permitido quando o registro não está em uso. Se estiver, o sistema pede para desativar.
- Edição é parcial: campo não enviado mantém o valor atual.

## O que nunca pode quebrar

- Isolamento entre clínicas: uma clínica nunca lê nem altera dados de outra. Isso é exigência da LGPD (dados de tutores são dados pessoais).
- O `clinicaId` vem sempre do token no backend. O frontend nunca envia `clinicaId`, e o backend nunca aceita um vindo do body ou da query.
- O Bris e o `https-server` não são alterados. As rotas da API continuam sob `/api/` e os nomes dos containers não mudam.
- Segredos não são lidos nem copiados: `.env`, chaves, senhas e tokens ficam fora de respostas, commits e arquivos.
- Testes e exemplos usam só dados fictícios (CPF gerado, nomes inventados).

## Domínios do backend

As pastas em `backend/Domain/` são:

- `AtendimentoAnimal`: animais, tutores, raças, consultas, avaliações e acompanhamento.
- `SaudeAnimal`: rações, dietas, registros de peso e cálculo energético.
- `GestaoClinica`: clínicas e dashboard.
- `Usuarios`: usuários, vínculos, perfil pessoal e e-mail.
- `Auth`: login, token, vínculos e Turnstile.
- `General` e `Utils`: extensões, `ServiceResult`, mapeamento e paginação.

## Ambientes

- Local: API com `make dev-up-api` e frontend com `make dev-up-frontend`, banco PostgreSQL local configurado no `.env`.
- Homologação: `imperium.evz.ufg.br/dev/` (branch `dev`), containers com sufixo `-homolog`.
- Produção: `imperium.evz.ufg.br/` (branch `main`).

## Pendências conhecidas

- Não há testes automatizados (veja a skill `testes`).
- `GET sistema/usuarios` ainda não segue o contrato de paginação.
- Há legado fora do padrão: `backend/Services/ValidateService.cs` e `backend/Interfaces/` na raiz, regra de negócio em `UsuarioController` e alguns nomes de arquivo do frontend sem `DaClinica`.
- O `frontend/src/App.test.js` é o teste padrão do CRA e está quebrado.
