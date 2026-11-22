# Grupo C — Avaliação nutricional, ECC, peso ideal, fase da vida e ração

Casos de teste extraídos da literatura do grupo C e comparados com o que o Imperium faz hoje (`sistema-resumo.md` + leitura do código: `Animal.FaseVida`, enum `CondicaoCorporal`, `PerfilMetabolicoResolver`, `FormularioAvaliacao.jsx`, `FichaGeralAnimal.jsx`).
Contas refeitas com `python3`.

Fontes lidas: freeman:2011, muller:2008, chun:2019, michel:2011 (sem entrada `@` no tese.bib — só aparece no comentário da linha 196), porsani:2020, silva:2021, anamnese dietética "Oliveira 2016" (ver nota), pasta `imagens/Wsava/` (9 PDFs: wsava:2020a = ECC cão, wsava:2020b = ECC gato; os outros 7 não têm bibkey no tese.bib).
Bezemer 2010 (`bezemer:2010`): é um artigo sobre manutenção de SaaS multi-tenant, sem conteúdo de nutrição. Não gerou caso.

Notas de página:
- freeman:2011: uso a numeração impressa no PDF (versão "Vol 00", p. 1–9); no periódico, o artigo ocupa as p. 385–396.
- Oliveira: o PDF tem 2 páginas (impressas 34–35, "Anexo I – Anamnese dietética"). A entrada `oliveira:2016` do tese.bib é outro texto (REVET, p. 36–49, sobre obesidade). A bibkey precisa ser confirmada.
- Os PDFs da WSAVA têm uma página cada; cito "p. única".

## Tabela-resumo

| ID | Tema | Fonte | Situação |
|---|---|---|---|
| C-ECC-01 | ECC/peso ideal | freeman:2011 p.3 | BATE |
| C-ECC-02 | ECC/peso ideal | wsava:2020a; freeman:2011 p.3 | DIVERGE |
| C-ECC-03 | ECC/peso ideal | wsava:2020b | BATE |
| C-ECC-04 | ECC/peso ideal | wsava:2020b; WSAVA Calorie Needs Cats | DIVERGE |
| C-ECC-05 | ECC/peso ideal | wsava:2020a; muller:2008 p.1040 | BATE |
| C-ECC-06 | ECC/peso ideal | wsava:2020b | DIVERGE |
| C-ECC-07 | ECC/peso ideal | porsani:2020 p.1 e 3; muller:2008 p.1041; chun:2019 p.367–368 | BATE |
| C-ECC-08 | ECC/peso ideal | silva:2021 p.22–23 (Anexo D) | BATE (a fonte erra) |
| C-ECC-09 | ECC/peso ideal | chun:2019 p.366 | BATE |
| C-ECC-10 | ECC/peso ideal | muller:2008 p.1042 | NÃO TESTÁVEL |
| C-ECC-11 | ECC/peso ideal | muller:2008 p.1039–1042 | NÃO TESTÁVEL |
| C-ECC-12 | ECC/peso ideal | chun:2019 p.367–369 | NÃO TESTÁVEL |
| C-ECC-13 | ECC/peso ideal | michel:2011 p.S57–S58; freeman:2011 p.3–4; WSAVA MCS cão/gato | NÃO TESTÁVEL |
| C-ECC-14 | ECC/peso ideal | WSAVA Diet History Form; chun:2019 p.366; muller:2008 p.1042 | NÃO TESTÁVEL |
| C-FV-01 | Fase da vida | muller:2008 p.1042 | BATE (exceto 25,0 kg) |
| C-FV-02 | Fase da vida | porsani:2020 p.2 | DIVERGE |
| C-FV-03 | Fase da vida | freeman:2011 p.3 | NÃO TESTÁVEL |
| C-FV-04 | Fase da vida | michel:2011 p.S58 | DIVERGE |
| C-FV-05 | Fase da vida | silva:2021 p.15 (Fig. 10) | DIVERGE |
| C-AV-01 | Avaliação | freeman:2011 p.2–3; WSAVA Implementing | DIVERGE |
| C-AV-02 | Avaliação | freeman:2011 p.3 (Tab. 1) | NÃO TESTÁVEL |
| C-AV-03 | Avaliação | WSAVA Diet History Form | BATE (parcial) |
| C-AV-04 | Avaliação | Oliveira 2016, Anexo I p.34–35 | NÃO TESTÁVEL |
| C-AV-05 | Avaliação | freeman:2011 p.7 | BATE |
| C-EN-01 | Ração/EM (energia) | WSAVA Feeding Guide Hospitalized | BATE (com K=70) |
| C-EN-02 | Ração/EM (energia) | WSAVA Calorie Needs Dogs | DIVERGE |
| C-EN-03 | Ração/EM (energia) | WSAVA Calorie Needs Cats | BATE |
| C-EN-04 | Ração/EM (energia) | silva:2021 p.3–4 e p.15 | BATE (com K=180) |
| C-EN-05 | Ração/EM (energia) | silva:2021 p.4 (Tab. 1) | DIVERGE (a fonte arredonda) |
| C-EN-06 | Ração/EM (energia) | silva:2021 p.4 | BATE (a fonte transcreve errado) |
| C-EN-07 | Ração/EM (energia) | silva:2021 p.4 | DIVERGE |
| C-EN-08 | Ração/EM (energia) | silva:2021 p.4–5 | DIVERGE |
| C-EN-09 | Ração/EM (energia) | chun:2019 p.367 | NÃO TESTÁVEL |
| C-RA-01 | Ração/EM | freeman:2011 p.5 e p.8 | BATE |
| C-OU-01 | Outro | porsani:2020 p.1 e 4 | NÃO TESTÁVEL |

