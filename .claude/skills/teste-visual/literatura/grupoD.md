# Grupo D: literatura nova com exemplos numéricos resolvidos

Busca de 21/11/2026. Todas as fontes abaixo foram abertas e conferidas. Nenhuma está no `tese.bib` nem nas pastas `artigos`, `artigos/novos` e `outros artigos`. As contas foram refeitas em python3.

Critério: **BATE** quando a diferença é ≤ 1%. "BATE (na precisão da fonte)" quando a fonte arredonda para 5 ou 10 kcal e o valor do sistema cai no mesmo arredondamento. **DIVERGE** traz a explicação. **NÃO TESTÁVEL** quando o sistema não tem o cálculo.

Conversão usada: 1 lb = 0,45359237 kg; 1 kcal = 4,184 kJ.

---

## 1. Fontes novas

| # | Referência | Acesso | Para que serve |
|---|---|---|---|
| F1 | NATIONAL RESEARCH COUNCIL (Hill, R. C.; Kallfelz, F. A.; Kienzle, E.; Morris, J. G.; Rogers, Q. R.; texto de D. Feuer). **Your Dog's Nutritional Needs: A Science-Based Guide for Pet Owners.** Washington, DC: National Academies Press, 2006. 16 p. https://nap.nationalacademies.org/resource/10668/dog_nutrition_final_fix.pdf | aberto (site oficial da NAP) | Tabela de kcal/dia por peso (10–90 lb) para cães inativos, ativos, adultos jovens ativos, idosos ativos e gestantes. Tabela de lactação (15 kg e 22,7 kg; 2/4/6/8 filhotes; semanas 1–4). Um filhote. É o melhor gabarito de manutenção, gestação e lactação de cadela. |
| F2 | NATIONAL RESEARCH COUNCIL (mesmo comitê). **Your Cat's Nutritional Needs: A Science-Based Guide for Pet Owners.** Washington, DC: National Academies Press, 2006. 16 p. https://nap.nationalacademies.org/resource/10668/cat_nutrition_final.pdf | aberto (NAP) | kcal/dia para gata magra, gata com sobrepeso e gata lactante com 4 filhotes no pico (5–20 lb). |
| F3 | WSAVA GLOBAL NUTRITION COMMITTEE. **Calorie Ranges for an Average Healthy Adult Dog in Ideal Body Condition** (atualizado em jul. 2020). https://wsava.org/wp-content/uploads/2020/07/Calorie-Needs-for-Healthy-Adult-Dogs-updated-July-2020.pdf | aberto | Faixas de kcal/dia de 2 a 49 kg. A nota diz que usou 130 e 95 × PC^0,75. |
| F4 | WSAVA GLOBAL NUTRITION COMMITTEE. **Calorie Needs for an Average Healthy Adult Cat in Ideal Body Condition** (atualizado em jul. 2020). https://wsava.org/wp-content/uploads/2020/07/Calorie-Needs-for-Healthy-Adult-Cats-updated-July-2020.pdf | aberto | Faixas de kcal/dia de 1 a 7 kg, calculadas com 100 × PC^0,67 e 130 × PC^0,4. |
| F5 | CALVEZ, J.; WEBER, M.; ECOCHARD, C.; KLEIM, L.; FLANAGAN, J.; BIOURGE, V.; GERMAN, A. J. **Metabolisable energy content in canine and feline foods is best predicted by the NRC2006 equation.** *PLoS ONE*, v. 14, n. 9, e0223099, 2019. DOI 10.1371/journal.pone.0223099. Planilhas suplementares S1 a S5 (xlsx). Há correção em *PLoS ONE* 20(3): e0319808, 2025, que muda só uma frase dos métodos. | aberto (CC-BY) | **335 rações com composição, EB predita, EM NRC 2006 (FB), Atwater modificado e EM medida.** É a melhor fonte para testar o RacaoService, alimento por alimento. |
| F6 | FUKUDA, N. R. N. **Estimativas de ingestão de nutrientes e energia de alimentos completos comerciais para cães da raça Labrador Retriever.** 2024. Monografia (Graduação em Zootecnia), Universidade Federal de Uberlândia, Uberlândia, 2024. Orientação: J. França. https://repositorio.ufu.br/bitstream/123456789/50454/1/EstimativasIngest%c3%a3oNutrientes.pdf | aberto | Crescimento NRC de um Labrador (semanas 8 e 24, com a curva de Hawthorne et al. 2004) e EM NRC de 3 rações. Fonte brasileira. |
| F7 | ASARO, N. J.; GUEVARA, M. A.; BERENDT, K.; ZIJLSTRA, R.; SHOVELLER, A. K. **Digestibility is similar between commercial diets that provide ingredients with different perceived glycemic responses and the inaccuracy of using the modified Atwater calculation to calculate metabolizable energy.** *Veterinary Sciences*, v. 4, n. 4, 54, 2017. DOI 10.3390/vetsci4040054 | aberto | 3 rações de gato com composição na MS, EM NRC e Atwater. |
| F8 | AAFCO. **Calorie Content** (página Resources > Startups). Association of American Feed Control Officials, 2025. https://www.aafco.org/resources/startups/calorie-content/ | aberto | Exemplo resolvido do Atwater modificado (média de 4 lotes). |
| F9 | FLANAGAN, J.; BISSOT, T.; HOURS, M.-A.; MORENO, B.; FEUGIER, A.; GERMAN, A. J. **Success of a weight loss plan for overweight dogs: the results of an international weight loss study.** *PLoS ONE*, v. 12, n. 9, e0184199, 2017. DOI 10.1371/journal.pone.0184199 | aberto | Regra ECC → peso-alvo (÷1,2, ÷1,3 e ÷1,4 para ECC 7, 8 e 9). Energia inicial de 60 a 80 kcal/kg^0,75 do peso-alvo. Janela de perda de 1–3%/semana. 926 cães. |
| F10 | FLANAGAN, J.; BISSOT, T.; HOURS, M.-A.; MORENO, B.; GERMAN, A. J. **An international multi-centre cohort study of weight loss in overweight cats: differences in outcome in different geographical locations.** *PLoS ONE*, v. 13, n. 7, e0200414, 2018. DOI 10.1371/journal.pone.0200414 | aberto | Mesma regra ECC → peso-alvo para gatos. 53 kcal × PCalvo^0,711. Janela de 0,5–2%/semana. |
| F11 | [Autoria não identificada no PDF.] **Avaliação da dinâmica dos programas de perda de peso em diferentes grupos de cães e ajuste da restrição energética com base no período do emagrecimento.** Trabalho do acervo do Prêmio PremieRpet de pesquisa, 15 p., [2023?]. https://premierpet.com.br/wp-content/uploads/2024/05/7-Avaliacao-da-dinamica-dos-programas-de-perda-de-peso-em-diferentes-grupos-de-caes-e-ajuste-da-restricao-energetica-com.pdf | aberto | 77 cães brasileiros. NEPP = 70 × PM^0,75 com PM = 80% do peso atual. Perda de 10 a 20 g/kg/semana. Fator ajustado por dia. *Autoria e ano precisam ser confirmados antes de citar.* |
| F12 | LUSTOZA, A. B.; SILVA, A. L. P.; CUNHA, M. B. **Protocolo de emagrecimento canino: relato de caso.** *Revista Eletrônica Biociências, Biotecnologia e Saúde*, Curitiba, n. 12, p. 7-10, maio/ago. 2015. https://interin.utp.br/index.php/GR1/article/download/1723/1466 | aberto | Caso real: Pinscher de 2,1 kg para 1,4 kg em 8 semanas. Bom caso negativo para o alerta de perda acima de 2%/semana. |
| F13 | SANDERSON, S. L. **Nutritional Requirements of Small Animals.** In: *Merck Veterinary Manual* (versão profissional). Rahway: Merck & Co., revisão nov. 2023, modificado set. 2024. https://www.merckvetmanual.com/management-and-nutrition/nutrition-small-animals/nutritional-requirements-of-small-animals | aberto | Esquema RER × fator (70 × PC^0,75 × 1,4–1,8). Usado como contraste com o modelo NRC do sistema. |
| F14 | [Autoria não exibida no trecho.] Artigo de nutrição por fase da vida de cães e gatos (parte 1). *Veterinary Nursing Journal* (BVNA), v. 38, jul. 2022, p. 32 (Tabela 5, reproduzindo FEDIAF 2021). https://vnj.bvna.org.uk/volume-38-july-2022/0150847001656508635/p32 | aberto | Fonte secundária com erro de transcrição na lactação de 5–8 filhotes. Serve como caso negativo. *Faltam autor e título.* |
| F15 | KLEIN, C.; THES, M.; BÖSWALD, L. F.; KIENZLE, E. **Metabolisable energy intake and growth of privately owned growing dogs in comparison with official recommendations on the growth curve and energy supply.** *Journal of Animal Physiology and Animal Nutrition*, v. 103, n. 6, p. 1952-1958, 2019. DOI 10.1111/jpn.13191 | aberto (CC-BY) | Equação do NRC para crescimento em MJ (0,544 MJ = 130 kcal). Ingestão real de 493 filhotes. A FEDIAF 2025 a cita, mas ela não está no bib. |
| F16 | PUTAROV, T. C. **Programa nutricional para cães e gatos.** Slides da disciplina Nutrição e Alimentação de Não Ruminantes (Prof. L. Hauschild), FCAV-Unesp, Jaboticabal, [s.d.]. https://www.fcav.unesp.br/Home/departamentos/zootecnia/lucianohauschild/nutricao-caes-e-gatos.pdf | aberto | Material de aula brasileiro com todas as fórmulas do sistema (perfis K, gestação, lactação com L, crescimento). Não tem exemplo numérico. Serve só para conferir as fórmulas. |

