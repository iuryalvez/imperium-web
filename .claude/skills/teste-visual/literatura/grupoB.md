# Grupo B — Obesidade e emagrecimento: casos de teste

Fontes lidas (página = página do PDF; entre parênteses a página impressa quando existe):

| Fonte | Bibkey | Observação |
|---|---|---|
| Guimarães e Tudury 2006, Vet. Not. 12(1):29-41 | `neves:2006` | PDF p.N = revista p.N+28 |
| Gomes 2016, monografia (obesidade felina) | `gomes:2016` | PDF p.N = impressa p.N−5 |
| Oliveira 2016, REVET 3(1) | `oliveira:2016` | PDF p.N = revista p.N+35 |
| Carciofi 2013, apostila "Obesidade e suas consequências…" | `carciofi:obesidade` | sem paginação impressa; uso a do PDF |
| Jericó, Lorenzini e Kanayama 2015, Manual de Obesidade Canina e Felina (Purina/ABEV, impressão VetSmart) | sem bibkey | PDF p.N = "N/24" do rodapé |
| Vargas 2014, "Obesidade: doença da pré-história e dos tempos modernos" | sem bibkey | PDF de 1 página (pôster/encarte) |
| "Gonçalves 2019" = Andrade Junior et al., Science and Animal Health 7(2):105-125 | `andrade:2020` (título idêntico; o arquivo leva "Gonçalves" no nome) | PDF p.N = revista p.N+104 |
| AAHA 2021 Nutrition and Weight Management Guidelines | `cline:2021` | PDF p.N = JAAHA p.N+152 |

Convenções:
- Sistema (de `sistema-resumo.md` e conferido no código): emagrecimento = K × (peso alvo)^P × fração; Severa 50 % (2 %/sem), Moderada 60 % (1,5 %/sem), Branda 70 % (1 %/sem); energia fixa até o alvo; plano de 15 semanas; projeção composta `peso_s = peso_atual × (1 − taxa)^(s−1)`; quando o alvo é atingido a linha passa a mostrar a manutenção do alvo (K × alvo^P, 100 %); alerta roxo quando a perda registrada > 2 %/semana em relação ao registro anterior.
- Na tela Dieta → Emagrecimento, K (1–1000) e P (0,1–2) são sempre editáveis (perfis "Cão/Gato em Emagrecimento" customizados), mas a fração só pode ser 50, 60 ou 70 %.
- **BATE\*** = bate (≤ 1 %) usando um "contorno": embutir no K um fator que o sistema não oferece (ex.: fonte usa 1,0 × RER = 70 × PC^0,75 → digitar K = 100 e nível Branda, porque 100 × 0,7 = 70). A conta é algebricamente idêntica, mas o fluxo natural da tela não leva a esse valor.
- Todas as contas foram refeitas em python3; valores com 1–2 casas.

## Tabela-resumo

