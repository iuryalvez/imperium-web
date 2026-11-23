// Descrição de cada escore de condição corporal (ECC, escala de 1 a 9) por espécie.
// Texto que antes ficava no Popover do formulário de avaliação.

export const DESCRICOES_ECC = {
  cao: {
    1: {
      classe: "Emaciado",
      texto:
        "Ausência de gordura corporal perceptível. Costelas e proeminências ósseas facilmente visíveis e palpáveis. Cintura ou reentrância abdominal acentuadas.",
    },
    2: {
      classe: "Muito magro",
      texto:
        "Ausência de gordura corporal palpável. Costelas e proeminências ósseas visíveis e facilmente palpáveis. Cintura ou reentrância abdominal acentuadas.",
    },
    3: {
      classe: "Magro",
      texto:
        "Ausência de gordura corporal palpável. Costelas e proeminências ósseas podem estar visíveis, facilmente palpáveis. Cintura ou reentrância abdominal acentuadas.",
    },
    4: {
      classe: "Ideal",
      texto:
        "Gordura corporal mínima. Costelas não visíveis, mas palpáveis. Cintura e reentrância abdominal aparentes.",
    },
    5: {
      classe: "Ideal",
      texto:
        "Gordura corporal presente, mas não excessiva. Costelas não visíveis, mas palpáveis. Cintura e reentrância abdominal presentes.",
    },
    6: {
      classe: "Sobrepeso",
      texto:
        "Gordura corporal moderada. Costelas palpáveis com alguma pressão. Cintura visível, mas não acentuada. Reentrância abdominal aparente.",
    },
    7: {
      classe: "Sobrepeso",
      detalhe: "cerca de 20% acima do ideal",
      texto:
        "Gordura corporal importante. Depósitos de gordura evidente sobre costelas, região lombar e base da cauda. Costelas palpáveis com dificuldade. Cintura e reentrância abdominal pouco perceptíveis ou ausentes.",
    },
    8: {
      classe: "Obesidade",
      detalhe: "cerca de 30% acima do ideal",
      texto:
        "Gordura corporal acentuada. Densos depósitos de gordura sobre costelas, região lombar e base da cauda. Costelas palpáveis somente com pressão acentuada. Cintura e reentrância abdominal ausentes, podendo existir distensão abdominal.",
    },
    9: {
      classe: "Obesidade grave",
      detalhe: "40% ou mais acima do ideal",
      texto:
        "Gordura corporal extremamente acentuada. Depósitos de gordura maciços sobre costelas, proeminências ósseas (não palpáveis), pescoço e membros. Cintura e reentrância abdominal ausentes. Distensão abdominal acentuada.",
    },
  },
  gato: {
    1: {
      classe: "Emaciado",
      texto:
        "Ausência de gordura corporal perceptível. Costelas e proeminências ósseas facilmente visíveis e palpáveis. Cintura ou reentrância abdominal acentuada.",
    },
    2: {
      classe: "Muito magro",
      texto:
        "Ausência de gordura corporal palpável. Costelas e proeminências ósseas podem estar visíveis, facilmente palpáveis. Reentrância abdominal marcada.",
    },
    3: {
      classe: "Magro",
      texto:
        "Gordura corporal mínima. Costelas podem estar visíveis, facilmente palpáveis. Cintura ou reentrância abdominal evidentes.",
    },
    4: {
      classe: "Magro",
      texto:
        "Costelas palpáveis, com cobertura mínima de gordura. Cintura perceptível atrás das costelas. Leve reentrância abdominal. Bolsa de gordura abdominal mínima.",
    },
    5: {
      classe: "Ideal",
      texto:
        "Gordura corporal presente, mas não excessiva. Costelas não visíveis, mas palpáveis. Cintura e reentrância abdominal presentes. Bolsa de gordura abdominal mínima.",
    },
    6: {
      classe: "Sobrepeso",
      texto:
        "Costelas palpáveis, com leve excesso de gordura. Cintura e bolsa de gordura abdominal perceptíveis, mas não evidentes. Reentrância abdominal ausente.",
    },
    7: {
      classe: "Sobrepeso",
      detalhe: "cerca de 20% acima do ideal",
      texto:
        "Gordura corporal importante. Costelas palpáveis com dificuldade. Cintura pouco perceptível. Evidente abaulamento abdominal, com bolsa de gordura moderada.",
    },
    8: {
      classe: "Obesidade grave",
      detalhe: "cerca de 30% acima do ideal",
      texto:
        "Gordura corporal acentuada. Denso depósito de gordura sobre costelas e região lombar. Distensão abdominal proeminente, com significativo depósito de gordura.",
    },
    9: {
      classe: "Obesidade grave",
      detalhe: "40% ou mais acima do ideal",
      texto:
        "Gordura corporal extremamente acentuada. Depósitos de gordura maciços sobre costelas, proeminências ósseas, face e membros. Cintura ausente. Distensão abdominal acentuada, com depósitos de gordura abdominal maciços.",
    },
  },
};

export const descricaoEcc = (especie, escore) =>
  DESCRICOES_ECC[especie]?.[escore] ?? null;