**Consultadas e sem caso aproveitável:** Pet Nutrition Alliance, *Calculating Calories Based on Pet Needs* (2017; só RER × fator, sem exemplo); Alexander, Colyer e Morris (2017), *J Nutr Sci* 6:e26, sobre Yorkshire terrier (a página não traz os pesos semanais); Sotta et al. (2016), *Rev. Eletr. Biociências* n. 15 (caso sem peso-alvo numérico); Machado e Geraldo (2011), *Nutrição Animal Fácil* (nada específico de cães e gatos).

**Tentadas sem acesso** (não entram como fonte): Norfolk terrier (*Animals* 2021, 11:1380, MDPI deu 403), Purina Institute PT (403), slides da USP no e-Disciplinas (pedem login), AAHA 2014 Weight Management (JAAHA deu 403).

### Resumo dos casos

| Fonte | Casos | BATE | DIVERGE | NÃO TESTÁVEL |
|---|---|---|---|---|
| F1 NRC cão (folheto) | 10 | 9 | 1 | 0 |
| F2 NRC gato (folheto) | 5 | 3 | 1 | 1 |
| F3/F4 WSAVA | 3 | 2 | 1 | 0 |
| F5 Calvez | 6 | 3 | 3 | 0 |
| F6 Fukuda | 6 | 5 | 0 | 1 |
| F7 Asaro | 4 | 1 | 3 | 0 |
| F8 AAFCO | 1 | 1 | 0 | 0 |
| F9 Flanagan cães | 3 | 0 | 2 | 1 |
| F10 Flanagan gatos | 3 | 1 | 0 | 2 |
| F11 PremieRpet | 3 | 1 | 1 | 1 |
| F12 Lustoza | 2 | 0 | 1 | 1 |
| F13 Merck | 1 | 0 | 1 | 0 |
| F14 BVNA | 2 | 1 | 1 | 0 |
| F15 Klein | 2 | 1 | 0 | 1 |
| F16 Putarov | 1 | 1 | 0 | 0 |
| **Total** | **52** | **29** | **15** | **8** |