| ID | Tema | Fonte (p. PDF) | Entrada | Fonte diz | Sistema dá | Situação |
|---|---|---|---|---|---|---|
| B-EMA-01 | Emagrecimento | neves:2006 p.9 | Cão 35 kg → alvo 28 kg, K 132, P 0,75, Moderada, ração 3065 kcal/kg | 960 kcal/d; 313 g/d | 964,04 kcal; 314,5 g | BATE (+0,42 %) |
| B-EMA-02 | Emagrecimento | neves:2006 p.9 | Gato 8 kg → 6 kg, K 60, P 1, Moderada, ração 3170 kcal/kg | 216 kcal; 68 g | 216,00 kcal; 68,1 g | BATE |
| B-EMA-03 | Emagrecimento | neves:2006 p.9 | Níveis 50/60/70 % | perda 2/1,5/1 %/sem | mesma tabela | BATE |
| B-EMA-04 | Emagrecimento | neves:2006 p.8–9 | Cão 35 → 28 kg, 3 níveis | perda em % do peso **inicial** (linear) | perda composta sobre o peso da semana | DIVERGE |
| B-EMA-05 | Emagrecimento | neves:2006 p.8 | Branda | ~1 %/sem, programa > 12 semanas | 1 %/sem, plano 15 sem | BATE |
| B-EMA-06 | Emagrecimento | neves:2006 p.8–9 | Cão 28 kg alvo, frações | 50–75 % (20–50 % de restrição) | só 50/60/70 % | DIVERGE (faixa parcial) |
| B-EMA-07 | Emagrecimento | neves:2006 p.8 | Cão 40 kg, ideal 28 kg | perder em fases de 20 % | um alvo por dieta | NÃO TESTÁVEL |
| B-EMA-08 | Emagrecimento | gomes:2016 p.33 | = B-EMA-02 | 216 kcal; 68 g | 216 kcal; 68,1 g | BATE |
| B-EMA-09 | Emagrecimento | gomes:2016 p.31–32; Purina p.21 | Gato 6 → 5 kg, 0,8 × 70 × PC^0,75 | 187,25 kcal | K 80, P 0,75, Branda → 187,25 | BATE\* |
| B-EMA-10 | Emagrecimento | gomes:2016 p.31 | Gato 6 kg, Branda, 4 semanas | 1 %/sem; 3–4 %/mês | 3,94 % em 4 sem (5,76 kg) | BATE |
| B-EMA-11 | Emagrecimento | carciofi:obesidade p.12–13 | Cão 30 → 25,5 kg, K 72, P 0,75 | 408,5 / 490,2 / 571,9 kcal | idem | BATE |
| B-EMA-12 | Emagrecimento | carciofi:obesidade p.13 | Gato 6 kg atual, 85 × PC_atual^0,4 | 87,0 / 104,4 / 121,8 kcal | sobre o alvo (5 kg): 80,9 / 97,1 / 113,3 | DIVERGE (base de peso) |
| B-EMA-13 | Emagrecimento | carciofi:obesidade p.12–13 | Taxas por espécie; duração | cão 1–2 %, gato 0,5–1 %; 8–40 sem | 1/1,5/2 % p/ ambas; 15 sem | DIVERGE |
| B-EMA-14 | Emagrecimento | Purina p.11 | Cão 40 → 35 kg, 1 × RER linear, ração 300 kcal/100 g | 1120 kcal; 373 g | 1007,3 kcal; 335,8 g (K 100, Branda) | DIVERGE (−10,1 %) |
| B-EMA-15 | Emagrecimento | Purina p.21–22 | Gato 4 → 3 kg, 0,8 × RER linear | 128 kcal; 213 g úmida; 37 g seca | 127,65 kcal; 212,8 g; 36,5 g | BATE\* (−0,27 %) |
| B-EMA-16 | Emagrecimento | Purina p.10 | Cão 40 kg perdendo 1 kg/sem | 1–3 %/sem recomendável | alerta "prejudicial" > 2 % | DIVERGE |
| B-EMA-17 | Emagrecimento | cline:2021 p.8–9 | Cão 30 → 25 kg (1,0 RER); gato 6 → 5 kg (0,8 RER) | 782,62 / 187,25 kcal | K 100 / K 80, Branda → idem | BATE\* |
| B-EMA-18 | Emagrecimento | cline:2021 p.9 | Cão alvo 25 kg | 63 ± 10,2 kcal/kg^0,75 → 704,4 (590–818) | K 90 Branda = 704,4; padrão Cão Ativo Moderada = 872,1 | BATE\* (padrão acima da faixa) |
| B-EMA-19 | Emagrecimento | cline:2021 p.9 | Gato alvo 5 kg | 52 ± 4,9 kcal/kg^0,711 → 163,3 (148–179) | K 104, P 0,711, Severa = 163,3 | BATE\* |
| B-EMA-20 | Emagrecimento | Vargas p.1 | Cão 30,2 → 25,67 kg, 70 × PM^0,75, ração 287 kcal/100 g | 798,30 kcal; 278 g | K 100, Branda → 798,30; 278,2 g | BATE\* |
| B-EMA-21 | Emagrecimento | Vargas p.1 | 2ª meta 25,67 → 21,82 kg | "70,6,70" kcal (erro tipográfico) | 706,71 kcal | BATE\* (fonte com erro) |
| B-EMA-22 | Emagrecimento | Vargas p.1 | 30,2 → 15 kg | 25–100 semanas | composto: 35–140 sem; plano 15 sem | DIVERGE |
| B-EMA-23 | Emagrecimento | oliveira:2016 p.10; carciofi:obesidade p.12; andrade:2020 p.12; Vargas p.1 | Cão 30 kg | 1ª meta = atual − 15 % (ou 20 %) = 25,5 kg | alvo é digitado | NÃO TESTÁVEL |
| B-EMA-24 | Emagrecimento | andrade:2020 p.13 | Cão 20 → 17 kg | 70 × PM^0,75 = 586,05 | K 100, Branda → 586,05 | BATE\* (fonte inconsistente) |
| B-ECC-01 | ECC/peso ideal | neves:2006 p.5 | Cão 30 kg, CCF 4/5 | 10–20 % acima → ideal 25,0–27,3 kg | — | NÃO TESTÁVEL |
| B-ECC-02 | ECC/peso ideal | Purina p.4 e p.14–15 | Cão 30 kg, ECC 7/9 | 20–30 % acima → ideal 23,1–25,0 kg | — | NÃO TESTÁVEL |
| B-ECC-03 | ECC/peso ideal | cline:2021 p.3 e p.5 | Cão 30 kg, ECC 7/9, GC 32 % | 20 % → 25,0 kg; por %GC → 25,5 kg | — | NÃO TESTÁVEL |
| B-ECC-04 | ECC/peso ideal | várias | Definições | sobrepeso/obesidade a partir de ~15 % | — | NÃO TESTÁVEL |
| B-ECC-05 | ECC/peso ideal | Vargas p.1 | Cocker 30,2 kg, ICC 9/9, padrão 15 kg | excesso real 101 % | — | NÃO TESTÁVEL |
| B-MAN-01 | Manutenção | neves:2006 p.2 e p.9 | Cão 28 kg, 132 × PC^0,75 | 1600 (exato 1606,73) | K 132 → 1606,73; padrão Cão Ativo 130 → 1582,4 | BATE (K custom) |
| B-MAN-02 | Manutenção | neves:2006 p.2 e p.9 | Gato 6 kg, 60 × PC | 360 kcal | K 60, P 1 → 360; padrão Magro → 332,2 | BATE (K custom) |
| B-MAN-03 | Manutenção | cline:2021 p.8 | Gato castrado 4,5 kg | RER 216, MER 259 | K 84, P 0,75 → 259,53; padrão Magro → 273,9 | BATE (K custom) |
| B-REA-01 | Outro (reajuste) | carciofi:obesidade p.14; gomes:2016 p.31 | Reajuste de 5–10 % | quinzenal, ±5–10 % | energia fixa até o alvo | NÃO TESTÁVEL |
| B-REA-02 | Outro (pós-alvo) | neves:2006 p.10; carciofi:obesidade p.15 | Cão 35 → 28 kg, Severa | aumentar aos poucos até a manutenção | salta para 100 % (1606,73) na semana 13 | DIVERGE |
| B-OUT-01 | Outro | neves:2006 p.10 | Cão 35 → 28 kg, Moderada | 7700 kcal por kg de gordura | — | NÃO TESTÁVEL |
| B-OUT-02 | Outro (EM da ração) | andrade:2020 p.14 | Dieta caseira cão | EM 1,45 kcal/g | 1,357 kcal/g | DIVERGE (−6,4 %) |
| B-OUT-03 | Outro (EM da ração) | andrade:2020 p.14 | Dieta caseira gato | EM 1,51 kcal/g | 1,496 kcal/g | BATE (−0,95 %) |

**Totais: 37 casos — 19 BATE (8 deles BATE\*, com contorno no K), 9 DIVERGE, 9 NÃO TESTÁVEL.**

---

## Casos

### B-EMA-01 — Labrador 35 kg (exemplo resolvido)
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.9 (revista p.37), Tabela 2 (citando Biourges 1997).
- **Entrada:** cão, peso atual 35 kg, CCF 5/5, peso ideal 28 kg; manutenção 132 × PC^0,75; consumo de 60 %; ração 3065 kcal/kg.
- **Fonte diz:** 132 × 28^0,75 ≈ 1600 kcal; 1600 × 0,6 = 960 kcal/dia; 960/3065 = 0,313 kg = 313 g/dia.
- **Conta:** 28^0,75 = 12,172 (a fonte também dá 12,17); 132 × 12,172 = 1606,73 kcal; × 0,6 = **964,04 kcal**; 964,04/3,065 = **314,5 g**. A fonte arredondou 1606,7 para 1600 antes de multiplicar.
  - Erro conceitual da fonte: chama 35 kg de "20 % de excesso" e tira 20 % do peso atual (35 × 0,8 = 28). Se o excesso fosse 20 % **sobre o ideal**, o ideal seria 35/1,2 = 29,17 kg (e a energia 994,0 kcal).