Totais: 35 casos. 14 BATE, 11 DIVERGE e 10 NÃO TESTÁVEL.

---

## ECC e peso ideal

### C-ECC-01 — Escala única de 9 pontos com o denominador registrado
- Tema: ECC/peso ideal
- Fonte: freeman:2011, p.3
- Regra: escolher uma escala, usá-la de forma consistente e registrar o denominador. A diretriz adota 1–9 e diz que não há escala validada acima de 9/9.
- Entrada: na Avaliação de um cão ou gato, tentar marcar ECC 0 e ECC 10.
- Esperado: só aceita valores de 1 a 9, e o rótulo deixa claro que a escala é "1 a 9".
- Sistema: o slider vai de `min=1` a `max=9`, com o rótulo "Condição Corporal (ECC - 1 a 9)". O enum `CondicaoCorporal` também vai de 1 a 9.
- Situação: **BATE**
- Onde testar: Acompanhamento → Nova avaliação → campo ECC. Conferir que o slider não passa de 9 e que a ficha mostra o valor sobre 9.

### C-ECC-02 — No cão, ECC 4 é ideal (meta 4–5/9)
- Tema: ECC/peso ideal
- Fonte: wsava:2020a (p. única: "IDEAL" cobre os pontos 4 e 5); freeman:2011 p.3 (meta de 4 a 5/9 para a maioria dos animais)
- Entrada: cão com avaliação ECC = 4.
- Esperado: classificado como "Ideal".
- Sistema: o popover do cão no formulário diz "4 (Ideal)", o que bate. Mas a ficha geral (`FichaGeralAnimal.jsx`) mostra `condicaoCorporalAtualLabel`, que vem do `[Display]` do enum: **"4 - Magro"**. O rótulo do 8 também difere: o popover diz "Obesidade" e o enum diz "Obesidade grave".
- Situação: **DIVERGE**. O enum usa o rótulo felino ("4 = magro") para as duas espécies.
- Onde testar: registrar uma avaliação de cão com ECC 4 e abrir a Ficha geral do animal → "Escore Corporal (ECC)". Hoje aparece "4 - Magro"; o esperado é "4 - Ideal". Para corrigir, o rótulo precisa depender da espécie.

### C-ECC-03 — No gato, o ideal é só o 5 (ficha WSAVA de 2020)
- Tema: ECC/peso ideal
- Fonte: wsava:2020b (p. única: "UNDER IDEAL" 1–4, "IDEAL" 5, "OVER IDEAL" 6–9)
- Entrada: gato com ECC 4 e depois com ECC 5.
- Esperado: 4 = abaixo do ideal; 5 = ideal.
- Sistema: o popover felino diz "4 (Magro)" e "5 (Ideal)", e o enum também tem "4 - Magro". O perfil energético usa ECC ≤ 5 → `GatoDomesticoMagro` (100 × PC^0,67).
- Situação: **BATE**. Observação: freeman:2011 (p.3) dá 4–5 como meta geral; a ficha felina de 2020 é mais restrita. Vale dizer na monografia qual das duas o sistema segue.
- Onde testar: Avaliação de um gato → ECC 4 → Ficha geral mostra "4 - Magro"; ECC 5 → "5 - Ideal".

### C-ECC-04 — Gato idoso com ECC 6 pode ser aceitável
- Tema: ECC/peso ideal
- Fonte: wsava:2020b (nota de rodapé: ECC 6/9 pode ser aceitável em alguns gatos, principalmente idosos); WSAVA "Calorie Needs for Healthy Adult Cats" (fórmulas lean 100 × PC^0,67 e obese-prone 130 × PC^0,4)
- Entrada: gato de 5 kg, 13 anos, ECC 6, dieta de manutenção.
- Esperado (cálculo): com o perfil magro, 100 × 5^0,67 = **293,97 kcal/dia**; com o perfil obeso, 130 × 5^0,4 = **247,48 kcal/dia**. A tabela WSAVA para 5 kg dá 250–290 kcal/dia.
- Sistema: `PerfilMetabolicoResolver` manda qualquer ECC ≥ 6 para `GatoDomesticoObeso`, sem olhar a idade. O resultado é 247,48 kcal/dia.
- Situação: **DIVERGE** (nuance). O sistema não tem a exceção do gato idoso; o valor fica no limite inferior da faixa WSAVA. Como K e p são editáveis, o veterinário pode corrigir à mão.
- Onde testar: gato de 5 kg nascido há 13 anos → Avaliação ECC 6 → Dieta de manutenção → conferir o perfil escolhido e as kcal/dia (≈ 247,5). Depois trocar o perfil para magro e conferir ≈ 294,0.