---

## 2. Casos

### F1 · NRC (2006), *Your Dog's Nutritional Needs*

A fonte traz uma tabela de "average daily energy needs" com pesos em libras. Os valores conferem com K × PC^0,75 nos perfis do sistema, o que confirma que o folheto foi gerado pelas mesmas equações do NRC 2006.

**D-NRCD-01 · Manutenção, cão inativo (95 × PC^0,75)**
- Entrada: PC = 10/30/50/70/90 lb = 4,54/13,61/22,68/31,75/40,82 kg.
- Fonte: 296 / 674 / 989 / 1.272 / 1.540 kcal/dia.
- Sistema: 95 × PC^0,75 = 295,3 / 673,1 / 987,3 / 1.270,7 / 1.534,3.
- Diferença: −0,25 / −0,14 / −0,17 / −0,10 / −0,37%. **BATE**

**D-NRCD-02 · Manutenção, cão ativo (130 × PC^0,75)**
- Fonte: 404 / 922 / 1.353 / 1.740 / 2.100.
- Sistema: 404,1 / 921,1 / 1.351,0 / 1.738,9 / 2.099,5.
- Diferença: +0,01 / −0,10 / −0,14 / −0,07 / −0,02%. **BATE**

**D-NRCD-03 · Manutenção, adulto jovem ativo (140 × PC^0,75)**
- Fonte: 436 / 993 / 1.451 / 1.876 / 2.264.
- Sistema: 435,1 / 991,9 / 1.455,0 / 1.872,6 / 2.261,0.
- Diferença: −0,20 / −0,11 / +0,27 / −0,18 / −0,13%. **BATE**

**D-NRCD-04 · Manutenção, idoso ativo (105 × PC^0,75)**
- Fonte: 327 / 745 / 1.093 / 1.407 / 1.700.
- Sistema: 326,4 / 743,9 / 1.091,2 / 1.404,5 / 1.695,8.
- Diferença: −0,20 a −0,25%. **BATE**

**D-NRCD-05 · Gestação de cadela, da 4ª semana ao parto (K × PC^0,75 + 26 × PC)**
- Fonte: 518 / 1.274 / 1.940 / 2.570 / 3.170.
- Sistema com K = 130 e "fase final" marcada: 522,0 / 1.274,9 / 1.940,7 / 2.564,4 / 3.160,9. Diferença: +0,77 / +0,07 / +0,04 / −0,22 / −0,29%. **BATE**
- Com K = 132 (valor da FEDIAF 2025, Tabela VII-8): +1,97 / +1,18 / +1,11 / +0,82 / +0,73%. Portanto o folheto usou 130.
- Atenção: no sistema o perfil "Cadela em Gestação" vem com K nulo, preenchido pelo usuário. Vale decidir se o padrão é 130 (NRC, folheto) ou 132 (FEDIAF). Os dois são defensáveis.

**D-NRCD-06 a 09 · Lactação de cadela (145 × PC^0,75 + PC × (24n + 12m) × L)**
- Entrada: PC = 33 lb (usar 15,0 kg) e 50 lb (usar 22,7 kg); filhotes 2/4/6/8; semanas 1–4 (L = 0,75 / 0,95 / 1,1 / 1,2).
- Conferência de uma célula: 15 kg, 2 filhotes, semana 1 = 145 × 15^0,75 + 15 × 48 × 0,75 = 1.105,2 + 540 = 1.645,2. A fonte dá 1.645.
- Diferença nas 32 células: com 15 kg e 22,7 kg, ≤ 0,1%. Com a conversão exata de libra (14,97 e 22,68 kg), fica entre −0,15 e −0,20%. **BATE** em todas.
  - D-NRCD-06 (2 filhotes). 33 lb: 1.645 / 1.789 / 1.897 / 1.969. 50 lb: 2.328 / 2.546 / 2.709 / 2.818.
  - D-NRCD-07 (4 filhotes). 33 lb: 2.185 / 2.473 / 2.689 / 2.833. 50 lb: 3.146 / 3.581 / 3.909 / 4.127.
  - D-NRCD-08 (6 filhotes, n = 4 e m = 2). 33 lb: 2.455 / 2.815 / 3.084 / 3.265. 50 lb: 3.555 / 4.100 / 4.509 / 4.782.
  - D-NRCD-09 (8 filhotes, n = 4 e m = 4). 33 lb: 2.725 / 3.157 / 3.481 / 3.697. 50 lb: 3.964 / 4.618 / 5.109 / 5.437.