- **Sistema:** perfil "Cão em Emagrecimento" (customizado), K = 132, P = 0,75, peso alvo 28, Moderada → 132 × 28^0,75 × 0,60 = **964,04 kcal**; com ração de EM 3,065 kcal/g → 314,5 g.
- **Situação:** **BATE** (+0,42 % sobre os 960 da fonte; diferença só do arredondamento).
- **Onde testar:** animal cão com peso 35 kg → Dieta → Emagrecimento: K 132; P 0,75; peso alvo 28; nível Moderada; ração com EM 3065 kcal/kg. Conferir 964,04 kcal/dia e ~314,5 g/dia; plano semanal com 964,04 kcal em todas as semanas antes do alvo.

### B-EMA-02 — Gato 8 kg (exemplo resolvido)
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.9 (revista p.37), Tabela 3.
- **Entrada:** gato doméstico pelo curto, 8 kg, CCF 5/5; manutenção 60 × PC (linear); 60 %; ração 3170 kcal/kg.
- **Fonte diz:** 20 % de 8 kg = 1,6 kg, arredondado para 2 kg → ideal 6 kg; 60 × 6 = 360 kcal; 360 × 0,6 = 216 kcal; 216/3170 = 0,068 kg = 68 g.
- **Conta:** 60 × 6 × 0,6 = **216,00 kcal**; 216/3,170 = **68,1 g**. Erros da fonte: (a) arredondar 1,6 kg para 2 kg muda o alvo (com 6,4 kg seriam 230,4 kcal); (b) com "20 % acima do ideal" o ideal seria 8/1,2 = 6,67 kg (240 kcal); (c) a própria tabela diz que a faixa da espécie é 3,5–5,0 kg, ou seja, 6 kg ainda fica acima dela.
- **Sistema:** "Gato em Emagrecimento", K = 60, P = 1, alvo 6, Moderada → 60 × 6^1 × 0,6 = **216,00 kcal**; 68,1 g. Com o perfil padrão "Gato Doméstico Magro" (100 × PC^0,67) daria 199,30 kcal; com "Obeso" (130 × PC^0,40), 159,72 kcal.
- **Situação:** **BATE** (com K e P digitados).
- **Onde testar:** gato 8 kg → Emagrecimento: K 60; P 1; alvo 6; Moderada; ração 3170 kcal/kg → 216 kcal e ~68,1 g.

### B-EMA-03 — Nível de restrição ↔ perda semanal
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.9 (revista p.37), texto após as tabelas.
- **Regra:** restrição para 50 % → perda de 2 %/sem; 60 % → 1,5 %/sem; 70 % → 1 %/sem.
- **Entrada (montada):** cão 35 kg → alvo 28 kg, K 132, P 0,75.
- **Fonte diz / conta:** energia 803,36 / 964,04 / 1124,71 kcal; perda de 0,70 / 0,525 / 0,35 kg na 1ª semana.
- **Sistema:** `NivelRestricaoDados` usa exatamente (0,50; 2 %), (0,60; 1,5 %), (0,70; 1 %). Semana 1 do plano: 35,000 → 34,300 (Severa), 34,475 (Moderada), 34,650 (Branda).
- **Situação:** **BATE**.
- **Onde testar:** mesmo animal; criar três dietas trocando o nível e conferir a kcal e a coluna "peso projetado" da semana 1.

### B-EMA-04 — Projeção: perda sobre o peso inicial × composta
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.8–9 (revista p.36–37). A fonte expressa a perda semanal como % **do peso inicial** (o texto da p.8 usa "1 % do peso inicial por semana"), isto é, perda constante em kg.
- **Entrada (montada):** cão 35 kg → 28 kg.
- **Fonte (linear):** Moderada 0,525 kg/sem → 13,3 semanas; Severa 0,70 kg/sem → 10 semanas; Branda 0,35 kg/sem → 20 semanas. Semana 2 (Moderada): 33,950 kg.
- **Sistema (composto):** Moderada atinge 28 kg na semana 15 (s15: 28,325 → 28,000, limitado ao alvo); Severa na semana 12 (s12: 28,026 → 28,000; s13 "Alvo atingido"); Branda não chega dentro das 15 semanas (s15: 30,406 → 30,102 kg). Semana 2 (Moderada): 33,958 kg.
- **Situação:** **DIVERGE** — método diferente (composto × linear). A diferença é pequena nas primeiras semanas (0,008 kg na s2) e cresce: 1,5 semana a mais na Moderada, 1 na Severa e > 2 na Branda.
- **Onde testar:** tabela do plano semanal, coluna "peso projetado", com os valores acima.

### B-EMA-05 — 1 % por semana e duração acima de 12 semanas
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.8 (revista p.36), citando Grogan 1995: perda de ~1 % do peso inicial por semana, total num período acima de 12 semanas; pesagem semanal e retorno quinzenal.
- **Entrada (montada):** cão 35 kg → 28 kg, Branda.
- **Fonte diz:** 1 %/sem; em 12 semanas, 12 % (35 → 30,8 kg, linear).
- **Sistema:** Branda = 1 %/sem; s12 projetada = 35 × 0,99^12 = 31,02 kg (composto); plano de 15 semanas (> 12).
- **Situação:** **BATE** (taxa e duração compatíveis; a pequena diferença na projeção é a de B-EMA-04).
- **Onde testar:** tabela do plano semanal (Branda), semana 12.

### B-EMA-06 — Faixa de restrição 50–75 % (ou 20–50 % de corte)
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.8 (corte de 20 a 50 % sobre a necessidade do peso ideal) e p.9 (consumo de 50 a 75 % nas Tabelas 2 e 3).
- **Entrada (montada):** cão alvo 28 kg, K 132.
- **Fonte:** 50 % = 803,36; 60 % = 964,04; 70 % = 1124,71; 75 % = 1205,05; 80 % (corte de 20 %) = 1285,38 kcal.
- **Sistema:** só 50/60/70 %. 75 % e 80 % não são selecionáveis (só com contorno no K, ex.: K 132 × 75/70 = 141,43 com Branda).
- **Situação:** **DIVERGE** (o sistema cobre só parte da faixa da fonte). Não é erro, é escolha de projeto; vale registrar.
- **Onde testar:** abrir o select "Nível de Restrição" e conferir que só há 50/60/70 %.