### C-ECC-05 — Descrição dos pontos 1–9 no cão
- Tema: ECC/peso ideal
- Fonte: wsava:2020a (p. única); muller:2008 p.1040 (Tabela 1, tradução de Laflamme 1997)
- Regra (resumo): 1–3 com costelas e ossos visíveis e pouca ou nenhuma gordura palpável; 4–5 com costelas palpáveis, cintura e abdome recolhido; 6 com leve excesso de gordura; 7 com costelas palpáveis com dificuldade e gordura lombar e na base da cauda; 8 com costelas palpáveis só com pressão e sem cintura; 9 com depósitos maciços no tórax, coluna, pescoço e membros.
- Sistema: o popover canino (`FormularioAvaliacao.jsx`) descreve os mesmos marcadores ponto a ponto (costelas, cintura, reentrância, base da cauda, pescoço e membros no 9).
- Situação: **BATE**
- Onde testar: Avaliação de cão → ícone de ajuda do ECC → comparar cada ponto com a ficha WSAVA.

### C-ECC-06 — Descrição dos pontos 1–9 no gato
- Tema: ECC/peso ideal
- Fonte: wsava:2020b (p. única)
- Regra: no ponto 6 da ficha felina, a reentrância abdominal está ausente. No ponto 9 há depósitos na região lombar, na face e nos membros.
- Sistema: o ponto 9 bate (face e membros). O ponto 6 do popover felino diz que "reentrância e gordura abdominal aparentes, mas não acentuadas", ou seja, a reentrância continua presente, o que contradiz a ficha. O ponto 4 também diverge um pouco: a ficha cita "slight abdominal tuck" e "minimal abdominal fat pads"; o sistema diz "costelas não visíveis".
- Situação: **DIVERGE** (texto do ponto 6, e em menor grau do 4)
- Onde testar: Avaliação de gato → popover do ECC → comparar os pontos 4 e 6 com a ficha WSAVA de 2020.

### C-ECC-07 — Faixas: ≤3 abaixo, 4–5 ideal, 6–7 sobrepeso, 8–9 obeso
- Tema: ECC/peso ideal
- Fonte: porsani:2020 p.1 (6–7 sobrepeso; 8–9 obesidade) e p.3 (≤3 abaixo; 4–5 ideal); muller:2008 p.1041 (Tabela 2 com os mesmos quatro grupos); chun:2019 p.367–368
- Entrada: ECC 3, 5, 6, 7, 8 e 9 em um cão.
- Esperado: abaixo / ideal / sobrepeso / sobrepeso / obeso / obeso.
- Sistema: o enum rotula 6 e 7 como "Sobrepeso" e 8 e 9 como "Obesidade grave" (o popover canino diz "Obesidade" no 8). Para 6–9 bate. O 4 tem o problema do C-ECC-02.
- Situação: **BATE** (para 3 e de 5 a 9)
- Onde testar: Ficha geral depois de avaliações sucessivas com esses valores.

### C-ECC-08 — Escala em silva:2021 que chama ECC 6 de "Ideal"
- Tema: ECC/peso ideal
- Fonte: silva:2021 p.22–23 (Anexo D, Tabela 14, "adaptado de Laflamme 1997")
- Achado: a tabela do Ração Simplex classifica o ponto 6 como "Ideal", em desacordo com a ficha WSAVA de cães, com muller:2008 (Tab. 1, 6 = sobrealimentado) e com porsani:2020.
- Sistema: 6 = "Sobrepeso", alinhado à WSAVA.
- Situação: **BATE** (o sistema está certo e a fonte erra). Não usar silva:2021 como referência da escala ECC.
- Onde testar: não precisa; serve como argumento na comparação com os trabalhos relacionados.

### C-ECC-09 — Obesidade = mais de 30% acima do peso ideal
- Tema: ECC/peso ideal
- Fonte: chun:2019 p.366 (cita a AVMA: obeso quando está mais de 30% acima do peso ideal); chun:2019 p.366 e porsani:2020 p.1 (obesidade = ECC 8–9)
- Esperado: o ponto em que começa a obesidade (ECC 8) deve corresponder a mais de 30% de excesso.
- Sistema: o rótulo/popover diz "8 (Obesidade - 31% a 45% acima)" e "7 (16% a 30%)". Bate com o limiar da AVMA.
- Situação: **BATE** (só nos rótulos; o sistema não calcula). Detalhe: o 9 diz "acima de 46%", o que deixa um buraco entre 45 e 46%; o melhor seria "acima de 45%".
- Onde testar: popover do ECC (cão e gato).

### C-ECC-10 — ±20% do peso ideal = fora do peso (Muller)
- Tema: ECC/peso ideal
- Fonte: muller:2008 p.1042 (conclusão)
- Regra: um cão 20% acima ou abaixo do peso ideal pode ser considerado fora do peso.
- Entrada: peso ideal 20 kg; pesos de 16, 23 e 24 kg.
- Esperado (Muller): 16 kg (−20%) e 24 kg (+20%) estão fora do peso; 23 kg (+15%) está dentro.
- Sistema: não classifica por peso atual ÷ peso ideal. Os rótulos de ECC tratam o 6 (até 15% acima) como sobrepeso, um limiar mais baixo que o de Muller.
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro, valeria mostrar a % de excesso (peso/peso ideal − 1) na avaliação, já que os dois campos existem.
- Onde testar: —

