# Casos de teste da literatura

Casos tirados de exemplos resolvidos e de regras numéricas da literatura de nutrição de cães e gatos, para conferir se o Imperium calcula o que as fontes dizem. Levantamento de 21/11/2026: 21 PDFs locais da monografia (`PFC-2/artigos`, `artigos/novos`, `outros artigos`), tabelas da WSAVA (`imagens/Wsava`) e uma busca dirigida na web. Ao todo são 192 casos. No levantamento, 101 batiam, 41 divergiam e 50 não eram testáveis. Depois das decisões de 22/11/2026, os 41 divergentes ficaram assim: 5 passam a bater (106 no total), 17 são diferenças mantidas e documentadas, 15 são erros da fonte e 4 são trabalho futuro. Os 50 não testáveis continuam iguais.

O detalhe de cada caso (fonte, página, conta refeita e onde testar) está em `literatura/`:
- `grupoA.md`: cálculo energético e EM da ração (NRC/Carciofi, Ogoshi, ABINPET, FEDIAF).
- `grupoB.md`: obesidade e emagrecimento (Guimarães e Tudury, Gomes, Oliveira, Carciofi, Purina, Vargas, Andrade, AAHA).
- `grupoC.md`: ECC, avaliação, fase da vida (WSAVA, Freeman, Muller, Chun, Michel, Porsani, Silva).
- `grupoD.md`: fontes novas da web, com URL (folhetos do NRC, WSAVA, Calvez, Fukuda, Flanagan e outras).
- `sistema-resumo.md`: o que o sistema calculava quando os casos foram classificados.

## Como usar

- **Situação:** BATE (diferença até 1 %), DIVERGE (o sistema e a fonte não concordam; ver a causa) ou NÃO TESTÁVEL (o sistema não tem a função). BATE\* = só bate digitando um K que embute um fator que o menu não oferece.
- **Teste automático:** os casos marcados com o nome do teste rodam no `make test`, em `backend.Tests/Unitarios/CasosLiteraturaTests.cs` (energia) e `backend.Tests/Racoes/RacaoLiteraturaTests.cs` (EM da ração). O ID do caso aparece na mensagem de falha. Se um deles falhar, leia o caso aqui antes de mexer no teste: a fonte é o gabarito, e não se ajusta o esperado para o teste passar.
- **Teste visual:** siga o roteiro abaixo depois de mudar cálculo, formulário de dieta, ração ou avaliação. Basta cobrir os casos do tipo de dieta que mudou, mais um caso de ração.
- **Dados:** use só animais e rações fictícios, com nome terminando em "(teste)". Avise o usuário dos dados criados no banco local.
- **Manter atualizado:** ao mudar uma fórmula, uma constante ou um perfil, reclassifique os casos afetados aqui e no `grupoX.md`. Ao ler uma fonte nova, acrescente os casos com o próximo ID do grupo (ou crie um grupo novo), sempre com bibkey (ou URL) e página.

## Preparação para o roteiro

Cadastre duas rações da clínica (Rações → Cadastrar, valores em %):

| Ração | Espécie | U | PB | EE | MM | FB | EM esperada | Caso |
|---|---|---|---|---|---|---|---|---|
| Cão NRC (teste) | Cão | 10 | 26 | 16 | 7 | 3 | 3,763 kcal/g (fonte 3,77) | A-EM-03 |
| Gato NRC (teste) | Gato | 10 | 35 | 20 | 6,5 | 2,5 | 4,041 kcal/g (fonte 4,04) | A-EM-04 |

Para cada caso de dieta: cadastre o animal com espécie e sexo do caso, marque uma consulta, preencha a avaliação com o peso do caso e calcule a dieta com o tipo, o perfil e os campos indicados.

## Roteiro: casos que batem

### Ração (tela Rações)