### B-EMA-07 — Perda em fases de 20 %
- **Tema:** Emagrecimento
- **Fonte:** `neves:2006`, PDF p.8: quem tem mais de 20 % de excesso deve perder em etapas de 20 %, reavaliando o objetivo.
- **Entrada (montada):** cão 40 kg, ideal 28 kg (excesso de 42,9 % sobre o ideal). Fase 1: 40 × 0,8 = 32 kg (se "20 % do atual") ou 40/1,2 = 33,3 kg (se "20 % sobre o alvo"); a fonte não diz qual.
- **Sistema:** aceita um alvo por dieta; não divide em fases. O veterinário pode digitar o alvo da fase 1 e abrir outra dieta depois.
- **Situação:** **NÃO TESTÁVEL** (regra não implementada).

### B-EMA-08 — Exemplo felino repetido em Gomes
- **Tema:** Emagrecimento
- **Fonte:** `gomes:2016`, PDF p.33 (impressa p.28), Figura 5, adaptada de Guimarães e Tudury.
- **Entrada / resultado:** os mesmos de B-EMA-02 (8 kg → 6 kg, 60 × PC, 60 %, 3170 kcal/kg → 216 kcal e 68 g).
- **Erro da fonte:** a fase 1 diz que o peso ideal fica entre 3 e 5 kg e, na mesma frase, toma 5 kg como se fosse o peso atual com 20 % de excesso (1,6 kg), terminando em meta de 6 kg. Só a conta final está coerente com Guimarães e Tudury.
- **Sistema:** 216,00 kcal; 68,1 g.
- **Situação:** **BATE**.
- **Onde testar:** igual a B-EMA-02.

### B-EMA-09 — Gato: perda de peso = 0,8 × RER
- **Tema:** Emagrecimento
- **Fonte:** `gomes:2016`, PDF p.31–32 (impressa p.26–27), Quadro 1 adaptado de Jericó et al. 2014; a mesma tabela está no Manual Purina, PDF p.21.
- **Entrada (montada):** gato 6 kg, ideal 5 kg. A fonte não diz se o RER é do peso atual ou do ideal; usei o ideal (como na AAHA).
- **Fonte:** RER = 70 × 5^0,75 = 234,06; × 0,8 = **187,25 kcal**. (Sobre o peso atual de 6 kg seria 214,68.) Pela fórmula linear: (30 × 5 + 70) × 0,8 = 176,0 kcal.
- **Erros da fonte:** (a) Gomes imprime a fórmula linear como 30 × [30 × PC + 70], o que daria 6600 kcal para 5 kg; o correto é 30 × PC + 70 (Purina p.21 traz certo). (b) Gomes e Purina dizem que o RER médio de mamíferos é "70 kcal/dia" ou "70 kcal/kg/dia"; o 70 é o coeficiente da fórmula (kcal/kg^0,75/dia).
- **Sistema:** a fração 0,8 não existe. Contorno: K = 80, P = 0,75, alvo 5, Branda → 80 × 5^0,75 × 0,7 = **187,25 kcal**.
- **Situação:** **BATE\***.
- **Onde testar:** gato 6 kg → Emagrecimento: K 80; P 0,75; alvo 5; Branda.

### B-EMA-10 — Meta de 1 %/semana e 3–4 % ao mês (gato)
- **Tema:** Emagrecimento
- **Fonte:** `gomes:2016`, PDF p.31 (impressa p.26), citando Zoran 2009.
- **Entrada (montada):** gato 6 kg → alvo 5 kg, Branda.
- **Fonte:** 1 %/sem; 3 a 4 % em um mês.
- **Conta / sistema:** composto em 4 semanas: 1 − 0,99^4 = 3,94 % → 6 × 0,99^4 = **5,764 kg** no fim da semana 4 (s4: 5,822 → 5,764).
- **Situação:** **BATE** (3,94 % fica dentro de 3–4 %).
- **Onde testar:** tabela do plano semanal, coluna "peso projetado" da semana 4.

### B-EMA-11 — Protocolo Carciofi, cão: 72 × (peso meta)^0,75 × 50–70 %
- **Tema:** Emagrecimento
- **Fonte:** `carciofi:obesidade`, PDF p.12–13 (protocolo de Carciofi et al. 2005, passos 3 e 5).
- **Entrada (montada):** cão 30 kg; peso meta = atual − 15 % = 25,5 kg.
- **Fonte / conta:** 72 × 25,5^0,75 = 816,9 kcal; × 0,5 = **408,5**; × 0,6 = **490,2**; × 0,7 = **571,9 kcal**. Com meta de −20 % (24 kg): 390,4 / 468,4 / 546,5.
- **Ambiguidade da fonte:** lida ao pé da letra, a ingestão fica em 36–50 kcal/kg^0,75, bem abaixo da média de 63 ± 10,2 da AAHA (B-EMA-18). Pode ser que o "72 × PM^0,75" já seja a energia do regime e não a manutenção. Vale confirmar no original de Carciofi et al. 2005.
- **Sistema:** "Cão em Emagrecimento", K 72, P 0,75, alvo 25,5 → 408,49 / 490,19 / 571,89.
- **Situação:** **BATE**.
- **Onde testar:** cão 30 kg → Emagrecimento: K 72; P 0,75; alvo 25,5; repetir com Severa, Moderada e Branda.

### B-EMA-12 — Protocolo Carciofi, gato: 85 × (peso **atual**)^0,4
- **Tema:** Emagrecimento
- **Fonte:** `carciofi:obesidade`, PDF p.13.
- **Entrada (montada):** gato 6 kg (atual), alvo 5 kg (−15 % ≈ 5,1 kg).
- **Fonte:** 85 × 6^0,4 = 174,05 kcal; × 0,5/0,6/0,7 = **87,0 / 104,4 / 121,8 kcal**.
- **Sistema:** usa o peso alvo. K 85, P 0,4, alvo 5 → 80,9 / 97,1 / 113,3 (−7 %); alvo 5,1 → 81,5 / 97,9 / 114,2 (−6,3 %). Como o alvo precisa ser menor que o atual, não há como digitar o peso atual.
- **Situação:** **DIVERGE** — outra base de peso (atual × alvo). A mesma ambiguidade de B-EMA-11 vale aqui: os valores parecem baixos para um regime.
- **Onde testar:** gato 6 kg → Emagrecimento: K 85; P 0,4; alvo 5; os três níveis.

