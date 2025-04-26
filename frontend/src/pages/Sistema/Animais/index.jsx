import React from "react";
import { animaisAPI } from "../../../api/sistema/animaisAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";

const Animais = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista de Animais do Sistema"
      entidade="animaisDoSistema"
      api={animaisAPI}
      nomeBotao="Cadastrar Animal"
      nomeEntidade="Animais do Sistema"
    />
  );
};

export default Animais;