| ID | Entrada (U/PB/EE/MM/FB, %) | Esperado | Teste automático |
|---|---|---|---|
| A-EM-01 | Gato 10/32/15/8/3 | EM 3,72 kcal/g | `RacaoLiteraturaTests` |
| A-EM-02 | Cão 10/24/12/8/3 | EM 3,53 kcal/g | `RacaoLiteraturaTests` |
| A-EM-03 | Cão 10/26/16/7/3 | EM 3,77 kcal/g | `RacaoLiteraturaTests` |
| A-EM-04 | Gato 10/35/20/6,5/2,5 | EM 4,04 kcal/g | `RacaoLiteraturaTests` |
| A-EM-05 | Cão 10/25/14/8/3 | Atwater: PB 87,5; total 346,5 kcal/100 g (a fonte erra: 346,7) | `RacaoLiteraturaTests` |
| D-CAL-01..06 | 335 rações de Calvez et al. (2019) | EM NRC das planilhas S1–S5 | não (ver `grupoD.md`) |

### Manutenção (Dieta → Manutenção)

| ID | Animal | Perfil (K × PC^p) | Esperado (kcal/dia) | g/dia com a ração de referência | Teste |
|---|---|---|---|---|---|
| A-MAN-01 | Cão 7 kg | Terrier (180/0,75) | 774,6 | 206 | `CasosLiteraturaTests` |
| A-MAN-02 | Cão 35 kg | Ativo (130/0,75) | 1870,7 | 496 | idem |
| A-MAN-03 | Cão 12,5 kg | Idoso ativo (105/0,75) | 698 | 185 | idem |
| A-MAN-04 | Cão 32 kg | Idoso ativo (105/0,75) | 1412,7 | 375 | idem |
| A-MAN-05 | Cão 53 kg | Inativo (95/0,75) | 1866 | 495 | idem |
| A-MAN-06 | Cão 2,8 kg | Inativo (95/0,75) | 205,6 | 55 | idem |
| A-MAN-07 | Gato 5,1 kg, ECC 5 | Magro/ideal (100/0,67) | 298 | 73,7 | idem |
| A-MAN-08 | Gato 6,8 kg, ECC 7 | Obeso (130/0,40) | 280 | 69,3 | idem |
| A-MAN-09 | Gato 4,5 kg, ECC 8 | Obeso (130/0,40) | 237 | 58,7 | idem |
| A-MAN-10 | Gato 4,2 kg, ECC 7 | Obeso (130/0,40) | 231 | 57,1 | idem |
| A-MAN-11 | Gato 3,2 kg, ECC 3 | Magro/ideal (100/0,67) | 218 | 54,0 | idem |
| A-MAN-12 | Gato 3,1 kg, ECC 4 | Magro/ideal (100/0,67) | 213 | 52,8 | idem |
| D-NRCD-01..04 | Cão 22,68 kg (50 lb) | 95 / 130 / 140 / 105 | 989 / 1353 / 1451 / 1093 | — | idem |
| A-MAN-13 | Gato 4 kg | K 100 e 75, p 0,67 | 253 e 189 | — | não |
| A-MAN-14 | Cão 15 kg | K 110 / 95 / 130 | 838 / 724 / ~1000 | — | não |

Fontes: nrc:2007 p.13–16 e carciofi:apostila p.16–17 e 102–103 (A-MAN-01..12); fediaf:2025 p.59 (A-MAN-13/14); folheto NRC do cão (D-NRCD).

### Crescimento (Dieta → Crescimento; peso adulto no campo do peso adulto esperado)

| ID | Animal | Peso / adulto | Esperado | Teste |
|---|---|---|---|---|
| A-CRE-01 | Gato | 0,8 / 5 kg | 179 | `CasosLiteraturaTests` |
| A-CRE-02 | Cão | 15 / 30 kg | 1735 | idem |
| A-CRE-03 | Cão | 17 / 35 kg | 1934 | idem |
| A-CRE-04 | Gato | 1 / 4 kg | 198 (sistema 196,9; a fonte usa 6,732 no lugar de 6,7) | não |
| D-FUK-01 | Cão (Labrador, 8 sem.) | 5,330 / 28,1 kg | 1091,29 | `CasosLiteraturaTests` |
| D-FUK-02 | Cão (Labrador, 24 sem.) | 19,022 / 28,1 kg | 1723,64 | idem |

### Gestação (Dieta → Gestação)

