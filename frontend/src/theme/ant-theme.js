import { colors } from "./colors";

export const themeConfig = {
  token: {
    colorPrimary: colors.verde.medio,
    colorSuccess: colors.verde.realce,
    colorError: colors.vermelho.delete,
    colorInfo: colors.verde.medio,
    colorBgLayout: colors.cinza.claro, // Fundo do conteúdo: cinza claro, para cards/tabelas/modais brancos se destacarem
    
    colorText: colors.verde.escuro,

    borderRadius: 6, // Deixa os cantos um pouco mais arredondados
    fontFamily: `'Work Sans', sans-serif`,
  },
  components: {
    Alert: {
      colorInfoBg: colors.verde.fraco,
      colorInfoBorder: colors.verde.claro,
    },
    Button: {
      colorPrimaryActive: colors.verde.escuro,
      colorPrimaryHover: colors.verde.realce,

      // Remove a sombra padrão dos botões
      primaryShadow: "none",
    },
    Layout: {
      headerBg: colors.cinza.background, // a logo (logo-unificada.png) tem fundo cinza opaco
      footerBg: colors.branco,
      footerPadding: 0,
    },
    Sider: {
      colorBgTrigger: colors.verde.escuro,
      colorTextTrigger: colors.branco,
      colorBgContainer: colors.verde.medio,
      colorText: "rgba(255, 255, 255, 0.85)", // Cor do texto dos itens de menu
    },
    Menu: {
      itemColor: colors.verde.medio,
      // itemHoverColor: colors.verde.realce,
      // itemSelectedColor: colors.verde.realce,
      itemSelectedBg: colors.verde.fraco,
    },
    Card: {
      colorBgContainer: colors.branco,
      boxShadow: "0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)",
      colorBorderSecondary: "rgba(39, 143, 84, 0.3)", // Verde claro suave (borda)
    },
    // Tabela "média e limpa": padding 12/16, só linhas horizontais, cabeçalho verde-fraco liso.
    // O visual vem daqui (tokens), não de CSS com !important na tela.
    Table: {
      colorBgContainer: colors.branco,
      borderColor: "rgba(39, 143, 84, 0.15)", // Verde muito suave para as linhas horizontais
      headerBg: colors.verde.fraco,
      headerColor: colors.verde.escuro,
      headerSplitColor: "transparent", // Sem divisores verticais no cabeçalho
      headerBorderRadius: 8,
      // Cor opaca (verde 5% sobre branco): a coluna fixa de Ações não pode ficar transparente no hover.
      rowHoverBg: "#F4F9F6",
      cellPaddingBlock: 12,
      cellPaddingInline: 16,
    },
    Modal: {
      titleFontSize: 18,
    },
  },
};
