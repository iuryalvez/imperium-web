# Grupo A — Casos de teste do cálculo energético (literatura x Imperium)

Fontes lidas: `nrc:2007` (apostila Carciofi baseada no NRC 2006, 16 p., sem numeração impressa), `carciofi:apostila` (109 p.; página impressa = página do arquivo − 2), `ogoshi:2015` (12 p.; impressa = arquivo + 63, revista p. 64–75), `abinpet:2024` (642 p.; só as p. 11–23 e 39–43 tratam de energia; impressa = arquivo), `fediaf:2025` (98 p.; impressa = arquivo, "Page N of 98").

Convenção de página: "p. X" é a página do arquivo PDF; "(impr. Y)" é o número impresso, quando difere.

Contas refeitas em Python, aplicando as fórmulas do código (`RacaoService`, `CalculoNecessidadeEnergetica`). Critério: BATE quando |diferença| ≤ 1 %.

## Tabela-resumo

| ID | Tema | Fonte (p.) | Entrada resumida | Fonte diz | Sistema dá | Situação |
|---|---|---|---|---|---|---|
| A-EM-01 | Ração/EM | nrc p.3; carciofi p.7 (impr. 5); abinpet p.22 | Gato: U10 PB32 EE15 MM8 FB3 | 3,72 kcal/g | 3,721 | BATE |
| A-EM-02 | Ração/EM | nrc p.4; carciofi p.8 (impr. 6); abinpet p.21; fediaf p.50 | Cão: U10 PB24 EE12 MM8 FB3 | 3,53 (abinpet 3.537 kcal/kg) | 3,538 | BATE |
| A-EM-03 | Ração/EM | nrc p.12 e p.15; carciofi p.15 e p.102 | Cão: U10 PB26 EE16 MM7 FB3 | 3,77 | 3,763 | BATE |
| A-EM-04 | Ração/EM | nrc p.13–14 e p.16; carciofi p.16–17 e p.103 | Gato: U10 PB35 EE20 MM6,5 FB2,5 | 4,04 | 4,041 | BATE |
| A-EM-05 | Ração/EM (distribuição) | nrc p.5 | PB25 EE14 ENN40 (Atwater mod.) | 346,7 kcal/100 g; PB = 25,3 % | 346,5; PB 25,25 % | BATE (fonte tem erro) |
| A-EM-06 | Ração/EM (Atwater mod.) | ogoshi p.7 (impr. 70) | U12 PB28 EE13 FB6 MM7,5 | 3257,5 kcal/kg | distribuição 325,75 kcal/100 g | BATE |
| A-EM-07 | Ração/quantidade | ogoshi p.7 (impr. 70) | 1892 kcal/dia ÷ ração do A-EM-06 | 580 g/dia | 569 g/dia (EM NRC 3,323) | DIVERGE (método) |
| A-EM-08 | Ração/EM (regra) | fediaf p.50 | 4 passos NRC | mesmas constantes | idem | BATE |
| A-EM-09 | Ração/EM (natural) | fediaf p.51; ogoshi p.6–7 | Atwater 4/9/4 (cão), 4/8,5/4 (gato) | — | não calcula | NÃO TESTÁVEL |
| A-EM-10 | Ração/EM (FDT) | nrc p.3–4; carciofi p.6–7 | CDE por fibra dietética total | 95,6−0,89·FDT / 96,6−0,95·FDT | não calcula | NÃO TESTÁVEL |
| A-EM-11 | Ração/EM (limite) | nrc p.4; fediaf p.50; abinpet p.22 | Cão com FB > 8 % na MS | equação pode subestimar | sem aviso | NÃO TESTÁVEL |
| A-EM-12 | EM in vivo | carciofi p.22–23 (impr. 20–21) | coleta total, cão | 3,7 e 3,8 kcal/g | não faz | NÃO TESTÁVEL |
| A-EM-13 | EM in vivo | carciofi p.37–38 (impr. 35–36) | indicador Cr2O3, cão | 3,57 kcal/g | não faz | NÃO TESTÁVEL |
| A-EM-14 | EM in vivo | abinpet p.39–40 | coleta total, cão | 3,31 e 3,39 kcal/g | não faz | NÃO TESTÁVEL |
| A-EM-15 | EM in vivo | abinpet p.42–43 | indicador, cão | 3,41 kcal/g | não faz | NÃO TESTÁVEL |
| A-EM-16 | EM in vivo | fediaf p.41–42 | coleta total, exemplos 1 e 2 | 3312 kcal/kg | não faz | NÃO TESTÁVEL |
| A-MAN-01..06 | Manutenção + g/dia | nrc p.13 e p.15; carciofi p.16 e p.102 | 6 cães do exercício | 774,6 … 205,6 kcal | idênticos | BATE (6) |
| A-MAN-07..12 | Manutenção + g/dia | nrc p.14 e p.16; carciofi p.17 e p.103 | 6 gatos do exercício | 298 … 213 kcal | idênticos | BATE (6) |
| A-MAN-13 | Manutenção | fediaf p.59 | gato 4 kg, K 100 e 75 | 253 e 189 kcal | 253,2 e 189,9 | BATE |
| A-MAN-14 | Manutenção | fediaf p.59 | cão 15 kg, K 110, 95, 130 | 838, 724, ~1000 | 838,4; 724,1; 990,9 | BATE |
| A-MAN-15 | Manutenção (idade) | fediaf p.54; abinpet p.22 | 130/110/95 por faixa etária | tabela | K manual | BATE (K manual) |
| A-MAN-16 | Manutenção (atividade) | fediaf p.55; abinpet p.22 | 95/110/125/150–175/≤90/200/105 | tabela | K manual | BATE (K manual) |
| A-MAN-17 | Manutenção (gato castrado) | fediaf p.57; abinpet p.23; carciofi p.9 (impr. 7) | 52–75 ou 75 × PC^0,67 | 35–45 kcal/kg (gato 4 kg) | 32,9–47,5 kcal/kg | BATE (K manual; tabela da fonte arredondada) |
| A-MAN-18 | Manutenção (exótico) | nrc p.6; carciofi p.9 | 55–260 × PC^0,75 | faixa | perfil exótico, K manual | BATE (K manual) |
| A-MAN-19 | Manutenção (variação) | nrc p.7; carciofi p.10; abinpet p.23 | erro individual > 50 % | aviso | — | NÃO TESTÁVEL |
| A-MAN-20 | Manutenção (ambiente) | fediaf p.53 e p.55 | 70–144; +2–5 kcal/kg^0,75 por °C | ajuste | não faz | NÃO TESTÁVEL |
| A-CRE-01 | Crescimento | nrc p.7–8; carciofi p.10–11 | gato 0,8 kg, adulto 5 kg | 179 | 178,98 | BATE |
| A-CRE-02 | Crescimento | nrc p.8; carciofi p.11 | cão 15 kg, adulto 30 kg | 1735 | 1735,24 | BATE |
| A-CRE-03 | Crescimento | ogoshi p.10 (impr. 73) | cão 17 kg, adulto 35 kg | 1934 | 1934,21 | BATE |
| A-CRE-04 | Crescimento | ogoshi p.10 (impr. 73) | gato 1 kg, adulto 4 kg | 198 | 196,88 | BATE (−0,57 %; fonte com erro de digitação) |
| A-CRE-05 | Crescimento | fediaf p.56; abinpet p.23 | (254,1 − 135·p)·PC^0,75 | 1422 (com 15/30) | 1735 | DIVERGE (método) |
| A-CRE-06 | Crescimento | abinpet p.23 | 210/175/140 por % do peso adulto | 1601 (com 15/30) | 1735 | DIVERGE (método) |
| A-CRE-07 | Crescimento (neonato) | nrc p.8; fediaf p.56; abinpet p.23 | 25 kcal/100 g PC | — | usa a fórmula exponencial | NÃO TESTÁVEL |
| A-CRE-08 | Crescimento (atividade) | nrc p.8 | filhote inativo −10 a 20 % | — | só via K | NÃO TESTÁVEL |
| A-CRE-09 | Crescimento (gato, idade) | fediaf p.57; abinpet p.23 | 2,0–2,5 / 1,75–2,0 / 1,5 × MER | — | usa p, não idade | NÃO TESTÁVEL |
| A-CRE-10 | Crescimento (curvas) | fediaf p.56 | % do peso adulto por idade | — | não faz | NÃO TESTÁVEL |
| A-GES-01 | Gestação | nrc p.8–9; carciofi p.12 | cadela 30 kg, fase final | 2446 | 2446,42 | BATE |
| A-GES-02 | Gestação | ogoshi p.11 (impr. 74) | cadela 22 kg, fase final | 1892 | 1892,57 | BATE |
| A-GES-03 | Gestação | fediaf p.56; abinpet p.23 | cadela, K = 132 | 2472 (30 kg) | 2446 com K 130 | DIVERGE (constante; BATE com K 132) |
| A-GES-04 | Gestação | ogoshi p.6; fediaf p.57; abinpet p.23 | gata 140 × PC^0,67 | 324 (3,5 kg, ilustrativo) | 324,08 | BATE |
| A-GES-05 | Gestação (gata) | nrc p.8; ogoshi p.6 | ganho de 40–50 % | — | — | NÃO TESTÁVEL |
| A-GES-06 | Gestação (início da fase final) | nrc p.8; ogoshi p.11; fediaf p.56 | quando somar 26 × PC | — | marcação manual | NÃO TESTÁVEL |
| A-LAC-01 | Lactação | nrc p.9; carciofi p.12–13 | gata 5 kg, 5 filhotes, sem. 3 | 714 | 713,97 | BATE |
| A-LAC-02 | Lactação | nrc p.9–10; carciofi p.13 | cadela 27 kg, 7 filhotes, sem. 1 | 4390 | 4390,48 | BATE |
| A-LAC-03 | Lactação | ogoshi p.11 | cadela 22 kg, 6 filhotes, sem. 3 | 4377 | 4376,94 | BATE |
| A-LAC-04 | Lactação | ogoshi p.12 | gata 3,5 kg, 4 filhotes, sem. 3 | 483 | 483,49 | BATE |
| A-LAC-05 | Lactação | fediaf p.56 | cadela 5–8 filhotes: 96 + 12(n−4) | = 24n + 12m | idem | BATE |
| A-LAC-06 | Lactação | abinpet p.23 | "(96 + 12n)" | 5362 (literal, 27/7/1) | 4390 | DIVERGE (erro da fonte) |
| A-LAC-07 | Lactação (fora da faixa) | nrc p.9–10; fediaf p.56–57 | > 8 filhotes; semana > 4 (cão) / > 7 (gato) | não definido | extrapola | NÃO TESTÁVEL |
| A-EMA-01 | Emagrecimento | fediaf p.55; abinpet p.22; nrc p.7 | ≤ 90 kcal/kg^0,75 (propensos à obesidade) | sem protocolo | método Guimarães e Tudury | NÃO TESTÁVEL |
| A-EMA-02 | Emagrecimento (gato obeso) | nrc p.6 | 130 × PC^0,40 | equação de manutenção do obeso | aceita o perfil no peso alvo | NÃO TESTÁVEL (atenção) |
| A-FV-01 | Fase da vida | fediaf p.54; abinpet p.22 | cão 1–2 / 3–7 / > 7 anos | sênior > 7 anos | sênior a partir de 7 (pequeno) ou 6 (grande) | DIVERGE (critério) |
| A-FV-02 | Fase da vida | fediaf p.56–57; ogoshi p.5 | filhote até 1 ano | até 12 meses | filhote < 1 ano | BATE |
| A-FV-03 | Fase da vida | fediaf p.13–15 | crescimento inicial < 14 sem / final ≥ 14 sem | — | não separa | NÃO TESTÁVEL |
| A-OUT-01 | Outro (quantidade) | nrc p.10; ogoshi p.7; abinpet p.23 | g/dia = kcal ÷ EM | regra | igual | BATE |
| A-OUT-02 | Outro (unidades) | fediaf p.48; nrc p.1 | 1 kcal = 4,184 kJ | — | só kcal | NÃO TESTÁVEL |
| A-OUT-03 | Outro (perda urinária) | fediaf p.38, 41, 51; carciofi p.21; abinpet p.39; ogoshi p.6 | 1,25 (cão); 0,86 ou 0,9 (gato) | — | não faz | NÃO TESTÁVEL |
| A-OUT-04 | Outro (densidade de referência) | fediaf p.13; abinpet p.11–13; carciofi p.58 | 4000 kcal EM/kg MS | — | não faz | NÃO TESTÁVEL |