### B-EMA-13 — Taxa semanal por espécie e duração de 8 a 40 semanas
- **Tema:** Emagrecimento
- **Fonte:** `carciofi:obesidade`, PDF p.12–13 (passo 4).
- **Regra:** perda de 1–2 %/sem em cães e 0,5–1 %/sem em gatos; 8 a 40 semanas para perder 15–20 %.
- **Conta:** linear: 15 % a 2 % = 7,5 sem; 20 % a 0,5 % = 40 sem (coincide com "8 a 40"). Composta: 8,0 e 44,5 sem.
- **Sistema:** as mesmas taxas (1; 1,5; 2 %) valem para as duas espécies; não há 0,5 %; para gato, Moderada (1,5 %) e Severa (2 %) passam do teto felino de 1 %. O plano tem 15 semanas fixas, então não mostra programas de 20–40 semanas.
- **Situação:** **DIVERGE** (sem distinção por espécie; duração).
- **Onde testar:** gato → Emagrecimento → nível Severa: o sistema aceita e projeta 2 %/sem sem aviso.

### B-EMA-14 — Manual Purina, cão 40 → 35 kg (exemplo resolvido)
- **Tema:** Emagrecimento
- **Fonte:** Purina/ABEV (sem bibkey), PDF p.11 (exemplo canino); a caixa dos fatores caninos está cortada na impressão, mas mostra "perda de peso = 1 × RER".
- **Entrada:** cão 40 kg, ideal 35 kg; RER linear sobre o ideal; 1 × RER; ração 300 kcal/100 g.
- **Fonte:** RER = 30 × 35 + 70 = **1120 kcal**; 1120/3,0 = **373,3 g**.
- **Sistema:** não há fórmula linear nem fração 1,0. O mais próximo é a forma alométrica do mesmo texto (70 × PC^0,75) com contorno: K 100, P 0,75, alvo 35, Branda → **1007,28 kcal** (−10,1 %); 335,8 g.
- **Situação:** **DIVERGE** — método diferente (RER linear × alométrico); acima de ~25 kg as duas formas se afastam.
- **Onde testar:** cão 40 kg → Emagrecimento: K 100; P 0,75; alvo 35; Branda; ração 3000 kcal/kg.

### B-EMA-15 — Manual Purina, gato 4 → 3 kg (exemplo resolvido)
- **Tema:** Emagrecimento
- **Fonte:** Purina/ABEV, PDF p.21–22.
- **Entrada:** gato 4 kg, ideal 3 kg; RER linear; 0,8 × RER; ração úmida 60 kcal/100 g ou seca 350 kcal/100 g.
- **Fonte:** RER = 30 × 3 + 70 = 160; × 0,8 = **128 kcal**; 213,3 g úmida; 36,6 g seca.
- **Sistema (contorno):** K 80, P 0,75, alvo 3, Branda → 80 × 3^0,75 × 0,7 = **127,65 kcal** (−0,27 %); 212,8 g úmida; 36,5 g seca. Perto de 3 kg as duas formas do RER coincidem (70 × 3^0,75 = 159,57).
- **Situação:** **BATE\***.
- **Onde testar:** gato 4 kg → Emagrecimento: K 80; P 0,75; alvo 3; Branda; cadastrar duas rações (600 e 3500 kcal/kg).

### B-EMA-16 — Taxa recomendável de 1–3 %/semana × alerta de 2 %
- **Tema:** Emagrecimento
- **Fonte:** Purina/ABEV, PDF p.10: recomenda perder entre 1 e 3 % do peso por semana, conforme o grau de obesidade, a idade e a condição clínica.
- **Entrada (montada):** cão 40 kg, alvo 35 kg, Severa; registro real de peso na semana 1 = 39,0 kg (perda de 2,5 %).
- **Fonte:** 2,5 % está dentro da faixa recomendável.
- **Sistema:** a variação é 1,0/40 = 2,50 % > 2 % → etiqueta roxa "⚠ 2,50%" e aviso de perda muito severa que pode ser prejudicial.
- **Situação:** **DIVERGE** — limite diferente (o sistema segue Guimarães e Tudury, onde 2 % já é o nível mais severo; Carciofi e Gonçalves também param em 2 % para cães). Vale decidir se o alerta deve depender da espécie (para gato o teto da literatura é 1 %).
- **Onde testar:** detalhes da dieta → registrar peso real 39,0 na semana 1 → conferir a etiqueta e o tooltip.

### B-EMA-17 — AAHA: perda de peso = 1,0 × RER (cão) e 0,8 × RER (gato) sobre o peso ideal
- **Tema:** Emagrecimento
- **Fonte:** `cline:2021`, PDF p.8 (JAAHA p.160), Box 1 (fator "weight loss": felino 0,8, canino 1,0) e PDF p.9 (p.161), que manda basear as contas no peso ideal.
- **Entrada (montada):** cão 30 kg, ECC 7 → ideal 25 kg; gato 6 kg, ECC 7 → ideal 5 kg.
- **Fonte / conta:** cão 70 × 25^0,75 × 1,0 = **782,62 kcal**; gato 70 × 5^0,75 × 0,8 = **187,25 kcal**.
- **Sistema (contorno):** cão K 100, P 0,75, alvo 25, Branda → 782,62; gato K 80, P 0,75, alvo 5, Branda → 187,25.
- **Situação:** **BATE\***. A taxa de perda que o sistema associa (1 %/sem, Branda) não vem da AAHA.
- **Onde testar:** Emagrecimento com os valores acima.

### B-EMA-18 — AAHA: ingestão média observada em cães = 63 ± 10,2 kcal/kg^0,75
- **Tema:** Emagrecimento
- **Fonte:** `cline:2021`, PDF p.9 (JAAHA p.161), dados de Flanagan et al. sobre 12 semanas.
- **Entrada (montada):** cão, alvo 25 kg.
- **Fonte:** 63 × 25^0,75 = **704,36 kcal** (faixa ±1 DP: 590,3 a 818,4).
- **Sistema:** contorno K 90, P 0,75, Branda (90 × 0,7 = 63) → 704,36 (**BATE\***). Com os perfis padrão: Cão Ativo (130) dá 65 / 78 / 91 kcal/kg^0,75 = 726,7 / 872,1 / 1017,4 kcal (Severa/Moderada/Branda); Moderada e Branda ficam **acima** de +1 DP. Cão Inativo (95) dá 47,5 / 57 / 66,5, dentro ou abaixo da faixa.
- **Situação:** **BATE\***, com alerta: os padrões do sistema para cão ativo dão mais energia que a média observada pela AAHA.
- **Onde testar:** cão (alvo 25) → Emagrecimento: K 90; P 0,75; Branda. Depois comparar com o perfil Cão Ativo nos três níveis.