- Os casos de 6 e 8 filhotes confirmam a regra n = min(filhotes, 4) e m = filhotes − 4 usada no código.

**D-NRCD-10 · Crescimento, filhote de 10 lb que chegará a 33 lb**
- Entrada: PC = 4,536 kg; PC adulto = 14,969 kg; p = 0,303.
- Fonte: 990 kcal/dia.
- Sistema: 130 × 4,536^0,75 × 3,2 × (e^(−0,87 × 0,303) − 0,1) = 404,1 × 3,2 × 0,6683 = 864,0 kcal/dia (−12,7%).
- **DIVERGE.** Não foi possível reproduzir o 990 com a equação do NRC. Para dar 990 seria preciso PC adulto ≈ 27,4 kg ou PC atual ≈ 6,5 kg. É provável que o folheto tenha um erro ou use outro critério. Não usar como gabarito. O caso D-FUK-01 cobre o crescimento canino com uma fonte que bate.

### F2 · NRC (2006), *Your Cat's Nutritional Needs*

**D-NRCG-01 · Gata magra (100 × PC^0,67)**
- Entrada: 5/10/15/20 lb = 2,268/4,536/6,804/9,072 kg.
- Fonte: 170 / 280 / 360 / 440.
- Sistema: 173,1 / 275,4 / 361,4 / 438,2. Diferença: +1,8 / −1,6 / +0,4 / −0,4%.
- **BATE (na precisão da fonte):** a tabela arredonda para dezenas, e os valores do sistema caem no mesmo arredondamento. No teste automatizado, use tolerância de ±5 kcal ou só os pesos de 15 e 20 lb.

**D-NRCG-02 · Gata com sobrepeso (130 × PC^0,40)**
- Fonte: 180 / 240 / 280 / 310.
- Sistema: 180,4 / 238,0 / 279,9 / 314,1. Diferença: +0,2 / −0,8 / −0,03 / +1,3%.
- **BATE (na precisão da fonte).**

**D-NRCG-03 · Gata lactante, 4 filhotes, pico (semanas 3–4, L = 1,2)**
- Fonte: 336 / 603 / 851 / 1.091.
- Sistema: 100 × PC^0,67 + 60 × PC × 1,2 = 336,4 / 602,0 / 851,2 / 1.091,4. Diferença: +0,11 / −0,17 / +0,03 / +0,03%.
- **BATE.** Confirma C = 60 para 3–4 filhotes e o L de pico.

**D-NRCG-04 · Gata adulta de 9 lb "consumindo 250 kcal"** (referência das tabelas de nutrientes)
- Sistema: 100 × 4,082^0,67 = 256,6 kcal (+2,6%).
- **DIVERGE (leve).** O folheto usa 250 como número redondo de referência, não como resultado de cálculo. Não usar como gabarito.

**D-NRCG-05 · Filhote após o desmame, 5 lb: 200 kcal**
- A fonte não informa o peso adulto esperado, então não dá para aplicar 6,7 × (e^(−0,189p) − 0,66). Para chegar a 200 kcal seria preciso p ≈ 0,97, o que não faz sentido para um filhote. **NÃO TESTÁVEL.**

### F3 e F4 · WSAVA, tabelas de calorias (2020)

**D-WSG-01 · Gato adulto em ECC ideal, 1 a 7 kg**
- Fonte (faixas): 1,0 kg 100–130; 1,5 kg 130–150; 2,0 kg 160–170; 2,5 kg 180–190; 3,0 kg 200–210; 3,5 kg 215–230; 4,0 kg 225–250; 4,5 kg 240–270; 5,0 kg 250–290; 5,5 kg 260–310; 6,0 kg 265–330; 6,5 kg 275–350; 7,0 kg 280–370.
- Sistema: os limites da faixa são o menor e o maior entre 100 × PC^0,67 (magro) e 130 × PC^0,4 (obeso). As curvas se cruzam perto de 2,3 kg.
  - 1,0 kg: 100,0 e 130,0 (0%).
  - 4,0 kg: 226,3 e 253,2 (+0,6 e +1,3%).
  - 6,5 kg: 274,9 e 350,5 (−0,05 e +0,13%).
  - Maior desvio: 2,5 kg, limite inferior 184,8 contra 180 (+2,65%), por causa do arredondamento a 5–10 kcal.
- **BATE (na precisão da fonte).** Bons pontos para teste com tolerância de 1%: 1,0, 3,5, 6,0 e 6,5 kg.

**D-WSD-01 · Cão adulto, limite superior da faixa (2 a 49 kg)**
- Fonte (exemplos): 2 kg 140–177; 10 kg 470–590; 20 kg 790–993; 30 kg 1.080–1.346; 40 kg 1.340–1.670; 49 kg 1.560–1.945.
- O limite superior é 105 × PC^0,75 em 47 dos 48 pesos (≤ 1%). Exemplos do sistema: 10 kg 590,5; 30 kg 1.346,0; 49 kg 1.944,6. Isso equivale ao perfil "idoso ativo/terra-nova" (105) do sistema.
- O único fora é 31 kg (1.397, quando 105 × 31^0,75 = 1.379,5). Parece erro de digitação da tabela, porque o valor rompe a progressão entre 30 e 32 kg.
- **BATE** com o perfil K = 105.