### C-ECC-11 — Índice de massa corporal canino (IMCC)
- Tema: ECC/peso ideal
- Fonte: muller:2008 p.1040 (fórmula), p.1041 (Tabela 2), p.1042 (exemplo e correções por porte)
- Regra: IMCC = peso (kg) / estatura² (m), com estatura = da nuca até o solo passando pela base da cauda. Ideal entre 11,8 e 15 para porte médio (10–25 kg); +20% no porte grande; −10% no miniatura. IMCC médio por ECC (Tab. 2): 1 = 6,470; 2 = 8,115; 3 = 11,693; 4 = 11,867; 5 = 14,304; 6 = 15,951; 7 = 17,594; 8 = 19,695; 9 = 25,000.
- Exemplo resolvido (p.1042): boxer de 20,8 kg e 1,10 m. O artigo dá IMCC = 17,25 e, tirando 20% (porte grande), 13,80, que cai na faixa ideal.
- Recalculado: 20,8 / 1,10² = **17,19** (não 17,25); × 0,8 = **13,75**. A conclusão (peso ideal) não muda, mas o artigo tem um erro aritmético pequeno.
- Sistema: não tem IMCC nem campo de estatura.
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro tem valor baixo: o índice foi validado só em cães SRD de porte médio e uma única população.
- Onde testar: —

### C-ECC-12 — Estimar o ECC pela diferença entre tórax e abdome (Chun)
- Tema: ECC/peso ideal
- Fonte: chun:2019 p.368 (Tabelas 1 e 2), p.369 (Tabela 3)
- Regra: a diferença entre a circunferência torácica e a abdominal cai conforme o ECC sobe. Médias agrupadas (cm): ECC 3 = 13,8; 4 = 12,9; 5 = 12,1; 6 = 11,4; 7 = 9,0; 8 = 7,0. Pearson com o ECC: −0,569 para a diferença e 0,847 para a circunferência abdominal.
- Entrada: medir as duas circunferências de um beagle adulto.
- Esperado: diferença ≈ 12–13 cm sugere ECC 4–5; ≈ 7 cm sugere ECC 8 (valores válidos só para beagles).
- Recálculo da Tabela 1 (360 avaliações): ECC 4–5 = 166/360 = **46,1%** (o texto diz 46%, mas escreve "BCS 4 and 6", um erro de digitação); ECC 6–8 = 153/360 = **42,5%** (o texto diz 42,2% e escreve "BCS 7 or 8"; 7–8 sozinhos seriam 14,2%). A fonte tem inconsistências internas.
- Sistema: não registra circunferências.
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro, um campo opcional de medidas na avaliação ajudaria no acompanhamento pelo tutor; a validade é limitada a uma raça.
- Onde testar: —

### C-ECC-13 — Escore de massa muscular (MCS/MMS)
- Tema: ECC/peso ideal
- Fonte: michel:2011 p.S57–S58 (escala 0–3: 3 normal, 2 leve, 1 moderada, 0 grave; palpação da coluna, escápulas, crânio e asas do ílio); freeman:2011 p.3–4 (MCS é diferente do ECC; um animal acima do peso pode ter perda muscular); fichas WSAVA de MCS para cão e gato (normal / perda leve / moderada / grave; avaliar ECC e MCS em toda consulta)
- Exemplo numérico (michel:2011 Fig. 1): massa magra (g) = 2557,28 + 507,78 × MMS → MMS 0 = 2557; 1 = 3065; 2 = 3573; 3 = 4081 g. Idade = 15,25 − 2,63 × MMS → MMS 3 ≈ 7,4 anos; MMS 0 ≈ 15,3 anos (regressões descritivas, não preditivas).
- Sistema: a avaliação não tem campo de massa muscular.
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro tem valor **alto**: a WSAVA põe o MCS ao lado do ECC na triagem, e basta um campo enum de 4 níveis.
- Onde testar: —

### C-ECC-14 — Do ECC à % de excesso e ao peso ideal
- Tema: ECC/peso ideal
- Fonte: WSAVA Diet History Form (p. única: o veterinário preenche peso atual, peso ideal e ECC /9 ou /5); chun:2019 p.366 (>30% = obeso); muller:2008 p.1042 (±20%)
- Regra: nenhuma fonte do grupo C dá a % de gordura ou de excesso por ponto de ECC. Só há o limiar de 30% (AVMA) e os ±20% de Muller. As faixas do sistema (6 ≤ 15%, 7 16–30%, 8 31–45%, 9 > 46%) não vêm desses textos.
- Sistema: o peso ideal é digitado à mão na Avaliação (`PesoIdeal`); não há cálculo de peso ideal a partir do ECC.
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro, sugerir peso ideal = peso atual ÷ (1 + excesso do ponto). Antes, a origem das faixas de % precisa ser citada (provavelmente outro grupo de fontes).
- Onde testar: —