### B-EMA-19 — AAHA: ingestão média observada em gatos = 52 ± 4,9 kcal/kg^0,711
- **Tema:** Emagrecimento
- **Fonte:** `cline:2021`, PDF p.9 (JAAHA p.161).
- **Entrada (montada):** gato, alvo 5 kg.
- **Fonte:** 52 × 5^0,711 = **163,29 kcal** (faixa ±1 DP: 147,9 a 178,7).
- **Sistema:** K 104, P 0,711, Severa (104 × 0,5 = 52) → **163,29** (BATE\*). Perfis padrão no alvo de 5 kg: Magro 100 × PC^0,67 → 147,0 / 176,4 / 205,8; Obeso 130 × PC^0,40 → 123,7 / 148,5 / 173,2. Só Moderada (Magro) e Branda (Obeso) caem dentro da faixa.
- **Situação:** **BATE\***.
- **Onde testar:** gato → Emagrecimento: K 104; P 0,711; alvo 5; Severa.

### B-EMA-20 — Vargas: Cocker 30,2 kg, primeira meta (exemplo resolvido)
- **Tema:** Emagrecimento
- **Fonte:** Vargas 2014 (sem bibkey), PDF p.1.
- **Entrada:** cão Cocker Spaniel macho inteiro, 30,2 kg, ICC 9/9; meta final 15 kg; meta 1 = atual − 15 %; NED = 70 × PM^0,75; ração 287 kcal/100 g.
- **Fonte:** PM1 = 30,2 − 4,53 = 25,67 kg; NED = **798,30 kcal**; **278 g/dia**; perda esperada de 0,5–2 %/sem = 151–604 g.
- **Conta:** 70 × 25,67^0,75 = 798,30 ✓; 798,30/2,87 = 278,2 g ✓; 30,2 × 0,005 = 0,151 e × 0,02 = 0,604 kg ✓.
- **Sistema (contorno):** K 100, P 0,75, alvo 25,67, Branda → **798,30 kcal**; 278,2 g.
- **Situação:** **BATE\***.
- **Onde testar:** cão 30,2 kg → Emagrecimento: K 100; P 0,75; alvo 25,67; Branda; ração 2870 kcal/kg.

### B-EMA-21 — Vargas: segunda meta
- **Tema:** Emagrecimento
- **Fonte:** Vargas 2014, PDF p.1.
- **Entrada:** peso de partida 25,67 kg; PM2 = 25,67 − 15 % = 21,82 kg.
- **Fonte:** PM2 = 21,82 kg; NED impresso como "70,6,70 Kcal" (erro tipográfico).
- **Conta:** 70 × 21,82^0,75 = **706,71 kcal** (provavelmente era 706,70). Na sequência a cadeia de metas seria 25,67 → 21,82 → 18,55 → 15,77 → (15) kg.
- **Sistema (contorno):** cão 25,67 kg → K 100, P 0,75, alvo 21,82, Branda → 706,71.
- **Situação:** **BATE\*** (a fonte tem erro de impressão).

### B-EMA-22 — Vargas: duração de 25 a 100 semanas
- **Tema:** Emagrecimento
- **Fonte:** Vargas 2014, PDF p.1.
- **Entrada:** 30,2 → 15 kg, perda de 0,5–2 %/sem.
- **Fonte:** cerca de 25 a 100 semanas, com reavaliação mensal.
- **Conta:** linear sobre o peso inicial: 15,2/0,604 = 25,2 sem e 15,2/0,151 = 100,7 sem (coincide com a fonte). Composta: 34,6 sem (2 %) e 139,6 sem (0,5 %).
- **Sistema:** projeção composta e plano de 15 semanas; com Severa, partindo de 30,2 kg e alvo 15 kg, a semana 15 mostra 30,2 × 0,98^15 = 22,30 kg (sem atingir o alvo).
- **Situação:** **DIVERGE** (linear × composto; o plano é curto para alvos distantes).
- **Onde testar:** cão 30,2 kg → Emagrecimento, alvo 15, Severa → semana 15 do plano = 22,30 kg.

### B-EMA-23 — Primeira meta = peso atual − 15 % (ou 20 %)
- **Tema:** Emagrecimento
- **Fontes:** `oliveira:2016` PDF p.10 (revista p.45), objetivo inicial de perder 15 % do peso; `carciofi:obesidade` PDF p.12, meta da 1ª etapa de −15 % ou −20 %; `andrade:2020` PDF p.12 (revista p.116), meta inicial de no máximo 15 % do atual; Vargas PDF p.1.
- **Entrada (montada):** cão 30 kg → 25,5 kg (−15 %) ou 24 kg (−20 %).
- **Sistema:** não calcula o alvo; só valida que é menor que o atual.
- **Situação:** **NÃO TESTÁVEL** (sugestão: o formulário poderia sugerir atual × 0,85).

### B-EMA-24 — Gonçalves/Andrade: 70 × (peso meta)^0,75
- **Tema:** Emagrecimento
- **Fonte:** `andrade:2020`, PDF p.13 (revista p.117), citando Pöppl 2018.
- **Entrada (montada):** cão 20 kg → meta 17 kg (−15 %).
- **Fonte:** 70 × 17^0,75 = **586,05 kcal**.
- **Inconsistência da fonte:** o mesmo parágrafo diz que a dieta usa 60 % (cão) ou 70 % (gato) da mantença e que a mantença é ~80 kcal/kg (cão) ou 70 kcal/kg (gato). Por esse caminho: 80 × 17 × 0,6 = 816 kcal (+39 % sobre a fórmula). O exemplo de ingredientes da p.15 também não fecha (diz 40 % de arroz e usa 60 % na regra de três, sem dar o total).
- **Sistema (contorno):** K 100, P 0,75, alvo 17, Branda → **586,05**.
- **Situação:** **BATE\*** (para a fórmula; a regra "kcal/kg × 60 %" não é reproduzível porque é linear).
- **Onde testar:** cão 20 kg → Emagrecimento: K 100; P 0,75; alvo 17; Branda.

### B-ECC-01 — CCF de 5 pontos (Biourges)
- **Tema:** ECC/peso ideal
- **Fonte:** `neves:2006`, PDF p.5 (revista p.33), Quadro 3.
- **Regra:** 1 = mais de 20 % abaixo do ideal; 2 = 10–20 % abaixo; 3 = normal; 4 = 10–20 % acima; 5 = mais de 20 % acima.
- **Entrada (montada):** cão 30 kg, CCF 4/5 → ideal = 30/1,2 a 30/1,1 = **25,0 a 27,3 kg**.
- **Sistema:** registra ECC de 1 a 9 e não calcula peso ideal; a escala de 5 pontos não existe.
- **Situação:** **NÃO TESTÁVEL**.

