// Geometria das silhuetas do ECC (escores 1, 3, 5, 7 e 9) para cão e gato.
// Cada silhueta tem a vista de cima (cintura) e a vista lateral (abdome), no viewBox 0 0 120 100.
// Os contornos são listas de pontos suavizadas por Catmull-Rom; só as medidas de cintura,
// costelas, quadril e abdome mudam com o escore, então as cinco figuras ficam coerentes.

export const ESCORES_SILHUETA = [1, 3, 5, 7, 9];

const fmt = (n) => Math.round(n * 10) / 10;

// Curva fechada e suave passando por todos os pontos.
export const caminhoSuave = (pontos) => {
  const n = pontos.length;
  const p = (i) => pontos[(i + n) % n];
  let d = `M${fmt(p(0)[0])},${fmt(p(0)[1])}`;
  for (let i = 0; i < n; i += 1) {
    const [x0, y0] = p(i - 1);
    const [x1, y1] = p(i);
    const [x2, y2] = p(i + 1);
    const [x3, y3] = p(i + 2);
    const c1 = [x1 + (x2 - x0) / 6, y1 + (y2 - y0) / 6];
    const c2 = [x2 - (x3 - x1) / 6, y2 - (y3 - y1) / 6];
    d += ` C${fmt(c1[0])},${fmt(c1[1])} ${fmt(c2[0])},${fmt(c2[1])} ${fmt(x2)},${fmt(y2)}`;
  }
  return `${d}Z`;
};

// Linha aberta (pernas), desenhada com traço grosso e ponta redonda.
const linha = (pontos) =>
  pontos.map(([x, y], i) => `${i === 0 ? "M" : "L"}${fmt(x)},${fmt(y)}`).join(" ");

// Curva aberta e suave (cauda).
const curva = (pontos) => {
  const n = pontos.length;
  const p = (i) => pontos[Math.min(Math.max(i, 0), n - 1)];
  let d = `M${fmt(p(0)[0])},${fmt(p(0)[1])}`;
  for (let i = 0; i < n - 1; i += 1) {
    const [x0, y0] = p(i - 1);
    const [x1, y1] = p(i);
    const [x2, y2] = p(i + 1);
    const [x3, y3] = p(i + 2);
    const c1 = [x1 + (x2 - x0) / 6, y1 + (y2 - y0) / 6];
    const c2 = [x2 - (x3 - x1) / 6, y2 - (y3 - y1) / 6];
    d += ` C${fmt(c1[0])},${fmt(c1[1])} ${fmt(c2[0])},${fmt(c2[1])} ${fmt(x2)},${fmt(y2)}`;
  }
  return d;
};

// Espelha a metade de cima de uma vista superior em torno do eixo y = eixo.
const vistaDeCima = (metade, eixo) => {
  const cima = metade.map(([x, l]) => [x, eixo - l]);
  const baixo = [...metade].reverse().map(([x, l]) => [x, eixo + l]);
  return caminhoSuave([...cima, ...baixo]);
};

// Medidas por escore. Vista de cima: meia-largura do quadril, cintura, costelas e ombros.
// Vista lateral: altura do flanco (reentrância) e do meio da barriga; dorso sobe um pouco no obeso.
const MEDIDAS = {
  cao: {
    1: { quadril: 5.6, cintura: 3.2, costelas: 6.6, ombros: 6.2, flanco: 56, barriga: 60, dorso: 0, perna: 5 },
    3: { quadril: 6.2, cintura: 4.4, costelas: 7.2, ombros: 6.6, flanco: 58, barriga: 62.5, dorso: 0, perna: 5.4 },
    5: { quadril: 6.8, cintura: 5.8, costelas: 7.8, ombros: 7, flanco: 60.5, barriga: 64.5, dorso: 0.5, perna: 5.8 },
    7: { quadril: 7.8, cintura: 8.2, costelas: 9, ombros: 7.8, flanco: 64, barriga: 68, dorso: 1.2, perna: 6.3 },
    9: { quadril: 9, cintura: 11.2, costelas: 10.6, ombros: 8.8, flanco: 68, barriga: 72.5, dorso: 2, perna: 7 },
  },
  gato: {
    1: { quadril: 5, cintura: 2.8, costelas: 5.8, ombros: 5.2, flanco: 69, barriga: 71, dorso: 0, perna: 3.8 },
    3: { quadril: 5.4, cintura: 3.8, costelas: 6.3, ombros: 5.5, flanco: 70.5, barriga: 73, dorso: 0, perna: 4.1 },
    5: { quadril: 6, cintura: 5.2, costelas: 6.9, ombros: 5.9, flanco: 72.5, barriga: 75, dorso: 0.4, perna: 4.4 },
    7: { quadril: 7, cintura: 7.6, costelas: 8, ombros: 6.6, flanco: 76, barriga: 79, dorso: 1, perna: 4.8 },
    9: { quadril: 8.4, cintura: 10.4, costelas: 9.4, ombros: 7.4, flanco: 80, barriga: 84, dorso: 1.8, perna: 5.3 },
  },
};