## Fase da vida

### C-FV-01 — Porte por peso: pequeno até 10, médio de 10 a 25, grande acima de 25 kg
- Tema: Fase da vida
- Fonte: muller:2008 p.1042 (cita Biourge e Pibot 2005)
- Regra: pequenos de 1–10 kg, médios de 10–25 kg e grandes com mais de 25 kg.
- Sistema: `Animal.FaseVida` usa `isGrandePorte = pesoReferencia >= 25.0`, com peso de referência = PesoMaximo, senão PesoIdeal, senão Peso. Só há dois portes (< 25 e ≥ 25).
- Entrada e esperado:
  - cão de 6 anos com 24,9 kg → sistema: Adulto (não grande, 3–6); pela fonte: médio. Bate.
  - cão de 6 anos com 25,0 kg → sistema: **Sênior** (grande, 6–8); pela fonte: 25 kg ainda é médio (">25" para grande).
- Situação: **BATE**, exceto no limite exato de 25,0 kg (≥ contra >).
- Onde testar: cadastro do animal com nascimento há 6 anos e peso de 24,9 e de 25,0 kg → Ficha do animal → Fase da vida.

### C-FV-02 — Faixas etárias por porte (Hosgood e Scholl, em Porsani)
- Tema: Fase da vida
- Fonte: porsani:2020 p.2
- Regra: pequeno (0–10 kg): jovem < 7, meia-idade 7–12, sênior > 12. Médio: < 6, 6–10, > 10. Grande: < 5, 5–9, > 9. Gigante: < 3, 3–7, > 7.
- Sistema: cão de 0 anos = Filhote; 1–2 = Jovem; < 25 kg: 3–6 Adulto, 7–10 Sênior, 11+ Geriátrico; ≥ 25 kg: 3–5 Adulto, 6–8 Sênior, 9+ Geriátrico. Não existe a classe "gigante".
- Entrada e esperado:
  - 5 kg, 11 anos → fonte: meia-idade; sistema: **Geriátrico**.
  - 30 kg, 6 anos → fonte: meia-idade; sistema: **Sênior**.
  - 60 kg, 4 anos → fonte (gigante): meia-idade; sistema: **Adulto**.
- Situação: **DIVERGE**. São classificações diferentes: o sistema segue a AAHA, com dois portes e cinco fases. Vale registrar na monografia que a fase da vida depende da referência e que o sistema não trata cães gigantes à parte.
- Onde testar: Ficha do animal com esses três cadastros.

### C-FV-03 — Idade abaixo de 1 ou acima de 7 anos pede mais atenção
- Tema: Fase da vida
- Fonte: freeman:2011 p.3
- Regra: atividade baixa ou alta, vários animais na casa, gestação, lactação ou idade < 1 ou > 7 anos pedem mais atenção na triagem.
- Sistema: os cortes de idade são parecidos (Filhote < 1; cão pequeno vira Sênior aos 7), mas o sistema não gera alerta de triagem. No gato, 7–10 anos fica como "Adulto".
- Situação: **NÃO TESTÁVEL** (não há alerta). Como trabalho futuro, um aviso na avaliação quando houver filhote, sênior, gestação ou lactação seria simples de fazer.
- Onde testar: —

### C-FV-04 — Gatos: jovem até 6, meia-idade de 6 a 10, idoso acima de 10 anos (Michel)
- Tema: Fase da vida
- Fonte: michel:2011 p.S58
- Regra: jovem < 6 anos, meia-idade 6–10, idoso > 10.
- Sistema (AAFP): Filhote < 1; Jovem 1–6; Adulto 7–10; Sênior 11–14; Geriátrico 15+.
- Entrada e esperado: gato de 6 anos → Michel: meia-idade; sistema: **Jovem**. Gato de 11 anos → idoso nos dois (bate).
- Situação: **DIVERGE** só na fronteira dos 6 anos; os cortes de 10/11 batem. É uma diferença de taxonomia, não um erro.
- Onde testar: gato nascido há 6 anos → Ficha → Fase da vida = Jovem.

### C-FV-05 — Boxer de 30 kg com quase 11 anos aparece como "Adulto" no Ração Simplex
- Tema: Fase da vida
- Fonte: silva:2021 p.15 (Figura 10: Hulk, boxer, "10 anos 53 semanas", 30 kg, tabela de nutrientes com fase "Adulto")
- Esperado pelo sistema: 30 kg ≥ 25 e idade inteira de 10 ou 11 anos → **Geriátrico** (9+).
- Situação: **DIVERGE**. A fonte usa só as fases das tabelas do NRC (crescimento/adulto). Isso mostra que o Imperium tem uma granularidade de fase que o trabalho relacionado não tem.
- Onde testar: cadastrar boxer de 30 kg nascido há 11 anos → Ficha → Fase = Geriátrico.

## Avaliação