**D-WSD-02 · Cão adulto, nota de rodapé da tabela**
- A nota diz que a tabela usa ativo = 130 × PC^0,75 e inativo = 95 × PC^0,75. Mas os valores impressos correspondem a ≈ 84 e 105 × PC^0,75.
  - 10 kg: 95 daria 534 e 130 daria 731; a tabela traz 470–590.
  - O limite inferior fica 12–14% abaixo de 95 × PC^0,75, e o superior 22–24% abaixo de 130 × PC^0,75.
- **DIVERGE** (inconsistência interna da fonte). A WSAVA parece ter aplicado um desconto de cerca de 20% na atualização de 2020 sem corrigir a nota. Não usar a nota como gabarito. Se for citar a tabela, citar os valores, que batem com 105.

### F5 · Calvez et al. (2019), PLoS ONE, com planilhas S1–S5

Composição (S1, % na matéria natural): umidade, PB, EE, FB, MM, e também FDT e EB medida. EM (S2 a S5): medida, Atwater modificado, NRC 2006 com FB e EB predita (a fórmula do sistema) e outras variantes.

**D-CAL-01 · 182 rações secas para cães, alimento por alimento (EB, CDE, ED, EM, Atwater)**
- EB predita: o sistema reproduz a coluna "GE predicted" nas 182 rações (diferença 0,000%).
- EM NRC (FB, EB predita): 180 de 182 com diferença ≤ 1%. Quase todas são idênticas até a primeira casa decimal.
- Atwater: 180 de 182 ≤ 1%. As exceções são DD20 e DD64, que têm inconsistência interna na planilha (EB bate, EM não).
- Exemplos para o teste (fonte = sistema):

| Código | Umid. | PB | EE | FB | MM | EB (kcal/kg) | CDE (%) | ED | EM NRC | Atwater | EM medida |
|---|---|---|---|---|---|---|---|---|---|---|---|
| DD1 | 7,0 | 23,7 | 13,4 | 3,0 | 6,3 | 4.644,1 | 86,587 | 4.021,2 | **3.774,7** | **3.599,5** | 3.788,1 |
| DD2 | 7,1 | 26,9 | 16,3 | 3,0 | 6,4 | 4.840,8 | 86,582 | 4.191,3 | **3.911,5** | **3.737,5** | 3.784,6 |
| DD4 | 7,4 | 25,2 | 11,4 | 4,5 | 9,0 | 4.435,0 | 84,251 | 3.736,5 | **3.474,4** | **3.338,5** | 3.551,8 |

- **BATE.**

**D-CAL-02 · 104 rações secas para gatos**
- EB: igual em 104 de 104 (diferença máxima 0,09%).
- EM NRC (gato: 87,9 − 0,88·FB_MS; −0,77·PB): 104 de 104 ≤ 1% (pior caso 0,21%).
- Atwater: 104 de 104 ≤ 0,36%.

| Código | Umid. | PB | EE | FB | MM | EB | CDE | ED | EM NRC | Atwater | EM medida |
|---|---|---|---|---|---|---|---|---|---|---|---|
| CD1 | 5,7 | 33,6 | 15,3 | 4,4 | 7,1 | 4.923,7 | 83,794 | 4.125,8 | **3.867,0** | **3.663,0** | 3.976,1 |
| CD2 | 6,7 | 39,5 | 11,3 | 5,28 | 8,1 | 4.724,1 | 82,920 | 3.917,2 | **3.613,1** | **3.362,2** | 3.704,9 |
| CD3 | 8,3 | 36,8 | 16,2 | 2,5 | 7,0 | 4.920,1 | 85,501 | 4.206,7 | **3.923,4** | **3.687,0** | 3.936,1 |

- **BATE.**

**D-CAL-03 · 25 rações úmidas para cães**
- EB: igual (0,000%).
- EM NRC: o sistema dá valores 8–26% menores que os publicados em todas as 25. Exemplo DW1 (umid. 79,7; PB 10,2; EE 5,2; FB 1,5; MM 1,6): sistema 866,0 kcal/kg, fonte 1.063,0, EM medida 912,0.
- Diagnóstico refeito: a coluna publicada é reproduzida com diferença de 0,00% quando se usa (a) a FB na matéria natural, e não na MS, no CDE, e (b) a correção de proteína 1,04 × PB% sem o fator 10 (kcal/100 g contra kcal/kg). Para rações secas o efeito é pequeno, mas com 80% de umidade fica grande.
- O sistema aplica a equação do NRC corretamente (FB na MS). Inclusive, chega mais perto da EM medida (866 contra 912) que o valor publicado.
- **DIVERGE** (erro de cálculo na planilha da fonte). Bom caso para documentar no texto que o sistema calcula FB na MS, como o NRC pede.

**D-CAL-04 · 24 rações úmidas para gatos**
- Mesmo padrão do D-CAL-03 (a hipótese reproduz a fonte com diferença ≤ 0,04%). Exemplo CW1 (79,7; 10,8; 3,6; 1,0; 1,2): sistema 875,1, fonte 989,9, medida 894,0.
- **DIVERGE** (erro da fonte).

**D-CAL-05 · Médias por grupo, rações secas (Tabelas 1 a 3 do artigo)**
- Cão seco (n = 182): umid. 7,4; PB 26,8; EE 16,0; FB 3,3; MM 6,1. Fonte: NRC 3.871 e Atwater 3.705. Sistema: 3.874 (+0,09%) e 3.712 (+0,19%).
- Gato seco (n = 104): 6,2; 34,0; 15,4; 4,3; 6,9. Fonte: NRC 3.874 e Atwater 3.666. Sistema: 3.867 (−0,18%) e 3.661 (−0,14%).
- **BATE.** Ressalva: é a fórmula aplicada à média, não a média das EM. Só funciona porque a fórmula é quase linear quando a umidade é parecida. Para úmidas não vale; veja D-CAL-03 e 04.