**Totais: 68 casos — 39 BATE, 6 DIVERGE, 23 NÃO TESTÁVEL.**

### Divergências principais
1. **A-CRE-05 / A-CRE-06 (crescimento de cães):** FEDIAF 2025 e ABINPET 2024 trazem outra equação de crescimento, (254,1 − 135·p) × PC^0,75, e a ABINPET também tem faixas fixas (210/175/140 kcal/kg^0,75). Com o Labrador de 15 kg e adulto de 30 kg, essas equações dão 1422 e 1601 kcal; o NRC (e o sistema) dá 1735, ou seja, +22 % e +8 %. O sistema segue o NRC, o que é coerente com a apostila, mas o texto da monografia precisa deixar claro qual referência foi adotada.
2. **A-GES-03 (gestação de cadela):** FEDIAF e ABINPET usam 132 × PC^0,75 (e não 130) e já aplicam essa base nas 4 primeiras semanas. Com K = 130 a diferença é −1,04 %. O sistema aceita K = 132, mas o "Ex: 130" do formulário induz ao valor do NRC.
3. **A-LAC-06 (lactação de cadela, ABINPET):** a tabela da ABINPET escreve "(96 + 12n)" onde a FEDIAF tem "96 + 12(n − 4)". Aplicada ao pé da letra, a fórmula superestima em 22 % (5362 contra 4390 kcal). O sistema está certo; é um erro de digitação da fonte, e o teste não deve se basear nela.
4. **A-EM-07 (quantidade de ração, Ogoshi):** a Ogoshi calcula a EM pelo Atwater modificado (3258 kcal/kg). O sistema usa as equações do NRC na ração (3323 kcal/kg para cão), o que dá 2 % a menos de ração (569 contra 580 g). A distribuição de Atwater do sistema bate com a fonte, mas a quantidade é calculada com a EM do NRC.
5. **A-FV-01 (fase da vida):** a FEDIAF considera sênior o cão com mais de 7 anos, sem diferenciar porte. O sistema usa AAHA (pequeno: sênior a partir de 7; grande: a partir de 6). Isso só afeta o rótulo, não o cálculo.
6. Erros das fontes, sem efeito no sistema: distribuição energética do nrc p.5 (25 × 3,5 escrito como 87,7, e "ENN × 2,5"); fator 6,732 no lugar de 6,7 na Ogoshi p.10; faixa "35–45 kcal/kg" da FEDIAF p.57, que pela própria equação vai de 32,9 a 47,5.

