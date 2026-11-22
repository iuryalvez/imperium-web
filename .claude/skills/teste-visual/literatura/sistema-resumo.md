# O que o Imperium calcula (resumo de 21/11/2026, usado na extração dos casos)

Use isto para decidir se um caso da literatura é testável no sistema.

## Ração (RacaoService) — EM a partir da análise garantida
Entradas: umidade, PB, EE (extrato etéreo), MM, FB (em % ou g/kg), espécie.
1. MS = 1 − umidade
2. ENN = 1 − (umidade + PB + EE + FB + MM)
3. EB (kcal/g) = 5,7·PB + 9,4·EE + 4,1·(ENN + FB)   (frações)
4. FB na MS (%) = FB/MS·100
   CDE cão = 91,2 − 1,43·FB_MS ; CDE gato = 87,9 − 0,88·FB_MS
5. ED = EB · CDE/100
6. EM cão = ED − 1,04·PB ; EM gato = ED − 0,77·PB   (PB em fração → kcal/g)
Também calcula distribuição energética com Atwater modificado (3,5 / 8,5 / 3,5 kcal/g).
Quantidade de alimento (g/dia) = necessidade (kcal/dia) ÷ EM (kcal/g).

## Necessidade energética (CalculoNecessidadeEnergetica) — kcal/dia
- Manutenção: K × PC^p (perfis: cão 130/0,75 ativo, 140 jovem ativo, 200 dogue alemão, 180 terrier, 105 idoso/terra-nova, 95 inativo; gato doméstico magro/ideal 100 × PC^0,67, obeso 130 × PC^0,40, exótico K? × PC^0,75). K e p editáveis.
- Crescimento (p = PC atual / PC adulto esperado):
  cão: K × PC^p_exp × 3,2 × (e^(−0,87·p) − 0,1)   (K padrão 130, expoente 0,75)
  gato: K × PC^p_exp × 6,7 × (e^(−0,189·p) − 0,66)  (K padrão 100, expoente 0,67)
- Gestação cadela: K × PC^p (+ 26 × PC se marcar "fase final"). Semana 1 a 9.
- Gestação gata: 140 × PC^0,67 (perfil), sem fator de semana. Semana 1 a 9.
- Lactação gata: 100 × PC^0,67 + C × PC × L ; C = 18 (<3 filhotes), 60 (3–4), 70 (>4);
  L por semana 1..7 = 0,9; 0,9; 1,2; 1,2; 1,1; 1,0; 0,8
- Lactação cadela: 145 × PC^0,75 + PC × (24·n + 12·m) × L ; n = min(filhotes,4), m = filhotes−4;
  L por semana 1..4 = 0,75; 0,95; 1,1; 1,2
- Emagrecimento (Guimarães e Tudury 2006): K × (peso alvo)^p × fração; níveis branda 70% (perda 1%/sem),
  moderada 60% (1,5%/sem), severa 50% (2%/sem); energia fixa até o alvo; plano de 15 semanas;
  projeção do peso composta; perda > 2%/semana é sinalizada como prejudicial.
- Fase da vida por idade e porte (cão: filhote <1 ano; jovem 1–2; adulto/sênior/geriátrico depende do porte; gato: filhote <1, jovem 1–6, adulto 7–10, sênior 11–14, geriátrico 15+).
- ECC (escala 1 a 9) é registrado na avaliação; não há cálculo automático de peso ideal a partir do ECC.

## O que o sistema NÃO faz (casos assim são "não testável", mas registre se forem relevantes)
- Formulação por nutrientes (proteína, minerais), anamnese dietética, IMC canino, índice de massa muscular,
  cálculo de peso ideal a partir do ECC/% de gordura, ECC por imagem.