**D-CAL-06 · Atwater modificado das rações úmidas**
- O sistema difere da fonte em até 8,3% (cão) e 10,3% (gato). Pela conta reversa, a fonte calculou o ENN com a FDT no lugar da FB (ENN = 100 − PB − EE − FDT − MM − umidade). O sistema usa FB, como faz a AAFCO (F8).
- **DIVERGE** (critério da fonte).

### F6 · Fukuda (2024), monografia UFU

**D-FUK-01 · Crescimento, Labrador com 8 semanas**
- Entrada: PC(t) = 28,1 / (1 + e^(−0,137 (t − 18,6))) (Hawthorne et al., 2004); t = 8 → PC = 5,330 kg; PC adulto = 28,1 kg; p = 0,1897.
- Fonte: 1.091,29 kcal/dia (NRC 2006).
- Sistema: 130 × 5,330^0,75 × 3,2 × (e^(−0,87 × 0,1897) − 0,1) = 1.091,31. Diferença +0,002%. **BATE**

**D-FUK-02 · Crescimento, Labrador com 24 semanas (pico)**
- Entrada: PC = 19,022 kg; p = 0,677.
- Fonte: 1.723,64. Sistema: 1.723,72 (+0,005%). **BATE**
- Outros pontos da mesma curva, para teste (sistema): semana 12 → 1.354,5; 16 → 1.563,2; 20 → 1.684,0; 28 → 1.715,7; 40 → 1.649,6; 52 → 1.625,8.

**D-FUK-03 a 05 · EM de rações secas para filhotes (NRC, cão)**
- A tabela diz "% na MS", mas o ENN foi calculado subtraindo a umidade. Ou seja, os números são da matéria natural, com umidade = 100 − MS.

| Caso | Ração | Umid. | PB | EE | FB | MM | ENN | Fonte EM | Sistema EM | Dif. |
|---|---|---|---|---|---|---|---|---|---|---|
| D-FUK-03 | Superpremium | 6,14 | 33,39 | 15,80 | 2,74 | 7,57 | 34,36 | 3.938,00 | 3.925,29 | −0,32% |
| D-FUK-04 | Premium | 7,09 | 28,89 | 11,26 | 3,81 | 8,73 | 40,22 | 3.567,60 | 3.548,54 | −0,53% |
| D-FUK-05 | Específico de raça | 7,19 | 29,14 | 13,31 | 3,01 | 8,13 | 39,22 | 3.732,42 | 3.716,51 | −0,43% |

- **BATE.** A pequena diferença vem de a autora ter usado a FB tal qual no CDE (91,2 − 1,43 × FB). Com isso a fonte é reproduzida exatamente (3.937,9 / 3.567,3 / 3.732,0). O sistema usa FB/MS, como o NRC pede.

**D-FUK-06 · Crescimento pela FEDIAF 2021 por faixas (210/175/140 kcal × PC^0,75 até 50%, 50–80% e 80–100% do peso adulto) e pela equação de Klein (2019)**
- O sistema não tem esses modelos. **NÃO TESTÁVEL** (fica como referência para a discussão: com essas equações, a necessidade das semanas 8–16 fica em ~77% da do NRC).

### F7 · Asaro et al. (2017), *Veterinary Sciences* (gatos)

Composição na MS, convertida para matéria natural (× (1 − umidade)).

**D-ASA-01 · Dieta "HighPGR"** (umid. 7,16; MS: PB 38,02; EE 10,83; FB 1,17; MM 6,36)
- Fonte: NRC 3.778 kcal/kg. Sistema: 3.778 (0,0%). **BATE**

**D-ASA-02 · "MediumPGR"** (6,76; 35,86; 20,02; 1,78; 6,31)
- Fonte: 4.301. Sistema: 4.151 (−3,5%). **DIVERGE**

**D-ASA-03 · "LowPGR"** (5,31; 42,06; 20,42; 2,58; 6,38)
- Fonte: 4.413. Sistema: 4.229 (−4,2%). **DIVERGE**
- Nos casos 02 e 03 a soma dos nutrientes "na MS" não fecha 100%, e o ENN publicado não confere. É provável que a tabela misture bases. Não usar como gabarito.

**D-ASA-04 · Atwater modificado das 3 dietas**
- Fonte: 3.752 / 4.081 / 4.137. Sistema: 3.507 / 3.933 / 3.984 (−6,5 / −3,6 / −3,7%). **DIVERGE** (mesma inconsistência).

### F8 · AAFCO, *Calorie Content*

**D-AAF-01 · Atwater modificado, média de 4 lotes**
- Entrada: PB 24,80; EE 14,58; FB 3,88; umidade 8,48; MM 3,10 (% na matéria natural). ENN = 45,16 (a fonte imprime 45,18, calculado das médias sem arredondar).
- Fonte: 3.688 kcal/kg. Conta: (3,5 × 24,80 + 8,5 × 14,58 + 3,5 × 45,18) × 10 = 3.688,6.
- Sistema: 3.687,9 (−0,002%). **BATE**
- Referência extra (a fonte não traz): o sistema daria EM NRC cão = 3.824,2 kcal/kg (EB 4.794,8; CDE 85,14; ED 4.082,1).

