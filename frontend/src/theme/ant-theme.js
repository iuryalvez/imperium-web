import { colors } from "./colors";

export const themeConfig = {
  token: {
    colorPrimary: colors.verde.medio,
    colorSuccess: colors.verde.realce,
    colorError: colors.vermelho.delete,
    colorInfo: colors.verde.medio,
    colorBgLayout: colors.verde.fraco, // Fundo do Content
    
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
      headerBg: colors.cinza.background,
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
    // Table: {
    //   // Applies palette colors to table headers globally
    //   headerBg: colors.cinza.background,
    //   headerColor: colors.verde.escuro,
    //   headerBorderRadius: 6,
    //   borderColor: colors.cinza.desativado,
    //   rowHoverBg: colors.cinza.background,
    // },
  },
};