### C-AV-01 — Triagem mínima: peso, ECC, MCS e histórico/atividade
- Tema: Avaliação
- Fonte: freeman:2011 p.2–3 (avaliação em duas etapas, triagem e estendida; registrar peso, ECC e MCS); WSAVA "Implementing the Guidelines" (triagem = histórico nutricional e atividade + peso + ECC/MCS)
- Sistema: a Avaliação registra peso, peso ideal, ECC (1–9), grau de atividade, gestação/lactação, tipo de dieta e ração. Não tem MCS nem histórico alimentar.
- Situação: **DIVERGE** (falta o MCS, ver C-ECC-13, e o histórico alimentar, ver C-AV-04)
- Onde testar: Nova avaliação → conferir os campos presentes.

### C-AV-02 — Fatores de risco nutricional (Tabela 1 da WSAVA)
- Tema: Avaliação
- Fonte: freeman:2011 p.3 (Tabela 1)
- Regra: são fatores de risco: alteração gastrointestinal, doença, medicamentos ou suplementos, dieta não convencional, petiscos acima de 10% das calorias, ECC < 4 ou > 5, qualquer perda muscular, mudança de peso sem explicação, doença dentária e pelagem ruim. Se houver algum, faz-se a avaliação estendida.
- Entrada (exemplo): cão com ECC 7.
- Esperado: o animal é marcado como "em risco nutricional" e indicado para avaliação estendida.
- Sistema: não há checklist nem alerta.
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro tem valor **alto e baixo custo**: o alerta de ECC fora de 4–5 já pode sair dos dados que o sistema tem.
- Onde testar: —

### C-AV-03 — Formulário curto de histórico alimentar da WSAVA
- Tema: Avaliação
- Fonte: WSAVA "Short Diet History Form" (p. única)
- Campos: espécie/raça, idade, sexo, castrado; atividade (muito / moderadamente / pouco ativo); peso percebido; ambiente; alimentos, petiscos e suplementos com quantidade e frequência; e, para o veterinário, peso atual, peso ideal, ECC /9 ou /5 e MCS (4 níveis).
- Sistema: tem sexo e `IsCastrado` (Animal), idade (pela data de nascimento), `GrauAtividade` em 3 níveis (inativo/moderado/ativo, mesma granularidade do formulário), peso, peso ideal e ECC /9. Faltam os alimentos e petiscos com quantidade, os suplementos, o ambiente, o MCS e a escala /5.
- Situação: **BATE (parcial)** nos campos clínicos; o resto entra em C-AV-04 e C-ECC-13.
- Onde testar: cadastro do animal (sexo, castração) e Nova avaliação (peso, peso ideal, ECC, atividade).

### C-AV-04 — Anamnese dietética (Oliveira, Anexo I)
- Tema: Avaliação
- Fonte: "2016-Oliveira-Anamnese-dietética.pdf", p.34–35 (bibkey a confirmar; ver nota no topo)
- Conteúdo: tipo de alimento (comercial/caseiro, seco/úmido) com marca, quantidade e frequência; petiscos e restos; alimento à vontade; apetite e água; outros animais e se comem juntos; ambiente; perda ou ganho de peso recente; vômito, fezes e urina; atividade diária, sono e mastigação.
- Sistema: não tem anamnese dietética estruturada (`sistema-resumo.md`).
- Situação: **NÃO TESTÁVEL**. Como trabalho futuro tem valor alto: serve de base para os fatores de risco do C-AV-02 (petiscos, dieta não convencional, alterações gastrointestinais).
- Onde testar: —

### C-AV-05 — A necessidade energética varia ±30% no cão e ±50% no gato, por isso K é editável
- Tema: Avaliação
- Fonte: freeman:2011 p.7
- Regra: as fórmulas são ponto de partida. A necessidade pode variar 30% para mais ou para menos no cão e 50% no gato; a necessidade de manutenção depende do ECC, de sexo e castração, da fase da vida, da atividade e do ambiente.
- Entrada: cão de 10 kg, adulto ativo (K = 130).
- Esperado: 130 × 10^0,75 = **731,04 kcal/dia**; faixa plausível de ±30% = **511,73 a 950,36**. Gato de 4 kg magro: 100 × 4^0,67 = **253,15**; ±50% = 126,58 a 379,73.
- Sistema: calcula 731,04 e permite editar K e p, o que cobre o ajuste individual. O perfil usa atividade, fase, raça e ECC (gato), mas **não usa castração**.
- Situação: **BATE**. Ressalva: a castração não entra no perfil, embora a diretriz a cite e porsani:2020 mostre associação com obesidade.
- Onde testar: Dieta de manutenção para cão de 10 kg, adulto, ativo → 731 kcal/dia; editar K para 91 e para 169 e conferir que o valor acompanha.

## Energia e ração