### F9 · Flanagan et al. (2017), cães

**D-FLC-01 · Peso-alvo a partir do ECC (escala 9)**
- Regra da fonte: peso-alvo = peso atual ÷ 1,2 (ECC 7), ÷ 1,3 (ECC 8) ou ÷ 1,4 (ECC 9). Isso supõe 10% de excesso por ponto acima de 5. Exemplo: 30 kg em ECC 7 → 25,0 kg; 30 kg em ECC 9 → 21,4 kg.
- **NÃO TESTÁVEL** (o sistema não calcula peso ideal pelo ECC). É a fonte primária aberta mais direta para o item 4 da busca, caso essa função seja implementada.

**D-FLC-02 · Energia inicial no emagrecimento**
- Fonte: 80 kcal (macho inteiro), 70 kcal (macho castrado e fêmea inteira) e 60 kcal (fêmea castrada) × PCalvo^0,75. Média em 12 semanas: 63 ± 10,2 kcal/kg^0,75. Exemplo: alvo de 25 kg → 894,4 / 782,6 / 670,8 kcal/dia.
- Sistema, com K = 130 × alvo^0,75: branda 1.017,4; moderada 872,1; severa 726,7.
- 60 kcal equivale a 46% de 130, abaixo do nível severo. 80 kcal (62%) fica perto do moderado, com −2,5%.
- **DIVERGE.** A fonte usa restrições mais fortes e diferentes por sexo. Para reproduzir no sistema, é preciso K customizado (por exemplo, K = 120 com severa 50% dá 60).

**D-FLC-03 · Ritmo semanal de perda**
- Fonte: meta de 1–3%/semana, com ajuste de ±10% da energia fora dessa faixa. Média real: 0,9 ± 0,45%/semana.
- Sistema: níveis de 1 / 1,5 / 2%/semana e alerta acima de 2%.
- **DIVERGE** na faixa de 2–3%, que a fonte aceita e o sistema marca como prejudicial. A média real de 0,9% fica abaixo da projeção branda do sistema. Bom argumento para a discussão: a projeção do sistema é otimista.

### F10 · Flanagan et al. (2018), gatos

**D-FLG-01 · Peso-alvo a partir do ECC:** mesmos divisores (1,2 / 1,3 / 1,4). **NÃO TESTÁVEL.**

**D-FLG-02 · Energia: 53 kcal × PCalvo^0,711**
- Exemplo: gato de 6 kg em ECC 8 → alvo 4,615 kg → 157,2 kcal/dia.
- Sistema (100 × alvo^0,67 × fração): branda 195,0; moderada 167,2; severa 139,3.
- O expoente 0,711 não existe no sistema. **NÃO TESTÁVEL** (se fosse comparado, ficaria entre a moderada e a severa).

**D-FLG-03 · Ritmo semanal:** meta de 0,5–2%/semana; média real de 0,8 ± 0,50%/semana. O limite superior coincide com o alerta de >2% do sistema. **BATE** no limite superior. O limite inferior de 0,5% fica abaixo do nível brando do sistema (1%).

### F11 · PremieRpet (77 cães, Brasil)

**D-PRE-01 · NEPP = 70 × PM^0,75, com PM = 80% do peso atual**
- Exemplo: cão de 20 kg → PM 16 kg → 70 × 8 = 560 kcal/dia.
- Sistema com K = 130 e alvo de 16 kg: branda 728; moderada 624; severa 520. O fator 70 equivale a 53,8% de 130, entre o moderado e o severo.
- **DIVERGE** (nível de restrição diferente; dá para reproduzir com K customizado).

**D-PRE-02 · Faixa de perda semanal: TSPP mínima = 10 g por kg do peso atual e máxima = 20 g/kg**
- Isso equivale a 1–2%/semana, a mesma faixa dos níveis do sistema (1% branda, 2% severa), e a fonte também corrige quando passa de 2%. **BATE**

**D-PRE-03 · Fator ajustado por dia de programa: fator = −0,08 × dias + 70**
- Exemplo da fonte: dia 18 → 68,56. Pela regressão original (−0,079x + 69,53), o resultado seria 68,11.
- O sistema mantém a energia fixa até o alvo. **NÃO TESTÁVEL** (conta só aritmética).

### F12 · Lustoza, Silva e Cunha (2015), relato de caso

**D-LUS-01 · Grau de excesso de peso**
- Entrada: peso de 2,1 kg; peso ideal de 1,4 kg.
- Fonte: "42,8% acima do ideal". Conta: 0,7/1,4 = 50% acima do ideal (ou 33,3% do peso atual). O 42,8% só sai com 2,0 kg (0,6/1,4).
- O sistema não calcula o excesso. **NÃO TESTÁVEL** (registrar como exemplo de conta inconsistente na literatura).

**D-LUS-02 · Ritmo de perda planejado e realizado**
- Planejado: perder 0,7 kg em 16 semanas. Isso dá 2,08% por semana do peso inicial (linear), ou 2,50%/semana composto, já que (1,4/2,1)^(1/16) = 0,9750.
- Realizado: 0,72 kg em 8 semanas, de 2,1 para 1,38 kg, ou 5,1%/semana composto.
- Sistema: os dois ritmos passam de 2%/semana e seriam marcados como prejudiciais. No plano severo (2%/sem) do sistema, em 15 semanas o cão chegaria a 2,1 × 0,98^15 = 1,55 kg. Para chegar a 1,4 kg seriam precisas ~20 semanas.
- **DIVERGE** (a conduta da fonte é mais agressiva). Bom caso negativo para testar o alerta.