---

## Ração / EM

### A-EM-01 — EM de ração para gatos (exemplo resolvido)
- **Fonte:** nrc:2007 p.3; repetido em carciofi:apostila p.7 (impr. 5) e abinpet:2024 p.22 (em kcal/kg).
- **Entrada:** gato; umidade 10 %, PB 32 %, EE 15 %, MM 8 %, FB 3 %.
- **Fonte diz:** ENN 32 %; EB 4,67; CDE 85; ED 3,97; EM 3,72 kcal/g (abinpet: 3.724 kcal/kg).
- **Recalculado:** ENN 32,000 %; EB 4,669; FB na MS 3,333 %; CDE 84,967; ED 3,967; **EM 3,7207 kcal/g**. A fonte arredonda o CDE para 85 e a EB para 4,67, sem erro relevante.
- **Sistema:** mesmas fórmulas, com EM = 3,721 kcal/g (+0,02 %). **BATE.**
- **Onde testar:** Rações → nova ração, espécie Gato, em %: umidade 10, PB 32, EE 15, MM 8, FB 3. Conferir ENN 32,000, EB 4,669, CDE 84,967, ED 3,967 e EM 3,721.

### A-EM-02 — EM de ração para cães (exemplo resolvido)
- **Fonte:** nrc:2007 p.4; carciofi:apostila p.8 (impr. 6); abinpet:2024 p.21; constantes iguais em fediaf:2025 p.50.
- **Entrada:** cão; U 10, PB 24, EE 12, MM 8, FB 3 (%).
- **Fonte diz:** ENN 43; EB 4,38; CDE 86,43; ED 3,78; EM 3,53 kcal/g (abinpet: EB 4.382; ED 3.787; EM 3.537 kcal/kg).
- **Recalculado:** EB 4,382; CDE 86,433; ED 3,7875; **EM 3,5379**. A nrc encadeia valores arredondados e chega a 3,53 (−0,2 %); a abinpet chega a 3,537.
- **Sistema:** 3,538. **BATE.**
- **Onde testar:** Rações → espécie Cão, U 10, PB 24, EE 12, MM 8, FB 3. Esperar ENN 43,000 e EM 3,538.

### A-EM-03 — Exercício: ração para cães
- **Fonte:** enunciado em nrc:2007 p.12, resposta em p.15; carciofi:apostila p.15 (impr. 13), resposta p.102 (impr. 100).
- **Entrada:** cão; U 10, PB 26, EE 16, FB 3, MM 7 (%). Os valores são de rótulo (máx./mín.).
- **Fonte diz:** ENN 38; EB 4,67; FB na MS 3,33; CDE 86,44; ED 4,04; EM 3,77 kcal/g.
- **Recalculado:** EB 4,667; CDE 86,433; ED 4,0338; **EM 3,7634**. A fonte arredonda a ED para 4,04 antes de subtrair 0,27, o que dá +0,2 %.
- **Sistema:** 3,763 (−0,17 %). **BATE.**
- **Onde testar:** Rações → Cão, U 10, PB 26, EE 16, MM 7, FB 3. Essa ração serve de base para os casos A-MAN-01..06.

### A-EM-04 — Exercício: ração para gatos
- **Fonte:** nrc:2007 p.13–14, resposta p.16; carciofi:apostila p.16–17, resposta p.103 (impr. 101).
- **Entrada:** gato; U 10, PB 35, EE 20, FB 2,5, MM 6,5 (%).
- **Fonte diz:** ENN 26; EB 5,04; FB na MS 2,78; CDE 85,45; ED 4,31; EM 4,04 kcal/g.
- **Recalculado:** EB 5,0435; CDE 85,456; ED 4,3100; **EM 4,0405**.
- **Sistema:** 4,041. **BATE.**
- **Onde testar:** Rações → Gato, U 10, PB 35, EE 20, MM 6,5, FB 2,5. Base dos casos A-MAN-07..12.

### A-EM-05 — Distribuição energética (Atwater modificado)
- **Fonte:** nrc:2007 p.5. Não aparece na carciofi:apostila.
- **Entrada:** PB 25 %, EE 14 %, ENN 40 %. No sistema, use U 10, PB 25, EE 14, MM 8, FB 3, que dá ENN 40.
- **Fonte diz:** PB 87,7 + ENN 140 + EE 119 = 346,7 kcal/100 g; PB = 25,3 % da energia.
- **Recalculado:** 25 × 3,5 = **87,5** (a fonte escreve 87,7, erro de conta) e a linha do ENN traz "× 2,5" por erro de digitação, embora use 3,5. O total correto é **346,5 kcal/100 g**, com PB = **25,25 %**.
- **Sistema:** EnergiaProteina 87,5; EnergiaENN 140; EnergiaEE 119; total 346,5 (−0,06 % em relação à fonte). **BATE** (o sistema está certo; a fonte tem um erro pequeno).
- **Onde testar:** Rações → Cão, U 10, PB 25, EE 14, MM 8, FB 3, e conferir os campos de distribuição energética.

### A-EM-06 — Atwater modificado sobre rótulo (Ogoshi)
- **Fonte:** ogoshi:2015 p.7 (impr. 70).
- **Entrada:** U 12, PB 28, EE 13, MF (FB) 6, MM 7,5 (%).
- **Fonte diz:** ENN 335 g/kg; EM = 280 × 3,5 + 130 × 8,5 + 335 × 3,5 = 3257,5 ≈ 3258 kcal/kg.
- **Recalculado:** 3257,5 kcal/kg, conta correta.
- **Sistema:** distribuição de Atwater = 325,75 kcal/100 g. **BATE.** Atenção: a EM que o sistema usa no cálculo da quantidade vem do NRC, que dá 3,323 kcal/g para cão (+2,0 %) e 3,419 kcal/g para gato (+4,9 %).
- **Onde testar:** Rações → Cão, U 12, PB 28, EE 13, MM 7,5, FB 6. Conferir o total de Atwater 325,75 e a EM de 3,323.