| ID | Animal | Entrada | Esperado | Teste |
|---|---|---|---|---|
| A-GES-01 | Cadela 30 kg | K 130, p 0,75, caixa "+26 × Peso" marcada | 2446 | `CasosLiteraturaTests` |
| A-GES-02 | Cadela 22 kg | idem | 1892 | idem |
| D-NRCD-05 | Cadela 22,68 kg | idem | 1940 | idem |
| A-GES-04 | Gata 3,5 kg | perfil Gata em Gestação (K e p vêm preenchidos: 140/0,67), semana 1–9 | 324, sem projeções semanais | idem |

| A-GES-03 | Cadela 30 kg | perfil Cadela em Gestação (130 × PC^0,75), que vem com K 130 e p 0,75 preenchidos; caixa "+26 × Peso" marcada | 2446 (NRC). FEDIAF/ABINPET usam K 132 (2472): divergência mantida | `CalculoNecessidadeEnergeticaTests` (perfil) |

Na tela, confira também: a semana não passa de 9, e a gata não mostra "Projeções Energéticas".

### Lactação (Dieta → Lactação; K e p vêm do perfil)

| ID | Animal | Filhotes / semana | Esperado | Teste |
|---|---|---|---|---|
| A-LAC-01 | Gata 5 kg | 5 / 3 | 714 | `CasosLiteraturaTests` |
| A-LAC-02 | Cadela 27 kg | 7 / 1 | 4390 | idem |
| A-LAC-03 | Cadela 22 kg | 6 / 3 | 4377 | idem |
| A-LAC-04 | Gata 3,5 kg | 4 / 3 | 483 | idem |
| D-NRCD-08 | Cadela 15 kg | 6 / 4 | 3265 | idem |
| D-NRCG-03 | Gata 9,072 kg | 4 / 3 | 1091 | idem |
| A-LAC-07 | Cadela | 9 filhotes | calcula, e mostra o aviso "Mais de 8 filhotes: fora da faixa da fonte" | `AlertasDieta.test.jsx` |
| D-NRCD-06..09 | Cadela 15 e 22,7 kg | 2/4/6/8 filhotes, semanas 1–4 | 32 valores do folheto NRC (ver `grupoD.md`) | não |

### Emagrecimento (Dieta → Emagrecimento)

| ID | Animal | Entrada | Esperado | Teste |
|---|---|---|---|---|
| B-EMA-01 | Cão 35 kg | alvo 28; K 132; p 0,75; Moderada; ração de 3065 kcal/kg | 960 (sistema 964,04, só arredondamento); ~314 g/dia; mesma energia em todas as semanas até o alvo | `CasosLiteraturaTests` |
| B-EMA-02 | Gato 8 kg | alvo 6; K 60; p 1; Moderada; ração de 3170 kcal/kg | 216; 68 g/dia | idem |
| B-EMA-11 | Cão 30 kg | alvo 25,5; K 72; p 0,75; Severa / Moderada / Branda | 408,5 / 490,2 / 571,9 | idem |
| B-EMA-03 | — | níveis 50/60/70 % | perda de 2 / 1,5 / 1 % por semana, na tela do plano | não |
| B-EMA-10 | Gato 6 kg | Branda, 4 semanas | cerca de 4 % em 4 semanas (5,76 kg na projeção) | não |
| B-EMA-04 | Cão 35 kg | alvo 28; K 132; p 0,75; Severa / Moderada / Branda | perda de 0,70 / 0,525 / 0,35 kg por semana (sobre o peso inicial); semana 2 termina em 33,60 / 33,95 / 34,30 kg; "Alvo atingido" na semana 11 / 15 / nunca (15 semanas) | `CasosLiteraturaTests` |
| B-EMA-13 | Gato | Moderada ou Severa | alerta "pode ser prejudicial ao gato" no formulário e na tela da dieta (recomendado 1 %/semana) | `AlertasDieta.test.jsx` |
| B-EMA-18 | Cão | abrir o formulário de emagrecimento | perfil base sugerido: Cão Inativo (95 × PC^0,75); gato: Magro ou Ideal (100 × PC^0,67) | idem |
| B-REA-02 | qualquer | abrir a dieta | aviso "Ao atingir o peso alvo": aumentar aos poucos até a manutenção e reavaliar | não |
| D-LUS-01 | Cão 2,1 → 1,4 kg em 8 semanas | registrar os pesos semanais | cerca de 5 %/semana: o alerta de perda acima de 2 % deve aparecer | não |