### F13 · Merck Veterinary Manual (Sanderson, 2023/2024)

**D-MSD-01 · Cão adulto castrado de 10 kg**
- Fonte (fórmula, sem exemplo): MER = 1,6 × 70 × PC^0,75 = 1,6 × 393,6 = 629,8 kcal/dia. Também lista 1,8 × RER (inteiro), 1,4 × RER (propenso à obesidade) e 1,2 / 1,4 / 1,0 × RER para gatos.
- Sistema (NRC): 130 × 10^0,75 = 731,0 (ativo) ou 95 × 10^0,75 = 534,2 (inativo). O valor do Merck fica entre os dois.
- **DIVERGE** (modelo diferente: RER × fator contra K × PC^0,75). Serve para mostrar, na discussão, que existem dois esquemas na literatura e que o sistema segue o do NRC/FEDIAF.

### F14 · *Veterinary Nursing Journal* (BVNA), 2022

**D-BVN-01 · Lactação de cadela com 5–8 filhotes**
- A fonte imprime 145 × PC^0,75 + (96 + 12n) × PC × L, com n = número de filhotes. A FEDIAF 2025 (Tabela VII-8, conferida no PDF local) e o sistema usam [96 + 12 (n − 4)].
- Exemplo: cadela de 20 kg com 6 filhotes na semana 3 (L = 1,1): FEDIAF/sistema = 145 × 9,457 + 120 × 20 × 1,1 = 4.011,3; pela fórmula da BVNA = 5.067,3 (+26%).
- **DIVERGE** (erro de transcrição na fonte secundária). Ela também diz "0–5 semanas" na gestação, enquanto a FEDIAF diz "primeiras 4 semanas". Não citar a BVNA para essas fórmulas.

**D-BVN-02 · Gata: gestação 140 × PC^0,67; lactação 100 × PC^0,67 + 18/60/70 × PC × L (L = 0,9 / 0,9 / 1,2 / 1,2 / 1,1 / 1,0 / 0,8)**
- Coincide termo a termo com o sistema. **BATE** (só a fórmula, sem exemplo numérico).

### F15 · Klein et al. (2019)

**D-KLE-01 · Equação do NRC para crescimento em MJ**
- Fonte: ME (MJ) = 0,544 × PC^0,75 × 3,2 × (e^(−0,87p) − 0,1). Como 0,544/0,004184 = 130,02, a equação equivale à do sistema com K = 130.
- Exemplo: filhote de 10 kg com adulto previsto de 30 kg dá 6,346 MJ = 1.516,7 kcal; o sistema dá 1.516,5 (0,01%).
- **BATE.**

**D-KLE-02 · Equação preditiva de Klein: ME (MJ) = (1,063 − 0,565 × p) × PC^0,75**
- Mesmo exemplo: 4,92 MJ = 1.175,6 kcal (78% do NRC).
- **NÃO TESTÁVEL** (o sistema não tem essa equação). Fica como referência: a ingestão real de 493 filhotes foi de 78% (8–17 semanas) e 83% (17–26 semanas) do NRC.

### F16 · Putarov (FCAV-Unesp), slides de aula

**D-PUT-01 · Conferência das fórmulas**
- Os slides trazem a mesma tabela de K (200 / 180 / 140 / 130 / 105 / 95), gato 100 × PC^0,67 (ECC ≤ 5) e 130 × PC^0,4 (ECC > 5), gestação de cadela NEM + 26 × PV com NEM = 130, gata gestante 140 × PC^0,67, lactação com os mesmos L e a definição "m = filhotes entre 4 e 8", e o crescimento 3,2 (…) / 6,7 (…).
- **BATE** (fórmulas). Detalhe: no slide de filhote de gato aparece "NEM = 130 × P^0,75", copiado do slide de cão, mas a equação do slide usa 100 × PCa^0,67, que é a do sistema.
- É útil como fonte brasileira de aula que associa o perfil do gato ao ECC (≤ 5 ou > 5).

---

## 3. O que vale levar para os testes

1. **RacaoService:** as planilhas S1/S2/S3 de Calvez et al. (2019) cobrem 286 rações secas com EB, EM NRC e Atwater; 284 batem. Um teste parametrizado com 5–10 linhas (DD1, DD2, DD4, CD1, CD2, CD3) já protege a fórmula inteira. As rações úmidas (D-CAL-03/04) servem de caso negativo documentado: o sistema está certo e a fonte, errada.
2. **Manutenção, gestação e lactação de cadela:** use o folheto do NRC (F1). São 20 valores de manutenção, 5 de gestação com K = 130 e 32 de lactação, todos com diferença ≤ 1%.
3. **Gata:** use F2 (lactação com 4 filhotes) e F4 (WSAVA magra/obesa).
4. **Crescimento de cão:** use Fukuda (F6), semanas 8 e 24. Evite o filhote do folheto F1.
5. **Emagrecimento:** não há fonte nova que use a escala do sistema (70/60/50% de 130). A faixa de 1–2%/semana e o alerta de >2% batem com F11 e F10. A regra ECC → peso-alvo (÷1,2/1,3/1,4) de F9/F10 é a referência primária aberta para uma futura função de peso ideal.
6. **Decisão pendente:** K padrão da gestação de cadela (130 do NRC ou 132 da FEDIAF). Hoje o campo vem vazio.