### A-EM-07 — Quantidade de alimento (Ogoshi)
- **Fonte:** ogoshi:2015 p.7 (impr. 70).
- **Entrada:** necessidade de 1892 kcal/dia (a da cadela gestante de 22 kg, A-GES-02) e ração do A-EM-06 com 3258 kcal/kg.
- **Fonte diz:** 1892/3258 = 0,580 kg, ou 580 g/dia.
- **Recalculado:** 580,7 g/dia.
- **Sistema:** 1892 ÷ 3,3231 = 569,3 g/dia (−2,0 %). **DIVERGE (método):** a fonte usa a EM de Atwater modificado e o sistema usa a EM pelas equações do NRC. A fórmula da quantidade é a mesma (A-OUT-01).
- **Onde testar:** cadela de 22 kg → Dieta → Gestação, K 130, P 0,75, "fase final" marcada, ração do A-EM-06. Esperar 1892,567 kcal e cerca de 569 g/dia.

### A-EM-08 — Equações de EM da FEDIAF (regra)
- **Fonte:** fediaf:2025 p.50 (e p.49 para os valores de EB 5,7/9,4/4,1).
- **Regra:** os 4 passos do NRC, com as constantes 91,2/1,43 e 87,9/0,88 no CDE e 1,04 e 0,77 na EM. A FEDIAF dá ainda o equivalente em kJ (4,35 e 3,22).
- **Sistema:** constantes idênticas. **BATE** (reusar as entradas de A-EM-02).

### A-EM-09 — Atwater "cheio" para alimentos naturais ou de alta digestibilidade
- **Fonte:** fediaf:2025 p.51 (cão 4/9/4; gato 4/8,5/4); ogoshi:2015 p.6–7 (4/9/4 para alta qualidade e dietas naturais).
- **Ilustração (minha conta, com a ração do A-EM-06):** cão 4/9/4 = 363,0 kcal/100 g; gato 4/8,5/4 = 356,5 kcal/100 g.
- **Sistema:** usa sempre as equações do NRC e o Atwater 3,5/8,5/3,5 só na distribuição. **NÃO TESTÁVEL.** É relevante se o sistema passar a cadastrar alimento natural.

### A-EM-10 — CDE por fibra dietética total
- **Fonte:** nrc:2007 p.3 (gato: 95,6 − 0,89 × FDT na MS) e p.4 (cão: 96,6 − 0,95 × FDT); carciofi:apostila p.6–7.
- **Sistema:** só aceita FB. **NÃO TESTÁVEL.**

### A-EM-11 — Limite de validade (FB > 8 % na MS, cão)
- **Fonte:** nrc:2007 p.4; fediaf:2025 p.50, onde a equação pode subestimar a energia; abinpet:2024 p.22 diz que a equação não se aplica a coadjuvantes hipocalóricos. A nrc p.4 também avisa que as equações não valem para substitutos do leite e dietas enterais.
- **Sistema:** calcula sem aviso. **NÃO TESTÁVEL** (sugestão: exibir alerta quando FB/MS > 8 % para cão).

### A-EM-12 — EM in vivo, coleta total (Carciofi)
- **Fonte:** carciofi:apostila p.22–23 (impr. 20–21).
- **Entrada:** cão; consumo de 1200 g MS, fezes 240 g MS, PB 24 % na dieta e 20 % nas fezes, EB 4,6 na dieta e 3,2 nas fezes, urina 400 mL com 0,5 kcal/mL, fator 1,25.
- **Fonte diz:** CDAMS 80 %; CDAPB 83,3 %; PDing 240 g; EMA 3,7 sem urina e 3,8 com urina.
- **Recalculado:** (5520 − 768 − 300)/1200 = 3,710; (5520 − 768 − 200)/1200 = 3,793. Correto.
- **Sistema:** **NÃO TESTÁVEL.**

### A-EM-13 — EM in vivo, indicador (Carciofi)
- **Fonte:** carciofi:apostila p.37–38 (impr. 35–36).
- **Entrada:** Cr2O3 0,35 % na ração e 3,15 % nas fezes; PB 24 e 20 %; EB 4,2 e 3,2.
- **Fonte diz:** CDAMS 88,89 %; CDAPB 90,74 %; ED 3,84; PD 21,78 %; EMA 3,57.
- **Recalculado:** ED 3,844; PD 21,78; EMA 3,572. Correto.
- **Sistema:** **NÃO TESTÁVEL.**

### A-EM-14 — EM in vivo, coleta total (ABINPET)
- **Fonte:** abinpet:2024 p.39–40.
- **Entrada:** como no A-EM-12, mas com EB do alimento de 4,2 kcal/g e PDing pelo CDAPB (83,33 %).
- **Fonte diz:** PDing 239,9 g; EMA 3,31 sem urina e 3,39 com urina.
- **Recalculado:** (5040 − 768 − 299,9)/1200 = 3,310; (5040 − 768 − 200)/1200 = 3,393. Correto.
- **Sistema:** **NÃO TESTÁVEL.**

### A-EM-15 — EM in vivo, indicador (ABINPET)
- **Fonte:** abinpet:2024 p.42–43.
- **Entrada:** Cr 0,35 % no alimento e 2,1 % nas fezes; PB 24 e 20 %; EB 4,2 e 3,2.
- **Fonte diz:** CDMS 83,34 %; CDAPB 86,11 %; ED 3,67; PD 20,68; EMA 3,41.
- **Recalculado:** CDMS 83,33 %; ED 3,667; PD 20,667; EMA 3,408. Correto, com diferenças de arredondamento.
- **Sistema:** **NÃO TESTÁVEL.**

### A-EM-16 — EM in vivo (FEDIAF, exemplos 1 e 2)
- **Fonte:** fediaf:2025 p.41–42.
- **Entrada:** EB do alimento 4,35; consumo 1250 g; EB das fezes 1,65; fezes 600 g; PB 24 % no alimento e 9 % nas fezes; fator 1,25. No exemplo 2, urina de 1230 mL com 0,25 kcal/mL.
- **Fonte diz:** 3312 kcal/kg nos dois exemplos.
- **Recalculado:** (5437,5 − 990 − 246 × 1,25)/1250 = 3,312; com urina, (4447,5 − 307,5)/1250 = 3,312. Correto. Os números foram escolhidos para dar o mesmo resultado.
- **Sistema:** **NÃO TESTÁVEL.**

---

## Manutenção