Casos BATE\* (com K digitado para embutir a fração da fonte, por exemplo 1,0 × RER = K 100 com Branda): B-EMA-09, 15, 17, 18, 19, 20, 21 e 24. Estão em `grupoB.md`.

### Avaliação, ECC e fase da vida

| ID | O que conferir | Esperado |
|---|---|---|
| C-ECC-01 | Escala do ECC na avaliação | 1 a 9, um valor por avaliação |
| C-ECC-03 | Gato | 5 é o ideal |
| C-ECC-05 / C-ECC-07 | Ajuda do ECC na avaliação e ficha do cão | descrição de cada ponto como na ficha WSAVA; 6–7 sobrepeso, 8–9 obesidade |
| C-ECC-02 | Ficha geral de um cão com ECC 4 | "4 - Ideal" (no gato, "4 - Magro") | `CalculoNecessidadeEnergeticaTests` (rótulo) |
| C-ECC-06 | Ajuda do ECC do gato, pontos 4 e 6 | 4: leve reentrância e bolsa de gordura mínima; 6: reentrância ausente, cintura e bolsa perceptíveis mas não evidentes | não |
| C-ECC (13) | Ajuda do ECC, pontos 7 a 9 | cerca de 20 % / 30 % / 40 % ou mais acima do ideal (Flanagan) | não |
| C-FV-01 | Fase da vida do cão | corte de porte em 25 kg, conforme Muller (diverge só no valor exato de 25,0 kg) |
| C-EN-01 / C-EN-03 / C-EN-04 | Manutenção com K 70 (RER), perfis felinos da WSAVA e boxer de 30 kg com K 180 | 70 × PC^0,75; tabela WSAVA de gatos; 2307,35 kcal |

## Divergências: decisões de 22/11/2026

As 13 divergências foram decididas com o iury em 22/11/2026. As decisões de código foram implementadas em 22/11/2026 (working tree, sem commit). Os casos resolvidos foram para o roteiro acima, com o teste automático de cada um.

| # | Divergência | Decisão | Tipo |
|---|---|---|---|
| 1 | Rótulo "4 - Magro" para cão (C-ECC-02) | Rótulo por espécie: no cão, "4 - Ideal"; no gato, continua "4 - Magro". O valor salvo (1 a 9) não muda, sem migration. | Código |
| 2 | Gato com ECC 6 vai para o perfil obeso (C-ECC-04/06) | Mantém o corte em 6 (conservador, já na monografia). Corrige a descrição do ponto 6 do popover felino, e de passagem a do ponto 4, conforme a ficha da WSAVA. | Código (texto) |
| 3 | Projeção do peso no emagrecimento (B-EMA-04, B-EMA-22) | Projeção linear: perda semanal = % do nível × peso **inicial** (Guimarães e Tudury). | Código |
| 4 | Taxas iguais para cão e gato (B-EMA-13, D-FLC/D-FLG) | Os três níveis continuam para gato. Acima de 1 %/semana, alerta: "pode ser prejudicial ao animal; o recomendado para gatos é 1 %". | Código |
| 5 | Energia padrão alta no emagrecimento do cão (B-EMA-18) | No emagrecimento do cão, o perfil base sugerido passa a ser Cão Inativo (K 95). O veterinário pode trocar. | Código |
| 6 | Volta à manutenção (B-REA-02) | Aviso no plano: ao atingir o alvo, aumentar a quantidade aos poucos e reavaliar. A transição automática fica como trabalho futuro. | Código (aviso) |
| 7 | K padrão da cadela gestante (A-GES-03) | O perfil passa a preencher K = 130 e p = 0,75 (NRC). | Código |
| 8 | Crescimento de cães (A-CRE-05/06) | Mantém a equação do NRC; a monografia diz que foi a adotada e cita a da FEDIAF/ABINPET como alternativa. | Documentar |
| 9 | EM do Atwater na quantidade (A-EM-07) | Mantém a EM do NRC (4 passos) na quantidade; Atwater só na distribuição energética. | Documentar |
| 10 | Fase da vida (A-FV-01, C-FV-02/04/05) | Mantém AAHA/AAFP e documenta as outras classificações e a ausência do porte gigante. | Documentar |
| 11 | Lactação fora da faixa (A-LAC-07) | Calcula do mesmo jeito e mostra alerta de que os dados estão fora da faixa da fonte e o valor é estimativa. | Código (aviso) |
| 12 | Gato obeso no emagrecimento (A-EMA-02, B-EMA-12) | No emagrecimento do gato, o perfil base sugerido passa a ser o do gato magro/ideal: 100 × (peso alvo)^0,67 × fração. | Código |
| 13 | Faixas de excesso de peso no enum (C-ECC) | Troca os comentários pelas faixas de Flanagan (2017, 2018): ECC 7 ≈ 20 %, 8 ≈ 30 %, 9 ≥ 40 % acima do ideal, com a citação. Nenhum comportamento muda. | Código (comentário) |