### C-EN-01 — Necessidade energética em repouso (RER) = 70 × PC^0,75 (tabela WSAVA de internados)
- Tema: Ração/EM (energia)
- Fonte: WSAVA "Feeding Guide for Hospitalized Dogs and Cats" (p. única, tabela de RER de 1 a 60 kg)
- Recalculado: 1 kg = 70,0 (tabela 70); 10 kg = 393,6 (394); 20 kg = 662,0 (662); 30 kg = 897,3 (897); 60 kg = 1509,1 (1509). Todos batem com 70 × PC^0,75.
- Sistema: não tem perfil de RER, mas com K = 70 e p = 0,75 o cálculo reproduz a tabela.
- Situação: **BATE (com K = 70 editado)**. O mesmo PDF dá a regra de intervenção para internados (1–2 dias de anorexia: escrever a prescrição; 3–4: provável suporte nutricional; ≥ 5: suporte obrigatório). Isso não é testável porque o sistema não tem módulo de internação.
- Onde testar: Dieta de manutenção → editar K = 70 e p = 0,75 → peso 10 kg → ≈ 393,6 kcal/dia; 30 kg → ≈ 897,3.

### C-EN-02 — Tabela WSAVA de calorias para cães adultos contradiz o próprio rodapé
- Tema: Ração/EM (energia)
- Fonte: WSAVA "Calorie Needs for Healthy Adult Dogs" (p. única; o rodapé cita NRC 2006: ativo 130 × PC^0,75 e inativo 95 × PC^0,75)
- Recalculado: as faixas da tabela não saem de 95–130. Saem de **1,2 a 1,5 × RER** (≈ 84 a 105 × PC^0,75). Exemplos:
  - 10 kg: tabela 470–590; 1,2–1,5 × RER = 472–590; sistema inativo/ativo = **534 / 731**.
  - 20 kg: tabela 790–993; 1,2–1,5 × RER = 794–993; sistema = **898 / 1229**.
  - 30 kg: tabela 1080–1346; 1,2–1,5 × RER = 1077–1346; sistema = **1218 / 1666**.
- Sistema: usa 95 e 130 (NRC), como diz o rodapé.
- Situação: **DIVERGE**. Os valores do sistema ficam 13–24% acima da faixa da tabela. Não é erro do sistema: a tabela usa outra conta. Na monografia, convém não validar o sistema contra essa tabela, ou explicar a diferença.
- Onde testar: Dieta de manutenção, cão de 10 kg inativo → 534; ativo → 731. Comparar com 470–590 da tabela WSAVA.

### C-EN-03 — Tabela WSAVA de calorias para gatos adultos
- Tema: Ração/EM (energia)
- Fonte: WSAVA "Calorie Needs for Healthy Adult Cats" (p. única; magro 100 × PC^0,67; propenso a obesidade 130 × PC^0,4)
- Recalculado (magro / obeso, contra a tabela): 3 kg = 209 / 202 (200–210); 4 kg = 253 / 226 (225–250); 5 kg = 294 / 247 (250–290); 7 kg = 368 / 283 (280–370). A faixa da tabela fica entre as duas fórmulas, arredondada.
- Sistema: mesmas fórmulas (perfis `GatoDomesticoMagro` e `GatoDomesticoObeso`).
- Situação: **BATE**
- Onde testar: gato de 4 kg com ECC 5 → ≈ 253 kcal/dia; com ECC 7 → ≈ 226 kcal/dia.

### C-EN-04 — Exemplo resolvido do Ração Simplex: boxer de 30 kg
- Tema: Ração/EM (energia)
- Fonte: silva:2021 p.3 (metabolismo basal = 70 × PM; PM = PC^0,75), p.4 (adultos de porte médio e grande: 180–200 kcal/PM), p.15 (Figura 10)
- Entrada: 30 kg.
- Esperado (da fonte, conferido): peso metabólico = 30^0,75 = **12,819**; metabolismo basal = 70 × 12,819 = **897,303 kcal/dia**; energia metabolizável = **2307,350 kcal/dia**, o que corresponde a K = 180 (180 × 12,8186 = 2307,35).
- Sistema: K = 180 é o perfil `CaoTerrierAdultoAtivo`. Para um boxer ele não é escolhido automaticamente (é só para terriers adultos e ativos), mas com K editado para 180 o resultado é igual.
- Situação: **BATE (com K = 180)**
- Onde testar: Dieta de manutenção, cão de 30 kg → editar K = 180, p = 0,75 → 2307,35 kcal/dia; com K = 70 → 897,30.

### C-EN-05 — Tabela de energia de manutenção por porte (Couto e Real, em Silva)
- Tema: Ração/EM (energia)
- Fonte: silva:2021 p.4 (Tabela 1, citando Couto e Real 2019, p.140)
- Regra implícita: energia de manutenção = 140 × PV^0,75, com o peso metabólico arredondado para inteiro.
- Recalculado (fonte → exato):
  - 8 kg: PM 5 → 4,76; energia 700 → **666,0**; kcal/PV 88 → 83,2.
  - 40 kg: PM 16 → 15,91; 2240 → **2226,8**; 56 → 55,7.
  - 65 kg: PM 24 → **22,89**; 3360 → **3204,9**; 52 → 49,3.
  - 100 kg: PM 32 → 31,62; 4480 → **4427,2**; 45 → 44,3.
- Sistema: K = 140 é o perfil jovem ativo; o sistema usa o PM sem arredondar.
- Situação: **DIVERGE** (a fonte arredonda; para 65 kg o PM 24 está errado e a diferença é de 4,8%). O sistema dá os valores exatos.
- Onde testar: cão jovem ativo (1–2 anos, atividade ≥ moderada) com 8 kg → ≈ 666 kcal/dia (não 700).