### A-MAN-01..06 — Exercício de cães (energia e quantidade)
- **Fonte:** nrc:2007 p.13 (enunciado) e p.15 (resposta); carciofi:apostila p.16 (impr. 14) e p.102 (impr. 100). A ração é a do A-EM-03, com 3,77 kcal/g pela fonte e 3,7634 pelo sistema.

| ID | Cão (fonte) | Peso | K escolhido pela fonte | Fonte kcal → g/dia | Recalculado kcal | Sistema g/dia | Situação |
|---|---|---|---|---|---|---|---|
| A-MAN-01 | Terrier, 1,5 a, ativ. alta | 7 | 180 | 774,6 → 206 | 774,63 | 205,8 | BATE |
| A-MAN-02 | Pit Bull, 1,6 a, alta | 35 | 130 | 1870,7 → 496 | 1870,66 | 497,1 | BATE |
| A-MAN-03 | Beagle, 3,5 a, moderada | 12,5 | 105 | 698 → 185 | 698,03 | 185,5 | BATE |
| A-MAN-04 | Labrador, 4 a, moderada | 32 | 105 | 1412,7 → 375 | 1412,71 | 375,4 | BATE |
| A-MAN-05 | Rottweiler, 9 a, moderada | 53 | 95 | 1866 → 495 | 1866,08 | 495,8 | BATE |
| A-MAN-06 | Poodle, 8 a, baixa | 2,8 | 95 | 205,6 → 55 | 205,63 | 54,6 | BATE |

- **Observação:** quem escolhe o K é a fonte, e nem sempre de forma coerente com a idade. O Beagle de 3,5 anos e o Labrador de 4 anos, com atividade moderada, recebem 105 ("idosos ativos ou Terra-nova"), e o Pit Bull de 1,6 ano com atividade alta recebe 130 em vez de 140. O teste deve usar o K da resposta.
- **Onde testar:** cadastrar o animal com o peso → Dieta → Manutenção, com o perfil correspondente (Terrier 180, Ativo 130, Idoso ativo/Terra-nova 105, Inativo 95) e a ração do A-EM-03.

### A-MAN-07..12 — Exercício de gatos (energia e quantidade)
- **Fonte:** nrc:2007 p.14 e p.16; carciofi:apostila p.17 (impr. 15) e p.103 (impr. 101). A ração é a do A-EM-04 (4,04 pela fonte, 4,0405 pelo sistema). ECC ≤ 5 usa 100 × PC^0,67; ECC > 5 usa 130 × PC^0,40.

| ID | Gato | Peso | ECC | Equação | Fonte kcal → g | Recalculado | Sistema g | Situação |
|---|---|---|---|---|---|---|---|---|
| A-MAN-07 | SRD 1,5 a | 5,1 | 5 | 100 × PC^0,67 | 298 → 73,7 | 297,90 | 73,7 | BATE |
| A-MAN-08 | A. Shorthair 2 a | 6,8 | 7 | 130 × PC^0,40 | 280 → 69,3 | 279,86 | 69,3 | BATE |
| A-MAN-09 | Persa 5 a | 4,5 | 8 | 130 × PC^0,40 | 237 → 58,7 | 237,26 | 58,7 | BATE |
| A-MAN-10 | Siamês 6 a | 4,2 | 7 | 130 × PC^0,40 | 231 → 57,1 | 230,80 | 57,1 | BATE |
| A-MAN-11 | S. Birmânia 9 a | 3,2 | 3 | 100 × PC^0,67 | 218 → 54,0 | 218,00 | 54,0 | BATE |
| A-MAN-12 | Angorá 8 a | 3,1 | 4 | 100 × PC^0,67 | 213 → 52,8 | 213,41 | 52,8 | BATE |

- **Onde testar:** Dieta → Manutenção, perfil "Gato doméstico magro/ideal" (100/0,67) ou "obeso" (130/0,40), com a ração do A-EM-04. O sistema não escolhe o perfil a partir do ECC registrado (sistema-resumo.md); isso também vale como teste.

### A-MAN-13 — Gato de 4 kg (FEDIAF)
- **Fonte:** fediaf:2025 p.59 (Tabela VII-12 e texto).
- **Fonte diz:** 100 × 4^0,67 = 253 kcal; 75 × 4^0,67 = 189 kcal. Com 400 kcal/100 g de MS, isso dá 63 g e 47 g de MS.
- **Recalculado:** 253,15 e 189,86 (a fonte trunca para 189); 63,3 g e 47,5 g.
- **Sistema:** 253,15 com o perfil 100/0,67 e 189,86 com K 75 manual. **BATE.**
- **Onde testar:** gato de 4 kg → Manutenção, perfil magro, e depois Gato em manutenção customizado com K 75 e P 0,67.

### A-MAN-14 — Cão de 15 kg (FEDIAF)
- **Fonte:** fediaf:2025 p.59.
- **Fonte diz:** 110 × 15^0,75 = 838; 95 × 15^0,75 = 724; o NRC adotaria 130, "1000 kcal". Com 400 kcal/100 g de MS: 210 g e 181 g.
- **Recalculado:** 838,42; 724,09; 990,86 (o "1000" é aproximado, −0,9 %); 209,6 g e 181,0 g.
- **Sistema:** igual com K 110 (manual), 95 (Inativo) e 130 (Ativo). **BATE.**
- **Onde testar:** cão de 15 kg → Manutenção, com cada K.

### A-MAN-15 — MER por idade (FEDIAF/ABINPET)
- **Fonte:** fediaf:2025 p.54 (Tabela VII-6); abinpet:2024 p.22 (Tabela 6).
- **Regra:** 1–2 anos: 130 (125–140); 3–7 anos: 110 (95–130); mais de 7 anos: 95 (80–120) kcal/kg^0,75.
- **Sistema:** os perfis seguem o NRC por atividade. Não existe perfil 110 nem escolha automática por idade, mas o K é editável. **BATE (K manual).** Exemplo: cão de 15 kg com 5 anos → 838,42 (igual ao A-MAN-14).

### A-MAN-16 — MER por atividade (FEDIAF/ABINPET)
- **Fonte:** fediaf:2025 p.55 (Tabela VII-7); abinpet:2024 p.22 (Tabela 7).
- **Regra:** baixa (< 1 h) 95; moderada de baixo impacto 110; moderada de alto impacto 125; alta (3–6 h) 150–175; propensos à obesidade ≤ 90; Dogue Alemão 200 (200–250); Terra-nova 105 (80–132).
- **Sistema:** 95, 105 e 200 coincidem com os perfis do NRC; 110, 125, 150–175 e 90 exigem K manual. **BATE (K manual).** Diferença de conceito: no sistema, 95 é "inativo"; na FEDIAF, 95 já é o cão que passeia na guia por menos de 1 h por dia.