const silhuetaCao = (m) => {
  const eixo = 16;
  const corpoCima = vistaDeCima(
    [
      [22, m.quadril * 0.55],
      [27, m.quadril],
      [35, m.quadril * 0.95],
      [46, m.cintura],
      [58, m.costelas * 0.97],
      [66, m.costelas],
      [76, m.ombros],
      [84, 4.2],
      [88, 4.4],
    ],
    eixo,
  );
  const cabecaCima = caminhoSuave([
    [86, 12], [92, 10.5], [97, 12], [102, 14.2], [105, 16], [102, 17.8], [97, 20], [92, 21.5], [86, 20],
  ]);
  const orelhasCima = caminhoSuave([
    [90, 10.5], [93, 6.5], [96, 7.5], [96, 11], [96, 21], [96, 24.5], [93, 25.5], [90, 21.5],
  ]);
  const d = m.dorso;
  const corpoLado = caminhoSuave([
    [83, 46 - d * 0.4], [70, 45.5 - d], [55, 45.5 - d], [40, 46 - d * 0.6], [30, 47],
    [25.5, 51.5], [25.5, 58], [29, 64.5], [35, 66.5],
    [43, m.flanco], [56, m.barriga], [69, m.barriga - 1.5 + d * 0.3],
    [77, 66.5], [84, 65], [88.5, 58], [88.5, 50],
  ]);
  const cabecaLado = caminhoSuave([
    [82, 47], [85, 38], [89.5, 32], [95, 30], [99.5, 33], [104.5, 35.5], [108.5, 38.5],
    [108, 42.5], [102, 44.5], [96, 45.5], [92, 50], [88, 56],
  ]);
  const orelhaLado = caminhoSuave([[91, 31.5], [95.5, 31.5], [95, 38], [92, 41.5], [89.5, 38]]);
  return {
    preenchidos: [corpoCima, cabecaCima, orelhasCima, corpoLado, cabecaLado, orelhaLado],
    tracos: [
      { d: curva([[21, 16], [15, 14.5], [13, 10.5], [16.5, 8.5]]), largura: 2.6 },
      { d: curva([[28, 49], [21.5, 46.5], [18.5, 40.5], [19.5, 35], [23.5, 34]]), largura: 3.2 },
    ],
    pernasFundo: [
      linha([[76, 64], [76, 80], [77.5, 93.5], [81, 94.5]]),
      linha([[40, 62], [45, 74], [42.5, 85], [44.5, 93.5], [47.5, 94.5]]),
    ],
    pernasFrente: [
      linha([[83, 62], [84.5, 80], [84.5, 93.5], [88, 94.5]]),
      linha([[32, 60], [38, 73], [34, 85], [35.5, 93.5], [39, 94.5]]),
    ],
  };
};

const silhuetaGato = (m) => {
  const eixo = 17;
  const corpoCima = vistaDeCima(
    [
      [26, m.quadril * 0.55],
      [31, m.quadril],
      [38, m.quadril * 0.95],
      [48, m.cintura],
      [60, m.costelas * 0.97],
      [68, m.costelas],
      [76, m.ombros],
      [82, 3.6],
      [85, 3.8],
    ],
    eixo,
  );
  const cabecaCima = caminhoSuave([
    [84, 13.5], [86, 9.5], [88.5, 11], [92, 11.5], [96.5, 14], [98, 17], [96.5, 20], [92, 22.5],
    [88.5, 23], [86, 24.5], [84, 20.5],
  ]);
  const d = m.dorso;
  const corpoLado = caminhoSuave([
    [80, 59 - d * 0.4], [68, 57.5 - d], [54, 57 - d], [40, 57.5 - d * 0.6], [31, 58.5],
    [26.5, 62.5], [26.5, 69], [30, 74.5], [36, 76],
    [43, m.flanco], [56, m.barriga], [69, m.barriga - 1.2 + d * 0.3],
    [76, 75.5], [82, 74], [85.5, 68], [85, 61.5],
  ]);
  const cabecaLado = caminhoSuave([
    [79, 60], [82, 54.5], [83.5, 49], [86, 52], [89, 51], [91.5, 46.5], [93, 52.5], [97, 56],
    [98.5, 59.5], [96, 62], [91, 63.5], [86, 66], [82, 68],
  ]);
  return {
    preenchidos: [corpoCima, cabecaCima, corpoLado, cabecaLado],
    tracos: [
      { d: curva([[27, 17], [18, 17.8], [10, 16.8], [5.5, 14.2]]), largura: 2.4 },
      { d: curva([[28.5, 61], [20.5, 58], [16.5, 50], [16.8, 41], [19.5, 36], [23, 35.5]]), largura: 2.8 },
    ],
    pernasFundo: [
      linha([[74, 73], [72.5, 85], [74, 94.5], [77, 95.5]]),
      linha([[39, 72], [45, 81], [42, 88], [43.5, 94.5], [46.5, 95.5]]),
    ],
    pernasFrente: [
      linha([[80, 72], [81.5, 85], [81.5, 94.5], [84.5, 95.5]]),
      linha([[32, 70], [38.5, 79.5], [34.5, 87.5], [35.5, 94.5], [38.5, 95.5]]),
    ],
  };
};

// Devolve os caminhos da silhueta da espécie ("cao" | "gato") no escore pedido (1, 3, 5, 7 ou 9).
export const geometriaSilhueta = (especie, escore) => {
  const tabela = MEDIDAS[especie] ?? MEDIDAS.cao;
  const medidas = tabela[escore] ?? tabela[5];
  const formas = especie === "gato" ? silhuetaGato(medidas) : silhuetaCao(medidas);
  return { ...formas, larguraPerna: medidas.perna };
};