### B-ECC-02 — Cada ponto de ECC (1–9) ≈ 10–15 % do peso ideal
- **Tema:** ECC/peso ideal
- **Fonte:** Purina/ABEV, PDF p.4 (cão e gato) e p.14–15 (gato): cada ponto acima de 5 ≈ +10–15 % sobre o peso ideal; ECC 7 ≈ 20–30 % de excesso.
- **Entrada (montada):** cão 30 kg, ECC 7 → ideal = 30/1,3 a 30/1,2 = **23,1 a 25,0 kg**.
- **Sistema:** não calcula.
- **Situação:** **NÃO TESTÁVEL** (é o principal candidato a funcionalidade nova: sugerir peso alvo a partir do ECC).

### B-ECC-03 — AAHA: Tabela 2 e peso ideal pelo % de gordura
- **Tema:** ECC/peso ideal
- **Fonte:** `cline:2021`, PDF p.3 (JAAHA p.155) e PDF p.5 (p.157), Tabela 2: ECC 6 = 10 %, 7 = 20 %, 8 = 30 %, 9 = 40 % de sobrepeso; cada ponto ≈ +5 % de gordura corporal; rodapé: peso ideal = peso atual × (100 − %GC) ÷ 0,8 (massa magra = 80 % do peso ideal, supondo 20 % de gordura).
- **Entrada (montada):** cão 30 kg, ECC 7, GC 32 % (meio da faixa 30–34 %).
- **Conta:** por sobrepeso: 30/1,2 = **25,0 kg**; pelo %GC: 30 × (100 − 32)/100 ÷ 0,8 = **25,5 kg** (faixa 24,75–26,25 kg). O rodapé omite o "/100": como escrito, daria 2550.
- **Sistema:** não calcula.
- **Situação:** **NÃO TESTÁVEL**.

### B-ECC-04 — Limite de sobrepeso/obesidade
- **Tema:** ECC/peso ideal
- **Fontes:** `neves:2006` PDF p.2 (até 15 % acima do ótimo é sobrepeso; acima disso, obesidade); `gomes:2016` PDF p.27–28 (> 15 % = obesidade); `oliveira:2016` PDF p.3 (~15 %); `carciofi:obesidade` PDF p.2 (15–30 % = moderadamente obeso); Purina PDF p.6/16 (> 15 %); `andrade:2020` PDF p.9 (10–25 %, Robertson); Vargas PDF p.1 (≥ 15 %).
- **Entrada (montada):** cão de ideal 20 kg → obeso a partir de 23 kg (15 %).
- **Sistema:** não classifica.
- **Situação:** **NÃO TESTÁVEL**. As fontes não concordam (Guimarães usa 20 % no Quadro 3 e 15 % no texto; Andrade, 10–25 %).

### B-ECC-05 — Vargas: ICC 9/9 × padrão da raça
- **Tema:** ECC/peso ideal
- **Fonte:** Vargas 2014, PDF p.1.
- **Entrada:** Cocker 30,2 kg, ICC 9/9, padrão da raça ≈ 15 kg.
- **Conta:** pela regra de 10 %/ponto (B-ECC-03), ICC 9 = 40 % → ideal 21,6 kg; pelo padrão da raça o excesso real é 101 %. Mostra o limite da escala de 9 pontos nos muito obesos (a AAHA, PDF p.4, também diz que o ECC perde precisão acima de ~45 % de gordura).
- **Situação:** **NÃO TESTÁVEL**. Serve de alerta para uma futura sugestão de peso ideal por ECC.

### B-MAN-01 — Manutenção canina 132 × PC^0,75
- **Tema:** Manutenção
- **Fonte:** `neves:2006`, PDF p.2 (Tabela 1, Baucells e Serrano 1992, e texto citando Biourges 1997) e p.9.
- **Entrada:** cão 28 kg (peso ideal do Labrador).
- **Fonte:** 1600 kcal (arredondado). Conta: **1606,73 kcal**.
- **Sistema:** Manutenção com perfil customizado K 132, P 0,75 → 1606,73. Perfil padrão Cão Ativo (130) → 1582,38 (−1,5 %).
- **Situação:** **BATE** (com K digitado).
- **Onde testar:** cão 28 kg → Dieta → Manutenção, perfil "Cão em Manutenção Customizado", K 132, P 0,75. Também aparece no plano de emagrecimento depois do alvo (B-REA-02).

### B-MAN-02 — Manutenção felina 60 × PC (linear)
- **Tema:** Manutenção
- **Fonte:** `neves:2006`, PDF p.2 e p.9 (Biourges 1997; Morgante 1999).
- **Entrada:** gato 6 kg.
- **Fonte:** **360 kcal**.
- **Sistema:** customizado K 60, P 1 → 360,00. Padrões: Magro 100 × 6^0,67 = 332,17 (−7,7 %); Obeso 130 × 6^0,40 = 266,20.
- **Situação:** **BATE** (com K e P digitados).
- **Onde testar:** gato 6 kg → Manutenção customizada K 60, P 1.

### B-MAN-03 — AAHA Box 1: gato castrado de 10 lb
- **Tema:** Manutenção
- **Fonte:** `cline:2021`, PDF p.8 (JAAHA p.160), Box 1.
- **Entrada:** gato castrado, 4,5 kg; RER = 70 × PC^0,75; MER = RER × 1,2.
- **Fonte:** RER = 216; MER = 259.
- **Conta:** 4,5^0,75 = 3,090 (a fonte escreve 3,11, que é 4,536^0,75, isto é, 10 lb exatas; o RER de 216 corresponde a 4,5 kg); RER = 216,28; MER = **259,53**.
- **Sistema:** Manutenção customizada K 84 (= 70 × 1,2), P 0,75 → 259,53. Padrões: Magro → 273,94 (+5,5 %); Obeso → 237,26 (−8,6 %).
- **Situação:** **BATE** (com K digitado).
- **Onde testar:** gato 4,5 kg → Manutenção, "Gato em Manutenção Customizado", K 84, P 0,75.

### B-REA-01 — Reajuste de 5–10 % durante o regime
- **Tema:** Outro (reajuste)
- **Fontes:** `carciofi:obesidade` PDF p.14 (reavaliar a cada duas semanas e ajustar a quantidade em 5 a 10 % para mais ou para menos, em gramas/dia); `gomes:2016` PDF p.31 (sem sucesso, reduzir as calorias em 5 ou 10 %); `neves:2006` PDF p.8 (pesagem semanal e retorno quinzenal).
- **Entrada (montada):** cão em Moderada com 964,04 kcal (B-EMA-01); perda real abaixo da esperada → −10 % = 867,6 kcal (−5 % = 915,8).
- **Sistema:** a energia fica fixa até o alvo; o reajuste só sai criando outra dieta ou mudando K/nível.
- **Situação:** **NÃO TESTÁVEL**.

