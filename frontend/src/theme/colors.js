// Este arquivo é a nossa única fonte da verdade para as cores.
export const colors = {
  branco: '#FFFFFF',
  cinza: {
    background: '#F7F7F7', // header
    claro: '#FBFBFB', // fundo da área de conteúdo (um pouco mais claro que o header)
    escuro: '#9E9E9E',
    desativado: '#C0C0C0',
    borda: '#E5E7EB', // linhas neutras (grade de gráfico, trilho de barra de progresso)
  },
  verde: {
    fraco: '#D9FFEC',
    realce: '#21BA45',
    claro: '#278F54',
    medio: '#006634',
    escuro: '#064420',
  },
  vermelho: {
    delete: '#D30A0A',
  },
  // Informativo: ícones de ajuda (ⓘ) e avisos "info". Azul escuro para ter contraste no branco.
  azul: {
    info: '#1D4ED8',
    hover: '#1E3A8A',
    fraco: '#EFF6FF', // fundo de Alert info
    borda: '#BFDBFE', // borda de Alert info
  },
  // Séries de gráfico que precisam se distinguir do verde (ex.: evolução de peso)
  grafico: {
    real: '#1D4ED8', // azul: medição real
    alvo: '#D97706', // laranja: meta / referência
  },
};