### C-EN-06 — Fórmula de crescimento do cão: o sistema segue o NRC e a transcrição de Silva tem um parêntese fora do lugar
- Tema: Ração/EM (energia)
- Fonte: silva:2021 p.4
- Achado: a fonte escreve a equação com o "− 0,1" fora do colchete da exponencial. O sistema usa 130 × PC^0,75 × 3,2 × (e^(−0,87·p) − 0,1).
- Entrada: peso atual 10 kg, peso adulto esperado 30 kg (p = 0,3333).
- Esperado: forma do sistema = **1516,51 kcal/dia**; leitura literal da fonte = 1750,34 kcal/dia (15% a mais).
- Situação: **BATE** (o sistema segue a forma do NRC). Na monografia, não copiar a equação da forma como aparece em silva:2021.
- Onde testar: Dieta de crescimento, cão de 10 kg com peso adulto de 30 kg → ≈ 1516,5 kcal/dia.

### C-EN-07 — Constante do crescimento felino: 6,732 em Silva, 6,7 no sistema
- Tema: Ração/EM (energia)
- Fonte: silva:2021 p.4
- Entrada: gato de 2 kg com peso adulto de 4 kg (p = 0,5).
- Esperado: com 6,7 → **266,32 kcal/dia**; com 6,732 → **267,59 kcal/dia** (diferença de 0,5%).
- Sistema: usa 6,7.
- Situação: **DIVERGE** (constante; impacto clínico desprezível). Vale conferir no NRC (outro grupo) qual valor é o original.
- Onde testar: Dieta de crescimento, gato de 2 kg com peso adulto de 4 kg → ≈ 266,3.

### C-EN-08 — Fatores de cão adulto e de gato exótico segundo Silva
- Tema: Ração/EM (energia)
- Fonte: silva:2021 p.4 (cães com pouca atividade 95; ativos em espaço amplo 130–140; "médio e grande porte" 180–200 kcal/PM) e p.5 (gatos exóticos de 55 a 260 × PV^0,75)
- Sistema: 95 inativo; 130 ativo; 140 jovem ativo; 180 **terrier**; 200 **dogue alemão**; 105 idoso / terra-nova. Gato exótico com K editável e p = 0,75.
- Situação: **DIVERGE**. Silva liga 180–200 ao porte; o sistema liga a raças específicas, como no NRC. Um cão de porte médio comum teria de 2307 a 2564 kcal/dia em Silva (30 kg, K 180–200) contra 1666 no sistema (K 130), uma diferença de 38–54%. A fonte parece simplificar; usar o NRC como referência.
- Onde testar: cão de 30 kg, adulto, ativo, raça comum → 1666 kcal/dia (perfil `CaoAtivo`).

### C-EN-09 — Beagles: 830 kcal/dia para manutenção e 996 para ganho de peso
- Tema: Ração/EM (energia)
- Fonte: chun:2019 p.367
- Dados: beagles de 2 anos; grupo normal com 830 kcal/dia ("necessidade diária"); grupo de ganho com 996 kcal/dia (+20%: 830 × 1,2 = 996, confere).
- Situação: **NÃO TESTÁVEL**. O artigo não informa o peso dos cães, então não dá para conferir K × PC^p.
- Onde testar: —

### C-RA-01 — Conferir a densidade calórica do alimento e prescrever em quantidade medida
- Tema: Ração/EM
- Fonte: freeman:2011 p.5 (conferir kcal por grama, lata ou copo, principalmente se o ECC estiver fora da meta) e p.8 (usar medidor adequado, por exemplo copo de 237 ml, e fornecer quantidades medidas)
- Regra: quantidade = necessidade (kcal/dia) ÷ densidade calórica.
- Sistema: calcula a EM em kcal/g a partir da análise garantida (`RacaoService`) e a quantidade em g/dia = kcal/dia ÷ EM.
- Situação: **BATE** (conceito). As fontes do grupo C **não trazem nenhum exemplo numérico** de EM a partir da análise garantida, então o teste numérico desse cálculo fica com outro grupo. Medidas por volume (copo, lata) não existem no sistema; seria um trabalho futuro de pouco valor.
- Onde testar: Rações → cadastrar uma ração → conferir a EM em kcal/g; na Dieta, conferir que g/dia × EM = kcal/dia.

## Outro

### C-OU-01 — Prevalência de sobrepeso e obesidade em São Paulo (dado de contexto)
- Tema: Outro
- Fonte: porsani:2020 p.1 (resumo) e p.4 (resultados)
- Recalculado: 285 cães, sendo 23 abaixo do peso (8,1%), 149 no ideal (52,3%), 75 com sobrepeso (26,3%) e 38 obesos (13,3%). Todos conferem. A soma bruta de sobrepeso e obesidade é 39,6%; depois da ponderação por região, 25,9 + 14,6 = **40,5%**, que é o número do resumo.
- Situação: **NÃO TESTÁVEL** (epidemiologia). Serve de motivação na monografia.
- Onde testar: —