### B-REA-02 — Depois de atingir o alvo
- **Tema:** Outro (pós-emagrecimento)
- **Fontes:** `neves:2006` PDF p.10 (revista p.38): depois do programa, aumentar a comida aos poucos até a manutenção; `carciofi:obesidade` PDF p.15: dieta de manutenção e pesagens 30, 60, 90 e 120 dias depois; `oliveira:2016` PDF p.11: reavaliar a cada três meses.
- **Entrada (montada):** cão 35 → 28 kg, K 132, P 0,75, Severa.
- **Fonte:** subida gradual de 803,36 kcal até 1606,73 kcal.
- **Sistema:** semanas 1–12 com 803,36 kcal; na semana 12 o peso projetado chega a 28,000 kg; da semana 13 em diante "Alvo atingido" com **1606,73 kcal** (100 % da manutenção do alvo) de uma vez.
- **Situação:** **DIVERGE** (salto direto × transição gradual).
- **Onde testar:** tabela do plano semanal, semanas 12–13.

### B-OUT-01 — 7700 kcal por kg de tecido adiposo
- **Tema:** Outro
- **Fonte:** `neves:2006`, PDF p.10 (revista p.38).
- **Entrada (montada):** Labrador de B-EMA-01 (35 → 28 kg, Moderada, 964,04 kcal).
- **Conta de coerência:** déficit sobre a manutenção do alvo (1606,73 − 964,04 = 642,7 kcal/d) → 0,584 kg/sem = 1,67 % de 35 kg; sobre a manutenção do peso atual (132 × 35^0,75 = 1899,44; déficit de 935,4) → 0,850 kg/sem = 2,43 %. Os dois ficam acima do 1,5 %/sem atribuído ao nível Moderada (o 7700 é para gordura pura; perder massa magra e adaptar o metabolismo reduzem a perda real).
- **Sistema:** não usa o 7700.
- **Situação:** **NÃO TESTÁVEL** (serve só de checagem de ordem de grandeza).

### B-OUT-02 — EM de dieta caseira para cães obesos
- **Tema:** Outro (EM da ração)
- **Fonte:** `andrade:2020`, PDF p.14 (revista p.118), Tabela 1 (sugerida por Carciofi 2013).
- **Entrada:** na matéria seca: PB 30,75; carboidrato (ENN) 53,95; EE 6,41; FB 4,74; MM 4,09; umidade 63,34 %. Na matéria original (MS = 36,66 %): PB 11,27; EE 2,35; FB 1,74; MM 1,50; ENN 19,78.
- **Fonte:** EM = **1,45 kcal/g** (o método não é dito).
- **Sistema (RacaoService, cão):** EB = 1,7465 kcal/g; FB na MS 4,74 % → CDE 84,42 %; ED = 1,4744; EM = ED − 1,04 × 0,1127 = **1,357 kcal/g** (−6,4 %). Atwater modificado: 1,287 kcal/g.
- **Situação:** **DIVERGE** (−6,4 %). A ED do sistema (1,474) fica perto do 1,45 da fonte; talvez a tabela informe outra base energética ou use outro método.
- **Onde testar:** cadastro de ração, espécie cão: umidade 63,34; PB 11,27; EE 2,35; FB 1,74; MM 1,50 (% na matéria original).

### B-OUT-03 — EM de dieta caseira para gatos obesos
- **Tema:** Outro (EM da ração)
- **Fonte:** `andrade:2020`, PDF p.14, Tabela 2.
- **Entrada:** na MS: PB 46,14; ENN 39,11; EE 6,85; FB 3,16; MM 3,48; umidade 62,14 %. Na MO (MS = 37,86 %): PB 17,47; EE 2,59; FB 1,20; MM 1,32; ENN 15,28.
- **Fonte:** EM = **1,51 kcal/g**.
- **Sistema (gato):** EB = 1,9152; CDE = 87,9 − 0,88 × 3,16 = 85,12 %; ED = 1,6302; EM = 1,6302 − 0,77 × 0,1747 = **1,496 kcal/g** (−0,95 %).
- **Situação:** **BATE**.
- **Onde testar:** cadastro de ração, espécie gato: umidade 62,14; PB 17,47; EE 2,59; FB 1,20; MM 1,32.

---

## Divergências mais importantes

1. **Projeção composta × linear (B-EMA-04, B-EMA-22).** Todas as fontes que dão duração (Guimarães e Tudury, Carciofi, Vargas) calculam a perda como % do peso **inicial**. O sistema aplica a taxa sobre o peso da semana, o que alonga o programa (Branda não chega a −20 % em 15 semanas).
2. **Energia padrão alta para cães (B-EMA-18).** Com o perfil Cão Ativo (K 130), Moderada e Branda dão 78 e 91 kcal/kg^0,75, acima do 63 ± 10,2 que a AAHA observou em cães que perderam peso.
3. **Taxas e alerta iguais para cães e gatos (B-EMA-13, B-EMA-16).** A literatura dá 0,5–1 %/sem para gatos; o sistema oferece 1,5 e 2 % para gato sem aviso. Já o Manual Purina aceita até 3 %/sem, acima do alerta de 2 %.
4. **Base de peso e frações fora do menu (B-EMA-06, B-EMA-12, B-EMA-14).** Frações 75 %, 80 % e 100 % do RER (AAHA, Jericó/Purina, Vargas, Pöppl) só saem com contorno no K. O RER linear (30 × PC + 70) e a energia felina de Carciofi sobre o peso atual não são reproduzíveis (até −10 %).
5. **Transição pós-alvo (B-REA-02)** e **ausência de cálculo ECC → peso ideal (B-ECC-02/03)** são as lacunas funcionais mais citadas.

Erros encontrados nas fontes: arredondamentos e "20 % do atual" × "20 % sobre o ideal" em Guimarães e Tudury (B-EMA-01/02); a fórmula linear impressa errada e o "70 kcal/dia" em Gomes (B-EMA-09); a fase 1 contraditória em Gomes (B-EMA-08); "70,6,70" em Vargas (B-EMA-21); a regra de três do arroz e o 80 kcal/kg × 60 % em Andrade/Gonçalves (B-EMA-24); 3,11 no lugar de 3,09 e o "/100" que falta no rodapé da AAHA (B-MAN-03, B-ECC-03); a possível leitura dupla do protocolo de Carciofi (B-EMA-11/12).