### A-MAN-17 — Gato castrado ou de interior
- **Fonte:** fediaf:2025 p.57 (Tabela VII-9); abinpet:2024 p.23 (Tabela 9); carciofi:apostila p.9 (impr. 7) sugere 75 × PC^0,67 para castrados domiciliados e 60–70 para fêmeas castradas.
- **Fonte diz:** 52–75 kcal/kg^0,67, ou 35–45 kcal/kg para um gato de 4 kg; gato ativo, 100, ou 60–65 kcal/kg.
- **Recalculado:** para 4 kg, 52 → 32,9 kcal/kg; 75 → 47,5 kcal/kg; 100 → 63,3 kcal/kg. A faixa "35–45" da fonte não bate exatamente com 52–75 (é aproximada).
- **Sistema:** não existe perfil 75, mas o K é editável. **BATE (K manual).** Exemplo: 4 kg com K 75 → 189,86 kcal.

### A-MAN-18 — Gatos exóticos
- **Fonte:** nrc:2007 p.6; carciofi:apostila p.9.
- **Regra:** 55–260 × PC^0,75, sem exemplo.
- **Sistema:** perfil Gato exótico (P 0,75, K obrigatório). **BATE (K manual).**

### A-MAN-19 — Variação individual > 50 %
- **Fonte:** nrc:2007 p.7; carciofi:apostila p.10 (impr. 8); abinpet:2024 p.23.
- **Regra:** as equações podem errar mais de 50 % para um indivíduo, então é preciso ajustar pelo acompanhamento do peso. **NÃO TESTÁVEL** (é orientação, não cálculo).

### A-MAN-20 — Ambiente e alojamento
- **Fonte:** fediaf:2025 p.53 (DER de 70 kcal/kg^0,75 em baia individual e acima de 144 em canil com interação; no inverno, ao ar livre, de 10 a 90 % a mais) e p.55 (+2–5 kcal/kg^0,75 por °C fora da zona termoneutra).
- **Sistema:** não ajusta. **NÃO TESTÁVEL.**

---

## Crescimento

### A-CRE-01 — Filhote de gato (NRC)
- **Fonte:** nrc:2007 p.7–8; carciofi:apostila p.10–11 (impr. 8–9).
- **Entrada:** gato com 0,8 kg e peso adulto esperado de 5 kg (p = 0,16).
- **Fonte diz:** 179 kcal/dia.
- **Recalculado:** 100 × 0,8^0,67 × 6,7 × (e^(−0,189 × 0,16) − 0,66) = **178,98**. Usar e = 2,718 em vez de e exato não muda o resultado.
- **Sistema:** 178,980. **BATE.**
- **Onde testar:** gato de 0,8 kg → Dieta → Crescimento, "Peso adulto estimado" 5, perfil magro (100/0,67).

### A-CRE-02 — Filhote de Labrador (NRC)
- **Fonte:** nrc:2007 p.8; carciofi:apostila p.11 (impr. 9).
- **Entrada:** cão com 16 semanas, 15 kg, adulto de 30 kg (p = 0,5).
- **Fonte diz:** 1735 kcal/dia.
- **Recalculado:** 130 × 15^0,75 × 3,2 × (e^(−0,435) − 0,1) = **1735,24**.
- **Sistema:** 1735,239. **BATE.**
- **Onde testar:** cão de 15 kg → Crescimento, peso adulto 30, K 130, P 0,75.

### A-CRE-03 — Filhote de Labrador (Ogoshi)
- **Fonte:** ogoshi:2015 p.10 (impr. 73), Tabela 4.
- **Entrada:** 16 semanas, 17 kg, adulto de 35 kg.
- **Fonte diz:** "1,934 kcal/dia", com o ponto como separador de milhar, ou seja, 1934.
- **Recalculado:** **1934,21**.
- **Sistema:** 1934,212. **BATE.**

### A-CRE-04 — Filhote de gato (Ogoshi)
- **Fonte:** ogoshi:2015 p.10 (impr. 73), Tabela 5.
- **Entrada:** 1 kg, adulto de 4 kg (p = 0,25).
- **Fonte diz:** 198 kcal/dia. A linha da conta traz "6.732" no lugar de 6,7.
- **Recalculado:** com 6,7, **196,88**; com 6,732, 197,82 (arredonda para 198). O 198 vem desse erro de digitação.
- **Sistema:** 196,879 (−0,57 %). **BATE**, mas registre o erro da fonte.

### A-CRE-05 — Equação de crescimento FEDIAF/ABINPET
- **Fonte:** fediaf:2025 p.56 (Tabela VII-8b, de 8 semanas a 1 ano, Klein et al. 2019); abinpet:2024 p.23 (Tabela 8).
- **Regra:** EM = (254,1 − 135,0 × PC atual/PC adulto) × PC atual^0,75. A fonte não traz exemplo resolvido.
- **Aplicação (minha, com as entradas dos casos NRC):** 15/30 kg → 1422,3 kcal; 17/35 kg → 1578,4 kcal.
- **Sistema:** 1735,2 e 1934,2 (+22 % e +23 %). **DIVERGE (método):** o sistema usa a equação exponencial do NRC.

### A-CRE-06 — Faixas por % do peso adulto (ABINPET)
- **Fonte:** abinpet:2024 p.23 (Tabela 8): até 50 % do peso adulto, 210 kcal/kg^0,75; de 50 a 80 %, 175; de 80 a 100 %, 140.
- **Aplicação (minha):** 15/30 kg, no limite de 50 %: 210 → 1600,6, ou 175 → 1333,8; 17/35 kg (48,6 %): 210 → 1758,2.
- **Sistema:** 1735,2 e 1934,2. **DIVERGE (método).** Na prática dá para reproduzir com o tipo Manutenção e K 210/175/140, mas não pelo tipo Crescimento.

### A-CRE-07 — Recém-nascidos
- **Fonte:** nrc:2007 p.8 (25 kcal por 100 g de PC, Kienzle 1985); fediaf:2025 p.56; abinpet:2024 p.23.
- **Ilustração (minha):** filhote de 0,4 kg → 100 kcal/dia. A fórmula NRC pós-desmame, se aplicada com adulto de 30 kg, daria 185,9.
- **Sistema:** aplica a fórmula exponencial a qualquer idade. **NÃO TESTÁVEL** (sugestão: avisar quando o filhote ainda não foi desmamado).

### A-CRE-08 — Filhotes inativos
- **Fonte:** nrc:2007 p.8: a manutenção de filhotes inativos pode cair de 10 a 20 %, e a de filhotes muito ativos pode ser maior.
- **Sistema:** só reduzindo o K (130 → 104–117); o resultado escala linearmente com o K. **NÃO TESTÁVEL** como regra.