### Os 41 casos divergentes, um a um

**Resolvidos pela implementação (5):** passam a bater ou ficam alinhados à fonte.
- B-EMA-04: decisão 3 (projeção linear).
- B-EMA-13: decisão 4 (alerta do gato acima de 1 %). A parte da duração (8–40 semanas) é trabalho futuro (F2).
- B-REA-02: decisão 6 (aviso; a transição automática é trabalho futuro, F5).
- C-ECC-02: decisão 1.
- C-ECC-06: decisão 2 (texto dos pontos 6 e 4).

**Divergência mantida por decisão, a documentar na monografia (17):**
- A-EM-07 (decisão 9); A-CRE-05 e A-CRE-06 (decisão 8); A-GES-03 (decisão 7: o padrão fica 130, 1 % abaixo dos 132 da FEDIAF/ABINPET); A-FV-01, C-FV-02, C-FV-04 e C-FV-05 (decisão 10); C-ECC-04 (decisão 2).
- B-EMA-12 (decisão 12): o sistema passa a usar 100 × alvo^0,67, ainda diferente dos 85 × atual^0,4 de Carciofi.
- B-EMA-14 e D-MSD-01: Purina e Merck usam RER × fator; o sistema segue K × PC^p (NRC/FEDIAF).
- B-EMA-16 e D-FLC-03: as fontes aceitam até 3 %/semana em cães; o sistema mantém o alerta acima de 2 % (Guimarães e Tudury, Carciofi).
- D-FLC-02 e D-PRE-01: restrições mais fortes ou por sexo; dá para reproduzir com K personalizado. A decisão 5 aproxima o padrão do sistema dessas fontes.
- C-EN-08: Silva liga K 180–200 ao porte; o sistema liga a raças, como o NRC.

**Erro ou inconsistência da fonte, sem ação (15):** A-LAC-06, D-BVN-01 (fórmula de 5–8 filhotes errada), C-EN-02 e D-WSD-02 (nota de rodapé da WSAVA), C-EN-05 e C-EN-07 (Silva arredonda; 6,732), D-NRCD-10 (filhote de 10 lb não se reproduz), D-NRCG-04 (250 kcal é número de referência), D-CAL-03, D-CAL-04 e D-CAL-06 (planilha de Calvez), D-ASA-02, D-ASA-03 e D-ASA-04 (só uma dieta de Asaro bate), D-LUS-02 (conduta mais agressiva; serve como caso negativo do alerta).

**Fora do escopo do sistema, trabalho futuro (4):** B-EMA-06 (F1), B-EMA-22 (F2), B-OUT-02 (F3), C-AV-01 (F4).

### Trabalhos futuros (não são erros)

Estes casos não são erros do sistema. São diferenças de implementação: o IMPERIUM segue de propósito o método de Guimarães e Tudury (neves:2006) para o emagrecimento e o NRC para a energia, e não implementa outras metodologias nem regras de negócio que esses métodos não trazem. Por isso, cada item abaixo é trabalho futuro (Cap. 4 da monografia), e o caso correspondente **não entra como gabarito** nem conta como divergência a corrigir.