### A-CRE-09 — Gatinhos por idade (múltiplos da MER)
- **Fonte:** fediaf:2025 p.57 (Tabela VII-10); abinpet:2024 p.23 (Tabela 10): até 4 meses, 2,0–2,5 × MER; de 4 a 9 meses, 1,75–2,0; de 9 a 12 meses, 1,5.
- **Comparação (minha):** o multiplicador do NRC, 6,7 × (e^(−0,189p) − 0,66), vale 2,08 com p = 0,16, 1,97 com p = 0,25, 1,67 com p = 0,5 e 1,12 com p = 1, coerente com a faixa da FEDIAF até a metade do crescimento.
- **Sistema:** usa p, não a idade. **NÃO TESTÁVEL.**

### A-CRE-10 — Curvas de crescimento
- **Fonte:** fediaf:2025 p.56 (Tabela VII-8a): % do peso adulto = a × ln(idade em semanas) − b, por faixa de peso adulto (por exemplo, ≤ 7 kg: 36,92 ln − 43,57).
- **Sistema:** não estima o peso esperado. **NÃO TESTÁVEL.**

---

## Gestação

### A-GES-01 — Cadela de 30 kg (NRC)
- **Fonte:** nrc:2007 p.8–9; carciofi:apostila p.12 (impr. 10).
- **Entrada:** cadela de 30 kg na fase final (de 4 semanas após o acasalamento até o parto).
- **Fonte diz:** 130 × 30^0,75 + 26 × 30 = 2446 kcal/dia.
- **Recalculado:** 1666,42 + 780 = **2446,42**.
- **Sistema:** 2446,419. **BATE.**
- **Onde testar:** cadela de 30 kg → Dieta → Gestação, semana 5 (por exemplo), perfil Cadela em gestação, K 130, P 0,75, com "Adicionar fator extra de Fase Final" marcado. Desmarcado, deve dar 1666,419.

### A-GES-02 — Cadela de 22 kg (Ogoshi)
- **Fonte:** ogoshi:2015 p.11 (impr. 74), Tabela 6.
- **Fonte diz:** 22^0,75 = 10,16; 1320 + 572 = 1892 kcal/dia.
- **Recalculado:** 22^0,75 = 10,158; **1892,57**.
- **Sistema:** 1892,567. **BATE.**

### A-GES-03 — Gestação com base 132 (FEDIAF/ABINPET)
- **Fonte:** fediaf:2025 p.56 (Tabela VII-8b, GfE 1989); abinpet:2024 p.23 (Tabela 8).
- **Regra:** 4 primeiras semanas, 132 × PC^0,75; últimas 5 semanas, 132 × PC^0,75 + 26 × PC.
- **Aplicação (minha, 30 kg):** início 1692,06; final 2472,06.
- **Sistema:** com K 130 dá 1666,42 e 2446,42 (−1,04 % nos dois casos). **DIVERGE (constante: 132 contra 130 do NRC).** Com K 132 digitado, BATE.

### A-GES-04 — Gata gestante
- **Fonte:** ogoshi:2015 p.6 (impr. 69); fediaf:2025 p.57 (Tabela VII-10); abinpet:2024 p.23 (Tabela 10). Regra: 140 × PC^0,67. Nenhuma das fontes traz exemplo resolvido.
- **Ilustração (minha):** gata de 3,5 kg → 324,08 kcal; de 4 kg → 354,41 kcal.
- **Sistema:** perfil Gata em gestação (140/0,67), semana obrigatória e sem fator de semana. **BATE.**
- **Onde testar:** gata de 3,5 kg → Gestação, semana 1 a 9, perfil Gata em gestação. O resultado não deve mudar com a semana.

### A-GES-05 — Gata: 40–50 %
- **Fonte:** nrc:2007 p.8: o NRC não tem equação específica para a gata gestante, e o **ganho de peso** na gestação deve ser de 40 a 50 %. A ogoshi:2015 p.6 interpreta como **aumento de energia** de 40 a 50 %, o que dá 140 × PC^0,67.
- **Observação:** as duas leituras são diferentes. O 140 que o sistema usa vem da Ogoshi e da FEDIAF, não do texto da apostila nrc.
- **Sistema:** não acompanha o ganho de peso esperado na gestação. **NÃO TESTÁVEL.**

### A-GES-06 — Quando entra o +26 × PC
- **Fonte:** nrc:2007 p.8 ("de 4 semanas após acasalamento até a parição"); ogoshi:2015 p.11 (título da Tabela 6: "4 semanas antes da parição"); fediaf:2025 p.56 ("últimas 5 semanas").
- **Observação:** em 9 semanas, NRC e FEDIAF põem o adicional nas semanas 5 a 9; a Ogoshi, nas semanas 6 a 9.
- **Sistema:** o adicional depende de uma caixa marcada à mão e não da semana informada. **NÃO TESTÁVEL** como regra automática. Teste sugerido: semana 3 com a caixa marcada deve somar 26 × PC mesmo assim, porque é o comportamento documentado no formulário.

---

## Lactação

### A-LAC-01 — Gata de 5 kg, 5 filhotes, semana 3 (NRC)
- **Fonte:** nrc:2007 p.9; carciofi:apostila p.12–13 (impr. 10–11). O fator L é o mesmo em fediaf:2025 p.57 e abinpet:2024 p.23.
- **Fonte diz:** 100 × 5^0,67 + 70 × 5 × 1,2 = 714 kcal/dia.
- **Recalculado:** 293,97 + 420 = **713,97**.
- **Sistema:** 713,975 (C = 70 para mais de 4 filhotes; L da semana 3 = 1,2). **BATE.**
- **Onde testar:** gata de 5 kg → Dieta → Lactação, 5 filhotes, semana 3, perfil Gata em lactação (100/0,67).

### A-LAC-02 — Cadela de 27 kg, 7 filhotes, semana 1 (NRC)
- **Fonte:** nrc:2007 p.9–10; carciofi:apostila p.13 (impr. 11).
- **Fonte diz:** 145 × 27^0,75 + 27 × (24 × 4 + 12 × 3) × 0,75 = 4390 kcal/dia.
- **Recalculado:** 1717,48 + 2673 = **4390,48**.
- **Sistema:** 4390,477 (n = 4, m = 3, L = 0,75). **BATE.**
- **Onde testar:** cadela de 27 kg → Lactação, 7 filhotes, semana 1, K 145, P 0,75. As previsões das semanas 2 a 4 devem ser 5103,28, 5637,88 e 5994,28 (minha conta: 1717,48 + 3564 × L).

### A-LAC-03 — Cadela de 22 kg, 6 filhotes, semana 3 (Ogoshi)
- **Fonte:** ogoshi:2015 p.11 (impr. 74), Tabela 7.
- **Fonte diz:** 1473 + 2904 = 4377 kcal/dia.
- **Recalculado:** 1472,94 + 22 × 120 × 1,1 = **4376,94**.
- **Sistema:** 4376,940. **BATE.**

### A-LAC-04 — Gata de 3,5 kg, 4 filhotes, semana 3 (Ogoshi)
- **Fonte:** ogoshi:2015 p.12 (impr. 75), Tabela 8.
- **Fonte diz:** 231 + 252 = 483 kcal.
- **Recalculado:** 231,49 + 252 = **483,49**.
- **Sistema:** 483,486 (C = 60 para 3 a 4 filhotes). **BATE.**

### A-LAC-05 — Forma da FEDIAF para 5 a 8 filhotes
- **Fonte:** fediaf:2025 p.56: 145 × PC^0,75 + [96 + 12 × (n − 4)] × PC × L.
- **Conferência:** 96 + 12(n − 4) = 24 × 4 + 12m, idêntico ao NRC. Com 27 kg, 7 filhotes e semana 1, dá 4390,48. **BATE.**

### A-LAC-06 — Erro de digitação na ABINPET
- **Fonte:** abinpet:2024 p.23 (Tabela 8), "5 a 8 filhotes: 145 × PC^0,75 + (96+12n) × PC × L".
- **Aplicação literal (minha):** 27 kg, 7 filhotes, semana 1 → 5362,48 (contra 4390,48); 22 kg, 6 filhotes, semana 3 → 5538,54 (contra 4376,94).
- **Sistema:** segue o NRC e a FEDIAF. **DIVERGE (erro da fonte).** Não usar a ABINPET como fonte deste teste; citar a FEDIAF ou o NRC.

### A-LAC-07 — Fora da faixa da tabela
- **Fonte:** nrc:2007 p.9–10 (m de 5 a 8 filhotes; L de 1 a 4 semanas para cadela e de 1 a 7 para gata); fediaf:2025 p.56–57.
- **Sistema:** m = filhotes − 4, sem limite, e semanas além da tabela usam o último L (1,2 para cadela, 0,8 para gata). Ilustração: cadela de 27 kg com 10 filhotes na semana 1 → 5119,48 no sistema; limitando m a 4, daria 4633,48 (+10,5 %).
- **Situação:** a fonte não define. **NÃO TESTÁVEL**, mas vale um teste de borda que trave o comportamento escolhido ou que exija validação acima de 8 filhotes.

---

## Emagrecimento

### A-EMA-01 — Não há protocolo de emagrecimento no grupo A
- **Fonte:** fediaf:2025 p.55 e abinpet:2024 p.22 ("cães propensos à obesidade ≤ 90 kcal/kg^0,75"); nrc:2007 p.7 e carciofi:apostila p.16 (as necessidades de idosos e obesos podem ser ainda menores).
- **Ilustração (minha):** cão de 15 kg com K 90 → 685,98 kcal (pelo tipo Manutenção).
- **Sistema:** o emagrecimento segue Guimarães e Tudury (2006): K × peso alvo^p × 0,7, 0,6 ou 0,5. Nenhuma fonte do grupo A traz frações de restrição, % de perda semanal nem plano de 15 semanas. **NÃO TESTÁVEL** com estas fontes; esses números precisam vir de outro grupo.

### A-EMA-02 — Equação do gato obeso usada no peso alvo
- **Fonte:** nrc:2007 p.6 e p.14: 130 × PC^0,40 é a manutenção do gato obeso (ECC > 5) no **peso atual**.
- **Ilustração (minha, gato 2 do exercício, 6,8 kg, com alvo de 5 kg, nível moderado de 60 %):** com o perfil obeso no peso alvo, 130 × 5^0,4 × 0,6 = 148,49 kcal; com o perfil magro no peso alvo, 100 × 5^0,67 × 0,6 = 176,38 kcal; manutenção atual do obeso = 279,86 kcal.
- **Situação:** o sistema aceita o perfil obeso no emagrecimento e o aplica ao peso alvo, uma combinação que a fonte não prevê. **NÃO TESTÁVEL (atenção):** convém decidir qual perfil vale no emagrecimento de gatos.

---

## Fase da vida

### A-FV-01 — Cão sênior
- **Fonte:** fediaf:2025 p.54 (Tabela VII-6); abinpet:2024 p.22: 1–2, 3–7 e mais de 7 anos ("cães idosos").
- **Sistema:** jovem de 1 a 2 anos; porte pequeno é adulto de 3 a 6 e sênior de 7 a 10; porte grande (≥ 25 kg) é adulto de 3 a 5 e sênior de 6 a 8 (AAHA).
- **Situação:** **DIVERGE (critério):** cão pequeno com 7 anos é sênior no sistema e adulto (3–7) na FEDIAF; cão grande com 6–7 anos também. Só o rótulo muda; o cálculo não depende da fase.
- **Onde testar:** cão de 10 kg com 7 anos deve aparecer como Sênior; cão de 30 kg com 6 anos, como Sênior.

### A-FV-02 — Filhote até 1 ano
- **Fonte:** fediaf:2025 p.56 (equação válida de 8 semanas a 1 ano) e p.57 (gatinhos até 12 meses); ogoshi:2015 p.5 (crescimento mais intenso até 7 meses; em raças grandes, até cerca de 12).
- **Sistema:** filhote quando a idade é menor que 1 ano, nas duas espécies. **BATE.**

### A-FV-03 — Crescimento inicial e final
- **Fonte:** fediaf:2025 p.13–15 (tabelas de nutrientes com crescimento inicial < 14 semanas e final ≥ 14 semanas); abinpet:2024 p.11–13.
- **Sistema:** não diferencia. **NÃO TESTÁVEL** (afeta nutrientes, não energia).

---

## Outro

### A-OUT-01 — Quantidade de alimento
- **Fonte:** nrc:2007 p.10; ogoshi:2015 p.7; abinpet:2024 p.23 (kcal/dia × 1000 ÷ kcal/kg).
- **Sistema:** g/dia = kcal/dia ÷ EM (kcal/g). **BATE** (ver A-MAN-01..12).

### A-OUT-02 — Unidades
- **Fonte:** fediaf:2025 p.48 (1 kcal = 4,184 kJ; 1 MJ = 239 kcal); nrc:2007 p.1 (1 cal = 4,18 J).
- **Sistema:** trabalha só em kcal. **NÃO TESTÁVEL.**

### A-OUT-03 — Fator de perda urinária
- **Fonte:** 1,25 kcal/g de proteína digestível para cão e 0,86 para gato (fediaf:2025 p.38 e p.41, Kienzle 1998; carciofi:apostila p.21; abinpet:2024 p.39). A ogoshi:2015 p.6 e a fediaf:2025 p.51, citando o NRC, usam 0,9 para gato.
- **Sistema:** não faz ensaio in vivo. **NÃO TESTÁVEL.** Fica registrado que o valor para gato varia entre as fontes (0,86 contra 0,9).

### A-OUT-04 — Densidade energética de referência
- **Fonte:** fediaf:2025 p.13 (perfis de nutrientes a 4,0 kcal EM/g MS); abinpet:2024 p.11–13; carciofi:apostila p.58 (corrigir dividindo a EM do alimento por 4000).
- **Sistema:** não formula nutrientes. **NÃO TESTÁVEL.**