- **F1. Frações de restrição livres (B-EMA-06).** Guimarães e Tudury admitem de 50 % a 75 % da energia; o sistema oferece os três níveis para os quais os autores dão a perda esperada (50, 60 e 70 %).
- **F2. Plano com duração até o alvo (B-EMA-13, B-EMA-22).** O plano tem 15 semanas fixas (`NivelRestricaoDados.DuracaoPlanoEmagrecimentoSemanas`). Com a Branda, 35 → 28 kg precisa de 20 semanas, e 30,2 → 15 kg (Vargas) precisa de 50; as fontes falam em 8 a 40 semanas ou mais. O peso registrado continua sendo comparado com a meta depois da 15ª semana.
- **F3. Outros métodos de EM para dietas caseiras (B-OUT-02).** Andrade (2020) dá 1,45 kcal/g sem dizer o método; o sistema dá 1,357 pelo NRC (−6,4 %). O sistema calcula rações pela análise garantida e não trata dieta caseira.
- **F4. Triagem nutricional completa (C-AV-01).** Freeman (2011) e a WSAVA pedem escore de massa muscular (MCS) e histórico alimentar, que a avaliação não tem (ver também C-ECC-13 e C-AV-04).
- **F5. Transição gradual depois do alvo (B-REA-02).** O sistema avisa para aumentar a quantidade aos poucos (decisão 6); gerar as semanas intermediárias fica para depois.
- **F6. Peso-alvo sugerido pelo ECC (D-FLC-01, D-FLG-01).** Flanagan dá peso-alvo = atual ÷ 1,2 / 1,3 / 1,4 para ECC 7 / 8 / 9; no sistema, o veterinário informa o peso alvo.
- Em geral, **outros planos de emagrecimento** além do de Guimarães e Tudury (RER × fator da Purina e do Merck, as restrições por sexo de Flanagan, o fator por dia da PremieRpet, 85 × peso atual^0,4 de Carciofi) também são trabalho futuro. Os 17 casos "mantidos por decisão" acima que vêm dessas fontes seguem a mesma lógica.

## Não usar como gabarito (erro da fonte)

- ABINPET p.23 e BVNA (2022): lactação de 5–8 filhotes escrita como "96 + 12n" (o certo é 96 + 12(n − 4)).
- nrc:2007 p.5: 25 × 3,5 escrito como 87,7.
- Ogoshi p.10: 6,732 no lugar de 6,7 no gato em crescimento.
- Silva (2021): ECC 6 chamado de ideal; "− 0,1" fora do lugar no crescimento do cão; peso metabólico arredondado na Tabela 1.
- Folheto NRC do cão: filhote de 10 lb (990 kcal) não se reproduz pela equação (D-NRCD-10).
- neves:2006: tira 20 % do peso atual para achar o ideal (o certo seria dividir por 1,2). Os exemplos batem assim mesmo, porque o alvo é dado.
- Vargas (2014): "70,6,70" no lugar de 706,7 kcal. Asaro (2017): só uma das três dietas da tabela bate.

## Não testável hoje (candidatos a trabalho futuro)

Escore de massa muscular (WSAVA, Michel); alerta de fator de risco nutricional; anamnese dietética, com petiscos acima de 10 % das calorias; peso ideal a partir do ECC ou do % de gordura; IMC canino (Muller); emagrecimento em fases e com reajuste quinzenal de 5–10 %; regra do neonato (25 kcal/100 g); CDE por fibra dietética total; aviso para fibra bruta acima de 8 % na MS; EM medida in vivo; ajuste por temperatura ambiente. Detalhes e fontes nos arquivos de `literatura/`.

## Fontes

- **Locais, com bibkey no `tese.bib`:** nrc:2007, carciofi:apostila, ogoshi:2015, abinpet:2024, fediaf:2025, neves:2006, gomes:2016, oliveira:2016, carciofi:obesidade, andrade:2020, cline:2021 (AAHA), freeman:2011, muller:2008, chun:2019, porsani:2020, silva:2021, wsava:2020a/b.
- **Locais sem bibkey:** Jericó et al. (2015, Manual Purina), michel:2011 (só em comentário), anamnese de Oliveira (2016; o PDF não corresponde à entrada `oliveira:2016`). Antes de citar, confira em `PFC-2/.claude/fontes-cap1.md`.
- **Novas (web, fora do bib):** 16 fontes com URL e tipo de acesso em `grupoD.md`. As melhores: folhetos do NRC (2006) para cão e gato, tabelas de calorias da WSAVA (2020), Calvez et al. (2019, PLoS ONE), Fukuda (2024, UFU) e Flanagan et al. (2017, 2018, PLoS ONE). Entram na monografia só se o usuário decidir.
